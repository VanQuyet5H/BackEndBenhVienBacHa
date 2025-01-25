ALTER TABLE [dbo].[YeuCauKhamBenhDonThuocChiTiet]
	ADD [SoLanTrenVien] [float] NULL
GO
ALTER TABLE [dbo].[YeuCauKhamBenhDonThuocChiTiet]
	ADD [LieuDungTrenNgay] [float] NULL
GO

ALTER TABLE [dbo].[YeuCauKhamBenhDonThuocChiTiet]
	ADD [SoThuTu] INT NULL
GO


Update dbo.CauHinh
Set [Value] = '2.7.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'