ALTER TABLE dbo.DuyetBaoHiemChiTiet
ALTER COLUMN GiaBaoHiemThanhToan decimal(15,2) NULL;
GO

Update dbo.CauHinh
Set [Value] = '0.7.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'