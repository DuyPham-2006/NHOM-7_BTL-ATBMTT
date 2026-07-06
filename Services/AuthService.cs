using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using BCrypt.Net;
using System.Security.Cryptography;
using ModernAuthSystem.Models;

namespace ModernAuthSystem.Services
{
    public class AuthService
    {
        // Sử dụng instance fields thay vì static để tránh shared state giữa các test cases
        private readonly List<User> _databaseUsers = new List<User>();
        private readonly List<LoginLog> _databaseLogs = new List<LoginLog>();

        // Chuỗi khóa bảo mật nội bộ dùng cho việc mã hóa ký Token JWT
        private readonly string _jwtSecretKey = "ChuoiKhoaBiMatJWTSieuCapBaoMatKhongDuocPhepTietLoVoiDoDaiTren32KyTu2026!";

        public string Login(string username, string password)
        {
            var user = _databaseUsers.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            
            if (user == null)
            {
                WriteSystemLog(username, "FAILED", "Tài khoản hoặc mật khẩu không chính xác (Username không tồn tại).");
                return "Sai tài khoản hoặc mật khẩu.";
            }

            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
            {
                TimeSpan remainingTime = user.LockoutEnd.Value - DateTime.UtcNow;
                WriteSystemLog(username, "LOCKOUT", $"Từ chối truy cập. Tài khoản đang bị khóa. Thời gian còn lại: {Math.Ceiling(remainingTime.TotalMinutes)} phút.");
                return $"Tài khoản này đã bị tạm khóa do nhập sai quá nhiều lần. Vui lòng thử lại sau {Math.Ceiling(remainingTime.TotalMinutes)} phút.";
            }

            // So khớp mã hash Bcrypt (có muối ngẫu nhiên cố định đi kèm trong chuỗi băm của tài khoản)
            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            if (isPasswordCorrect)
            {
                user.FailedLoginAttempts = 0;
                user.LockoutEnd = null;

                WriteSystemLog(username, "SUCCESS", "Người dùng đăng nhập thành công vào hệ thống.");
                return CreateSessionToken(user);
            }
            else
            {
                user.FailedLoginAttempts++;
                WriteSystemLog(username, "FAILED", $"Nhập sai mật khẩu lần thứ {user.FailedLoginAttempts}.");

                if (user.FailedLoginAttempts >= 5)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                    WriteSystemLog(username, "LOCKOUT", "Hệ thống tự động kích hoạt lệnh khóa tài khoản 15 phút do nghi ngờ Brute-force.");
                    return "Tài khoản của bạn đã bị tạm khóa 15 phút do nhập sai mật khẩu quá 5 lần.";
                }

                return "Sai tài khoản hoặc mật khẩu.";
            }
        }

        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            var user = _databaseUsers.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user == null) return false;

            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
            {
                WriteSystemLog(username, "FAILED_CHANGE_PASSWORD", "Yêu cầu đổi mật khẩu bị từ chối do cung cấp sai mật khẩu cũ.");
                return false;
            }

            if (oldPassword == newPassword)
            {
                WriteSystemLog(username, "FAILED_CHANGE_PASSWORD", "Yêu cầu đổi mật khẩu thất bại do mật khẩu mới trùng mật khẩu cũ.");
                return false;
            }

            // Tái sinh muối (Salt) ngẫu nhiên hoàn toàn mới và thực hiện băm lại mật khẩu mới
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword, workFactor: 12);
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;

            WriteSystemLog(username, "SUCCESS_CHANGE_PASSWORD", "Đổi mật khẩu thành công. Chuỗi Hash và muối cũ đã bị xóa bỏ.");
            return true;
        }

        private string CreateSessionToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecretKey);
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] 
                { 
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim("GeneratedTime", DateTime.UtcNow.ToString("o", System.Globalization.CultureInfo.InvariantCulture))
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private void WriteSystemLog(string username, string status, string message)
        {
            var logEntry = new LoginLog
            {
                Id = _databaseLogs.Count + 1,
                AttemptedUsername = username,
                Status = status,
                Message = message, 
                Timestamp = DateTime.UtcNow
            };
            
            _databaseLogs.Add(logEntry);
            Console.WriteLine($"[AUDIT LOG] [{DateTime.Now:HH:mm:ss}] Thao tác: {status} | User: {username} | Nội dung: {message}");
        }

        public void ClearDatabase()
        {
            _databaseUsers.Clear();
            _databaseLogs.Clear();
        }

        public void SeedUserToDatabase(string username, string plaintextPassword)
        {
            if (_databaseUsers.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                // User already exists, skip seeding
                return;
            }
            _databaseUsers.Add(new User
            {
                Id = _databaseUsers.Count + 1,
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(plaintextPassword, workFactor: 12),
                FailedLoginAttempts = 0,
                LockoutEnd = null
            }); 
        }
        public string DemoPlaintext(string password) => password;

        public string DemoSHA256(string password)
{
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
}
    }
}