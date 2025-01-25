ALTER TABLE dbo.HopDongThauDuocPham
ALTER COLUMN SoHopDong NVARCHAR(50) NULL;
GO

ALTER TABLE dbo.HopDongThauDuocPham
ALTER COLUMN NgayKy DATETIME NULL;
GO

ALTER TABLE dbo.HopDongThauVatTu
ALTER COLUMN SoHopDong NVARCHAR(50) NULL;
GO

ALTER TABLE dbo.HopDongThauVatTu
ALTER COLUMN NgayKy DATETIME NULL;
GO

Update dbo.CauHinh
Set [Value] = '2.1.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'
