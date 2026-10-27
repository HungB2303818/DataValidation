-- =============================================================================
-- KỊCH BẢN KHỞI TẠO CƠ SỞ DỮ LIỆU - CT287 KIỂM CHỨNG MÔ HÌNH (BUỔI 03)
-- Dự án: IdentityValidationApp (C# WinForms .NET 8 - MVP)
-- Cơ chế: Defense-in-Depth Validation (Phòng thủ đa tầng trên MySQL 8.0+)
-- =============================================================================

-- 1. Tạo Cơ sở dữ liệu IdentityValidationDb
CREATE DATABASE IF NOT EXISTS IdentityValidationDb
CHARACTER SET utf8mb4
COLLATE utf8mb4_unicode_ci;

USE IdentityValidationDb;

-- 2. Tạo bảng Citizens lưu trữ thông tin định danh công dân
-- Tích hợp ràng buộc UNIQUE (chống trùng lặp) và CHECK REGEXP_LIKE (chốt chặn Regex tại CSDL)
CREATE TABLE IF NOT EXISTS Citizens (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    PersonalId VARCHAR(12) NOT NULL UNIQUE,
    Email VARCHAR(150) NOT NULL UNIQUE,
    PhoneNumber VARCHAR(15) NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    -- Ràng buộc hợp thức dữ liệu bằng Regex tại tầng CSDL (MySQL 8.0+)
    -- Đảm bảo dữ liệu ghi xuống luôn đúng quy tắc ngay cả khi bị bypass tầng ứng dụng
    CONSTRAINT chk_personal_id CHECK (REGEXP_LIKE(PersonalId, '^[0-9]{12}$')),
    CONSTRAINT chk_email CHECK (REGEXP_LIKE(Email, '^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$')),
    CONSTRAINT chk_phone CHECK (REGEXP_LIKE(PhoneNumber, '^(0|\\+84)(3|5|7|8|9)[0-9]{8}$'))
);

-- 3. Dữ liệu mẫu kiểm thử hợp lệ (Uncomment nếu muốn nạp thử)
-- INSERT INTO Citizens (PersonalId, Email, PhoneNumber) 
-- VALUES ('092205001234', 'student@ctu.edu.vn', '0912345678');
