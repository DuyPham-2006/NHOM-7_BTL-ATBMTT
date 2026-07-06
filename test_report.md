# Test Report - Modern Password Authentication System

## 1. Giới thiệu
Hệ thống xác thực bảo mật sử dụng Argon2id để hash mật khẩu và cơ chế khóa tài khoản/backoff khi đăng nhập sai. Mục tiêu là chứng minh toàn bộ chức năng đăng ký, đăng nhập, khóa tài khoản, đổi mật khẩu và logging hoạt động.

## 2. Môi trường
- Python 3.x
- FastAPI
- SQLite
- argon2-cffi
- requests
- Windows 11 / Windows 10

## 3. Danh sách test case
Xem `test_cases.md` để biết chi tiết 14 test case đã định nghĩa.

## 4. Kết quả kiểm thử
- Đăng ký tài khoản: Pass
- Đăng nhập đúng: Pass
- Đăng nhập sai: Pass
- Sai nhiều lần dẫn đến khoá tài khoản: Pass
- Đổi mật khẩu: Pass
- Không login bằng mật khẩu cũ sau khi đổi: Pass
- Hash mật khẩu lưu trong DB không chứa plaintext: Pass
- Hash của hai user cùng mật khẩu khác nhau: Pass
- Log chứa các sự kiện: Pass

## 5. Ảnh minh họa
Các ảnh minh họa cần chụp khi chạy demo:
- Đăng ký thành công
- Đăng nhập thành công
- Đăng nhập sai
- Account Locked
- Password Changed
- Nội dung bảng `users`
- Nội dung file `auth.log`

## 6. Nhận xét
Hệ thống đã đáp ứng các yêu cầu bảo mật của đề tài: không lưu mật khẩu plaintext, sử dụng Argon2id, có salt ngẫu nhiên nhúng trong hash, có cơ chế khóa tài khoản và ghi log sự kiện. Các log không chứa password, hash hoặc khóa bí mật.
