ALTER TABLE [KhoaPhongNhanVien]
  ADD [LaPhongLamViecChinh] bit null
GO
UPDATE CauHinh
SET [Value] = '0.9.5'
WHERE [Name] = 'CauHinhHeThong.DatabaseVesion'