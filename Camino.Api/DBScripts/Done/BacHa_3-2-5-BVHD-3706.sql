ALTER TABLE YeuCauKhamBenh 
ADD TrieuChungTiepNhan NVARCHAR(200) NULL
GO

UPDATE y
SET y.TrieuChungTiepNhan = t.TrieuChungTiepNhan
FROM YeuCauKhamBenh y 
LEFT JOIN YeuCauTiepNhan t on y.YeuCauTiepNhanId = t.Id
WHERE t.TrieuChungTiepNhan IS NOT NULL
Go


Update dbo.CauHinh
Set [Value] = '3.2.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'
