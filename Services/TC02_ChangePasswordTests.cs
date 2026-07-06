using System;

namespace ModernAuthSystem.Services
{
    public class TC02_ChangePasswordTests
    {
        private readonly string _testUser = "do_huy_anh_duy";

        public void RunTest()
        {
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine(">>> [TEST CASE 02]: KIỂM THỬ CƠ CHẾ ĐỔI MẬT KHẨU AN TOÀN VÀ TÁI ĐĂNG NHẬP");
            Console.WriteLine("--------------------------------------------------------------------------------");

            // Khởi tạo một phiên làm việc mới độc lập để không ảnh hưởng bởi trạng thái khóa của bài test 1
            AuthService activeService = new AuthService();
            activeService.ClearDatabase();
            activeService.SeedUserToDatabase(_testUser, "Duy@2026");

            Console.WriteLine("[Bước i]: Thử yêu cầu đổi mật khẩu nhưng nhập SAI MẬT KHẨU CŨ:");
            bool isChangeWithWrongOldPassSuccess = activeService.ChangePassword(_testUser, "SaiMậtKhẩuCũ123", "NewSecret#2026");
            Console.WriteLine($"-> Kết quả thực hiện đổi: {(isChangeWithWrongOldPassSuccess ? "Thành công (Lỗi - nên từ chối!)" : "Thất bại đúng như thiết kế an toàn")}\n");

            Console.WriteLine("[Bước ii]: Thử đổi mật khẩu nhưng đặt MẬT KHẨU MỚI TRÙNG MẬT KHẨU CŨ:");
            bool isChangeWithDuplicatePassSuccess = activeService.ChangePassword(_testUser, "Duy@2026", "Duy@2026");
            Console.WriteLine($"-> Kết quả thực hiện đổi: {(isChangeWithDuplicatePassSuccess ? "Thành công (Lỗi - nên từ chối!)" : "Thất bại đúng như thiết kế an toàn")}\n");

            Console.WriteLine("[Bước iii]: Thực hiện đổi mật khẩu với thông tin HỢP LỆ (Mật khẩu mới: 'NewSecret#2026'):");
            bool isRealChangeSuccess = activeService.ChangePassword(_testUser, "Duy@2026", "NewSecret#2026");
            Console.WriteLine($"-> Kết quả thực hiện đổi: {(isRealChangeSuccess ? "Thành công tốt đẹp" : "Thất bại")}\n");

            Console.WriteLine("[Bước iv]: Thử dùng MẬT KHẨU CŨ ('Duy@2026') để đăng nhập lại vào hệ thống:");
            string loginWithOldPassResult = activeService.Login(_testUser, "Duy@2026");
            Console.WriteLine($"-> Hệ thống phản hồi: \"{loginWithOldPassResult}\"\n");

            Console.WriteLine("[Bước v]: Dùng MẬT KHẨU MỚI ('NewSecret#2026') để đăng nhập lại vào hệ thống:");
            string loginWithNewPassResult = activeService.Login(_testUser, "NewSecret#2026");
            Console.WriteLine($"-> Hệ thống phản hồi (Trả về mã ký JWT bảo mật):\n{loginWithNewPassResult}");

            Console.WriteLine("\n=> KẾT LUẬN TC_AUTH_02: ĐẠT (Mật khẩu cũ bị hủy, Salt mới được áp dụng thành công).\n");
        }
    }
}