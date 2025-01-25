  ALTER TABLE [dbo].[DuocPham]
    ADD  MaATC [nvarchar](100) NULL
GO
UPDATE [dbo].[DuocPham] SET MaATC = MaHoatChat

Update dbo.CauHinh
Set [Value] = '2.7.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'
