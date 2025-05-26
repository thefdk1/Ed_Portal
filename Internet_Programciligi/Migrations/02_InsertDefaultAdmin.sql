-- 02_InsertDefaultAdmin.sql
USE EgitimPortalDB;
GO

-- Admin kullanıcısını oluşturma
-- Not: PasswordHash gerçek bir hash olmalıdır, bu örnekte güvenlik için basitleştirilmiştir ve üretimde bu şekilde kullanılmamalıdır
DECLARE @AdminId NVARCHAR(450) = NEWID();
DECLARE @AdminRoleId NVARCHAR(450);

-- Admin rolünün ID'sini al
SELECT @AdminRoleId = Id FROM AspNetRoles WHERE Name = 'Admin';

-- Admin kullanıcısını ekle (eğer yoksa)
IF NOT EXISTS (SELECT * FROM AspNetUsers WHERE UserName = 'admin')
BEGIN
    INSERT INTO AspNetUsers (
        Id, 
        UserName, 
        NormalizedUserName, 
        Email, 
        NormalizedEmail, 
        EmailConfirmed, 
        PasswordHash, -- Gerçek bir uygulama için bu, Identity sisteminin ürettiği bir hash olmalıdır
        SecurityStamp, 
        ConcurrencyStamp, 
        PhoneNumber, 
        PhoneNumberConfirmed, 
        TwoFactorEnabled, 
        LockoutEnd, 
        LockoutEnabled, 
        AccessFailedCount,
        FirstName,
        LastName
    )
    VALUES (
        @AdminId, 
        'admin', 
        'ADMIN', 
        'admin@example.com', 
        'ADMIN@EXAMPLE.COM', 
        1, -- EmailConfirmed
        'AQAAAAIAAYagAAAAELyUvPvR+RCz4Xp9EQPw9YnfIISN+YHcm1eI89xwxg7Zq10ILHh0AtNxvEXw/g==', -- Bu örnek bir hash, gerçek uygulamada Identity tarafından oluşturulur
        NEWID(), -- SecurityStamp
        NEWID(), -- ConcurrencyStamp
        NULL, -- PhoneNumber
        0, -- PhoneNumberConfirmed
        0, -- TwoFactorEnabled
        NULL, -- LockoutEnd
        1, -- LockoutEnabled
        0, -- AccessFailedCount
        'Admin', -- FirstName
        'User' -- LastName
    );

    -- Admin rolünü ata
    INSERT INTO AspNetUserRoles (UserId, RoleId)
    VALUES (@AdminId, @AdminRoleId);
    
    PRINT 'Admin kullanıcısı oluşturuldu ve Admin rolü atandı.';
END
ELSE
BEGIN
    PRINT 'Admin kullanıcısı zaten mevcut.';
END
GO

-- Örnek bir kurs ekleme
IF NOT EXISTS (SELECT * FROM Courses WHERE Name = 'Web Programlama Temelleri')
BEGIN
    INSERT INTO Courses (Name, Description, YoutubeLink, IsActive, CreatedDate)
    VALUES ('Web Programlama Temelleri', 'HTML, CSS ve JavaScript temellerini öğrenin', 'https://www.youtube.com/embed/example', 1, GETDATE());
    
    PRINT 'Örnek kurs eklendi.';
END
ELSE
BEGIN
    PRINT 'Örnek kurs zaten mevcut.';
END
GO 