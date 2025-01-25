CREATE TABLE [dbo].[NgayLeTet](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](200) NOT NULL,
	[Ngay] [int] NOT NULL,
	[Thang] [int] NOT NULL,
	[Nam] [int] NULL,
	[LeHangNam] [bit] NOT NULL,
	[GhiChu] [nvarchar](1000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_NgayLeTet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[LoaiThuePhongPhauThuat](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](200) NOT NULL,	
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_LoaiThuePhongPhauThuat] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[LoaiThuePhongNoiThucHien](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](200) NOT NULL,	
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_LoaiThuePhongNoiThucHien] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[CauHinhThuePhong](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](200) NOT NULL,
	[LoaiThuePhongPhauThuatId] [bigint] NOT NULL,
	[LoaiThuePhongNoiThucHienId] [bigint] NOT NULL,
	[BlockThoiGianTheoPhut] [int] NOT NULL,
	[GiaThue] [decimal](15, 2) NOT NULL,
	[PhanTramNgoaiGio] [int] NOT NULL,
	[PhanTramLeTet] [int] NOT NULL,
	[GiaThuePhatSinh] [decimal](15, 2) NOT NULL,
	[PhanTramPhatSinhNgoaiGio] [int] NOT NULL,
	[PhanTramPhatSinhLeTet] [int] NOT NULL,
	[HieuLuc] [bit] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_CauHinhThuePhong] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CauHinhThuePhong]  WITH CHECK ADD  CONSTRAINT [FK_CauHinhThuePhong_LoaiThuePhongNoiThucHien] FOREIGN KEY([LoaiThuePhongNoiThucHienId])
REFERENCES [dbo].[LoaiThuePhongNoiThucHien] ([Id])
GO

ALTER TABLE [dbo].[CauHinhThuePhong] CHECK CONSTRAINT [FK_CauHinhThuePhong_LoaiThuePhongNoiThucHien]
GO

ALTER TABLE [dbo].[CauHinhThuePhong]  WITH CHECK ADD  CONSTRAINT [FK_CauHinhThuePhong_LoaiThuePhongPhauThuat] FOREIGN KEY([LoaiThuePhongPhauThuatId])
REFERENCES [dbo].[LoaiThuePhongPhauThuat] ([Id])
GO

ALTER TABLE [dbo].[CauHinhThuePhong] CHECK CONSTRAINT [FK_CauHinhThuePhong_LoaiThuePhongPhauThuat]
GO

CREATE TABLE [dbo].[ThuePhong](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[YeuCauTiepNhanId] [bigint] NOT NULL,
	[YeuCauDichVuKyThuatThuePhongId] [bigint] NOT NULL,
	[CauHinhThuePhongId] [bigint] NOT NULL,
	[TenCauHinhThuePhong] [nvarchar](200) NOT NULL,
	[LoaiThuePhongPhauThuatId] [bigint] NOT NULL,
	[LoaiThuePhongNoiThucHienId] [bigint] NOT NULL,
	[BlockThoiGianTheoPhut] [int] NOT NULL,
	[GiaThue] [decimal](15, 2) NOT NULL,
	[PhanTramNgoaiGio] [int] NOT NULL,
	[PhanTramLeTet] [int] NOT NULL,
	[GiaThuePhatSinh] [decimal](15, 2) NOT NULL,
	[PhanTramPhatSinhNgoaiGio] [int] NOT NULL,
	[PhanTramPhatSinhLeTet] [int] NOT NULL,
	[ThoiDiemBatDau] [datetime] NOT NULL,
	[ThoiDiemKetThuc] [datetime] NOT NULL,
	[NhanVienChiDinhId] [bigint] NOT NULL,
	[NoiChiDinhId] [bigint] NOT NULL,
	[YeuCauDichVuKyThuatTinhChiPhiId] [bigint] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_ThuePhong] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ThuePhong]  WITH CHECK ADD  CONSTRAINT [FK_ThuePhong_CauHinhThuePhong] FOREIGN KEY([CauHinhThuePhongId])
REFERENCES [dbo].[CauHinhThuePhong] ([Id])
GO

ALTER TABLE [dbo].[ThuePhong] CHECK CONSTRAINT [FK_ThuePhong_CauHinhThuePhong]
GO

