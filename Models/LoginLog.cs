using System;

namespace ModernAuthSystem.Models
{
    public class LoginLog
    {
        // Khóa chính của dòng log
        public int Id { get; set; } = 0;

        // Tên tài khoản được nhập vào khi cố gắng đăng nhập
        public string AttemptedUsername { get; set; } = string.Empty;

        // Trạng thái: "SUCCESS" (Thành công), "FAILED" (Thất bại), "LOCKOUT" (Bị khóa)
        public string Status { get; set; } = string.Empty;

        // Chi tiết hành vi hệ thống (Tuyệt đối không lưu chuỗi password rõ của user)
        public string Message { get; set; } = string.Empty;

        // Thời gian hệ thống ghi nhận sự kiện
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}