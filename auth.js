const sql = require('mssql');
const argon2 = require('argon2');

const config = {
  user: 'sa',
  password: '123456789',   // thay đúng password thật của bạn
  server: 'DESKTOP-LFQRP5A\\SQLEXPRESS',
  database: 'PasswordAuthDB',
  options: {
    encrypt: false,
    trustServerCertificate: true
  }
};

// ===== ĐĂNG KÝ TÀI KHOẢN =====
async function registerUser(username, password) {
  await sql.connect(config);

  // Kiểm tra username đã tồn tại chưa
  const existing = await sql.query`SELECT id FROM users WHERE username = ${username}`;
  if (existing.recordset.length > 0) {
    return { success: false, message: 'Username đã tồn tại' };
  }

  // Hash password bằng Argon2id
  const hash = await argon2.hash(password, {
    type: argon2.argon2id,
    memoryCost: 19456,  // ~19 MB, theo khuyến nghị OWASP
    timeCost: 2,
    parallelism: 1
  });

  // Lưu vào database
  await sql.query`INSERT INTO users (username, password_hash) VALUES (${username}, ${hash})`;

  return { success: true, message: 'Đăng ký thành công' };
}

// ===== ĐĂNG NHẬP =====
async function loginUser(username, password) {
  await sql.connect(config);

  const result = await sql.query`SELECT * FROM users WHERE username = ${username}`;
  const user = result.recordset[0];

  if (!user) {
    return { success: false, message: 'Sai username hoặc password' }; // không nói rõ "user không tồn tại"
  }

  const isValid = await argon2.verify(user.password_hash, password);

  if (!isValid) {
    return { success: false, message: 'Sai username hoặc password' };
  }

  return { success: true, message: 'Đăng nhập thành công', userId: user.id };
}

// ===== TEST THỬ =====
async function main() {
  console.log('--- Test đăng ký ---');
  const regResult = await registerUser('alice', 'MyPassword123!');
  console.log(regResult);

  console.log('--- Test đăng nhập đúng ---');
  const loginResult1 = await loginUser('alice', 'MyPassword123!');
  console.log(loginResult1);

  console.log('--- Test đăng nhập sai password ---');
  const loginResult2 = await loginUser('alice', 'SaiPassword');
  console.log(loginResult2);

  sql.close();
}

main().catch(err => {
  console.error('Lỗi:', err);
  sql.close();
});