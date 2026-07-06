using System;

namespace ModernAuthSystem.Services
{
    public class TC03_SecurityAuditTests
    {
        public void RunTest()
        {
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine(">>> [TEST CASE 03]: ĐÁNH GIÁ ĐỐI CHIẾU CÁC CƠ CHẾ BẢO MẬT MẬT KHẨU (ẢNH VÍ DỤ)");
            Console.WriteLine("--------------------------------------------------------------------------------");
            
            AuthService auth = new AuthService();
            string passAn = "123456";
            string passBinh = "123456";

            Console.WriteLine("[Ví dụ minh họa 1] - Lưu mật khẩu dạng rõ (Plaintext):");
            Console.WriteLine($"-> Mật khẩu của An: {auth.DemoPlaintext(passAn)}");
            Console.WriteLine($"-> Mật khẩu của Bình: {auth.DemoPlaintext(passBinh)}");
            Console.WriteLine("=> NGUY CƠ: Hacker lấy được Database sẽ đọc được ngay mật khẩu gốc.\n");

            Console.WriteLine("[Ví dụ minh họa 2] - Băm bằng SHA-256 trần (Không có Salt):");
            Console.WriteLine($"-> SHA256 của An: {auth.DemoSHA256(passAn)}");
            Console.WriteLine($"-> SHA256 của Bình: {auth.DemoSHA256(passBinh)}");
            Console.WriteLine("=> NGUY CƠ: Trùng chuỗi băm (giống nhau)! Hacker dễ dàng dùng Rainbow Table để tra cứu ngược.\n");

            Console.WriteLine("[Ví dụ minh họa 3] - Sử dụng BCrypt (Có Salt ngẫu nhiên cho từng người & Chạy chậm):");
            // Sinh ngẫu nhiên 2 chuỗi hash độc lập dù cùng password đầu vào
            string bcryptAn = BCrypt.Net.BCrypt.HashPassword(passAn, workFactor: 12);
            string bcryptBinh = BCrypt.Net.BCrypt.HashPassword(passBinh, workFactor: 12);
            Console.WriteLine($"-> BCrypt của An (Salt 1): {bcryptAn}");
            Console.WriteLine($"-> BCrypt của Bình (Salt 2): {bcryptBinh}");
            Console.WriteLine("=> ĐÁNH GIÁ: Chuỗi băm hoàn toàn khác nhau! Vô hiệu hóa hoàn toàn Rainbow Table.");
            Console.WriteLine("--------------------------------------------------------------------------------\n");
        }
    }
}