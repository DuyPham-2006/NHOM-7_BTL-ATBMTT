import requests

BASE_URL = "http://127.0.0.1:8000"


def reg(username, password):
    r = requests.post(f"{BASE_URL}/register", json={"username": username, "password": password})
    print("REGISTER", username, r.status_code, r.text)


def login(username, password):
    r = requests.post(f"{BASE_URL}/login", json={"username": username, "password": password})
    print("LOGIN", username, r.status_code, r.text)


def change_password(username, old_password, new_password):
    r = requests.post(
        f"{BASE_URL}/change-password",
        json={"username": username, "old_password": old_password, "new_password": new_password},
    )
    print("CHANGE_PASSWORD", username, r.status_code, r.text)


def main():
    # reset server-side not implemented; run with fresh DB or delete app.db manually
    password = "123456"

    reg("userA", password)
    reg("userB", password)

    # demonstrate lockout
    for i in range(6):
        login("userA", "wrong-password")

    # after lockout window expires you can try correct password again;
    # for demo run after waiting or just show that correct works when not locked.
    login("userB", password)

    change_password("userB", password, "newpass123")
    login("userB", "newpass123")


if __name__ == "__main__":
    main()

