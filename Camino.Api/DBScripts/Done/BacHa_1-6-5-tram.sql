
alter table VatTu add HeSoDinhMucDonViTinh int null;
alter table DuocPham add HeSoDinhMucDonViTinh int null;
alter table NhapKhoDuocPhamChiTiet add MaRef nvarchar(200) null;
alter table NhapKhoVatTuChiTiet add MaRef nvarchar(200) null;
alter table YeuCauNhapKhoDuocPhamChiTiet add MaRef nvarchar(200) null;
alter table YeuCauNhapKhoVatTuChiTiet add MaRef nvarchar(200) null;
alter table YeuCauTraVatTuChiTiet add MaRef nvarchar(200) null;
alter table YeuCauTraDuocPhamChiTiet add MaRef nvarchar(200) null;

GO
Update CauHinh
Set [Value] = '1.6.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'