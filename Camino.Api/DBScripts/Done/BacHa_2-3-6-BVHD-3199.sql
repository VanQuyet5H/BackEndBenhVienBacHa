ALTER TABLE [NhapKhoDuocPhamChiTiet]
ADD [PhuongPhapTinhGiaTriTonKho] [int] NULL;
GO
UPDATE A
SET A.[PhuongPhapTinhGiaTriTonKho] = 2
FROM [NhapKhoDuocPhamChiTiet] A
INNER JOIN [NhapKhoDuocPham] B ON A.NhapKhoDuocPhamId = B.Id
WHERE B.KhoId = 6 --KhoNhaThuoc

UPDATE A
SET A.[PhuongPhapTinhGiaTriTonKho] = 1
FROM [NhapKhoDuocPhamChiTiet] A
INNER JOIN [NhapKhoDuocPham] B ON A.NhapKhoDuocPhamId = B.Id
WHERE B.KhoId != 6 --KhoNhaThuoc

ALTER TABLE [NhapKhoDuocPhamChiTiet]
ALTER COLUMN [PhuongPhapTinhGiaTriTonKho] [int] NOT NULL;

ALTER TABLE [NhapKhoDuocPhamChiTiet]
ADD [DonGiaTonKho] AS (round(([DonGiaNhap]+([DonGiaNhap]*(CASE WHEN [PhuongPhapTinhGiaTriTonKho] = 1 THEN [VAT] ELSE 0 END))/(100)),(2)))
GO

ALTER TABLE [NhapKhoVatTuChiTiet]
ADD [PhuongPhapTinhGiaTriTonKho] [int] NULL;
GO
UPDATE A
SET A.[PhuongPhapTinhGiaTriTonKho] = 2
FROM [NhapKhoVatTuChiTiet] A
INNER JOIN [NhapKhoVatTu] B ON A.NhapKhoVatTuId = B.Id
WHERE B.KhoId = 6 --KhoNhaThuoc

UPDATE A
SET A.[PhuongPhapTinhGiaTriTonKho] = 1
FROM [NhapKhoVatTuChiTiet] A
INNER JOIN [NhapKhoVatTu] B ON A.NhapKhoVatTuId = B.Id
WHERE B.KhoId != 6 --KhoNhaThuoc

ALTER TABLE [NhapKhoVatTuChiTiet]
ALTER COLUMN [PhuongPhapTinhGiaTriTonKho] [int] NOT NULL;

ALTER TABLE [NhapKhoVatTuChiTiet]
ADD [DonGiaTonKho] AS (round(([DonGiaNhap]+([DonGiaNhap]*(CASE WHEN [PhuongPhapTinhGiaTriTonKho] = 1 THEN [VAT] ELSE 0 END))/(100)),(2)))
GO
CREATE TABLE [dbo].[YeuCauDieuChuyenDuocPham](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[KhoXuatId] [bigint] NOT NULL,
	[KhoNhapId] [bigint] NOT NULL,
	[NguoiXuatId] [bigint] NOT NULL,
	[NguoiNhapId] [bigint] NOT NULL,
	[DuocKeToanDuyet] [bit] NULL,
	[NgayDuyet] [datetime] NULL,
	[NhanVienDuyetId] [bigint] NULL,
	[LyDoKhongDuyet] [nvarchar](4000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__YeuCauDieuChuyenDuocPham] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[YeuCauDieuChuyenDuocPham]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDieuChuyenDuocPham_Kho] FOREIGN KEY([KhoXuatId])
REFERENCES [dbo].[Kho] ([Id])
GO

ALTER TABLE [dbo].[YeuCauDieuChuyenDuocPham] CHECK CONSTRAINT [FK_YeuCauDieuChuyenDuocPham_Kho]
GO

ALTER TABLE [dbo].[YeuCauDieuChuyenDuocPham]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDieuChuyenDuocPham_Kho1] FOREIGN KEY([KhoNhapId])
REFERENCES [dbo].[Kho] ([Id])
GO

ALTER TABLE [dbo].[YeuCauDieuChuyenDuocPham] CHECK CONSTRAINT [FK_YeuCauDieuChuyenDuocPham_Kho1]
GO

ALTER TABLE [dbo].[YeuCauDieuChuyenDuocPham]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDieuChuyenDuocPham_NhanVien] FOREIGN KEY([NguoiXuatId])
REFERENCES [dbo].[NhanVien] ([Id])
GO

ALTER TABLE [dbo].[YeuCauDieuChuyenDuocPham] CHECK CONSTRAINT [FK_YeuCauDieuChuyenDuocPham_NhanVien]
GO

ALTER TABLE [dbo].[YeuCauDieuChuyenDuocPham]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDieuChuyenDuocPham_NhanVien1] FOREIGN KEY([NguoiNhapId])
REFERENCES [dbo].[NhanVien] ([Id])
GO

