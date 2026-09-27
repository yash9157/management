export interface LoginResponse { accessToken: string; expiresAtUtc: string; username: string; role: string; }

export interface RegisterRequest { username: string; password: string; confirmPassword: string; }
export interface RegisterResponse { username: string; role: string; }
