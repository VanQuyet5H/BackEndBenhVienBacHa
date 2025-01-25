ALTER TABLE [YeuCauKhamBenh]
ALTER COLUMN [MaDichVuTT37] nvarchar(50) NULL;
GO
ALTER TABLE [YeuCauDichVuKyThuat]
ALTER COLUMN [MaGiaDichVu] nvarchar(50) NULL;
GO
ALTER TABLE [YeuCauDichVuGiuongBenhVien]
ALTER COLUMN [MaTT37] nvarchar(50) NULL;
GO

Update CauHinh
Set [Value] = '0.1.6' where [Name] = 'CauHinhHeThong.DatabaseVesion'