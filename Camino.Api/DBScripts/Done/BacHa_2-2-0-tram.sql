ALTER TABLE dbo.KetQuaVaKetLuanMau
ALTER COLUMN KetQuaMau NVARCHAR(max) NOT NULL;
GO

Update dbo.CauHinh
Set [Value] = '2.2.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'
