ALTER TABLE [DichVuKhamBenhBenhVien]
ALTER COLUMN DichVuKhamBenhId bigint NULL;
GO

ALTER TABLE [DichVuKyThuatBenhVien]
ALTER COLUMN DichVuKyThuatId bigint NULL;
GO

ALTER TABLE [DichVuGiuongBenhVien]
ALTER COLUMN DichVuGiuongId bigint NULL;
GO


ALTER TABLE [DichVuKhamBenhBenhVien]
DROP CONSTRAINT  FK_DichVuKhamBenhBenhVien_KhoaPhong;
GO
ALTER TABLE [DichVuKhamBenhBenhVien]
DROP COLUMN KhoaPhongId;
GO

ALTER TABLE [DichVuKyThuatBenhVien]
DROP CONSTRAINT  FK_DichVuKyThuatBenhVien_KhoaPhong;
GO
ALTER TABLE [DichVuKyThuatBenhVien]
DROP COLUMN KhoaPhongId;
GO

ALTER TABLE [DichVuGiuongBenhVien]
DROP COLUMN KhoaPhongId;
GO

GO
Update CauHinh
Set [Value] = '0.1.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'