ALTER TABLE YeuCauDichVuKyThuatTuongTrinhPTTT
ALTER COLUMN MaPhuongPhapPTTT NVARCHAR(MAX)

ALTER TABLE YeuCauDichVuKyThuatTuongTrinhPTTT
ALTER COLUMN TenPhuongPhapPTTT NVARCHAR(MAX)

UPDATE CauHinh
Set [Value] = '1.8.8' where [Name] = 'CauHinhHeThong.DatabaseVesion'