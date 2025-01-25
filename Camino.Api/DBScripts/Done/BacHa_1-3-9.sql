CREATE TABLE [dbo].[NoiTruChiDinhDuocPham](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[YeuCauTiepNhanId] [bigint] NOT NULL,
	[DuocPhamBenhVienId] [bigint] NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[TenTiengAnh] [nvarchar](250) NULL,
	[SoDangKy] [nvarchar](100) NOT NULL,
	[STTHoatChat] [int] NULL,
	[MaHoatChat] [nvarchar](20) NOT NULL,
	[HoatChat] [nvarchar](550) NOT NULL,
	[LoaiThuocHoacHoatChat] [int] NOT NULL,
	[NhaSanXuat] [nvarchar](250) NULL,
	[NuocSanXuat] [nvarchar](250) NULL,
	[DuongDungId] [bigint] NOT NULL,
	[HamLuong] [nvarchar](500) NULL,
	[QuyCach] [nvarchar](250) NULL,
	[TieuChuan] [nvarchar](50) NULL,
	[DangBaoChe] [nvarchar](250) NULL,
	[DonViTinhId] [bigint] NOT NULL,
	[HuongDan] [nvarchar](4000) NULL,
	[MoTa] [nvarchar](4000) NULL,
	[ChiDinh] [nvarchar](4000) NULL,
	[ChongChiDinh] [nvarchar](4000) NULL,
	[LieuLuongCachDung] [nvarchar](4000) NULL,
	[TacDungPhu] [nvarchar](4000) NULL,
	[ChuYDePhong] [nvarchar](4000) NULL,
	[SoLuong] [float] NOT NULL,
	[NhanVienChiDinhId] [bigint] NOT NULL,
	[NoiChiDinhId] [bigint] NOT NULL,
	[ThoiDiemChiDinh] [datetime] NOT NULL,
	[NoiCapThuocId] [bigint] NULL,
	[NhanVienCapThuocId] [bigint] NULL,
	[ThoiDiemCapThuoc] [datetime] NULL,
	[DuocHuongBaoHiem] [bit] NOT NULL,
	[DaCapThuoc] [bit] NOT NULL,
	[TrangThai] [int] NOT NULL,
	[GhiChu] [nvarchar](1000) NULL,
	[LaDuocPhamBHYT] [bit] NOT NULL,
	[NoiTruPhieuDieuTriId] [bigint] NULL,
	[SoLanDungTrongNgay] [int] NULL,
	[DungSang] [float] NULL,
	[DungTrua] [float] NULL,
	[DungChieu] [float] NULL,
	[DungToi] [float] NULL,
	[ThoiGianDungSang] [int] NULL,
	[ThoiGianDungTrua] [int] NULL,
	[ThoiGianDungChieu] [int] NULL,
	[ThoiGianDungToi] [int] NULL,
	[LaDichTruyen] [bit] NULL,
	[TocDoTruyen] [int] NULL,
	[DonViTocDoTruyen] [int] NULL,
	[ThoiGianBatDauTruyen] [int] NULL,
	[CachGioTruyenDich] [float] NULL,
	[TheTich] [int] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__NoiTruChiDinhDuocPham] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[NoiTruChiDinhDuocPham]  WITH CHECK ADD  CONSTRAINT [FK_NoiTruChiDinhDuocPham_DonViTinh] FOREIGN KEY([DonViTinhId])
REFERENCES [dbo].[DonViTinh] ([Id])
GO

ALTER TABLE [dbo].[NoiTruChiDinhDuocPham] CHECK CONSTRAINT [FK_NoiTruChiDinhDuocPham_DonViTinh]
GO

ALTER TABLE [dbo].[NoiTruChiDinhDuocPham]  WITH CHECK ADD  CONSTRAINT [FK_NoiTruChiDinhDuocPham_DuocPhamBenhVien] FOREIGN KEY([DuocPhamBenhVienId])
REFERENCES [dbo].[DuocPhamBenhVien] ([Id])
GO

ALTER TABLE [dbo].[NoiTruChiDinhDuocPham] CHECK CONSTRAINT [FK_NoiTruChiDinhDuocPham_DuocPhamBenhVien]
GO

