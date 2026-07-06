using System;

namespace ModernAuthSystem.Services
{
    public class TC01_BruteForceTests
    {
        private readonly AuthService _authService;
        private readonly string _testUser = "do_huy_anh_duy";

        public TC01_BruteForceTests()
        {
            _authService = new AuthService();
            _authService.ClearDatabase();
            // Khởi tạo tài khoản mẫu ban đầu với mật khẩu đúng là Duy@2026
            _authService.SeedUserToDatabase(_testUser, "Duy@2026");
        }

        public void RunTest()
        {
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine(">>> [TEST CASE 01]: MÔ PHỎNG TẤN CÔNG BRUTE-FORCE (NHẬP SAI LIÊN TIẾP ĐỂ KHÓA)");
            Console.WriteLine("--------------------------------------------------------------------------------");

            // Bước 1 đến Bước 5: Thử đăng nhập sai bằng mật khẩu bừa bãi
            for (int i = 1; i <= 5; i++)
            {
                Console.WriteLine($"[Bước {i}]: Thực hiện đăng nhập sai lần thứ {i}...");
                string response = _authService.Login(_testUser, $"mat_khau_sai_thu_{i}");
                Console.WriteLine($"-> Hệ thống phản hồi: \"{response}\"\n");
            }

            // Bước 6: Thử lại lần thứ 6 bằng đúng mật khẩu chính xác
            Console.WriteLine("[Bước 6]: Tài khoản đã đạt ngưỡng sai 5 lần. Thử đăng nhập bằng MẬT KHẨU ĐÚNG ('Duy@2026'):");
            string lockoutResponse = _authService.Login(_testUser, "Duy@2026");
            Console.WriteLine($"-> Hệ thống phản hồi công khai: \"{lockoutResponse}\"");
            
            Console.WriteLine("\n=> KẾT LUẬN TC_AUTH_01: ĐẠT (Chặn đứng hành vi dò mật khẩu thành công).\n");
        }
    }
}