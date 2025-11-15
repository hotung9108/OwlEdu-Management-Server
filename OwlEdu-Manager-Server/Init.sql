-- =============================
-- DATABASE: EnglishCenterManagement
-- =============================

CREATE DATABASE EnglishCenterManagement;
GO
USE EnglishCenterManagement;
GO

-- =============================
-- TABLE: account
-- =============================
CREATE TABLE account (
    id NVARCHAR(50) PRIMARY KEY,
    username NVARCHAR(100) NOT NULL,
    password VARCHAR(255) NOT NULL,
    avatar VARCHAR(255) NULL,
    role NVARCHAR(20) CHECK (role IN ('admin', 'student', 'teacher')),
    status BIT NOT NULL DEFAULT 1,
    email VARCHAR(255) NULL,
    created_at DATETIME DEFAULT GETDATE(),
    update_at DATETIME DEFAULT GETDATE()
);
GO

-- =============================
-- TABLE: student
-- =============================
CREATE TABLE student (
    id NVARCHAR(50) PRIMARY KEY,
    account_id NVARCHAR(50) FOREIGN KEY REFERENCES account(id),
    full_name NVARCHAR(255) NOT NULL,
    birth_date DATE,
    phone_number VARCHAR(20),
    address NVARCHAR(255),
    gender NVARCHAR(10)
);
GO

-- =============================
-- TABLE: teacher
-- =============================
CREATE TABLE teacher (
    id NVARCHAR(50) PRIMARY KEY,
    account_id NVARCHAR(50) FOREIGN KEY REFERENCES account(id),
    full_name NVARCHAR(255) NOT NULL,
    specialization NVARCHAR(255),
    qualification NVARCHAR(255),
    phone_number VARCHAR(20),
    address NVARCHAR(255),
    gender NVARCHAR(10)
);
GO

-- =============================
-- TABLE: course
-- =============================
CREATE TABLE course (
    id NVARCHAR(50) PRIMARY KEY,
    name NVARCHAR(255) NOT NULL,
    description NVARCHAR(MAX),
    status BIT NOT NULL DEFAULT 1,
    number_of_lessons INT,
    fee DECIMAL(12,2)
);
GO

-- =============================
-- TABLE: class
-- =============================
CREATE TABLE class (
    id NVARCHAR(50) PRIMARY KEY,
    course_id NVARCHAR(50) FOREIGN KEY REFERENCES course(id),
    status BIT NOT NULL DEFAULT 1,
    name NVARCHAR(255),
    require DECIMAL(5,2),
    target DECIMAL(5,2),
    max_students INT,
    start_date DATE,
    end_date DATE,
    teacher_id NVARCHAR(50) FOREIGN KEY REFERENCES teacher(id)
);
GO

-- =============================
-- TABLE: enrollment
-- =============================
CREATE TABLE enrollment (
    id NVARCHAR(50) PRIMARY KEY,
    student_id NVARCHAR(50) FOREIGN KEY REFERENCES student(id),
    course_id NVARCHAR(50) FOREIGN KEY REFERENCES course(id),
    enrollment_date DATE DEFAULT GETDATE(),
    status NVARCHAR(20) CHECK (status IN ('active', 'completed', 'cancelled')),
    created_by NVARCHAR(50)
);
GO

-- =============================
-- TABLE: payment
-- =============================
CREATE TABLE payment (
    id NVARCHAR(50) PRIMARY KEY,
    enrollment_id NVARCHAR(50) FOREIGN KEY REFERENCES enrollment(id),
    amount DECIMAL(12,2),
    payment_date DATE DEFAULT GETDATE(),
    fee_collector_id NVARCHAR(50) FOREIGN KEY REFERENCES account(id),
    payer_id NVARCHAR(50) FOREIGN KEY REFERENCES account(id),
    method NVARCHAR(30) CHECK (method IN ('cash', 'bank_transfer', 'credit_card', 'momo')),
    status NVARCHAR(20) CHECK (status IN ('paid', 'pending', 'failed'))
);
GO

-- =============================
-- TABLE: classAssignment
-- =============================
CREATE TABLE classAssignment (
    student_id NVARCHAR(50) FOREIGN KEY REFERENCES student(id),
    class_id NVARCHAR(50) FOREIGN KEY REFERENCES class(id),
    assigned_date DATE DEFAULT GETDATE(),
    status NVARCHAR(20) CHECK (status IN ('learning', 'pass', 'not pass')),
    PRIMARY KEY (student_id, class_id)
);
GO

-- =============================
-- TABLE: schedule
-- =============================
CREATE TABLE schedule (
    id NVARCHAR(50) PRIMARY KEY,
    class_id NVARCHAR(50) FOREIGN KEY REFERENCES class(id),
    session_date DATE,
    start_time TIME,
    end_time TIME,
    room NVARCHAR(50),
    teacher_id NVARCHAR(50) FOREIGN KEY REFERENCES teacher(id)
);
GO

-- =============================
-- TABLE: attendance
-- =============================
CREATE TABLE attendance (
    student_id NVARCHAR(50) FOREIGN KEY REFERENCES student(id),
    schedule_id NVARCHAR(50) FOREIGN KEY REFERENCES schedule(id),
    status NVARCHAR(20) CHECK (status IN ('present', 'absent', 'late', 'excused')),
    note NVARCHAR(255),
    teacher_id NVARCHAR(50) FOREIGN KEY REFERENCES teacher(id),
    PRIMARY KEY (student_id, schedule_id)
);
GO

