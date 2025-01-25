ALTER TABLE dbo.[DonThuocThanhToanChiTiet] DROP COLUMN [DonGiaBan];
GO
ALTER TABLE dbo.[DonThuocThanhToanChiTiet] DROP COLUMN [GiaBan];
GO
ALTER TABLE [DonThuocThanhToanChiTiet]
	ADD [DonGiaBan]  AS ROUND(((([DonGiaNhap]+([DonGiaNhap]*[TiLeTheoThapGia])/(100))+([DonGiaNhap]*[VAT])/(100))+(([DonGiaNhap]*[TiLeTheoThapGia])*[VAT])/(10000)),2)
GO
ALTER TABLE [DonThuocThanhToanChiTiet]
	ADD [GiaBan]  AS (ROUND(((([DonGiaNhap]+([DonGiaNhap]*[TiLeTheoThapGia])/(100))+([DonGiaNhap]*[VAT])/(100))+(([DonGiaNhap]*[TiLeTheoThapGia])*[VAT])/(10000)),2)*CONVERT([decimal](9,2),[SoLuong]))

GO
ALTER TABLE dbo.[DonVTYTThanhToanChiTiet] DROP COLUMN [DonGiaBan];
GO
ALTER TABLE dbo.[DonVTYTThanhToanChiTiet] DROP COLUMN [GiaBan];
GO
ALTER TABLE [DonVTYTThanhToanChiTiet]
	ADD [DonGiaBan]  AS ROUND(((([DonGiaNhap]+([DonGiaNhap]*[TiLeTheoThapGia])/(100))+([DonGiaNhap]*[VAT])/(100))+(([DonGiaNhap]*[TiLeTheoThapGia])*[VAT])/(10000)),2)
GO
ALTER TABLE [DonVTYTThanhToanChiTiet]
	ADD [GiaBan]  AS (ROUND(((([DonGiaNhap]+([DonGiaNhap]*[TiLeTheoThapGia])/(100))+([DonGiaNhap]*[VAT])/(100))+(([DonGiaNhap]*[TiLeTheoThapGia])*[VAT])/(10000)),2)*CONVERT([decimal](9,2),[SoLuong]))

GO
ALTER TABLE dbo.[NhapKhoDuocPhamChiTiet] DROP COLUMN [DonGiaBan];
GO
ALTER TABLE [NhapKhoDuocPhamChiTiet]
	ADD [DonGiaBan]  AS ROUND(((([DonGiaNhap]+([DonGiaNhap]*[TiLeTheoThapGia])/(100))+([DonGiaNhap]*[VAT])/(100))+(([DonGiaNhap]*[TiLeTheoThapGia])*[VAT])/(10000)),2)

GO
ALTER TABLE dbo.[NhapKhoVatTuChiTiet] DROP COLUMN [DonGiaBan];
GO
ALTER TABLE [NhapKhoVatTuChiTiet]
	ADD [DonGiaBan]  AS ROUND(((([DonGiaNhap]+([DonGiaNhap]*[TiLeTheoThapGia])/(100))+([DonGiaNhap]*[VAT])/(100))+(([DonGiaNhap]*[TiLeTheoThapGia])*[VAT])/(10000)),2)

GO
ALTER TABLE dbo.[YeuCauDuocPhamBenhVien] DROP COLUMN [DonGiaBan];
GO
ALTER TABLE dbo.[YeuCauDuocPhamBenhVien] DROP COLUMN [GiaBan];
GO
ALTER TABLE [YeuCauDuocPhamBenhVien]
	ADD [DonGiaBan]  AS ROUND(((([DonGiaNhap]+([DonGiaNhap]*[TiLeTheoThapGia])/(100))+([DonGiaNhap]*[VAT])/(100))+(([DonGiaNhap]*[TiLeTheoThapGia])*[VAT])/(10000)),2)
GO
ALTER TABLE [YeuCauDuocPhamBenhVien]
	ADD [GiaBan]  AS (ROUND(((([DonGiaNhap]+([DonGiaNhap]*[TiLeTheoThapGia])/(100))+([DonGiaNhap]*[VAT])/(100))+(([DonGiaNhap]*[TiLeTheoThapGia])*[VAT])/(10000)),2)*CONVERT([decimal](9,2),[SoLuong]))

GO
ALTER TABLE dbo.[YeuCauVatTuBenhVien] DROP COLUMN [DonGiaBan];
GO
ALTER TABLE dbo.[YeuCauVatTuBenhVien] DROP COLUMN [GiaBan];
GO
ALTER TABLE [YeuCauVatTuBenhVien]
	ADD [DonGiaBan]  AS ROUND(((([DonGiaNhap]+([DonGiaNhap]*[TiLeTheoThapGia])/(100))+([DonGiaNhap]*[VAT])/(100))+(([DonGiaNhap]*[TiLeTheoThapGia])*[VAT])/(10000)),2)
GO
ALTER TABLE [YeuCauVatTuBenhVien]
	ADD [GiaBan]  AS (ROUND(((([DonGiaNhap]+([DonGiaNhap]*[TiLeTheoThapGia])/(100))+([DonGiaNhap]*[VAT])/(100))+(([DonGiaNhap]*[TiLeTheoThapGia])*[VAT])/(10000)),2)*CONVERT([decimal](9,2),[SoLuong]))

GO
ALTER TABLE dbo.[YeuCauNhapKhoDuocPhamChiTiet] DROP COLUMN [DonGiaBan];
GO
ALTER TABLE [YeuCauNhapKhoDuocPhamChiTiet]
	ADD [DonGiaBan]  AS ROUND(((([DonGiaNhap]+([DonGiaNhap]*[TiLeTheoThapGia])/(100))+([DonGiaNhap]*[VAT])/(100))+(([DonGiaNhap]*[TiLeTheoThapGia])*[VAT])/(10000)),2)

GO
ALTER TABLE dbo.[YeuCauNhapKhoVatTuChiTiet] DROP COLUMN [DonGiaBan];
GO
ALTER TABLE [YeuCauNhapKhoVatTuChiTiet]
	ADD [DonGiaBan]  AS ROUND(((([DonGiaNhap]+([DonGiaNhap]*[TiLeTheoThapGia])/(100))+([DonGiaNhap]*[VAT])/(100))+(([DonGiaNhap]*[TiLeTheoThapGia])*[VAT])/(10000)),2)

Update CauHinh
Set [Value] = '0.9.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'