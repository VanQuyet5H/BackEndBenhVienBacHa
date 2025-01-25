ALTER TABLE [dbo].[KetQuaSinhHieu]
    ADD  KSKPhanLoaiTheLuc INT NULL
GO

Update dbo.CauHinh
Set [Value] = '2.6.6' where [Name] = 'CauHinhHeThong.DatabaseVesion'
