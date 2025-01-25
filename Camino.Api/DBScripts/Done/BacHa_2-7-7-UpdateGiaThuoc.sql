CREATE FUNCTION [dbo].[LayDonGiaBanDuocPham] (@NhapKhoDuocPhamId bigint, @PhuongPhapTinhGiaTriTonKho int, @DonGiaNhap decimal(15, 2), @VAT int,@TiLeTheoThapGia int)  
RETURNS decimal(15, 2)
AS  
BEGIN 
	DECLARE @KhoId bigint;
	DECLARE @GiaTonKho decimal(15, 6);
	DECLARE @GiaBanNhaThuocChuaVat decimal(15, 6);
	DECLARE @DonGiaBan decimal(15, 2);

    SELECT @KhoId = [KhoId]
    FROM [NhapKhoDuocPham]
    WHERE [Id] = @NhapKhoDuocPhamId;

	SET @GiaTonKho = @DonGiaNhap + (@DonGiaNhap * case when @PhuongPhapTinhGiaTriTonKho = (1) then @VAT else (0) end)/(100)
	SET @GiaBanNhaThuocChuaVat = @GiaTonKho + (@GiaTonKho * @TiLeTheoThapGia / 100)

	SET @DonGiaBan = CASE WHEN @KhoId = 6 -- kho nha thuoc
					THEN 
						round(@GiaBanNhaThuocChuaVat + (@GiaBanNhaThuocChuaVat * @VAT / 100),2)
					ELSE 
						round(@GiaTonKho + (@GiaTonKho * @TiLeTheoThapGia / 100),2)
					END;						
	
	RETURN @DonGiaBan;
END;
GO
ALter TABLE [dbo].[NhapKhoDuocPhamChiTiet]
	Drop column [DonGiaBan]
Go
ALter TABLE [dbo].[NhapKhoDuocPhamChiTiet]
	Add [DonGiaBan] AS ([dbo].[LayDonGiaBanDuocPham]([NhapKhoDuocPhamId],[PhuongPhapTinhGiaTriTonKho],[DonGiaNhap],[VAT],[TiLeTheoThapGia]))
	
Go
CREATE FUNCTION [dbo].[LayDonGiaBanDonThuoc] (@XuatKhoDuocPhamChiTietViTriId bigint, @DonGiaNhap decimal(15, 2), @VAT int,@TiLeTheoThapGia int)  
RETURNS decimal(15, 2)
AS  
BEGIN 
	DECLARE @KhoId bigint;
	DECLARE @PhuongPhapTinhGiaTriTonKho int;
	DECLARE @GiaTonKho decimal(15, 6);
	DECLARE @GiaBanNhaThuocChuaVat decimal(15, 6);
	DECLARE @DonGiaBan decimal(15, 2);
	   
	SELECT @PhuongPhapTinhGiaTriTonKho = B.PhuongPhapTinhGiaTriTonKho
    FROM [XuatKhoDuocPhamChiTietViTri] A
	INNER JOIN NhapKhoDuocPhamChiTiet B ON A.NhapKhoDuocPhamChiTietId = B.Id
    WHERE A.Id = @XuatKhoDuocPhamChiTietViTriId;

	SET @GiaTonKho = @DonGiaNhap + (@DonGiaNhap * case when @PhuongPhapTinhGiaTriTonKho = (1) then @VAT else (0) end)/(100)

	SET @GiaBanNhaThuocChuaVat = @GiaTonKho + (@GiaTonKho * @TiLeTheoThapGia / 100)

	SET @DonGiaBan = round(@GiaBanNhaThuocChuaVat + (@GiaBanNhaThuocChuaVat * @VAT / 100),2);					
	
	RETURN @DonGiaBan;
END;
GO
ALter TABLE [dbo].[DonThuocThanhToanChiTiet]
	Drop column [DonGiaBan]
Go
ALter TABLE [dbo].[DonThuocThanhToanChiTiet]
	Add [DonGiaBan] AS ([dbo].[LayDonGiaBanDonThuoc]([XuatKhoDuocPhamChiTietViTriId],[DonGiaNhap],[VAT],[TiLeTheoThapGia]))
Go
CREATE FUNCTION [dbo].[LayGiaBanDonThuoc] (@XuatKhoDuocPhamChiTietViTriId bigint, @DonGiaNhap decimal(15, 2), @VAT int,@TiLeTheoThapGia int, @SoLuong float)  
RETURNS decimal(15, 4)
AS  
BEGIN 
	DECLARE @KhoId bigint;
	DECLARE @PhuongPhapTinhGiaTriTonKho int;
	DECLARE @GiaTonKho decimal(15, 6);
	DECLARE @GiaBanNhaThuocChuaVat decimal(15, 6);
	DECLARE @DonGiaBan decimal(15, 2);
	DECLARE @GiaBan decimal(15, 2);
	   
	SELECT @PhuongPhapTinhGiaTriTonKho = B.PhuongPhapTinhGiaTriTonKho
    FROM [XuatKhoDuocPhamChiTietViTri] A
	INNER JOIN NhapKhoDuocPhamChiTiet B ON A.NhapKhoDuocPhamChiTietId = B.Id
    WHERE A.Id = @XuatKhoDuocPhamChiTietViTriId;

	SET @GiaTonKho = @DonGiaNhap + (@DonGiaNhap * case when @PhuongPhapTinhGiaTriTonKho = (1) then @VAT else (0) end)/(100)

	SET @GiaBanNhaThuocChuaVat = @GiaTonKho + (@GiaTonKho * @TiLeTheoThapGia / 100)

	SET @DonGiaBan = round(@GiaBanNhaThuocChuaVat + (@GiaBanNhaThuocChuaVat * @VAT / 100),2);					
	SET @GiaBan = @DonGiaBan * CONVERT([decimal](9,2),@SoLuong);
	RETURN @GiaBan;
