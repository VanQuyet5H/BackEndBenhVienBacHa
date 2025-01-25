ALTER TABLE YeuCauNhapKhoDuocPham
	ADD [DaXuatExcel] [bit] NULL
GO
ALTER TABLE YeuCauNhapKhoVatTu
	ADD [DaXuatExcel] [bit] NULL

Go
UPDATE CauHinh
Set [Value] = '3.4.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'