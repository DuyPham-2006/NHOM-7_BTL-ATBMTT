import datetime
import sqlite3
import sys
import time
from pathlib import Path

from fastapi.testclient import TestClient

ROOT = Path(__file__).resolve().parent.parent
sys.path.insert(0, str(ROOT))

from app.db import Base, engine
from app.main import app

DB_PATH = ROOT / "app.db"
LOG_PATH = ROOT / "auth.log"

client = TestClient(app)


def reset_environment():
    if DB_PATH.exists():
        DB_PATH.unlink()
    if LOG_PATH.exists():
        try:
            LOG_PATH.write_text("", encoding="utf-8")
        except PermissionError:
            pass

    Base.metadata.create_all(bind=engine)


def read_log():
    if not LOG_PATH.exists():
        return ""
    return LOG_PATH.read_text(encoding="utf-8")


def query_users():
    if not DB_PATH.exists():
        return []
    conn = sqlite3.connect(DB_PATH)
    try:
        cur = conn.cursor()
        cur.execute("SELECT id, username, password_hash, failed_attempts, locked_until FROM users")
        rows = cur.fetchall()
        return rows
    finally:
        conn.close()


def assert_response(response, expected_status_code, expected_message=None):
    assert response.status_code == expected_status_code, f"Expected {expected_status_code}, got {response.status_code}: {response.text}"
    if expected_message is not None:
        json_data = response.json()
        assert expected_message in json_data.get("message", "") or expected_message in json_data.get("status", ""), (
            f"Expected message '{expected_message}', got {json_data}"
        )


def run_tests():
    reset_environment()

    results = []

    # Test 1: Register new account
    r = client.post("/register", json={"username": "admin", "password": "123456"})
    passed = r.status_code == 200 and r.json().get("username") == "admin"
    results.append(("Register account", passed, r.text))

    # Test 2: Login correct password
    r = client.post("/login", json={"username": "admin", "password": "123456"})
    passed = r.status_code == 200 and r.json().get("status") == "success"
    results.append(("Login correct password", passed, r.text))

    # Test 3: Wrong password
    r = client.post("/login", json={"username": "admin", "password": "111111"})
    passed = r.status_code == 200 and r.json().get("status") == "fail"
    results.append(("Login wrong password", passed, r.text))

    # Test 4: Wrong password 5 times => lock account
    locked = False
    for i in range(5):
        r = client.post("/login", json={"username": "admin", "password": "111111"})
    if r.status_code == 200 and r.json().get("status") == "fail":
        locked = "Account locked" in r.json().get("message", "") or "Invalid credentials" in r.json().get("message", "")
    results.append(("Login wrong password 5 times", locked, r.text))

    # Test 5: Login after lockout with correct password should still fail if lock active
    r = client.post("/login", json={"username": "admin", "password": "123456"})
    results.append(("Login during lockout", r.status_code == 200 and r.json().get("status") == "fail", r.text))

    # Test 6: Create second user with same password to check hash uniqueness
    r = client.post("/register", json={"username": "user2", "password": "123456"})
    results.append(("Register second user", r.status_code == 200 and r.json().get("username") == "user2", r.text))

    # Database checks
    users = query_users()
    password_hashes = [row[2] for row in users]
    no_plaintext = all("123456" not in ph for ph in password_hashes)
    hash_unique = len(set(password_hashes)) == len(password_hashes)
    results.append(("Database does not store plaintext", no_plaintext, str(password_hashes)))
    results.append(("Hashes unique for same password", hash_unique, str(password_hashes)))

    # Test 7: Change password
    r = client.post(
        "/change-password",
        json={"username": "admin", "old_password": "123456", "new_password": "654321"},
    )
    results.append(("Change password", r.status_code == 200 and r.json().get("status") == "success", r.text))

    # Test 8: Login with old password fails
    r = client.post("/login", json={"username": "admin", "password": "123456"})
    results.append(("Login with old password after change", r.status_code == 200 and r.json().get("status") == "fail", r.text))

    # Wait for transient backoff to expire before trying the new password
    time.sleep(2)

    # Test 9: Login with new password succeeds
    r = client.post("/login", json={"username": "admin", "password": "654321"})
    results.append(("Login with new password", r.status_code == 200 and r.json().get("status") == "success", r.text))

    # Log checks
    log_text = read_log()
    has_login_success = "Action: Login Success" in log_text
    has_login_failed = "Action: Login Failed" in log_text
    has_account_locked = "Action: Account Locked" in log_text
    has_password_changed = "Action: Password Changed Successfully" in log_text
    has_register = "Action: Register Success" in log_text
    results.append(("Log contains register", has_register, log_text))
    results.append(("Log contains login success", has_login_success, log_text))
    results.append(("Log contains login failed", has_login_failed, log_text))
    results.append(("Log contains account locked", has_account_locked, log_text))
    results.append(("Log contains password change", has_password_changed, log_text))

    # Write summary
    summary_path = ROOT / "test_execution_results.md"
    with summary_path.open("w", encoding="utf-8") as out:
        out.write("# Automated Test Results\n\n")
        out.write(f"Run at: {datetime.datetime.utcnow().isoformat()}Z\n\n")
        out.write("| Test case | Pass | Details |\n")
        out.write("|-----------|------|---------|\n")
        for name, passed, detail in results:
            out.write(f"| {name} | { 'Pass' if passed else 'Fail'} | {detail.replace('|', '/')[:120]} |\n")

    print(f"Test execution finished. See {summary_path}")
    for name, passed, detail in results:
        print(f"{name}: {'PASS' if passed else 'FAIL'}")


if __name__ == "__main__":
    run_tests()
