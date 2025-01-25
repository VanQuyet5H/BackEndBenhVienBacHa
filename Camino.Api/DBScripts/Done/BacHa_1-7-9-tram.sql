alter table YeuCauNhapKhoDuocPhamChiTiet add NguoiNhapSauKhiDuyetId bigint null;
alter table YeuCauNhapKhoDuocPhamChiTiet add KhoNhapSauKhiDuyetId bigint null;
alter table YeuCauNhapKhoVatTuChiTiet add NguoiNhapSauKhiDuyetId bigint null;
alter table YeuCauNhapKhoVatTuChiTiet add KhoNhapSauKhiDuyetId bigint null;
alter table YeuCauNhapKhoVatTu add NgayHoaDon datetime null;
alter table YeuCauNhapKhoDuocPham add NgayHoaDon datetime null;
GO
Update CauHinh
Set [Value] = '1.7.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'