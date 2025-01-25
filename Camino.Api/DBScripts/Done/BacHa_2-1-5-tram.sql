alter table NhapKhoDuocPhamChiTiet alter column DuocPhamBenhVienPhanNhomId bigint null;
alter table YeuCauTraDuocPhamChiTiet alter column DuocPhamBenhVienPhanNhomId bigint null;
alter table YeuCauNhapKhoDuocPhamChiTiet alter column DuocPhamBenhVienPhanNhomId bigint null;
GO
UPDATE CauHinh
Set [Value] = '2.1.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'