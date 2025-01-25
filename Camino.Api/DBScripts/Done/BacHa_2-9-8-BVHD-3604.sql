ALTER TABLE [dbo].[NoiTruDonThuocChiTiet]
	ADD [SoThuTu] INT NULL
GO

GO
Update dbo.CauHinh
Set [Value] = '2.9.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'