ALTER TABLE [dbo].[YeuCauDichVuKyThuat]
	ADD [DataKetQuaCanLamSang] nvarchar(max) NULL
GO

Update dbo.CauHinh
Set [Value] = '1.4.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'