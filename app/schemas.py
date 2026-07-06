from pydantic import BaseModel


class RegisterRequest(BaseModel):
    username: str
    password: str


class RegisterResponse(BaseModel):
    id: int
    username: str


class LoginRequest(BaseModel):
    username: str
    password: str


class LoginResponse(BaseModel):
    status: str
    message: str


class ChangePasswordRequest(BaseModel):
    username: str
    old_password: str
    new_password: str


class ChangePasswordResponse(BaseModel):
    status: str
    message: str

