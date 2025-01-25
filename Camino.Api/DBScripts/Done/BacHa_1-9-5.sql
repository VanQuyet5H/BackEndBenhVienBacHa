ALTER TABLE PhauThuatThuThuatEkipDieuDuong
ALTER COLUMN VaiTroDieuDuong INT NULL

ALTER TABLE PhauThuatThuThuatEkipBacSi
ALTER COLUMN VaiTroBacSi INT NULL

UPDATE CauHinh
Set [Value] = '1.9.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'