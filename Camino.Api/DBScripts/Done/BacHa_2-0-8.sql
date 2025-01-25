ALTER TABLE DuocPham
ALTER COLUMN MaHoatChat NVARCHAR(100) NOT NULL;

ALTER TABLE DuocPhamBenhVien
ADD QuanLyDacBiet NVARCHAR(2000) NULL;

ALTER TABLE DuocPhamBenhVienPhanNhom
ADD TenVietTat NVARCHAR(50) NULL;

ALTER TABLE DuocPham
ALTER COLUMN HamLuong NVARCHAR(1000) NULL;

GO
UPDATE CauHinh
Set [Value] = '2.0.8' where [Name] = 'CauHinhHeThong.DatabaseVesion'