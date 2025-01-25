ALTER TABLE YeuCauDichVuKyThuat
ADD GiaBenhVienTaiThoiDiemChiDinh DECIMAL(15,2) NULL

Update dbo.CauHinh
Set [Value] = '2.4.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'