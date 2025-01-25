ALTER TABLE YeuCauKhamBenh
ADD ThongTinKhamTheoDichVuTemplate NVARCHAR(MAX) NULL;
Go

ALTER TABLE YeuCauKhamBenh
ADD ThongTinKhamTheoDichVuData NVARCHAR(MAX) NULL;
Go

ALTER TABLE YeuCauKhamBenh
DROP COLUMN TuanHoan, HoHap,TieuHoa,ThanTietNieuSinhDuc,ThanKinh,CoXuongKhop,TaiMuiHong,RangHamMat,NoiTietDinhDuong,SanPhuKhoa,DaLieu;
GO

Update CauHinh
Set [Value] = '0.2.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'