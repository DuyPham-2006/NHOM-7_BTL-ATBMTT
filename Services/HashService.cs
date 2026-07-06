using System.Linq;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;
using Konscious.Security.Cryptography;

namespace SecurityBenchmark.Services
{
    public static class HashService
    {
        // ==========================
        // SHA-256
        // ==========================
        public static string HashSHA256(string password)
        {
            using SHA256 sha = SHA256.Create();

            byte[] inputBytes = Encoding.UTF8.GetBytes(password);

            byte[] hashBytes = sha.ComputeHash(inputBytes);

            return Convert.ToHexString(hashBytes);
        }

        // ==========================
        // BCrypt
        // ==========================
        public static string HashBCrypt(string password, int workFactor = 12)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor);
        }

        public static bool VerifyBCrypt(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        // ==========================
        // Argon2id
        // ==========================
        public static async Task<string> HashArgon2id(
            string password,
            int iterations = 4,
            int memorySize = 65536,
            int parallelism = 2)
        {
            // Sinh salt ngẫu nhiên 16 byte
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            var argon = new Argon2id(Encoding.UTF8.GetBytes(password));

            argon.Salt = salt;
            argon.Iterations = iterations;
            argon.MemorySize = memorySize;
            argon.DegreeOfParallelism = parallelism;

            byte[] hash = await argon.GetBytesAsync(32);

            // Encode dạng: $argon2id$v=19$m=65536,t=4,p=2$<base64_salt>$<base64_hash>
            string saltB64 = Convert.ToBase64String(salt);
            string hashB64 = Convert.ToBase64String(hash);
            return $"$argon2id$v=19$m={memorySize},t={iterations},p={parallelism}${saltB64}${hashB64}";
        }

        public static async Task<bool> VerifyArgon2id(string password, string hash)
        {
            // Parse hash string để lấy salt và tham số
            if (!hash.StartsWith("$argon2id$")) return false;

            var parts = hash.Split('$');
            if (parts.Length < 5) return false;

            var parms = parts[3].Split(',');  // "m=65536,t=4,p=2"
            var saltB64 = parts[4];
            var hashB64 = parts[5];

            byte[] salt = Convert.FromBase64String(saltB64);
            byte[] expectedHash = Convert.FromBase64String(hashB64);

            // Parse parameters
            int memory = 65536, iterations = 4, parallelism = 2;
            foreach (var p in parms)
            {
                if (p.StartsWith("m=")) memory = int.Parse(p.Substring(2));
                else if (p.StartsWith("t=")) iterations = int.Parse(p.Substring(2));
                else if (p.StartsWith("p=")) parallelism = int.Parse(p.Substring(2));
            }

            var argon = new Argon2id(Encoding.UTF8.GetBytes(password));
            argon.Salt = salt;
            argon.DegreeOfParallelism = parallelism;
            argon.MemorySize = memory;
            argon.Iterations = iterations;

            byte[] computedHash = await argon.GetBytesAsync(32);

            return computedHash.SequenceEqual(expectedHash);
        }
    }
}