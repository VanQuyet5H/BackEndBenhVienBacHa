-- hot fix: đã chạy script trên His
ALTER TABLE [dbo].[PhienXetNghiem]
	ALTER COLUMN [KetLuan] [nvarchar](1000) NULL
GO
ALTER TABLE [dbo].[PhienXetNghiemChiTiet]
	ALTER COLUMN [KetLuan] [nvarchar](1000) NULL
GO

Update dbo.CauHinh
Set [Value] = '3.2.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'