ALTER TABLE dbo.NoiTruPhieuDieuTriTuVanThuoc ADD LaDichTruyen BIT NULL;
GO
ALTER TABLE dbo.NoiTruPhieuDieuTriTuVanThuoc ADD TocDoTruyen INT NULL;
GO
ALTER TABLE dbo.NoiTruPhieuDieuTriTuVanThuoc ADD DonViTocDoTruyen INT NULL;
GO
ALTER TABLE dbo.NoiTruPhieuDieuTriTuVanThuoc ADD ThoiGianBatDauTruyen INT NULL;
GO
ALTER TABLE dbo.NoiTruPhieuDieuTriTuVanThuoc ADD CachGioTruyenDich FLOAT NULL;
GO
ALTER TABLE dbo.NoiTruPhieuDieuTriTuVanThuoc ADD TheTich INT NULL;
GO

ALTER TABLE dbo.NoiTruPhieuDieuTriTuVanThuoc ADD ThoiDiemChiDinh DATETIME  NOT NULL;
GO

ALTER TABLE dbo.NoiTruPhieuDieuTriTuVanThuoc ADD NhanVienChiDinhId BIGINT NOT NULL;
GO

ALTER TABLE dbo.NoiTruPhieuDieuTriTuVanThuoc ADD NoiChiDinhId BIGINT NOT NULL;
GO

ALTER TABLE dbo.NoiTruPhieuDieuTriTuVanThuoc ADD SoLanDungTrongNgay INT  NULL;
GO

UPDATE CauHinh
Set [Value] = '2.0.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'