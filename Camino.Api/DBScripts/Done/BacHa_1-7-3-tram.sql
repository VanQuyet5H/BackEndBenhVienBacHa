alter table NhapKhoDuocPham add NgayHoaDon datetime null;
alter table NhapKhoVatTu add NgayHoaDon datetime null;
alter table NhapKhoDuocPhamChiTiet add KhoNhapSauKhiDuyetId bigint null;
alter table NhapKhoVatTuChiTiet add KhoNhapSauKhiDuyetId bigint null;
GO
Update CauHinh
Set [Value] = '1.7.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'