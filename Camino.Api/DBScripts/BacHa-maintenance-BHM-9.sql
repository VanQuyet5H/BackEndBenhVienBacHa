ALTER TABLE [dbo].[HopDongKhamSucKhoe]
ADD [SoDienThoaiBacSyTuVan] nvarchar(10) NULL;

UPDATE CauHinh
Set [Value] = '3.4.6' where [Name] = 'CauHinhHeThong.DatabaseVesion'