ALTER TABLE dbo.YeuCauLinhDuocPham
  ADD DaGui BIT	NULL;

ALTER TABLE dbo.YeuCauLinhVatTu
  ADD DaGui BIT	NULL;

UPDATE dbo.YeuCauLinhDuocPham 
SET	 DaGui = 1

UPDATE dbo.YeuCauLinhVatTu 
SET	 DaGui = 1
 
Update dbo.CauHinh
Set [Value] = '2.5.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'
