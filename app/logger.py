import logging
from pathlib import Path

LOG_FILE = Path(__file__).resolve().parent.parent / "auth.log"
LOG_FILE.parent.mkdir(parents=True, exist_ok=True)

logger = logging.getLogger("auth")
logger.setLevel(logging.INFO)
if not logger.handlers:
    handler = logging.FileHandler(LOG_FILE, encoding="utf-8")
    formatter = logging.Formatter("[%(asctime)s] %(message)s", datefmt="%Y-%m-%d %H:%M:%S")
    handler.setFormatter(formatter)
    logger.addHandler(handler)


def log_action(username: str, action: str, reason: str = None, ip_address: str = ""):
    parts = [f"User: {username}", f"Action: {action}"]
    if reason:
        parts.append(f"Reason: {reason}")
    if ip_address:
        parts.append(f"IP: {ip_address}")
    logger.info(" ".join(parts))
