ALTER TABLE DuocPhamBenhVien
ADD MaDuocPhamBenhVien NVARCHAR(50) NULL

Update CauHinh
Set [Value] = '0.8.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'