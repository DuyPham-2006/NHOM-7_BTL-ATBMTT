# Automated Test Results

Run at: 2026-07-05T05:44:09.007888Z

| Test case | Pass | Details |
|-----------|------|---------|
| Register account | Pass | {"id":1,"username":"admin"} |
| Login correct password | Pass | {"status":"success","message":"Login success"} |
| Login wrong password | Pass | {"status":"fail","message":"Invalid credentials"} |
| Login wrong password 5 times | Pass | {"status":"fail","message":"Account locked. Try again at 2026-07-05T05:44:07.646261Z"} |
| Login during lockout | Pass | {"status":"fail","message":"Account locked. Try again at 2026-07-05T05:44:07.646261Z"} |
| Register second user | Pass | {"id":2,"username":"user2"} |
| Database does not store plaintext | Pass | ['$argon2id$v=19$m=102400,t=2,p=8$B4d88tdDW7emindKRvs7hA$/IyjMvyjLToWjLos5hZPHLldWMXfWHAdbIK2EGV9NNE', '$argon2id$v=19$m |
| Hashes unique for same password | Pass | ['$argon2id$v=19$m=102400,t=2,p=8$B4d88tdDW7emindKRvs7hA$/IyjMvyjLToWjLos5hZPHLldWMXfWHAdbIK2EGV9NNE', '$argon2id$v=19$m |
| Change password | Pass | {"status":"success","message":"Password updated"} |
| Login with old password after change | Pass | {"status":"fail","message":"Invalid credentials"} |
| Login with new password | Pass | {"status":"success","message":"Login success"} |
| Log contains register | Pass | [2026-07-05 12:44:06] User: admin Action: Register Success
[2026-07-05 12:44:06] User: admin Action: Login Success
[2026 |
| Log contains login success | Pass | [2026-07-05 12:44:06] User: admin Action: Register Success
[2026-07-05 12:44:06] User: admin Action: Login Success
[2026 |
| Log contains login failed | Pass | [2026-07-05 12:44:06] User: admin Action: Register Success
[2026-07-05 12:44:06] User: admin Action: Login Success
[2026 |
| Log contains account locked | Pass | [2026-07-05 12:44:06] User: admin Action: Register Success
[2026-07-05 12:44:06] User: admin Action: Login Success
[2026 |
| Log contains password change | Pass | [2026-07-05 12:44:06] User: admin Action: Register Success
[2026-07-05 12:44:06] User: admin Action: Login Success
[2026 |
