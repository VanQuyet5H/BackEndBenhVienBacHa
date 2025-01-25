ALTER TABLE NhanVien
ADD MaNhanVien NVARCHAR(20) NULL
Go
ALTER TABLE NhanVien
ADD NgayDangKyHanhNghe DATETIME NULL
GO
Update dbo.CauHinh
Set [Value] = '3.8.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'