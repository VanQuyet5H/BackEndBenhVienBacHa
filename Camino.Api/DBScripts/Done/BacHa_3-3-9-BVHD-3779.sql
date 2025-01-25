ALTER TABLE YeuCauDichVuKyThuat
ADD BuaAn INT NULL
GO
Update dbo.CauHinh
Set [Value] = '3.3.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'