ALTER TABLE [dbo].[YeuCauDieuChuyenDuocPham] CHECK CONSTRAINT [FK_YeuCauDieuChuyenDuocPham_NhanVien1]
GO

ALTER TABLE [dbo].[YeuCauDieuChuyenDuocPham]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDieuChuyenDuocPham_NhanVien2] FOREIGN KEY([NhanVienDuyetId])
REFERENCES [dbo].[NhanVien] ([Id])
GO

ALTER TABLE [dbo].[YeuCauDieuChuyenDuocPham] CHECK CONSTRAINT [FK_YeuCauDieuChuyenDuocPham_NhanVien2]
GO
CREATE TABLE [dbo].[YeuCauDieuChuyenDuocPhamChiTiet](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[YeuCauDieuChuyenDuocPhamId] [bigint] NOT NULL,
	[XuatKhoDuocPhamChiTietViTriId] [bigint] NULL,
	[SoLuongDieuChuyen] [float] NOT NULL,
	[DuocPhamBenhVienId] [bigint] NOT NULL,
	[HopDongThauDuocPhamId] [bigint] NOT NULL,
	[LaDuocPhamBHYT] [bit] NOT NULL,
	[Solo] [nvarchar](50) NOT NULL,
	[HanSuDung] [datetime] NOT NULL,
	[NgayNhapVaoBenhVien] [datetime] NOT NULL,
	[DonGiaNhap] [decimal](15, 2) NOT NULL,
	[TiLeTheoThapGia] [int] NOT NULL,
	[VAT] [int] NOT NULL,
	[PhuongPhapTinhGiaTriTonKho] [int] NOT NULL,
	[MaVach] [nvarchar](100) NULL,
	[TiLeBHYTThanhToan] [int] NULL,
	[MaRef] [nvarchar](200) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_YeuCauDieuChuyenDuocPhamChiTiet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[YeuCauDieuChuyenDuocPhamChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDieuChuyenDuocPhamChiTiet_DuocPhamBenhVien] FOREIGN KEY([DuocPhamBenhVienId])
REFERENCES [dbo].[DuocPhamBenhVien] ([Id])
GO

ALTER TABLE [dbo].[YeuCauDieuChuyenDuocPhamChiTiet] CHECK CONSTRAINT [FK_YeuCauDieuChuyenDuocPhamChiTiet_DuocPhamBenhVien]
GO

ALTER TABLE [dbo].[YeuCauDieuChuyenDuocPhamChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDieuChuyenDuocPhamChiTiet_HopDongThauDuocPham] FOREIGN KEY([HopDongThauDuocPhamId])
REFERENCES [dbo].[HopDongThauDuocPham] ([Id])
GO

ALTER TABLE [dbo].[YeuCauDieuChuyenDuocPhamChiTiet] CHECK CONSTRAINT [FK_YeuCauDieuChuyenDuocPhamChiTiet_HopDongThauDuocPham]
GO

ALTER TABLE [dbo].[YeuCauDieuChuyenDuocPhamChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDieuChuyenDuocPhamChiTiet_XuatKhoDuocPhamChiTietViTri] FOREIGN KEY([XuatKhoDuocPhamChiTietViTriId])
REFERENCES [dbo].[XuatKhoDuocPhamChiTietViTri] ([Id])
GO

ALTER TABLE [dbo].[YeuCauDieuChuyenDuocPhamChiTiet] CHECK CONSTRAINT [FK_YeuCauDieuChuyenDuocPhamChiTiet_XuatKhoDuocPhamChiTietViTri]
GO

ALTER TABLE [dbo].[YeuCauDieuChuyenDuocPhamChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDieuChuyenDuocPhamChiTiet_YeuCauDieuChuyenDuocPham] FOREIGN KEY([YeuCauDieuChuyenDuocPhamId])
REFERENCES [dbo].[YeuCauDieuChuyenDuocPham] ([Id])
GO

ALTER TABLE [dbo].[YeuCauDieuChuyenDuocPhamChiTiet] CHECK CONSTRAINT [FK_YeuCauDieuChuyenDuocPhamChiTiet_YeuCauDieuChuyenDuocPham]
GO


ALTER TABLE [YeuCauDieuChuyenDuocPhamChiTiet]
ADD [DonGiaTonKho] AS (round(([DonGiaNhap]+([DonGiaNhap]*(CASE WHEN [PhuongPhapTinhGiaTriTonKho] = 1 THEN [VAT] ELSE 0 END))/(100)),(2)))
GO
Update dbo.CauHinh
Set [Value] = '2.3.6' where [Name] = 'CauHinhHeThong.DatabaseVesion'