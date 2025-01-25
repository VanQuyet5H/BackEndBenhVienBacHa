
ALTER TABLE YeuCauTiepNhan
ADD LaCapCuu BIT NULL
GO 

Update dbo.CauHinh
Set [Value] = '3.4.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'
