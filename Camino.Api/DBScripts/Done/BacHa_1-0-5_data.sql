
INSERT dbo.CauHinh
(
    Name,
    DataType,
    Description,
    Value,
    CreatedById,
    LastUserId,
    LastTime,
    CreatedOn
)
VALUES
(   N'CauHinhTiepNhan.MucTranChiPhiKeToa',       -- Name - nvarchar(255)
    4,         -- DataType - int
    N'Mức trần chi phí kê toa',       -- Description - nvarchar(255)
    N'1.500.000',       -- Value - nvarchar(max)
    1,         -- CreatedById - bigint
    1,         -- LastUserId - bigint
    GETDATE(), -- LastTime - datetime
    GETDATE()  -- CreatedOn - datetime
    )


Update CauHinh
Set [Value] = '1.0.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'