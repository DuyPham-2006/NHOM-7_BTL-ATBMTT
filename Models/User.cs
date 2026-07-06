using System;

namespace ModernAuthSystem.Models
{
    public class User
    {
        // Khóa chính tăng tự động
        public int Id { get; set; } = 0;

        // Tên tài khoản (Duy nhất)
        public string Username { get; set; } = string.Empty;

        // Chuỗi băm mật khẩu bằng bcrypt (Đã bao gồm cấu trúc Salt ngẫu nhiên lồng bên trong)
        public string PasswordHash { get; set; } = string.Empty;

        // Số lần đăng nhập thất bại liên tiếp (Phục vụ chống Brute-force)
        public int FailedLoginAttempts { get; set; } = 0;

        // Mốc thời gian kết thúc thời hạn khóa tài khoản (Nếu bằng null nghĩa là không bị khóa)
        public DateTime? LockoutEnd { get; set; } = null;
    }
}