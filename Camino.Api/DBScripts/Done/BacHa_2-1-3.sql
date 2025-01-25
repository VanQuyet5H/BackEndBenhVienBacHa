ALTER TABLE dbo.Kho
  ADD LoaiDuocPham BIT NULL;

ALTER TABLE dbo.Kho
  ADD LoaiVatTu BIT NULL;

Update dbo.CauHinh
Set [Value] = '2.1.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'