Create FUNCTION [dbo].[LayDonGiaBanDonThuocV2] (@PhuongPhapTinhGiaTriTonKho int, @DonGiaNhap decimal(15, 2), @VAT int,@TiLeTheoThapGia int)  
RETURNS decimal(15, 2)
AS  
BEGIN 
	
	DECLARE @GiaTonKho decimal(15, 6);
	DECLARE @GiaBanNhaThuocChuaVat decimal(15, 6);
	DECLARE @DonGiaBan decimal(15, 2);
	
	SET @GiaTonKho = @DonGiaNhap + (@DonGiaNhap * case when @PhuongPhapTinhGiaTriTonKho = (1) then @VAT else (0) end)/(100)

	SET @GiaBanNhaThuocChuaVat = @GiaTonKho + (@GiaTonKho * @TiLeTheoThapGia / 100)

	SET @DonGiaBan = round(@GiaBanNhaThuocChuaVat + (@GiaBanNhaThuocChuaVat * @VAT / 100),2);					
	
	RETURN @DonGiaBan;
END;

GO
CREATE FUNCTION [dbo].[LayDonGiaBanYeuCauDuocPhamV2] (@PhuongPhapTinhGiaTriTonKho int, @DonGiaNhap decimal(15, 2), @VAT int,@TiLeTheoThapGia int)  
RETURNS decimal(15, 2)
AS  
BEGIN
	DECLARE @GiaTonKho decimal(15, 6);
	DECLARE @GiaBanNhaThuocChuaVat decimal(15, 6);
	DECLARE @DonGiaBan decimal(15, 2);
	

	IF @PhuongPhapTinhGiaTriTonKho is null
	BEGIN
		Set @PhuongPhapTinhGiaTriTonKho = 1
	END

	SET @GiaTonKho = @DonGiaNhap + (@DonGiaNhap * case when @PhuongPhapTinhGiaTriTonKho = (1) then @VAT else (0) end)/(100)
	
	SET @DonGiaBan = round(@GiaTonKho + (@GiaTonKho * @TiLeTheoThapGia / 100),2);					
	
	RETURN @DonGiaBan;
END;

GO
CREATE FUNCTION [dbo].[LayGiaBanDonThuocV2] (@PhuongPhapTinhGiaTriTonKho int, @DonGiaNhap decimal(15, 2), @VAT int,@TiLeTheoThapGia int, @SoLuong float)  
RETURNS decimal(15, 4)
AS  
BEGIN 	
	DECLARE @GiaTonKho decimal(15, 6);
	DECLARE @GiaBanNhaThuocChuaVat decimal(15, 6);
	DECLARE @DonGiaBan decimal(15, 2);
	DECLARE @GiaBan decimal(15, 2);

	SET @GiaTonKho = @DonGiaNhap + (@DonGiaNhap * case when @PhuongPhapTinhGiaTriTonKho = (1) then @VAT else (0) end)/(100)

	SET @GiaBanNhaThuocChuaVat = @GiaTonKho + (@GiaTonKho * @TiLeTheoThapGia / 100)

	SET @DonGiaBan = round(@GiaBanNhaThuocChuaVat + (@GiaBanNhaThuocChuaVat * @VAT / 100),2);					
	SET @GiaBan = @DonGiaBan * CONVERT([decimal](9,2),@SoLuong);
	RETURN @GiaBan;
END;
GO

