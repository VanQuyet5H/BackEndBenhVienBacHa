ALTER TABLE YeuCauTiepNhan
  ADD KSKKetLuanData NVARCHAR(max) NULL;

Update dbo.CauHinh
Set [Value] = '2.4.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'
