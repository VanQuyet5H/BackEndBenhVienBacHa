ALTER TABLE dbo.[NhapKhoVatTuChiTiet]
ADD [DonGiaBan]  AS (DonGiaNhap + DonGiaNhap * TiLeTheoThapGia / 100 + DonGiaNhap * VAT /100 + DonGiaNhap * TiLeTheoThapGia * VAT / 10000)
GO
ALTER TABLE dbo.[NhapKhoDuocPhamChiTiet]
ADD [DonGiaBan]  AS (DonGiaNhap + DonGiaNhap * TiLeTheoThapGia / 100 + DonGiaNhap * VAT /100 + DonGiaNhap * TiLeTheoThapGia * VAT / 10000)
GO
ALTER TABLE dbo.[YeuCauNhapKhoVatTuChiTiet]
ADD [DonGiaBan]  AS (DonGiaNhap + DonGiaNhap * TiLeTheoThapGia / 100 + DonGiaNhap * VAT /100 + DonGiaNhap * TiLeTheoThapGia * VAT / 10000)
GO
ALTER TABLE dbo.[YeuCauNhapKhoDuocPhamChiTiet]
ADD [DonGiaBan]  AS (DonGiaNhap + DonGiaNhap * TiLeTheoThapGia / 100 + DonGiaNhap * VAT /100 + DonGiaNhap * TiLeTheoThapGia * VAT / 10000)
GO
ALTER TABLE dbo.[NhapKhoVatTuChiTiet]
ADD TiLeBHYTThanhToan INT NULL
GO
ALTER TABLE dbo.[NhapKhoDuocPhamChiTiet]
ADD TiLeBHYTThanhToan INT NULL
GO
ALTER TABLE dbo.[YeuCauNhapKhoVatTuChiTiet]
ADD TiLeBHYTThanhToan INT NULL
GO
ALTER TABLE dbo.[YeuCauNhapKhoDuocPhamChiTiet]
ADD TiLeBHYTThanhToan INT NULL
GO
Update CauHinh
Set [Value] = '0.7.8' where [Name] = 'CauHinhHeThong.DatabaseVesion'







