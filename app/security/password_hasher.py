from argon2 import PasswordHasher, Type

ph = PasswordHasher(
    type=Type.ID,
    # Tham số có thể chỉnh theo yêu cầu lớp học; set vừa phải để chạy nhanh khi demo
    time_cost=2,
    memory_cost=102400,  # ~100MB
    parallelism=8,
    hash_len=32,
    salt_len=16,
)


def hash_password(password: str) -> str:
    return ph.hash(password)


def verify_password(password_hash: str, password: str) -> bool:
    try:
        return ph.verify(password_hash, password)
    except Exception:
        return False

