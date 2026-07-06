using System;
using ModernAuthSystem.Services;
using SecurityBenchmark;

namespace ModernAuthSystem
{
    public class Program
    {
        // Định danh tài khoản điều hành hệ thống kiểm toán an ninh
        private static readonly string TargetOperator = "do_huy_anh_duy";

        public static void Main(string[] args)
        {
            // Thiết lập cấu hình hiển thị mã ký tự tiếng Việt có dấu trên Terminal/Console
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            BuildSystemHeader();

            // ====================================================================
            // TIẾN TRÌNH 01: ĐÁNH GIÁ HIỆU NĂNG THUẬT TOÁN MÃ HÓA & BĂM MẬT KHẨU
            // ====================================================================
            ExecuteCryptographyBenchmark();

            // ====================================================================
            // TIẾN TRÌNH 02: KIỂM THỬ TÍCH HỢP ĐỒNG BỘ MODULE CHỐNG BRUTE-FORCE
            // ====================================================================
            ExecuteBruteForceMitigationPipeline();

            // ====================================================================
            // TIẾN TRÌNH 03: KIỂM THỬ CHU KỲ VÒNG ĐỜI THÔNG TIN XÁC THỰC (ĐỔI PASS)
            // ====================================================================
            ExecuteCredentialLifecyclePipeline();

            BuildSystemFooter();
        }

        private static void ExecuteCryptographyBenchmark()
        {
            Console.WriteLine("[HỆ THỐNG] Khởi tạo tiến trình phân tích đối chiếu cơ chế lưu trữ mật mã...");
            Console.WriteLine("[GIÁM SÁT] Đang nạp cấu hình bộ kiểm thử thuật toán độc lập: TC03_SecurityAuditTests");

            var auditSuite = new TC03_SecurityAuditTests();

            try
            {
                // Kích hoạt luồng chạy so sánh trực quan Plaintext, SHA-256 trần và BCrypt có Salt
                auditSuite.RunTest();
                Console.WriteLine("[THÀNH CÔNG] Khối kiểm toán mật mã đã thực thi và xuất báo cáo đối chiếu đạt chuẩn.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CẢNH BÁO] Tiến trình phân tích thuật toán xảy ra lỗi ngoại lệ hệ thống: {ex.Message}\n");
            }

            // Run benchmark from SecurityBenchmark project
            Console.WriteLine();
            SecurityBenchmark.BenchmarkProgram.Run();
            Console.WriteLine();
        }

        private static void ExecuteBruteForceMitigationPipeline()
        {
            Console.WriteLine("[HỆ THỐNG] Kích hoạt chuỗi kiểm thử tích hợp mô phỏng tấn công từ chối dịch vụ / dò mật mã...");
            Console.WriteLine("[GIÁM SÁT] Đang nạp cấu hình bộ kiểm thử phòng chống Brute-Force: TC01_BruteForceTests");

            var bruteForcePipeline = new TC01_BruteForceTests();

            try
            {
                // Kịch bản kiểm tra cơ chế đếm số lần đăng nhập sai và tự động kích hoạt ngưỡng khóa tài khoản
                bruteForcePipeline.RunTest();
                Console.WriteLine("[THÀNH CÔNG] Tiến trình TC_AUTH_01: Xác thực cơ chế Account Lockout tự động chặn đứng Brute-Force thành công.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[THẤT BẠI] Tiến trình TC_AUTH_01: Lỗi trong quá trình giám sát chuỗi Brute-Force. Chi tiết: {ex.Message}");
            }

            Console.WriteLine();
        }

        private static void ExecuteCredentialLifecyclePipeline()
        {
            Console.WriteLine("[HỆ THỐNG] Kích hoạt chuỗi kiểm thử quy trình thay đổi thông tin xác thực an toàn...");
            Console.WriteLine("[GIÁM SÁT] Đang nạp cấu hình bộ kiểm thử quản lý phiên và thông tin mật mã: TC02_ChangePasswordTests");

            var credentialPipeline = new TC02_ChangePasswordTests();

            try
            {
                // Kịch bản xác thực việc hủy muối cũ, sinh muối mới, chặn mật khẩu trùng và kiểm tra Token session
                credentialPipeline.RunTest();
                Console.WriteLine("[THÀNH CÔNG] Tiến trình TC_AUTH_02: Xác thực chu kỳ quản lý phiên làm việc và Token JWT đạt chuẩn an toàn.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[THẤT BẠI] Tiến trình TC_AUTH_02: Lỗi kiểm tra quy trình thay đổi mã xác thực phiên. Chi tiết: {ex.Message}");
            }
        }

        private static void BuildSystemHeader()
        {
            Console.WriteLine("================================================================================");
            Console.WriteLine("  NHẬT KÝ KIỂM TOÁN TÍCH HỢP HỆ THỐNG & XÁC THỰC AN NINH BẢO MẬT THÔNG TIN");
            Console.WriteLine($"  THỜI GIAN KÍCH HOẠT HỆ THỐNG : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"  MÃ SỐ ĐIỀU HÀNH VIÊN         : {TargetOperator.ToUpper()}");
            Console.WriteLine("================================================================================\n");
        }

        private static void BuildSystemFooter()
        {
            Console.WriteLine("\n================================================================================");
            Console.WriteLine("   KẾT THÚC TIẾN TRÌNH - TOÀN BỘ ĐƯỜNG ỐNG KIỂM TRA ĐÃ ĐƯỢC THỰC THI HOÀN CHỈNH");
            Console.WriteLine("================================================================================");
            Console.WriteLine("\nNhấn một phím bất kỳ trên bàn phím để giải phóng tài nguyên hệ thống...");
            try { Console.ReadKey(); } catch { /* non-interactive mode */ }
        }
    }
}