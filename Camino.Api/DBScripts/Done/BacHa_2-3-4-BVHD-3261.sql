INSERT dbo.InputStringStored
(
    [Set],
    Value,
    CreatedById,
    LastUserId,
    LastTime,
    CreatedOn
)
VALUES
(   6,         -- Set - int
    N'Phẫu thuật',       -- Value - nvarchar(4000)
    1,         -- CreatedById - bigint
    1,         -- LastUserId - bigint
    GETDATE(), -- LastTime - datetime
    GETDATE()  -- CreatedOn - datetime
    )

	INSERT dbo.InputStringStored
(
    [Set],
    Value,
    CreatedById,
    LastUserId,
    LastTime,
    CreatedOn
)
VALUES
(   6,         -- Set - int
    N'Cần nhập viện',       -- Value - nvarchar(4000)
    1,         -- CreatedById - bigint
    1,         -- LastUserId - bigint
    GETDATE(), -- LastTime - datetime
    GETDATE()  -- CreatedOn - datetime
    )

	INSERT dbo.InputStringStored
(
    [Set],
    Value,
    CreatedById,
    LastUserId,
    LastTime,
    CreatedOn
)
VALUES
(   6,         -- Set - int
    N'Hóa trị',       -- Value - nvarchar(4000)
    1,         -- CreatedById - bigint
    1,         -- LastUserId - bigint
    GETDATE(), -- LastTime - datetime
    GETDATE()  -- CreatedOn - datetime
    )

	INSERT dbo.InputStringStored
(
    [Set],
    Value,
    CreatedById,
    LastUserId,
    LastTime,
    CreatedOn
)
VALUES
(   6,         -- Set - int
    N'Xin nhập viện',       -- Value - nvarchar(4000)
    1,         -- CreatedById - bigint
    1,         -- LastUserId - bigint
    GETDATE(), -- LastTime - datetime
    GETDATE()  -- CreatedOn - datetime
    )


	
	INSERT dbo.InputStringStored
(
    [Set],
    Value,
    CreatedById,
    LastUserId,
    LastTime,
    CreatedOn
)
VALUES
(   6,         -- Set - int
    N'Tai nạn',       -- Value - nvarchar(4000)
    1,         -- CreatedById - bigint
    1,         -- LastUserId - bigint
    GETDATE(), -- LastTime - datetime
    GETDATE()  -- CreatedOn - datetime
    )

		INSERT dbo.InputStringStored
(
    [Set],
    Value,
    CreatedById,
    LastUserId,
    LastTime,
    CreatedOn
)
VALUES
(   6,         -- Set - int
    N'Bệnh nặng',       -- Value - nvarchar(4000)
    1,         -- CreatedById - bigint
    1,         -- LastUserId - bigint
    GETDATE(), -- LastTime - datetime
    GETDATE()  -- CreatedOn - datetime
    )


Update dbo.CauHinh
Set [Value] = '2.3.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'