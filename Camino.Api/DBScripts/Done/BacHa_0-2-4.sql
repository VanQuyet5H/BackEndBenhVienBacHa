ALTER TABLE BenhNhanTienSuBenh
ALTER COLUMN TenBenh nvarchar(200) NULL;
GO


Update CauHinh
Set [Value] = '0.2.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'