ALTER TABLE [dbo].[ThuePhong]  WITH CHECK ADD  CONSTRAINT [FK_ThuePhong_LoaiThuePhongNoiThucHien] FOREIGN KEY([LoaiThuePhongNoiThucHienId])
REFERENCES [dbo].[LoaiThuePhongNoiThucHien] ([Id])
GO

ALTER TABLE [dbo].[ThuePhong] CHECK CONSTRAINT [FK_ThuePhong_LoaiThuePhongNoiThucHien]
GO

ALTER TABLE [dbo].[ThuePhong]  WITH CHECK ADD  CONSTRAINT [FK_ThuePhong_LoaiThuePhongPhauThuat] FOREIGN KEY([LoaiThuePhongPhauThuatId])
REFERENCES [dbo].[LoaiThuePhongPhauThuat] ([Id])
GO

ALTER TABLE [dbo].[ThuePhong] CHECK CONSTRAINT [FK_ThuePhong_LoaiThuePhongPhauThuat]
GO

ALTER TABLE [dbo].[ThuePhong]  WITH CHECK ADD  CONSTRAINT [FK_ThuePhong_NhanVien] FOREIGN KEY([NhanVienChiDinhId])
REFERENCES [dbo].[NhanVien] ([Id])
GO

ALTER TABLE [dbo].[ThuePhong] CHECK CONSTRAINT [FK_ThuePhong_NhanVien]
GO

ALTER TABLE [dbo].[ThuePhong]  WITH CHECK ADD  CONSTRAINT [FK_ThuePhong_PhongBenhVien] FOREIGN KEY([NoiChiDinhId])
REFERENCES [dbo].[PhongBenhVien] ([Id])
GO

ALTER TABLE [dbo].[ThuePhong] CHECK CONSTRAINT [FK_ThuePhong_PhongBenhVien]
GO

ALTER TABLE [dbo].[ThuePhong]  WITH CHECK ADD  CONSTRAINT [FK_ThuePhong_YeuCauDichVuKyThuat] FOREIGN KEY([YeuCauDichVuKyThuatThuePhongId])
REFERENCES [dbo].[YeuCauDichVuKyThuat] ([Id])
GO

ALTER TABLE [dbo].[ThuePhong] CHECK CONSTRAINT [FK_ThuePhong_YeuCauDichVuKyThuat]
GO

ALTER TABLE [dbo].[ThuePhong]  WITH CHECK ADD  CONSTRAINT [FK_ThuePhong_YeuCauDichVuKyThuat1] FOREIGN KEY([YeuCauDichVuKyThuatTinhChiPhiId])
REFERENCES [dbo].[YeuCauDichVuKyThuat] ([Id])
GO

ALTER TABLE [dbo].[ThuePhong] CHECK CONSTRAINT [FK_ThuePhong_YeuCauDichVuKyThuat1]
GO

ALTER TABLE [dbo].[ThuePhong]  WITH CHECK ADD  CONSTRAINT [FK_ThuePhong_YeuCauTiepNhan] FOREIGN KEY([YeuCauTiepNhanId])
REFERENCES [dbo].[YeuCauTiepNhan] ([Id])
GO

ALTER TABLE [dbo].[ThuePhong] CHECK CONSTRAINT [FK_ThuePhong_YeuCauTiepNhan]
GO

CREATE TABLE [dbo].[NoiGioiThieuChiTietMienGiam](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[NoiGioiThieuId] [bigint] NOT NULL,
	[DichVuKhamBenhBenhVienId] [bigint] NULL,
	[DichVuKyThuatBenhVienId] [bigint] NULL,
	[DichVuGiuongBenhVienId] [bigint] NULL,
	[NhomGiaDichVuKhamBenhBenhVienId] [bigint] NULL,
	[NhomGiaDichVuKyThuatBenhVienId] [bigint] NULL,
	[NhomGiaDichVuGiuongBenhVienId] [bigint] NULL,
	[DuocPhamBenhVienId] [bigint] NULL,
	[VatTuBenhVienId] [bigint] NULL,
	[LoaiChietKhau] [int] NOT NULL,
	[TiLeChietKhau] [int] NULL,
	[SoTienChietKhau] [decimal](15, 2) NULL,
	[GhiChu] [nvarchar](1000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_NoiGioiThieuChiTietMienGiam] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[NoiGioiThieuChiTietMienGiam]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuChiTietMienGiam_DichVuGiuongBenhVien] FOREIGN KEY([DichVuGiuongBenhVienId])
