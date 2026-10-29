-- 1. Tạo Cơ sở dữ liệu IdentityValidationDb
CREATE DATABASE IF NOT EXISTS IdentityValidationDb
CHARACTER SET utf8mb4
COLLATE utf8mb4_unicode_ci;

USE IdentityValidationDb;

-- 2. Tạo bảng Citizens lưu trữ thông tin định danh công dân
-- Tích hợp ràng buộc UNIQUE (chống trùng lặp) và CHECK REGEXP_LIKE (chốt chặn Regex tại CSDL)
CREATE TABLE IF NOT EXISTS Citizens (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    FullName VARCHAR(100) NOT NULL,
    PersonalId VARCHAR(12) NOT NULL UNIQUE,
    Email VARCHAR(150) NOT NULL UNIQUE,
    PhoneNumber VARCHAR(15) NOT NULL,
    StartDate DATE NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT chk_personal_id CHECK (REGEXP_LIKE(PersonalId, '^[0-9]{12}$')),
    CONSTRAINT chk_email CHECK (REGEXP_LIKE(Email, '^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$')),
    CONSTRAINT chk_phone CHECK (REGEXP_LIKE(PhoneNumber, '^(0|\\+84)(3|5|7|8|9)[0-9]{8}$')),
    CONSTRAINT chk_start_date CHECK (REGEXP_LIKE(StartDate, '^[0-9]{4}-(0[1-9]|1[0-2])-(0[1-9]|[12][0-9]|3[01])$'))
);

