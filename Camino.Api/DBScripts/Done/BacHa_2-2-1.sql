ALTER TABLE dbo.DuocPhamBenhVien
  ADD LoaiThuocTheoQuanLy INT NULL;
GO

Update dbo.CauHinh
Set [Value] = '2.2.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'