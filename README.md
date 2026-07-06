# Modern Password Authentication System

Ứng dụng FastAPI demo hệ thống đăng ký, đăng nhập và đổi mật khẩu an toàn với Argon2id. Hỗ trợ log sự kiện và khoá tài khoản khi đăng nhập sai nhiều lần.

## Chạy ứng dụng
1. Tạo môi trường ảo và cài dependencies:
```powershell
python -m venv venv
venv\Scripts\activate
python -m pip install -r requirements.txt
```
2. Chạy server:
```powershell
uvicorn app.main:app --reload
```
3. API endpoints:
- `POST /register`
- `POST /login`
- `POST /change-password`

## File quan trọng
- `app/routes/auth.py`: logic đăng ký, đăng nhập, đổi mật khẩu
- `app/security/password_hasher.py`: hash Argon2id
- `app/security/lockout.py`: cơ chế khóa tài khoản/backoff
- `app/logger.py`: cấu hình ghi log file
- `auth.log`: file log sự kiện
- `schema.sql`: schema database mẫu
- `test_cases.md`: danh sách test case
- `test_report.md`: báo cáo kiểm thử mẫu

## Logging
File log được ghi vào `auth.log` với định dạng:
```
[YYYY-MM-DD HH:MM:SS] User: <username> Action: <action> Reason: <reason> IP: <ip>
```

Không log mật khẩu, password hash hay secret key.