END;
Go
ALter TABLE [dbo].[DonThuocThanhToanChiTiet]
	Drop column [GiaBan]
Go
ALter TABLE [dbo].[DonThuocThanhToanChiTiet]
	Add [GiaBan] AS ([dbo].[LayGiaBanDonThuoc]([XuatKhoDuocPhamChiTietViTriId],[DonGiaNhap],[VAT],[TiLeTheoThapGia],[SoLuong]))	
GO
CREATE FUNCTION [dbo].[LayDonGiaBanYeuCauDuocPham] (@XuatKhoDuocPhamChiTietId bigint, @DonGiaNhap decimal(15, 2), @VAT int,@TiLeTheoThapGia int)  
RETURNS decimal(15, 2)
AS  
BEGIN 
	DECLARE @KhoId bigint;
	DECLARE @PhuongPhapTinhGiaTriTonKho int;
	DECLARE @GiaTonKho decimal(15, 6);
	DECLARE @GiaBanNhaThuocChuaVat decimal(15, 6);
	DECLARE @DonGiaBan decimal(15, 2);
	   
	IF @XuatKhoDuocPhamChiTietId is null
	BEGIN
		Set @PhuongPhapTinhGiaTriTonKho = 1
	END
	ELSE
	BEGIN
		SELECT TOP 1 @PhuongPhapTinhGiaTriTonKho = B.PhuongPhapTinhGiaTriTonKho
		FROM [XuatKhoDuocPhamChiTietViTri] A
		INNER JOIN NhapKhoDuocPhamChiTiet B ON A.NhapKhoDuocPhamChiTietId = B.Id
		WHERE A.XuatKhoDuocPhamChiTietId = @XuatKhoDuocPhamChiTietId
	END

	IF @PhuongPhapTinhGiaTriTonKho is null
	BEGIN
		Set @PhuongPhapTinhGiaTriTonKho = 1
	END

	SET @GiaTonKho = @DonGiaNhap + (@DonGiaNhap * case when @PhuongPhapTinhGiaTriTonKho = (1) then @VAT else (0) end)/(100)
	
	SET @DonGiaBan = round(@GiaTonKho + (@GiaTonKho * @TiLeTheoThapGia / 100),2);					
	
	RETURN @DonGiaBan;
END;
GO
ALter TABLE [dbo].[YeuCauDuocPhamBenhVien]
	Drop column [DonGiaBan]
Go
ALter TABLE [dbo].[YeuCauDuocPhamBenhVien]
	Add [DonGiaBan] AS ([dbo].[LayDonGiaBanYeuCauDuocPham]([XuatKhoDuocPhamChiTietId],[DonGiaNhap],[VAT],[TiLeTheoThapGia]))

Go

CREATE FUNCTION [dbo].[LayGiaBanYeuCauDuocPham] (@XuatKhoDuocPhamChiTietId bigint, @DonGiaNhap decimal(15, 2), @VAT int,@TiLeTheoThapGia int, @SoLuong float)  
RETURNS decimal(15, 4)
AS  
BEGIN 
	DECLARE @KhoId bigint;
	DECLARE @PhuongPhapTinhGiaTriTonKho int;
	DECLARE @GiaTonKho decimal(15, 6);
	DECLARE @GiaBanNhaThuocChuaVat decimal(15, 6);
	DECLARE @DonGiaBan decimal(15, 2);
	DECLARE @GiaBan decimal(15, 2);
	   
	IF @XuatKhoDuocPhamChiTietId is null
	BEGIN
		Set @PhuongPhapTinhGiaTriTonKho = 1
	END
	ELSE
	BEGIN
		SELECT TOP 1 @PhuongPhapTinhGiaTriTonKho = B.PhuongPhapTinhGiaTriTonKho
		FROM [XuatKhoDuocPhamChiTietViTri] A
		INNER JOIN NhapKhoDuocPhamChiTiet B ON A.NhapKhoDuocPhamChiTietId = B.Id
		WHERE A.XuatKhoDuocPhamChiTietId = @XuatKhoDuocPhamChiTietId
	END

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
ALter TABLE [dbo].[YeuCauDuocPhamBenhVien]
	Drop column [GiaBan]
Go
ALter TABLE [dbo].[YeuCauDuocPhamBenhVien]
	Add [GiaBan] AS ([dbo].[LayGiaBanYeuCauDuocPham]([XuatKhoDuocPhamChiTietId],[DonGiaNhap],[VAT],[TiLeTheoThapGia],[SoLuong]))

Go
Update dbo.CauHinh
Set [Value] = '2.7.7' where [Name] = 'CauHinhHeThong.DatabaseVesion'
