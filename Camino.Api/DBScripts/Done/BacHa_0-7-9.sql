ALTER TABLE dbo.[YeuCauDuocPhamBenhVien]
ADD [DonGiaBan]  AS (DonGiaNhap + DonGiaNhap * TiLeTheoThapGia / 100 + DonGiaNhap * VAT /100 + DonGiaNhap * TiLeTheoThapGia * VAT / 10000)
ALTER TABLE dbo.[YeuCauDuocPhamBenhVien]
ADD [GiaBan]  AS (DonGiaNhap + DonGiaNhap * TiLeTheoThapGia / 100 + DonGiaNhap * VAT /100 + DonGiaNhap * TiLeTheoThapGia * VAT / 10000) * CAST(SoLuong as decimal(9,2))
ALTER TABLE dbo.[YeuCauVatTuBenhVien]
ADD [DonGiaBan]  AS (DonGiaNhap + DonGiaNhap * TiLeTheoThapGia / 100 + DonGiaNhap * VAT /100 + DonGiaNhap * TiLeTheoThapGia * VAT / 10000)
ALTER TABLE dbo.[YeuCauVatTuBenhVien]
ADD [GiaBan]  AS (DonGiaNhap + DonGiaNhap * TiLeTheoThapGia / 100 + DonGiaNhap * VAT /100 + DonGiaNhap * TiLeTheoThapGia * VAT / 10000) * CAST(SoLuong as decimal(9,2))
ALTER TABLE dbo.[DonThuocThanhToanChiTiet]
ADD [DonGiaBan]  AS (DonGiaNhap + DonGiaNhap * TiLeTheoThapGia / 100 + DonGiaNhap * VAT /100 + DonGiaNhap * TiLeTheoThapGia * VAT / 10000)
ALTER TABLE dbo.[DonThuocThanhToanChiTiet]
ADD [GiaBan]  AS (DonGiaNhap + DonGiaNhap * TiLeTheoThapGia / 100 + DonGiaNhap * VAT /100 + DonGiaNhap * TiLeTheoThapGia * VAT / 10000) * CAST(SoLuong as decimal(9,2))
ALTER TABLE dbo.[DonVTYTThanhToanChiTiet]
ADD [DonGiaBan]  AS (DonGiaNhap + DonGiaNhap * TiLeTheoThapGia / 100 + DonGiaNhap * VAT /100 + DonGiaNhap * TiLeTheoThapGia * VAT / 10000)
ALTER TABLE dbo.[DonVTYTThanhToanChiTiet]
ADD [GiaBan]  AS (DonGiaNhap + DonGiaNhap * TiLeTheoThapGia / 100 + DonGiaNhap * VAT /100 + DonGiaNhap * TiLeTheoThapGia * VAT / 10000) * CAST(SoLuong as decimal(9,2))
GO
Update CauHinh
Set [Value] = '0.7.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'







