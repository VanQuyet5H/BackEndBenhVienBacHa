ALTER TABLE DichVuBenhVienTongHop
DROP CONSTRAINT FK_DichVuBenhVienTongHop_DichVuKhamBenhBenhVien; 

ALTER TABLE DichVuBenhVienTongHop
DROP CONSTRAINT FK_DichVuBenhVienTongHop_DichVuGiuongBenhVien; 

ALTER TABLE DichVuBenhVienTongHop
DROP CONSTRAINT FK_DichVuBenhVienTongHop_DichVuKyThuatBenhVien; 

Update dbo.CauHinh
Set [Value] = '0.5.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'