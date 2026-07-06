import datetime as dt

from fastapi import APIRouter, Depends, HTTPException, Request
from sqlalchemy import select
from sqlalchemy.orm import Session

from ..db import get_db
from ..models import LoginLog, User
from ..schemas import (
    ChangePasswordRequest,
    ChangePasswordResponse,
    LoginRequest,
    LoginResponse,
    RegisterRequest,
    RegisterResponse,
)
from ..logger import log_action
from ..security.lockout import LockoutPolicy
from ..security.password_hasher import hash_password, verify_password

router = APIRouter()
policy = LockoutPolicy(max_failed_attempts=5, lock_seconds=15 * 60)


def _get_ip(request: Request) -> str:
    forwarded = request.headers.get("X-Forwarded-For")
    if forwarded:
        return forwarded.split(",")[0].strip()
    return request.client.host if request.client else ""


@router.post('/register', response_model=RegisterResponse)
def register(req: RegisterRequest, db: Session = Depends(get_db)):
    username = req.username.strip()
    if not username:
        raise HTTPException(status_code=400, detail="Username is required")
    if not req.password or len(req.password) < 6:
        raise HTTPException(status_code=400, detail="Password must be at least 6 characters")

    existing = db.execute(select(User).where(User.username == username)).scalar_one_or_none()
    if existing:
        raise HTTPException(status_code=409, detail="Username already exists")

    user = User(
        username=username,
        password_hash=hash_password(req.password),
        failed_attempts=0,
        locked_until=None,
        created_at=dt.datetime.utcnow(),
    )
    db.add(user)
    db.commit()
    db.refresh(user)
    log_action(username, "Register Success")
    return RegisterResponse(id=user.id, username=user.username)


@router.post('/login')
def login(req: LoginRequest, request: Request, db: Session = Depends(get_db)):
    username = req.username.strip()
    user: User | None = db.execute(select(User).where(User.username == username)).scalar_one_or_none()
    if not user:
        # không tiết lộ username có tồn tại hay không
        return LoginResponse(status="fail", message="Invalid credentials")

    now = dt.datetime.utcnow()

    # lockout check
    if user.locked_until and user.locked_until > now:
        ip = _get_ip(request)
        log_action(username, "Account Locked", "Attempt during lockout", ip)
        return LoginResponse(
            status="fail",
            message=f"Account locked. Try again at {user.locked_until.isoformat()}Z",
        )

    ok = verify_password(user.password_hash, req.password)
    if ok:
        user.failed_attempts = 0
        user.locked_until = None
        db.add(user)
        ip = _get_ip(request)
        db.add(LoginLog(user_id=user.id, status="success", ip_address=ip, timestamp=now))
        db.commit()
        log_action(username, "Login Success", ip_address=ip)
        return LoginResponse(status="success", message="Login success")

    # fail path
    user.failed_attempts += 1

    # progress delay + lockout
    # tăng delay (backoff) trong báo cáo; implementation dùng locked_until khi vượt ngưỡng
    backoff_seconds = policy.compute_backoff_seconds(user.failed_attempts)

    if user.failed_attempts >= policy.max_failed_attempts:
        user.locked_until = policy.compute_locked_until(now)
        ip = _get_ip(request)
        db.add(LoginLog(user_id=user.id, status="fail", ip_address=ip, timestamp=now))
        db.commit()
        log_action(username, "Account Locked", "Too many failed login attempts", ip)
        return LoginResponse(status="fail", message="Invalid credentials")

    # progressive delay: tạm khóa ngắn để tránh brute-force trong demo
    user.locked_until = now + dt.timedelta(seconds=backoff_seconds)
    ip = _get_ip(request)
    db.add(LoginLog(user_id=user.id, status="fail", ip_address=ip, timestamp=now))
    db.commit()
    log_action(username, "Login Failed", "Wrong Password", ip)
    return LoginResponse(status="fail", message="Invalid credentials")


@router.post('/change-password', response_model=ChangePasswordResponse)
def change_password(req: ChangePasswordRequest, db: Session = Depends(get_db)):
    username = req.username.strip()

    user: User | None = db.execute(select(User).where(User.username == username)).scalar_one_or_none()
    if not user:
        raise HTTPException(status_code=404, detail="User not found")

    if len(req.new_password) < 6:
        raise HTTPException(status_code=400, detail="New password must be at least 6 characters")

    if req.old_password == req.new_password:
        raise HTTPException(status_code=400, detail="New password must be different from old password")

    old_ok = verify_password(user.password_hash, req.old_password)
    if not old_ok:
        raise HTTPException(status_code=400, detail="Old password incorrect")

    # hash mới (salt mới tự sinh bởi argon2)
    user.password_hash = hash_password(req.new_password)
    user.failed_attempts = 0
    user.locked_until = None
    db.add(user)
    db.commit()
    db.refresh(user)
    log_action(username, "Password Changed Successfully")

    return ChangePasswordResponse(status="success", message="Password updated")


