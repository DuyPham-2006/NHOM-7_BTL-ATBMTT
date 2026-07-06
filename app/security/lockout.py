import datetime as dt


class LockoutPolicy:
    def __init__(
        self,
        max_failed_attempts: int = 5,
        lock_seconds: int = 15 * 60,
        # backoff: 1s,2s,4s,... (áp dụng tối đa 30s để tránh quá khắc nghiệt)
        max_backoff_seconds: int = 30,
    ):
        self.max_failed_attempts = max_failed_attempts
        self.lock_seconds = lock_seconds
        self.max_backoff_seconds = max_backoff_seconds

    def compute_locked_until(self, now: dt.datetime) -> dt.datetime:
        return now + dt.timedelta(seconds=self.lock_seconds)

    def compute_backoff_seconds(self, failed_attempts: int) -> int:
        # failed_attempts là số lần FAIL đã xảy ra sau lần sai hiện tại
        # Ví dụ: 1->1s, 2->2s, 3->4s...
        seconds = 2 ** max(0, failed_attempts - 1)
        return int(min(seconds, self.max_backoff_seconds))

