ALTER TABLE [dbo].[DichVuKyThuatBenhVien]
	Add  [KhoaPhongTinhDoanhThuId] [bigint] NULL
GO

ALTER TABLE [dbo].[DichVuKyThuatBenhVien]  WITH CHECK ADD  CONSTRAINT [FK__DichVuKyThuatBenhVien__KhoaPhongTinhDoanhThu] FOREIGN KEY([KhoaPhongTinhDoanhThuId])
REFERENCES [dbo].[KhoaPhong] ([Id])
GO

CREATE TABLE [dbo].[NoiGioiThieuHopDong](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[NoiGioiThieuId] [bigint] NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[NgayBatDau] [datetime] NOT NULL,
	[NgayKetThuc] [datetime] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[NoiGioiThieuHopDongChiTietHeSoDichVuGiuong](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[NoiGioiThieuHopDongId] [bigint] NOT NULL,
	[DichVuGiuongBenhVienId] [bigint] NOT NULL,
	[NhomGiaDichVuGiuongBenhVienId] [bigint] NOT NULL,
	[DonGiaBenhVien] [decimal](15, 2) NOT NULL,
	[DonGiaGioiThieuTuLan1] [decimal](15, 2) NOT NULL,
	[HeSoGioiThieuTuLan1] [decimal](15, 2) NOT NULL,
	[DonGiaGioiThieuTuLan2] [decimal](15, 2) NULL,
	[HeSoGioiThieuTuLan2] [decimal](15, 2) NULL,
	[DonGiaGioiThieuTuLan3] [decimal](15, 2) NULL,
	[HeSoGioiThieuTuLan3] [decimal](15, 2) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[NoiGioiThieuHopDongId] [bigint] NOT NULL,
	[DichVuKhamBenhBenhVienId] [bigint] NOT NULL,
	[NhomGiaDichVuKhamBenhBenhVienId] [bigint] NOT NULL,
	[DonGiaBenhVien] [decimal](15, 2) NOT NULL,
	[DonGiaGioiThieuTuLan1] [decimal](15, 2) NOT NULL,
	[HeSoGioiThieuTuLan1] [decimal](15, 2) NOT NULL,
	[DonGiaGioiThieuTuLan2] [decimal](15, 2) NULL,
	[HeSoGioiThieuTuLan2] [decimal](15, 2) NULL,
	[DonGiaGioiThieuTuLan3] [decimal](15, 2) NULL,
	[HeSoGioiThieuTuLan3] [decimal](15, 2) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[NoiGioiThieuHopDongChiTietHeSoDichVuKyThuat](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[NoiGioiThieuHopDongId] [bigint] NOT NULL,
	[DichVuKyThuatBenhVienId] [bigint] NOT NULL,
	[NhomGiaDichVuKyThuatBenhVienId] [bigint] NOT NULL,
	[DonGiaBenhVien] [decimal](15, 2) NOT NULL,
	[DonGiaGioiThieuTuLan1] [decimal](15, 2) NOT NULL,
	[HeSoGioiThieuTuLan1] [decimal](15, 2) NOT NULL,
	[DonGiaGioiThieuTuLan2] [decimal](15, 2) NULL,
	[HeSoGioiThieuTuLan2] [decimal](15, 2) NULL,
	[DonGiaGioiThieuTuLan3] [decimal](15, 2) NULL,
	[HeSoGioiThieuTuLan3] [decimal](15, 2) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[NoiGioiThieuHopDongChiTietHeSoDuocPham](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[NoiGioiThieuHopDongId] [bigint] NOT NULL,
	[DuocPhamBenhVienId] [bigint] NOT NULL,
	[LoaiGia] [int] NOT NULL,
	[HeSo] [decimal](15, 2) NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[NoiGioiThieuHopDongChiTietHeSoVatTu](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[NoiGioiThieuHopDongId] [bigint] NOT NULL,
	[VatTuBenhVienId] [bigint] NOT NULL,
	[LoaiGia] [int] NOT NULL,
	[HeSo] [decimal](15, 2) NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[NoiGioiThieuHopDongChiTietHoaHongDichVuGiuong](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[NoiGioiThieuHopDongId] [bigint] NOT NULL,
	[DichVuGiuongBenhVienId] [bigint] NOT NULL,
	[NhomGiaDichVuGiuongBenhVienId] [bigint] NOT NULL,
	[DonGiaBenhVien] [decimal](15, 2) NOT NULL,
	[LoaiHoaHong] [int] NOT NULL,
	[SoTienHoaHong] [decimal](15, 2) NULL,
	[TiLeHoaHong] [decimal](15, 2) NULL,
	[ApDungTuLan] [int] NOT NULL,
	[ApDungDenLan] [int] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[NoiGioiThieuHopDongId] [bigint] NOT NULL,
	[DichVuKhamBenhBenhVienId] [bigint] NOT NULL,
	[NhomGiaDichVuKhamBenhBenhVienId] [bigint] NOT NULL,
	[DonGiaBenhVien] [decimal](15, 2) NOT NULL,
	[LoaiHoaHong] [int] NOT NULL,
	[SoTienHoaHong] [decimal](15, 2) NULL,
	[TiLeHoaHong] [decimal](15, 2) NULL,
	[ApDungTuLan] [int] NOT NULL,
	[ApDungDenLan] [int] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[NoiGioiThieuHopDongId] [bigint] NOT NULL,
	[DichVuKyThuatBenhVienId] [bigint] NOT NULL,
	[NhomGiaDichVuKyThuatBenhVienId] [bigint] NOT NULL,
	[DonGiaBenhVien] [decimal](15, 2) NOT NULL,
	[LoaiHoaHong] [int] NOT NULL,
	[SoTienHoaHong] [decimal](15, 2) NULL,
	[TiLeHoaHong] [decimal](15, 2) NULL,
	[ApDungTuLan] [int] NOT NULL,
	[ApDungDenLan] [int] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[NoiGioiThieuHopDongChiTietHoaHongDuocPham](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[NoiGioiThieuHopDongId] [bigint] NOT NULL,
	[DuocPhamBenhVienId] [bigint] NOT NULL,
	[TiLeHoaHong] [decimal](15, 2) NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[NoiGioiThieuHopDongChiTietHoaHongVatTu](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[NoiGioiThieuHopDongId] [bigint] NOT NULL,
	[VatTuBenhVienId] [bigint] NOT NULL,
	[TiLeHoaHong] [decimal](15, 2) NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDong]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuHopDong_NoiGioiThieu] FOREIGN KEY([NoiGioiThieuId])
REFERENCES [dbo].[NoiGioiThieu] ([Id])
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDong] CHECK CONSTRAINT [FK_NoiGioiThieuHopDong_NoiGioiThieu]
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHeSoDichVuGiuong]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuHopDongChiTietDichVuGiuong_DichVuGiuongBenhVien] FOREIGN KEY([DichVuGiuongBenhVienId])
REFERENCES [dbo].[DichVuGiuongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHeSoDichVuGiuong] CHECK CONSTRAINT [FK_NoiGioiThieuHopDongChiTietDichVuGiuong_DichVuGiuongBenhVien]
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHeSoDichVuGiuong]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuHopDongChiTietDichVuGiuong_NhomGiaDichVuGiuongBenhVien] FOREIGN KEY([NhomGiaDichVuGiuongBenhVienId])
REFERENCES [dbo].[NhomGiaDichVuGiuongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHeSoDichVuGiuong] CHECK CONSTRAINT [FK_NoiGioiThieuHopDongChiTietDichVuGiuong_NhomGiaDichVuGiuongBenhVien]
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHeSoDichVuGiuong]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuHopDongChiTietDichVuGiuong_NoiGioiThieuHopDong] FOREIGN KEY([NoiGioiThieuHopDongId])
REFERENCES [dbo].[NoiGioiThieuHopDong] ([Id])
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHeSoDichVuGiuong] CHECK CONSTRAINT [FK_NoiGioiThieuHopDongChiTietDichVuGiuong_NoiGioiThieuHopDong]
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuHopDongChiTietDichVuKhamBenh_DichVuKhamBenhBenhVien] FOREIGN KEY([DichVuKhamBenhBenhVienId])
REFERENCES [dbo].[DichVuKhamBenhBenhVien] ([Id])
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh] CHECK CONSTRAINT [FK_NoiGioiThieuHopDongChiTietDichVuKhamBenh_DichVuKhamBenhBenhVien]
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuHopDongChiTietDichVuKhamBenh_NhomGiaDichVuKhamBenhBenhVien] FOREIGN KEY([NhomGiaDichVuKhamBenhBenhVienId])
REFERENCES [dbo].[NhomGiaDichVuKhamBenhBenhVien] ([Id])
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh] CHECK CONSTRAINT [FK_NoiGioiThieuHopDongChiTietDichVuKhamBenh_NhomGiaDichVuKhamBenhBenhVien]
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuHopDongChiTietDichVuKhamBenh_NoiGioiThieuHopDong] FOREIGN KEY([NoiGioiThieuHopDongId])
REFERENCES [dbo].[NoiGioiThieuHopDong] ([Id])
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh] CHECK CONSTRAINT [FK_NoiGioiThieuHopDongChiTietDichVuKhamBenh_NoiGioiThieuHopDong]
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHeSoDichVuKyThuat]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuHopDongChiTietDichVuKyThuat_DichVuKyThuatBenhVien] FOREIGN KEY([DichVuKyThuatBenhVienId])
REFERENCES [dbo].[DichVuKyThuatBenhVien] ([Id])
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHeSoDichVuKyThuat] CHECK CONSTRAINT [FK_NoiGioiThieuHopDongChiTietDichVuKyThuat_DichVuKyThuatBenhVien]
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHeSoDichVuKyThuat]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuHopDongChiTietDichVuKyThuat_NhomGiaDichVuKyThuatBenhVien] FOREIGN KEY([NhomGiaDichVuKyThuatBenhVienId])
REFERENCES [dbo].[NhomGiaDichVuKyThuatBenhVien] ([Id])
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHeSoDichVuKyThuat] CHECK CONSTRAINT [FK_NoiGioiThieuHopDongChiTietDichVuKyThuat_NhomGiaDichVuKyThuatBenhVien]
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHeSoDichVuKyThuat]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuHopDongChiTietDichVuKyThuat_NoiGioiThieuHopDong] FOREIGN KEY([NoiGioiThieuHopDongId])
REFERENCES [dbo].[NoiGioiThieuHopDong] ([Id])
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHeSoDichVuKyThuat] CHECK CONSTRAINT [FK_NoiGioiThieuHopDongChiTietDichVuKyThuat_NoiGioiThieuHopDong]
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHeSoDuocPham]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuHopDongChiTietDuocPham_DuocPhamBenhVien] FOREIGN KEY([DuocPhamBenhVienId])
REFERENCES [dbo].[DuocPhamBenhVien] ([Id])
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHeSoDuocPham] CHECK CONSTRAINT [FK_NoiGioiThieuHopDongChiTietDuocPham_DuocPhamBenhVien]
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHeSoDuocPham]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuHopDongChiTietDuocPham_NoiGioiThieuHopDong] FOREIGN KEY([NoiGioiThieuHopDongId])
REFERENCES [dbo].[NoiGioiThieuHopDong] ([Id])
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHeSoDuocPham] CHECK CONSTRAINT [FK_NoiGioiThieuHopDongChiTietDuocPham_NoiGioiThieuHopDong]
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHeSoVatTu]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuHopDongChiTietVatTu_NoiGioiThieuHopDong] FOREIGN KEY([NoiGioiThieuHopDongId])
REFERENCES [dbo].[NoiGioiThieuHopDong] ([Id])
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHeSoVatTu] CHECK CONSTRAINT [FK_NoiGioiThieuHopDongChiTietVatTu_NoiGioiThieuHopDong]
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHeSoVatTu]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuHopDongChiTietVatTu_VatTuBenhVien] FOREIGN KEY([VatTuBenhVienId])
REFERENCES [dbo].[VatTuBenhVien] ([Id])
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHeSoVatTu] CHECK CONSTRAINT [FK_NoiGioiThieuHopDongChiTietVatTu_VatTuBenhVien]
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHoaHongDichVuGiuong]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuHopDongChiTietHoaHongDichVuGiuong_DichVuGiuongBenhVien] FOREIGN KEY([DichVuGiuongBenhVienId])
REFERENCES [dbo].[DichVuGiuongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHoaHongDichVuGiuong] CHECK CONSTRAINT [FK_NoiGioiThieuHopDongChiTietHoaHongDichVuGiuong_DichVuGiuongBenhVien]
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHoaHongDichVuGiuong]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuHopDongChiTietHoaHongDichVuGiuong_NhomGiaDichVuGiuongBenhVien] FOREIGN KEY([NhomGiaDichVuGiuongBenhVienId])
REFERENCES [dbo].[NhomGiaDichVuGiuongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHoaHongDichVuGiuong] CHECK CONSTRAINT [FK_NoiGioiThieuHopDongChiTietHoaHongDichVuGiuong_NhomGiaDichVuGiuongBenhVien]
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHoaHongDichVuGiuong]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuHopDongChiTietHoaHongDichVuGiuong_NoiGioiThieuHopDong] FOREIGN KEY([NoiGioiThieuHopDongId])
REFERENCES [dbo].[NoiGioiThieuHopDong] ([Id])
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHoaHongDichVuGiuong] CHECK CONSTRAINT [FK_NoiGioiThieuHopDongChiTietHoaHongDichVuGiuong_NoiGioiThieuHopDong]
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh_DichVuKhamBenhBenhVien] FOREIGN KEY([DichVuKhamBenhBenhVienId])
REFERENCES [dbo].[DichVuKhamBenhBenhVien] ([Id])
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh] CHECK CONSTRAINT [FK_NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh_DichVuKhamBenhBenhVien]
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh_NhomGiaDichVuKhamBenhBenhVien] FOREIGN KEY([NhomGiaDichVuKhamBenhBenhVienId])
REFERENCES [dbo].[NhomGiaDichVuKhamBenhBenhVien] ([Id])
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh] CHECK CONSTRAINT [FK_NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh_NhomGiaDichVuKhamBenhBenhVien]
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh_NoiGioiThieuHopDong] FOREIGN KEY([NoiGioiThieuHopDongId])
REFERENCES [dbo].[NoiGioiThieuHopDong] ([Id])
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh] CHECK CONSTRAINT [FK_NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh_NoiGioiThieuHopDong]
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat_DichVuKyThuatBenhVien] FOREIGN KEY([DichVuKyThuatBenhVienId])
REFERENCES [dbo].[DichVuKyThuatBenhVien] ([Id])
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat] CHECK CONSTRAINT [FK_NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat_DichVuKyThuatBenhVien]
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat_NhomGiaDichVuKyThuatBenhVien] FOREIGN KEY([NhomGiaDichVuKyThuatBenhVienId])
REFERENCES [dbo].[NhomGiaDichVuKyThuatBenhVien] ([Id])
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat] CHECK CONSTRAINT [FK_NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat_NhomGiaDichVuKyThuatBenhVien]
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat_NoiGioiThieuHopDong] FOREIGN KEY([NoiGioiThieuHopDongId])
REFERENCES [dbo].[NoiGioiThieuHopDong] ([Id])
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat] CHECK CONSTRAINT [FK_NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat_NoiGioiThieuHopDong]
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHoaHongDuocPham]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuHopDongChiTietHoaHongDuocPham_DuocPhamBenhVien] FOREIGN KEY([DuocPhamBenhVienId])
REFERENCES [dbo].[DuocPhamBenhVien] ([Id])
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHoaHongDuocPham] CHECK CONSTRAINT [FK_NoiGioiThieuHopDongChiTietHoaHongDuocPham_DuocPhamBenhVien]
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHoaHongDuocPham]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuHopDongChiTietHoaHongDuocPham_NoiGioiThieuHopDong] FOREIGN KEY([NoiGioiThieuHopDongId])
REFERENCES [dbo].[NoiGioiThieuHopDong] ([Id])
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHoaHongDuocPham] CHECK CONSTRAINT [FK_NoiGioiThieuHopDongChiTietHoaHongDuocPham_NoiGioiThieuHopDong]
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHoaHongVatTu]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuHopDongChiTietHoaHongVatTu_NoiGioiThieuHopDong] FOREIGN KEY([NoiGioiThieuHopDongId])
REFERENCES [dbo].[NoiGioiThieuHopDong] ([Id])
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHoaHongVatTu] CHECK CONSTRAINT [FK_NoiGioiThieuHopDongChiTietHoaHongVatTu_NoiGioiThieuHopDong]
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHoaHongVatTu]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuHopDongChiTietHoaHongVatTu_VatTuBenhVien] FOREIGN KEY([VatTuBenhVienId])
REFERENCES [dbo].[VatTuBenhVien] ([Id])
GO
ALTER TABLE [dbo].[NoiGioiThieuHopDongChiTietHoaHongVatTu] CHECK CONSTRAINT [FK_NoiGioiThieuHopDongChiTietHoaHongVatTu_VatTuBenhVien]
GO

UPDATE CauHinh
Set [Value] = '3.4.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'