REFERENCES [dbo].[DichVuGiuongBenhVien] ([Id])
GO

ALTER TABLE [dbo].[NoiGioiThieuChiTietMienGiam] CHECK CONSTRAINT [FK_NoiGioiThieuChiTietMienGiam_DichVuGiuongBenhVien]
GO

ALTER TABLE [dbo].[NoiGioiThieuChiTietMienGiam]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuChiTietMienGiam_DichVuKhamBenhBenhVien] FOREIGN KEY([DichVuKhamBenhBenhVienId])
REFERENCES [dbo].[DichVuKhamBenhBenhVien] ([Id])
GO

ALTER TABLE [dbo].[NoiGioiThieuChiTietMienGiam] CHECK CONSTRAINT [FK_NoiGioiThieuChiTietMienGiam_DichVuKhamBenhBenhVien]
GO

ALTER TABLE [dbo].[NoiGioiThieuChiTietMienGiam]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuChiTietMienGiam_DichVuKyThuatBenhVien] FOREIGN KEY([DichVuKyThuatBenhVienId])
REFERENCES [dbo].[DichVuKyThuatBenhVien] ([Id])
GO

ALTER TABLE [dbo].[NoiGioiThieuChiTietMienGiam] CHECK CONSTRAINT [FK_NoiGioiThieuChiTietMienGiam_DichVuKyThuatBenhVien]
GO

ALTER TABLE [dbo].[NoiGioiThieuChiTietMienGiam]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuChiTietMienGiam_DuocPhamBenhVien] FOREIGN KEY([DuocPhamBenhVienId])
REFERENCES [dbo].[DuocPhamBenhVien] ([Id])
GO

ALTER TABLE [dbo].[NoiGioiThieuChiTietMienGiam] CHECK CONSTRAINT [FK_NoiGioiThieuChiTietMienGiam_DuocPhamBenhVien]
GO

ALTER TABLE [dbo].[NoiGioiThieuChiTietMienGiam]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuChiTietMienGiam_NhomGiaDichVuGiuongBenhVien] FOREIGN KEY([NhomGiaDichVuGiuongBenhVienId])
REFERENCES [dbo].[NhomGiaDichVuGiuongBenhVien] ([Id])
GO

ALTER TABLE [dbo].[NoiGioiThieuChiTietMienGiam] CHECK CONSTRAINT [FK_NoiGioiThieuChiTietMienGiam_NhomGiaDichVuGiuongBenhVien]
GO

ALTER TABLE [dbo].[NoiGioiThieuChiTietMienGiam]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuChiTietMienGiam_NhomGiaDichVuKhamBenhBenhVien] FOREIGN KEY([NhomGiaDichVuKhamBenhBenhVienId])
REFERENCES [dbo].[NhomGiaDichVuKhamBenhBenhVien] ([Id])
GO

ALTER TABLE [dbo].[NoiGioiThieuChiTietMienGiam] CHECK CONSTRAINT [FK_NoiGioiThieuChiTietMienGiam_NhomGiaDichVuKhamBenhBenhVien]
GO

ALTER TABLE [dbo].[NoiGioiThieuChiTietMienGiam]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuChiTietMienGiam_NhomGiaDichVuKyThuatBenhVien] FOREIGN KEY([NhomGiaDichVuKyThuatBenhVienId])
REFERENCES [dbo].[NhomGiaDichVuKyThuatBenhVien] ([Id])
GO

ALTER TABLE [dbo].[NoiGioiThieuChiTietMienGiam] CHECK CONSTRAINT [FK_NoiGioiThieuChiTietMienGiam_NhomGiaDichVuKyThuatBenhVien]
GO

ALTER TABLE [dbo].[NoiGioiThieuChiTietMienGiam]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuChiTietMienGiam_NoiGioiThieu] FOREIGN KEY([NoiGioiThieuId])
REFERENCES [dbo].[NoiGioiThieu] ([Id])
GO

