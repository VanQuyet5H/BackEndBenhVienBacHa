
ALTER TABLE GoiDichVu
ADD BoPhanId int NULL
GO 

Update dbo.CauHinh
Set [Value] = '3.5.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'
