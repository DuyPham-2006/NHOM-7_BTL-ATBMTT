/**
 * legacy_comparison.js
 * Demo (mô tả) so sánh ý tưởng:
 * - SHA-256: nhanh => không phù hợp để lưu mật khẩu
 * - TripleDES: mã hóa 2 chiều => nếu key lộ có thể giải mã ngược
 * - Argon2id: password hashing chuyên dụng (salt + cost)
 *
 * Lưu ý: Đây là script demo cho báo cáo; dự án hiện chạy backend bằng Python.
 */

import crypto from 'crypto';

// --- SHA-256 (không salt, nhanh) ---
function sha256(password) {
  return crypto.createHash('sha256').update(password).digest('hex');
}

// --- TripleDES (minh họa triết lý 'encrypt/decrypt') ---
function tripleDesEncrypt(password, key) {
  // key phải đúng kích thước; demo đơn giản hóa.
  const algo = 'des-ede3-cbc';
  const iv = Buffer.alloc(8, 0);
  const cipher = crypto.createCipheriv(algo, key, iv);
  let encrypted = cipher.update(password, 'utf8', 'base64');
  encrypted += cipher.final('base64');
  return encrypted;
}

function tripleDesDecrypt(cipherText, key) {
  const algo = 'des-ede3-cbc';
  const iv = Buffer.alloc(8, 0);
  const decipher = crypto.createDecipheriv(algo, key, iv);
  let decrypted = decipher.update(cipherText, 'base64', 'utf8');
  decrypted += decipher.final('utf8');
  return decrypted;
}

// --- Argon2id (placeholder): backend Python dùng argon2-cffi ---
async function argon2idPlaceholder(password) {
  return '(Argon2id hash được thực hiện trong Python backend)';
}

async function main() {
  const password = '123456';

  console.log('== SHA-256 (nhanh, dễ brute-force) ==');
  const h1 = sha256(password);
  const h2 = sha256(password);
  console.log('hash1 == hash2 ?', h1 === h2);
  console.log('hash:', h1);

  console.log('\n== TripleDES (encrypt/decrypt => nếu key lộ thì nguy hiểm) ==');
  // 24 bytes key cho 3DES
  const key = crypto.randomBytes(24);
  const ct = tripleDesEncrypt(password, key);
  const back = tripleDesDecrypt(ct, key);
  console.log('cipherText:', ct);
  console.log('decrypted equals original?', back === password);

  console.log('\n== Argon2id (đúng cho password hashing) ==');
  console.log(await argon2idPlaceholder(password));
}

main().catch(console.error);