CREATE FUNCTION [dbo].[LayGiaBanYeuCauDuocPhamV2] (@PhuongPhapTinhGiaTriTonKho int, @DonGiaNhap decimal(15, 2), @VAT int,@TiLeTheoThapGia int, @SoLuong float)  
RETURNS decimal(15, 4)
AS  
BEGIN 	
	DECLARE @GiaTonKho decimal(15, 6);
	DECLARE @GiaBanNhaThuocChuaVat decimal(15, 6);
	DECLARE @DonGiaBan decimal(15, 2);
	DECLARE @GiaBan decimal(15, 2);

	IF @PhuongPhapTinhGiaTriTonKho is null
	BEGIN
		Set @PhuongPhapTinhGiaTriTonKho = 1
	END

	SET @GiaTonKho = @DonGiaNhap + (@DonGiaNhap * case when @PhuongPhapTinhGiaTriTonKho = (1) then @VAT else (0) end)/(100)
	
	SET @DonGiaBan = round(@GiaTonKho + (@GiaTonKho * @TiLeTheoThapGia / 100),2);
	SET @GiaBan = @DonGiaBan * CONVERT([decimal](9,2),@SoLuong);
	RETURN @GiaBan;
END;
GO
ALTER TABLE [dbo].[DonThuocThanhToanChiTiet]
	ADD [PhuongPhapTinhGiaTriTonKho] [int] NULL
GO

ALTER TABLE [dbo].[DonThuocThanhToanChiTiet]
	DROP COLUMN DonGiaBan
GO
ALTER TABLE [dbo].[DonThuocThanhToanChiTiet]
	DROP COLUMN GiaBan
GO

ALTER TABLE [dbo].[DonThuocThanhToanChiTiet]
ADD
	[DonGiaBan]  AS (case when [PhuongPhapTinhGiaTriTonKho] is null then [dbo].[LayDonGiaBanDonThuoc]([XuatKhoDuocPhamChiTietViTriId],[DonGiaNhap],[VAT],[TiLeTheoThapGia])
								else [dbo].[LayDonGiaBanDonThuocV2]([PhuongPhapTinhGiaTriTonKho],[DonGiaNhap],[VAT],[TiLeTheoThapGia]) end)
GO
ALTER TABLE [dbo].[DonThuocThanhToanChiTiet]
ADD
	[GiaBan]  AS (case when [PhuongPhapTinhGiaTriTonKho] is null then [dbo].[LayGiaBanDonThuoc]([XuatKhoDuocPhamChiTietViTriId],[DonGiaNhap],[VAT],[TiLeTheoThapGia],[SoLuong])
							else [dbo].[LayGiaBanDonThuocV2]([PhuongPhapTinhGiaTriTonKho],[DonGiaNhap],[VAT],[TiLeTheoThapGia],[SoLuong]) end)

GO
ALTER TABLE [dbo].[YeuCauDuocPhamBenhVien]
	ADD [PhuongPhapTinhGiaTriTonKho] [int] NULL
GO
ALTER TABLE [dbo].[YeuCauDuocPhamBenhVien]
	DROP COLUMN DonGiaBan
GO
ALTER TABLE [dbo].[YeuCauDuocPhamBenhVien]
	DROP COLUMN GiaBan
GO

ALTER TABLE [dbo].[YeuCauDuocPhamBenhVien]
	ADD [DonGiaBan]  AS (case when [PhuongPhapTinhGiaTriTonKho] is null then [dbo].[LayDonGiaBanYeuCauDuocPham]([XuatKhoDuocPhamChiTietId],[DonGiaNhap],[VAT],[TiLeTheoThapGia])
									else [dbo].[LayDonGiaBanYeuCauDuocPhamV2]([PhuongPhapTinhGiaTriTonKho],[DonGiaNhap],[VAT],[TiLeTheoThapGia]) end)
GO
ALTER TABLE [dbo].[YeuCauDuocPhamBenhVien]
	ADD [GiaBan]  AS (case when [PhuongPhapTinhGiaTriTonKho] is null then [dbo].[LayGiaBanYeuCauDuocPham]([XuatKhoDuocPhamChiTietId],[DonGiaNhap],[VAT],[TiLeTheoThapGia],[SoLuong])
									else [dbo].[LayGiaBanYeuCauDuocPhamV2]([PhuongPhapTinhGiaTriTonKho],[DonGiaNhap],[VAT],[TiLeTheoThapGia],[SoLuong]) end)
									


Go
UPDATE CauHinh
Set [Value] = '4.1.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'