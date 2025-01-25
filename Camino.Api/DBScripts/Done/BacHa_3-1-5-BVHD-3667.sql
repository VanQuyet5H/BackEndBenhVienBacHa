ALTER TABLE dbo.[NhaThau]
ADD Ma [nvarchar](50) NULL


Update dbo.CauHinh
Set [Value] = '3.1.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'