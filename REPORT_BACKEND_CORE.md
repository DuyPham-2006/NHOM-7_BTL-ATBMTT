# Backend Core & Cơ chế mã hóa mật khẩu (Giai đoạn 1)

## 1) Vị trí code bàn giao
- API auth (register + login + change password): `app/routes/auth.py`
- Schema dữ liệu (SQL tham khảo): `schema.sql`
- Demo legacy comparison (mô tả SHA-256 vs TripleDES vs Argon2id): `scripts/legacy_comparison.js`
- Demo chạy backend (register/login/lockout/change password): `scripts/demo.py`

## 2) Các hàm API (để gọi cho Anh Duy/Minh Duy/Tuấn Minh)
Base URL (mặc định): `http://127.0.0.1:8000`

### 2.1) Register
**Endpoint**: `POST /register`

**Request body (JSON)**:
```json
{"username":"<string>","password":"<string>"}
```

**Response body** (JSON, status 200):
```json
{"id": <int>, "username": "<string>"}
```

**Lỗi phổ biến**:
- 400: username rỗng hoặc password < 6 ký tự
- 409: username đã tồn tại

### 2.2) Login
**Endpoint**: `POST /login`

**Request body (JSON)**:
```json
{"username":"<string>","password":"<string>"}
```

**Response body** (JSON):
```json
{"status":"success|fail","message":"..."}
```

**Hành vi bảo mật**:
- Kiểm tra `locked_until` trước khi verify password.
- Verify mật khẩu bằng Argon2id `verify_password()` (không so sánh chuỗi hash thủ công).
- Thất bại tăng `failed_attempts` và áp dụng backoff/lockout.
- Ghi `login_logs` cho cả success và fail.

### 2.3) Change password
**Endpoint**: `POST /change-password`

**Request body (JSON)**:
```json
{"username":"<string>","old_password":"<string>","new_password":"<string>"}
```

**Response body** (JSON):
```json
{"status":"success|fail","message":"..."}
```

**Điều kiện**:
- `old_password` phải đúng (verify bằng Argon2id)
- `new_password` tối thiểu 6 ký tự

## 3) Lockout/backoff
Trong `app/security/lockout.py`:
- `max_failed_attempts=5`
- `lock_seconds=15*60`
- `compute_backoff_seconds(failed_attempts)` dùng backoff dạng lũy thừa: 1s, 2s, 4s...

Trong `app/routes/auth.py`:
- Nếu `failed_attempts >= 5` => đặt `locked_until = now + lock_seconds`
- Nếu chưa đủ ngưỡng => đặt `locked_until = now + backoff_seconds` (progressive delay)

## 4) Password hashing (đúng nguyên tắc)
Trong `app/security/password_hasher.py`:
- Dùng `argon2-cffi` (Argon2id) tạo chuỗi hash đã nhúng salt.
- `hash_password(password)` trả về string hash dùng để lưu vào DB.
- `verify_password(password_hash, password)` xác minh an toàn.

=> DB chỉ lưu `password_hash`, không lưu plaintext.

## 5) Legacy comparison script
`/scripts/legacy_comparison.js` (demo mô tả):
- SHA-256: nhanh => không phù hợp để lưu mật khẩu.
- TripleDES: thuộc nhóm encrypt/decrypt => triết lý sai cho password storage.
- Argon2id: password hashing chuyên dụng (salt + cost), backend Python thực hiện.

## 6) Bằng chứng cần chụp (DB)
Khi chạy thành công, chụp 2 thứ:
1. Bảng `users`:
   - cột `password_hash` bắt đầu/giống định dạng Argon2id (chuỗi hash, không phải mật khẩu gốc)
2. Bảng `login_logs`:
   - các dòng `status=success|fail` theo thời gian

(Hiện tại môi trường của bạn bị chặn cài pip trong venv nên chưa chạy được để tạo ảnh chụp DB tự động.)

