ALTER TABLE dbo.KhoaPhong
ADD CoKhamNoiTru BIT NULL
GO

Update dbo.CauHinh
Set [Value] = '3.4.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'

