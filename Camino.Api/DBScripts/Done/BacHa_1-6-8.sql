
ALTER TABLE DichVuGiuongBenhVienGiaBenhVien DROP COLUMN BaoPhong;
GO
Update CauHinh
Set [Value] = '1.6.8' where [Name] = 'CauHinhHeThong.DatabaseVesion'