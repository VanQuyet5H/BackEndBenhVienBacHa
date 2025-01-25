ALTER TABLE dbo.XuatKhoDuocPham
   ADD TraNCC BIT NULL;

ALTER TABLE dbo.XuatKhoVatTu
   ADD TraNCC BIT NULL;


Update CauHinh
Set [Value] = '2.3.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'
