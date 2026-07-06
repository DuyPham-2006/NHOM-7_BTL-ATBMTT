from fastapi import FastAPI

from .db import Base, engine
from .router import router
from .models import User, LoginLog

app = FastAPI(title="Modern Password Authentication System")
app.include_router(router)


@app.on_event("startup")
def _startup():
    # tạo bảng
    Base.metadata.create_all(bind=engine)


