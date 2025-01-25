ALTER TABLE [BenhNhan]
ADD [MaBN] AS CAST(Id AS NVARCHAR(20));
GO

Update CauHinh
Set [Value] = '0.4.7' where [Name] = 'CauHinhHeThong.DatabaseVesion'
