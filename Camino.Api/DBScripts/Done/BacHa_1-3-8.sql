ALTER TABLE dbo.DuocPham
ADD TheTich INT NULL
GO

ALTER TABLE dbo.YeuCauDuocPhamBenhVien
ADD TheTich INT NULL
GO

Update dbo.CauHinh
Set [Value] = '1.3.8' where [Name] = 'CauHinhHeThong.DatabaseVesion'