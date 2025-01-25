ALTER TABLE YeuCauDuocPhamBenhVien
ALTER COLUMN SoDangKy NVARCHAR(100) NOT NULL;

ALTER TABLE YeuCauDuocPhamBenhVien
ALTER COLUMN HoatChat NVARCHAR(550) NOT NULL;

Update dbo.CauHinh
Set [Value] = '0.7.6' where [Name] = 'CauHinhHeThong.DatabaseVesion'