ALTER TABLE [dbo].[NoiGioiThieuChiTietMienGiam] CHECK CONSTRAINT [FK_NoiGioiThieuChiTietMienGiam_NoiGioiThieu]
GO

ALTER TABLE [dbo].[NoiGioiThieuChiTietMienGiam]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieuChiTietMienGiam_VatTuBenhVien] FOREIGN KEY([VatTuBenhVienId])
REFERENCES [dbo].[VatTuBenhVien] ([Id])
GO

ALTER TABLE [dbo].[NoiGioiThieuChiTietMienGiam] CHECK CONSTRAINT [FK_NoiGioiThieuChiTietMienGiam_VatTuBenhVien]
GO

ALTER TABLE [dbo].[MienGiamChiPhi]	
	ADD [NoiGioiThieuId] [bigint] NULL
GO
ALTER TABLE [dbo].[MienGiamChiPhi]  WITH CHECK ADD  CONSTRAINT [FK_MienGiamChiPhi_NoiGioiThieu] FOREIGN KEY([NoiGioiThieuId])
REFERENCES [dbo].[NoiGioiThieu] ([Id])
GO
ALTER TABLE [dbo].[MienGiamChiPhi] CHECK CONSTRAINT [FK_MienGiamChiPhi_NoiGioiThieu]
GO

begin tran
   update [dbo].[CauHinh] with (serializable) set [Value] = N'27000'
   where [Name] = N'CauHinhChung.GioBatDauLamViec'

   if @@rowcount = 0
   begin
      INSERT [dbo].[CauHinh] ([Name], [DataType], [Description], [Value], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) 
		VALUES (N'CauHinhChung.GioBatDauLamViec', 6, N'Giờ bắt đầu làm việc', N'27000', 1, 1, '2022-04-20', '2022-04-20')
   end
commit tran
GO
begin tran
   update [dbo].[CauHinh] with (serializable) set [Value] = N'61200'
   where [Name] = N'CauHinhChung.GioKetThucLamViec'

   if @@rowcount = 0
   begin
      INSERT [dbo].[CauHinh] ([Name], [DataType], [Description], [Value], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) 
		VALUES (N'CauHinhChung.GioKetThucLamViec', 6, N'Giờ kết thúc làm việc', N'61200', 1, 1, '2022-04-20', '2022-04-20')
   end
commit tran
GO
begin tran
   update [dbo].[CauHinh] with (serializable) set [Value] = N'1111110'
   where [Name] = N'CauHinhChung.NgayLamViec'

   if @@rowcount = 0
   begin
      INSERT [dbo].[CauHinh] ([Name], [DataType], [Description], [Value], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) 
		VALUES (N'CauHinhChung.NgayLamViec', 3, N'Ngày làm việc', N'1111110', 1, 1, '2022-04-20', '2022-04-20')
   end
commit tran
GO
INSERT [dbo].[LoaiThuePhongPhauThuat]([Ten],[CreatedById],[LastUserId],[LastTime],[CreatedOn])
     VALUES(N'Tiểu phẫu gây tê',1, 1, '2022-04-20', '2022-04-20')
GO
INSERT [dbo].[LoaiThuePhongPhauThuat]([Ten],[CreatedById],[LastUserId],[LastTime],[CreatedOn])
     VALUES(N'Tiễu phẫu tiền mê',1, 1, '2022-04-20', '2022-04-20')
GO
INSERT [dbo].[LoaiThuePhongPhauThuat]([Ten],[CreatedById],[LastUserId],[LastTime],[CreatedOn])
     VALUES(N'Đại phẫu',1, 1, '2022-04-20', '2022-04-20')
GO
INSERT [dbo].[LoaiThuePhongNoiThucHien]([Ten],[CreatedById],[LastUserId],[LastTime],[CreatedOn])
     VALUES(N'Phòng tiễu phẫu',1, 1, '2022-04-20', '2022-04-20')
GO
INSERT [dbo].[LoaiThuePhongNoiThucHien]([Ten],[CreatedById],[LastUserId],[LastTime],[CreatedOn])
     VALUES(N'Phòng mổ lớn',1, 1, '2022-04-20', '2022-04-20')
	 
