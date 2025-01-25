ALTER TABLE [DuyetBaoHiemChiTiet]
ADD [SoLuong] float NULL;
GO

UPDATE [DuyetBaoHiemChiTiet] SET [SoLuong] = 1
GO

ALTER TABLE [DuyetBaoHiemChiTiet]
ALTER COLUMN [SoLuong] float NOT NULL;
GO

Update CauHinh
Set [Value] = '0.4.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'