
ALTER TABLE YeuCauLinhDuocPham
ADD LoaiThuocTao INT NULL
GO

Go
UPDATE CauHinh
Set [Value] = '3.4.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'