ALTER TABLE [dbo].[NoiTruChiDinhDuocPham]  WITH CHECK ADD  CONSTRAINT [FK_NoiTruChiDinhDuocPham_DuongDung] FOREIGN KEY([DuongDungId])
REFERENCES [dbo].[DuongDung] ([Id])
GO

ALTER TABLE [dbo].[NoiTruChiDinhDuocPham] CHECK CONSTRAINT [FK_NoiTruChiDinhDuocPham_DuongDung]
GO

ALTER TABLE [dbo].[NoiTruChiDinhDuocPham]  WITH CHECK ADD  CONSTRAINT [FK_NoiTruChiDinhDuocPham_NhanVien] FOREIGN KEY([NhanVienChiDinhId])
REFERENCES [dbo].[NhanVien] ([Id])
GO

ALTER TABLE [dbo].[NoiTruChiDinhDuocPham] CHECK CONSTRAINT [FK_NoiTruChiDinhDuocPham_NhanVien]
GO

ALTER TABLE [dbo].[NoiTruChiDinhDuocPham]  WITH CHECK ADD  CONSTRAINT [FK_NoiTruChiDinhDuocPham_NhanVien1] FOREIGN KEY([NhanVienCapThuocId])
REFERENCES [dbo].[NhanVien] ([Id])
GO

ALTER TABLE [dbo].[NoiTruChiDinhDuocPham] CHECK CONSTRAINT [FK_NoiTruChiDinhDuocPham_NhanVien1]
GO

ALTER TABLE [dbo].[NoiTruChiDinhDuocPham]  WITH CHECK ADD  CONSTRAINT [FK_NoiTruChiDinhDuocPham_NoiTruPhieuDieuTri] FOREIGN KEY([NoiTruPhieuDieuTriId])
REFERENCES [dbo].[NoiTruPhieuDieuTri] ([Id])
GO

ALTER TABLE [dbo].[NoiTruChiDinhDuocPham] CHECK CONSTRAINT [FK_NoiTruChiDinhDuocPham_NoiTruPhieuDieuTri]
GO

ALTER TABLE [dbo].[NoiTruChiDinhDuocPham]  WITH CHECK ADD  CONSTRAINT [FK_NoiTruChiDinhDuocPham_PhongBenhVien] FOREIGN KEY([NoiChiDinhId])
REFERENCES [dbo].[PhongBenhVien] ([Id])
GO

ALTER TABLE [dbo].[NoiTruChiDinhDuocPham] CHECK CONSTRAINT [FK_NoiTruChiDinhDuocPham_PhongBenhVien]
GO

ALTER TABLE [dbo].[NoiTruChiDinhDuocPham]  WITH CHECK ADD  CONSTRAINT [FK_NoiTruChiDinhDuocPham_PhongBenhVien1] FOREIGN KEY([NoiCapThuocId])
REFERENCES [dbo].[PhongBenhVien] ([Id])
GO

ALTER TABLE [dbo].[NoiTruChiDinhDuocPham] CHECK CONSTRAINT [FK_NoiTruChiDinhDuocPham_PhongBenhVien1]
GO

ALTER TABLE [dbo].[NoiTruChiDinhDuocPham]  WITH CHECK ADD  CONSTRAINT [FK_NoiTruChiDinhDuocPham_YeuCauTiepNhan] FOREIGN KEY([YeuCauTiepNhanId])
REFERENCES [dbo].[YeuCauTiepNhan] ([Id])
GO

ALTER TABLE [dbo].[NoiTruChiDinhDuocPham] CHECK CONSTRAINT [FK_NoiTruChiDinhDuocPham_YeuCauTiepNhan]
GO

ALTER TABLE [dbo].[YeuCauDuocPhamBenhVien]
	ADD [NoiTruChiDinhDuocPhamId] [bigint] NULL
GO

ALTER TABLE [dbo].[YeuCauDuocPhamBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDuocPhamBenhVien_NoiTruChiDinhDuocPham] FOREIGN KEY([NoiTruChiDinhDuocPhamId])
REFERENCES [dbo].[NoiTruChiDinhDuocPham] ([Id])
GO

ALTER TABLE [dbo].[YeuCauDuocPhamBenhVien] CHECK CONSTRAINT [FK_YeuCauDuocPhamBenhVien_NoiTruChiDinhDuocPham]
GO

Update dbo.CauHinh
Set [Value] = '1.3.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'