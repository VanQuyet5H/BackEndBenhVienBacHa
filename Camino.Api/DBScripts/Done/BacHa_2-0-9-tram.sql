alter table DuocPham ALTER COLUMN SoDangKy nvarchar(100) null;
alter table DuocPham ALTER COLUMN MaHoatChat nvarchar(100) null;
alter table DuocPham ALTER COLUMN HoatChat nvarchar(550) null;
alter table [DonThuocThanhToanChiTiet] ALTER COLUMN SoDangKy nvarchar(100) null;
alter table [DonThuocThanhToanChiTietTheoPhieuThu] ALTER COLUMN SoDangKy nvarchar(100) null;
alter table [NoiTruChiDinhDuocPham] ALTER COLUMN SoDangKy nvarchar(100) null;
alter table [NoiTruPhieuDieuTriTuVanThuoc] ALTER COLUMN SoDangKy nvarchar(100) null;
alter table [TuVanThuocKhamSucKhoe] ALTER COLUMN SoDangKy nvarchar(100) null;
alter table [YeuCauDuocPhamBenhVien] ALTER COLUMN SoDangKy nvarchar(100) null;
alter table [YeuCauKhamBenhDonThuocChiTiet] ALTER COLUMN SoDangKy nvarchar(100) null;

alter table [DonThuocThanhToanChiTiet] ALTER COLUMN MaHoatChat nvarchar(100) null;
alter table [DonThuocThanhToanChiTietTheoPhieuThu] ALTER COLUMN MaHoatChat nvarchar(100) null;
alter table [NoiTruChiDinhDuocPham] ALTER COLUMN MaHoatChat nvarchar(100) null;
alter table [NoiTruPhieuDieuTriTuVanThuoc] ALTER COLUMN MaHoatChat nvarchar(100) null;
alter table [TuVanThuocKhamSucKhoe] ALTER COLUMN MaHoatChat nvarchar(100) null;
alter table [YeuCauDuocPhamBenhVien] ALTER COLUMN MaHoatChat nvarchar(100) null;
alter table [YeuCauKhamBenhDonThuocChiTiet] ALTER COLUMN MaHoatChat nvarchar(100) null;

alter table [DonThuocThanhToanChiTiet] ALTER COLUMN HoatChat nvarchar(550) null;
alter table [DonThuocThanhToanChiTietTheoPhieuThu] ALTER COLUMN HoatChat nvarchar(550) null;
alter table [NoiTruChiDinhDuocPham] ALTER COLUMN HoatChat nvarchar(550) null;
alter table [NoiTruPhieuDieuTriTuVanThuoc] ALTER COLUMN HoatChat nvarchar(550) null;
alter table [TuVanThuocKhamSucKhoe] ALTER COLUMN HoatChat nvarchar(550) null;
alter table [YeuCauDuocPhamBenhVien] ALTER COLUMN HoatChat nvarchar(550) null;
alter table [YeuCauKhamBenhDonThuocChiTiet] ALTER COLUMN HoatChat nvarchar(550) null;
GO
UPDATE CauHinh
Set [Value] = '2.0.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'