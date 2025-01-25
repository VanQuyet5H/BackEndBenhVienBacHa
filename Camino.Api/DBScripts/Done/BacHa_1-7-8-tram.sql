alter table NhapKhoDuocPhamChiTiet add NguoiNhapSauKhiDuyetId bigint null;
alter table NhapKhoVatTuChiTiet add NguoiNhapSauKhiDuyetId bigint null;
GO
Update CauHinh
Set [Value] = '1.7.8' where [Name] = 'CauHinhHeThong.DatabaseVesion'