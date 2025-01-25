ALTER TABLE GachNo
ADD SuDungGoi BIT NULL

GO
Update dbo.CauHinh
Set [Value] = '3.0.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'