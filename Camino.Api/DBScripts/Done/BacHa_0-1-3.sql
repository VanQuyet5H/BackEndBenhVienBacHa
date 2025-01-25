ALTER TABLE [DonThuocThanhToanChiTiet]
ALTER COLUMN [SoDangKy] nvarchar(100) NOT NULL;
GO
ALTER TABLE [YeuCauKhamBenhDonThuocChiTiet]
ALTER COLUMN [SoDangKy] nvarchar(100) NOT NULL;
GO

Update CauHinh
Set [Value] = '0.1.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'