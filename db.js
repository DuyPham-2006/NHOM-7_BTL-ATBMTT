const sql = require('mssql');

const config = {
  user: 'sa',
  password: '123456789',       // thay đúng password bạn vừa đặt
  server: 'DESKTOP-LFQRP5A\\SQLEXPRESS',
  database: 'PasswordAuthDB',
  options: {
    encrypt: false,
    trustServerCertificate: true
  }
};

async function testConnection() {
  try {
    await sql.connect(config);
    console.log('✅ Kết nối thành công tới SQL Server');
    const result = await sql.query('SELECT * FROM users');
    console.log(result.recordset);
  } catch (err) {
    console.error('❌ Lỗi kết nối:', err.message);
  }
}

testConnection();