GO
SET IDENTITY_INSERT [dbo].[DichVuKyThuatBenhVien] ON 
GO
INSERT [dbo].[DichVuKyThuatBenhVien] ([Id], [DichVuKyThuatId], [NgayBatDau], [ThongTu], [QuyetDinh], [NoiBanHanh], [SoMayThucHien], [SoCanBoChuyenMon], [ThoiGianThucHien], [SoCaChoPhep], [CoUuDai], [DieuKienBaoHiemThanhToan], [MoTa], [HieuLuc], [CreatedById], [LastUserId], [LastTime], [CreatedOn], [Ma], [Ten], [NhomDichVuBenhVienId], [DichVuXetNghiemId], [LoaiMauXetNghiem], [LoaiPhauThuatThuThuat], [CoInKetQuaKemHinhAnh], [TenKyThuat], [DichVuCoKetQuaLau], [DichVuKhongKetQua], [SoLanThucHienXetNghiem], [ChuyenKhoaChuyenNganhId], [SoPhimXquang]) 
VALUES (5500, NULL, '2022-04-20', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, 1, 1, 1, '2022-04-20', '2022-04-20', N'DVTPM', N'Dịch vụ khác (CPTPM)', 118, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[DichVuKyThuatBenhVien] OFF
GO
INSERT [dbo].[DichVuKyThuatBenhVienGiaBenhVien] ([DichVuKyThuatBenhVienId], [NhomGiaDichVuKyThuatBenhVienId], [Gia], [TuNgay], [DenNgay], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) 
	VALUES (5500, 1, CAST(0.00 AS Decimal(15, 2)), CAST(N'2022-04-20T00:00:00.000' AS DateTime), NULL, 1, 1, CAST(N'2022-04-20T11:46:53.667' AS DateTime), CAST(N'2022-04-20T11:46:53.667' AS DateTime))
GO
INSERT [dbo].[DichVuKyThuatBenhVienNoiThucHien] ([DichVuKyThuatBenhVienId], [KhoaPhongId], [PhongBenhVienId], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) 
	VALUES (5500, 2, NULL, 1, 1, CAST(N'2022-04-20T11:46:53.667' AS DateTime), CAST(N'2022-04-20T11:46:53.667' AS DateTime))
GO
INSERT [dbo].[DichVuKyThuatBenhVienNoiThucHien] ([DichVuKyThuatBenhVienId], [KhoaPhongId], [PhongBenhVienId], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) 
	VALUES (5500, 5, NULL, 1, 1, CAST(N'2022-04-20T11:46:53.667' AS DateTime), CAST(N'2022-04-20T11:46:53.667' AS DateTime))
GO
INSERT [dbo].[DichVuKyThuatBenhVienNoiThucHien] ([DichVuKyThuatBenhVienId], [KhoaPhongId], [PhongBenhVienId], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) 
	VALUES (5500, 6, NULL, 1, 1, CAST(N'2022-04-20T11:46:53.667' AS DateTime), CAST(N'2022-04-20T11:46:53.667' AS DateTime))
GO
INSERT [dbo].[DichVuKyThuatBenhVienNoiThucHien] ([DichVuKyThuatBenhVienId], [KhoaPhongId], [PhongBenhVienId], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) 
	VALUES (5500, 7, NULL, 1, 1, CAST(N'2022-04-20T11:46:53.667' AS DateTime), CAST(N'2022-04-20T11:46:53.667' AS DateTime))
GO
INSERT [dbo].[DichVuKyThuatBenhVienNoiThucHien] ([DichVuKyThuatBenhVienId], [KhoaPhongId], [PhongBenhVienId], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) 
	VALUES (5500, 8, NULL, 1, 1, CAST(N'2022-04-20T11:46:53.667' AS DateTime), CAST(N'2022-04-20T11:46:53.667' AS DateTime))
GO
INSERT [dbo].[DichVuKyThuatBenhVienNoiThucHien] ([DichVuKyThuatBenhVienId], [KhoaPhongId], [PhongBenhVienId], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) 
	VALUES (5500, 9, NULL, 1, 1, CAST(N'2022-04-20T11:46:53.667' AS DateTime), CAST(N'2022-04-20T11:46:53.667' AS DateTime))
GO

Update dbo.CauHinh
Set [Value] = '3.4.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'