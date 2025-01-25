ALTER TABLE YeuCauNhapKhoDuocPham
  ADD KyHieuHoaDon NVARCHAR(50) NULL;

ALTER TABLE YeuCauNhapKhoVatTu
  ADD KyHieuHoaDon NVARCHAR(50) NULL;

Update dbo.CauHinh
Set [Value] = '2.4.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'
