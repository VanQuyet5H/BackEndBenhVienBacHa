ALTER TABLE [dbo].[KetQuaXetNghiemChiTiet] ALTER COLUMN [GiaTriNhapTay] [nvarchar](100) NULL;
ALTER TABLE [dbo].[KetQuaXetNghiemChiTiet] ALTER COLUMN [GiaTriDuyet] [nvarchar](100) NULL;
ALTER TABLE [dbo].[KetQuaXetNghiemChiTiet] ALTER COLUMN [GiaTriTuMay] [nvarchar](100) NULL;
ALTER TABLE [dbo].[KetQuaXetNghiemChiTiet] ALTER COLUMN [GiaTriCu] [nvarchar](100) NULL;

GO
Update dbo.CauHinh
Set [Value] = '2.1.6' where [Name] = 'CauHinhHeThong.DatabaseVesion'
