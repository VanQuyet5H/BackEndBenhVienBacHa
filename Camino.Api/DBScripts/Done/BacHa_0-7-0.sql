ALTER TABLE dbo.TaiKhoanBenhNhan
ADD SoDuTaiKhoan decimal(15,2) NULL;
GO

UPDATE dbo.TaiKhoanBenhNhan SET SoDuTaiKhoan = 0;
GO

ALTER TABLE dbo.TaiKhoanBenhNhan
ALTER COLUMN SoDuTaiKhoan decimal(15,2) NOT NULL;
GO

Update dbo.CauHinh
Set [Value] = '0.7.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'