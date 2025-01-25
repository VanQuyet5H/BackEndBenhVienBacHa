alter table HopDongKhamSucKhoeNhanVien add NgayCapChungMinhThu datetime null;
alter table HopDongKhamSucKhoeNhanVien add NoiCapChungMinhThu bigint null;
alter table HopDongKhamSucKhoeNhanVien add NgayBatDauLamViec datetime null;
alter table HopDongKhamSucKhoeNhanVien add NgheCongViecTruocDay nvarchar(2000) null;

 

alter table HopDongKhamSucKhoeNhanVien add HoKhauTinhThanhId bigint null;
alter table HopDongKhamSucKhoeNhanVien add HoKhauQuanHuyenId bigint null;
alter table HopDongKhamSucKhoeNhanVien add HoKhauPhuongXaId bigint null;
alter table HopDongKhamSucKhoeNhanVien add HoKhauDiaChi nvarchar(200) null;

GO
Update CauHinh
Set [Value] = '1.6.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'