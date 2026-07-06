# Test Cases - Modern Password Authentication System

| STT | Chức năng | Input | Kết quả mong đợi | Actual | Trạng thái |
|-----|-----------|-------|------------------|--------|-----------|
| 1 | Đăng ký tài khoản mới | username=admin, password=123456 | Register Success | | |
| 2 | Đăng nhập đúng | username=admin, password=123456 | Login Success | | |
| 3 | Sai mật khẩu | username=admin, password=111111 | Wrong Password | | |
| 4 | Sai mật khẩu nhiều lần | username=admin, password=111111 x5 | Account Locked | | |
| 5 | Đăng nhập khi bị khoá | username=admin, password=123456 | Account Locked | | |
| 6 | Đổi mật khẩu đúng | username=admin, old_password=123456, new_password=654321 | Password Changed | | |
| 7 | Đăng nhập bằng mật khẩu cũ | username=admin, password=123456 | Failed / Wrong Password | | |
| 8 | Đăng nhập bằng mật khẩu mới | username=admin, password=654321 | Login Success | | |
| 9 | Kiểm tra database không lưu plaintext | query users table | password_hash chứa Argon2id string, không chứa plaintext | | |
| 10 | Kiểm tra hash khác nhau cho từng user | tạo 2 user cùng password | hash khác nhau và chứa salt nhúng | | |
| 11 | Kiểm tra log login success | login thành công | auth.log có dòng Login Success | | |
| 12 | Kiểm tra log login failed | password sai | auth.log có dòng Login Failed | | |
| 13 | Kiểm tra log account lock | sai nhiều lần | auth.log có dòng Account Locked | | |
| 14 | Kiểm tra log password change | đổi mật khẩu | auth.log có dòng Password Changed Successfully | | |