-- =============================
-- TABLE: score
-- =============================
CREATE TABLE score (
    title NVARCHAR(255),
    student_id NVARCHAR(50) FOREIGN KEY REFERENCES student(id),
    class_id NVARCHAR(50) FOREIGN KEY REFERENCES class(id),
    teacher_id NVARCHAR(50) FOREIGN KEY REFERENCES teacher(id),
    lisening DECIMAL(5,2) NULL,
    speaking DECIMAL(5,2) NULL,
    reading DECIMAL(5,2) NULL,
    writing DECIMAL(5,2) NULL,
    type NVARCHAR(20) CHECK (type IN ('assignment', 'test', 'final-test')),
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME DEFAULT GETDATE(),
    PRIMARY KEY (student_id, class_id, type)
);
GO

INSERT INTO account (id, username, password, avatar, role, status, email)
VALUES
('U000000001', 'admin01', '123456', NULL, 'admin', 1, 'admin01@center.com'),
('U000000002', 'student01', '123456', NULL, 'student', 1, 'student01@gmail.com'),
('U000000003', 'student02', '123456', NULL, 'student', 1, 'student02@gmail.com'),
('U000000004', 'teacher01', '123456', NULL, 'teacher', 1, 'teacher01@center.com'),
('U000000005', 'teacher02', '123456', NULL, 'teacher', 1, 'teacher02@center.com');

INSERT INTO student (id, account_id, full_name, birth_date, phone_number, address, gender)
VALUES
('HV20251114001', 'U000000002', N'Nguyễn Văn A', '2005-02-10', '0901111111', N'Hà Nội', N'Nam'),
('HV20251114002', 'U000000003', N'Trần Thị B', '2004-07-22', '0902222222', N'Hồ Chí Minh', N'Nữ');

INSERT INTO teacher (id, account_id, full_name, specialization, qualification, phone_number, address, gender)
VALUES
('GV20251114001', 'U000000004', N'Lê Văn Thắng', N'IELTS', N'Thạc sĩ Ngôn ngữ Anh', '0903333333', N'Hà Nội', N'Nam'),
('GV20251114002', 'U000000005', N'Phạm Thu Cúc', N'TOEIC', N'Cử nhân Sư phạm Anh', '0904444444', N'Đà Nẵng', N'Nữ');

INSERT INTO course (id, name, description, number_of_lessons, fee)
VALUES
('KH001', N'IELTS Foundation', N'Khóa học nền tảng IELTS', 25, 3500000),
('KH002', N'TOEIC 450+', N'Luyện thi TOEIC cơ bản', 20, 2500000);

INSERT INTO class (id, course_id, status, name, require, target, max_students, start_date, end_date, teacher_id)
VALUES
('LH000001', 'KH001', 1, N'IELTS F1 - Evening', 3.5, 5.0, 20, '2025-11-20', '2026-02-20', 'GV20251114001'),
('LH000002', 'KH002', 1, N'TOEIC Basic - Morning', 300, 450, 25, '2025-11-21', '2026-02-15', 'GV20251114002');

INSERT INTO enrollment (id, student_id, course_id, status, created_by)
VALUES
('DK20251114001', 'HV20251114001', 'KH001', 'active', 'U000000001'),
('DK20251114002', 'HV20251114002', 'KH002', 'active', 'U000000001');

INSERT INTO payment (id, enrollment_id, amount, fee_collector_id, payer_id, method, status)
VALUES
('HD20251114001', 'DK20251114001', 3500000, 'U000000001', 'U000000002', 'cash', 'paid'),
('HD20251114002', 'DK20251114002', 2500000, 'U000000001', 'U000000003', 'bank_transfer', 'paid');

INSERT INTO classAssignment (student_id, class_id, status)
VALUES
('HV20251114001', 'LH000001', 'learning'),
('HV20251114002', 'LH000002', 'learning');

INSERT INTO schedule (id, class_id, session_date, start_time, end_time, room, teacher_id)
VALUES
('BH000000001', 'LH000001', '2025-11-25', '18:00', '20:00', 'A101', 'GV20251114001'),
('BH000000002', 'LH000002', '2025-11-26', '08:00', '10:00', 'B201', 'GV20251114002');

INSERT INTO attendance (student_id, schedule_id, status, note, teacher_id)
VALUES
('HV20251114001', 'BH000000001', 'present', N'Đúng giờ', 'GV20251114001'),
('HV20251114002', 'BH000000002', 'late', N'Đến muộn 10 phút', 'GV20251114002');

INSERT INTO score (title, student_id, class_id, teacher_id, lisening, speaking, reading, writing, type)
VALUES
(N'Bài kiểm tra giữa kỳ', 'HV20251114001', 'LH000001', 'GV20251114001', 5.0, 5.5, 6.0, 5.0, 'test'),
(N'Bài kiểm tra giữa kỳ', 'HV20251114002', 'LH000002', 'GV20251114002', 350, 360, 370, 345, 'test');

