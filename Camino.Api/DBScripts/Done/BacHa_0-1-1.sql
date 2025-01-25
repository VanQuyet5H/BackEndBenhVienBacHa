ALTER TABLE [DichVuKhamBenhBenhVien]
ADD 
	[Ma] nvarchar(50) NULL,
	[Ten] nvarchar(250) NULL;
GO

ALTER TABLE [DichVuKyThuatBenhVien]
ADD 
	[Ma] nvarchar(50) NULL,
	[Ten] nvarchar(250) NULL;
GO

ALTER TABLE [DichVuGiuongBenhVien]
ADD 
	[Ma] nvarchar(50) NULL,
	[Ten] nvarchar(250) NULL;
GO

UPDATE A
SET
    A.Ma = B.Ma,
    A.Ten = B.Ten
FROM
    [DichVuKhamBenhBenhVien] AS A
    INNER JOIN [DichVuKhamBenh] AS B
        ON A.DichVuKhamBenhId = B.Id
GO

ALTER TABLE [DichVuKhamBenhBenhVien]
ALTER COLUMN Ten nvarchar(250) NOT NULL;
GO

UPDATE A
SET
    A.Ma = B.Ma,
    A.Ten = B.Ten
FROM
    [DichVuKyThuatBenhVien] AS A
    INNER JOIN [DichVuKyThuat] AS B
        ON A.DichVuKyThuatId = B.Id
GO

ALTER TABLE [DichVuKyThuatBenhVien]
ALTER COLUMN Ten nvarchar(250) NOT NULL;
GO

UPDATE A
SET
    A.Ma = B.Ma,
    A.Ten = B.Ten
FROM
    [DichVuGiuongBenhVien] AS A
    INNER JOIN [DichVuGiuong] AS B
        ON A.DichVuGiuongId = B.Id
GO

ALTER TABLE [DichVuGiuongBenhVien]
ALTER COLUMN Ten nvarchar(250) NOT NULL;
GO
sp_rename 'DichVuKhamBenh.Ma', 'MaChung', 'COLUMN';
GO
sp_rename 'DichVuKhamBenh.Ten', 'TenChung', 'COLUMN';
GO
sp_rename 'DichVuKyThuat.Ma', 'MaChung', 'COLUMN';
GO
sp_rename 'DichVuKyThuat.Ten', 'TenChung', 'COLUMN';
GO
sp_rename 'DichVuGiuong.Ma', 'MaChung', 'COLUMN';
GO
sp_rename 'DichVuGiuong.Ten', 'TenChung', 'COLUMN';

GO
Update CauHinh
Set [Value] = '0.1.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'