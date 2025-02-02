USE [BvBacHa]
GO
/****** Object:  Table [dbo].[ADR]    Script Date: 3/23/2020 4:47:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ADR](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ThuocHoacHoatChat1Id] [bigint] NOT NULL,
	[ThuocHoacHoatChat2Id] [bigint] NOT NULL,
	[MucDoChuYKhiChiDinh] [int] NULL,
	[MucDoTuongTac] [int] NULL,
	[DuocDongHoc] [bit] NULL,
	[DuocLucHoc] [bit] NULL,
	[ThuocThucAn] [bit] NULL,
	[QuyTac] [bit] NULL,
	[TuongTacHauQua] [nvarchar](1000) NOT NULL,
	[CachXuLy] [nvarchar](1000) NULL,
	[GhiChu] [nvarchar](1000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__ADR__3899E9B0125BB8B5] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[ThuocHoacHoatChat1Id] ASC,
	[ThuocHoacHoatChat2Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditColumn]    Script Date: 3/23/2020 4:47:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditColumn](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[AuditTableId] [bigint] NOT NULL,
	[ColumnName] [nvarchar](100) NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_AuditColumn] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditTable]    Script Date: 3/23/2020 4:47:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditTable](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TableName] [nvarchar](100) NOT NULL,
	[ParentName] [nvarchar](100) NULL,
	[IsRoot] [bit] NULL,
	[IsReference1n] [bit] NULL,
	[ReferenceKey] [nvarchar](100) NULL,
	[ChildReferenceKey] [nvarchar](100) NULL,
	[ChildReferenceName] [nvarchar](100) NULL,
	[CreatedById] [bigint] NULL,
	[LastUserId] [bigint] NULL,
	[LastTime] [datetime] NULL,
	[CreatedOn] [datetime] NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_AuditTable] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BenhNhan]    Script Date: 3/23/2020 4:47:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BenhNhan](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[HoTen] [nvarchar](100) NOT NULL,
	[NgaySinh] [int] NULL,
	[ThangSinh] [int] NULL,
	[NamSinh] [int] NULL,
	[SoChungMinhThu] [nvarchar](12) NULL,
	[GioiTinh] [int] NULL,
	[NhomMau] [int] NULL,
	[NgheNghiepId] [bigint] NULL,
	[NoiLamViec] [nvarchar](300) NULL,
	[QuocTichId] [bigint] NULL,
	[DanTocId] [bigint] NULL,
	[DiaChi] [nvarchar](200) NULL,
	[PhuongXaId] [bigint] NULL,
	[QuanHuyenId] [bigint] NULL,
	[TinhThanhId] [bigint] NULL,
	[SoDienThoai] [nvarchar](12) NULL,
	[Email] [nvarchar](200) NULL,
	[NguoiLienHeHoTen] [nvarchar](100) NULL,
	[NguoiLienHeQuanHeNhanThanId] [bigint] NULL,
	[NguoiLienHeSoDienThoai] [nvarchar](12) NULL,
	[NguoiLienHeEmail] [nvarchar](200) NULL,
	[NguoiLienHeDiaChi] [nvarchar](200) NULL,
	[NguoiLienHePhuongXaId] [bigint] NULL,
	[NguoiLienHeQuanHuyenId] [bigint] NULL,
	[NguoiLienHeTinhThanhId] [bigint] NULL,
	[CoBHYT] [bit] NULL,
	[BHYTMaSoThe] [nvarchar](20) NULL,
	[BHYTMaDKBD] [nvarchar](20) NULL,
	[BHYTNgayHieuLuc] [datetime] NULL,
	[BHYTNgayHetHan] [datetime] NULL,
	[BHYTDiaChi] [nvarchar](200) NULL,
	[BHYTCoQuanBHXH] [nvarchar](200) NULL,
	[BHYTNgayDu5Nam] [datetime] NULL,
	[BHYTNgayDuocMienCungChiTra] [datetime] NULL,
	[BHYTMaKhuVuc] [nvarchar](5) NULL,
	[BHYTDuocMienCungChiTra] [bit] NULL,
	[BHYTGiayMienCungChiTraId] [bigint] NULL,
	[CoBHTN] [bit] NULL,
	[GhiChuTienSuBenh] [nvarchar](4000) NULL,
	[GhiChuDiUngThuoc] [nvarchar](4000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__BenhNhan__3214EC07B85AAF76] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BenhNhanCongTyBaoHiemTuNhan]    Script Date: 3/23/2020 4:47:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BenhNhanCongTyBaoHiemTuNhan](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[BenhNhanId] [bigint] NOT NULL,
	[CongTyBaoHiemTuNhanId] [bigint] NOT NULL,
	[MaSoThe] [nvarchar](20) NULL,
	[DiaChi] [nvarchar](200) NULL,
	[SoDienThoai] [nvarchar](20) NULL,
	[NgayHieuLuc] [datetime] NULL,
	[NgayHetHan] [datetime] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BenhNhanDiUngThuoc]    Script Date: 3/23/2020 4:47:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BenhNhanDiUngThuoc](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[BenhNhanId] [bigint] NOT NULL,
	[MaHoatChat] [nvarchar](20) NOT NULL,
	[HoatChat] [nvarchar](400) NULL,
	[BieuHienDiUng] [nvarchar](2000) NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__BenhNhan__3214EC073B288102] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BenhNhanTienSuBenh]    Script Date: 3/23/2020 4:47:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BenhNhanTienSuBenh](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[BenhNhanId] [bigint] NOT NULL,
	[ICDId] [bigint] NOT NULL,
	[NgayPhatHien] [datetime] NULL,
	[TinhTrangBenh] [nvarchar](200) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__BenhNhan__3214EC0799D65D71] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BenhVien]    Script Date: 3/23/2020 4:47:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BenhVien](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ma] [nvarchar](20) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[TenVietTat] [nvarchar](50) NOT NULL,
	[DiaChi] [nvarchar](250) NOT NULL,
	[LoaiBenhVienId] [bigint] NOT NULL,
	[HangBenhVien] [int] NOT NULL,
	[TuyenChuyenMonKyThuat] [int] NOT NULL,
	[SoDienThoaiLanhDao] [nvarchar](20) NULL,
	[DonViHanhChinhId] [bigint] NOT NULL,
	[HieuLuc] [bit] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_BenhVien] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CapQuanLyBenhVien]    Script Date: 3/23/2020 4:47:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CapQuanLyBenhVien](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[MoTa] [nvarchar](2000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__CapQuanL__3214EC0752A9CAC3] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CauHinh]    Script Date: 3/23/2020 4:47:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CauHinh](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[DataType] [int] NOT NULL,
	[Description] [nvarchar](255) NULL,
	[Value] [nvarchar](max) NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_CauHinh] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[CauHinh] ([Name], [DataType], [Description], [Value], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (N'CauHinhHeThong.DatabaseVesion', 3, NULL, N'0.0.1', 1, 1, CAST(N'2020-02-02T00:00:00.000' AS DateTime), CAST(N'2020-02-02T00:00:00.000' AS DateTime))
GO
/****** Object:  Table [dbo].[CauHinhTheoThoiGian]    Script Date: 3/23/2020 4:47:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CauHinhTheoThoiGian](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[DataType] [int] NOT NULL,
	[Description] [nvarchar](255) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CauHinhTheoThoiGianChiTiet]    Script Date: 3/23/2020 4:47:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CauHinhTheoThoiGianChiTiet](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CauHinhTheoThoiGianId] [bigint] NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
	[FromDate] [datetime] NOT NULL,
	[ToDate] [datetime] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ChiSoXetNghiem]    Script Date: 3/23/2020 4:47:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChiSoXetNghiem](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ma] [nvarchar](50) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[TenTiengAnh] [nvarchar](250) NULL,
	[ChiSoBinhThuongNam] [nvarchar](50) NULL,
	[ChiSoBinhThuongNu] [nvarchar](50) NULL,
	[LoaiXetNghiem] [int] NOT NULL,
	[MoTa] [nvarchar](4000) NULL,
	[HieuLuc] [bit] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ChuanDoan]    Script Date: 3/23/2020 4:47:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChuanDoan](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DanhMucChuanDoanId] [bigint] NOT NULL,
	[Ma] [nvarchar](20) NOT NULL,
	[TenTiengViet] [nvarchar](250) NOT NULL,
	[TenTiengAnh] [nvarchar](250) NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ChuanDoanHinhAnh]    Script Date: 3/23/2020 4:47:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChuanDoanHinhAnh](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ma] [nvarchar](50) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[TenTiengAnh] [nvarchar](250) NULL,
	[LoaiChuanDoanHinhAnh] [int] NOT NULL,
	[MoTa] [nvarchar](4000) NULL,
	[HieuLuc] [bit] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ChuanDoanLienKetICD]    Script Date: 3/23/2020 4:47:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChuanDoanLienKetICD](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ChuanDoanId] [bigint] NOT NULL,
	[ICDId] [bigint] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ChucDanh]    Script Date: 3/23/2020 4:47:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChucDanh](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[Ma] [nvarchar](50) NOT NULL,
	[NhomChucDanhId] [bigint] NOT NULL,
	[MoTa] [nvarchar](2000) NULL,
	[IsDefault] [bit] NOT NULL,
	[IsDisabled] [bit] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__ChucDanh__3214EC07F83AEC89] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ChucVu]    Script Date: 3/23/2020 4:47:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChucVu](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[TenVietTat] [nvarchar](50) NOT NULL,
	[MoTa] [nvarchar](2000) NULL,
	[IsDisabled] [bit] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__ChucVu__3214EC07FBC071A1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ChuongICD]    Script Date: 3/23/2020 4:47:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChuongICD](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[SoChuong] [nvarchar](20) NOT NULL,
	[Ma] [nvarchar](20) NOT NULL,
	[TenTiengViet] [nvarchar](250) NOT NULL,
	[TenTiengAnh] [nvarchar](250) NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CongTyBaoHiemTuNhan]    Script Date: 3/23/2020 4:47:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CongTyBaoHiemTuNhan](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ma] [nvarchar](50) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[DiaChi] [nvarchar](250) NOT NULL,
	[SoDienThoai] [nvarchar](20) NOT NULL,
	[Email] [nvarchar](200) NULL,
	[HinhThucBaoHiem] [int] NOT NULL,
	[PhamViBaoHiem] [int] NOT NULL,
	[GhiChu] [nvarchar](4000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CongTyBaoHiemTuNhanCongNo]    Script Date: 3/23/2020 4:47:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CongTyBaoHiemTuNhanCongNo](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TaiKhoanBenhNhanThuId] [bigint] NOT NULL,
	[CongTyBaoHiemTuNhanId] [bigint] NOT NULL,
	[SoTien] [decimal](15, 2) NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__CongTyBa__3214EC07A709E085] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CongTyUuDai]    Script Date: 3/23/2020 4:47:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CongTyUuDai](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](500) NOT NULL,
	[MoTa] [nvarchar](2000) NULL,
	[IsDisabled] [bit] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DanhMucChuanDoan]    Script Date: 3/23/2020 4:47:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DanhMucChuanDoan](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TenTiengViet] [nvarchar](250) NOT NULL,
	[TenTiengAnh] [nvarchar](250) NOT NULL,
	[GhiChu] [nvarchar](250) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DanToc]    Script Date: 3/23/2020 4:47:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DanToc](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ma] [nvarchar](20) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[QuocGiaId] [bigint] NOT NULL,
	[IsDisabled] [bit] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DichVuGiuong]    Script Date: 3/23/2020 4:47:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DichVuGiuong](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ma] [nvarchar](50) NOT NULL,
	[MaTT37] [nvarchar](50) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[KhoaId] [bigint] NULL,
	[HangBenhVien] [int] NOT NULL,
	[NgoaiQuyDinhBHYT] [bit] NULL,
	[MoTa] [nvarchar](4000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__DichVuGi__3214EC07F862CD2F] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DichVuGiuongBenhVien]    Script Date: 3/23/2020 4:47:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DichVuGiuongBenhVien](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DichVuGiuongId] [bigint] NOT NULL,
	[KhoaPhongId] [bigint] NULL,
	[LoaiGiuong] [int] NOT NULL,
	[MoTa] [nvarchar](4000) NULL,
	[HieuLuc] [bit] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__DichVuGi__3214EC0748B32754] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DichVuGiuongBenhVienGiaBaoHiem]    Script Date: 3/23/2020 4:47:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DichVuGiuongBenhVienGiaBaoHiem](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DichVuGiuongBenhVienId] [bigint] NOT NULL,
	[Gia] [decimal](15, 2) NOT NULL,
	[TuNgay] [datetime] NOT NULL,
	[DenNgay] [datetime] NULL,
	[TiLeBaoHiemThanhToan] [int] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DichVuGiuongBenhVienGiaBenhVien]    Script Date: 3/23/2020 4:47:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DichVuGiuongBenhVienGiaBenhVien](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DichVuGiuongBenhVienId] [bigint] NOT NULL,
	[NhomGiaDichVuGiuongBenhVienId] [bigint] NOT NULL,
	[Gia] [decimal](15, 2) NOT NULL,
	[TuNgay] [datetime] NOT NULL,
	[DenNgay] [datetime] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DichVuGiuongThongTinGia]    Script Date: 3/23/2020 4:47:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DichVuGiuongThongTinGia](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DichVuGiuongId] [bigint] NOT NULL,
	[Gia] [decimal](15, 2) NOT NULL,
	[TuNgay] [datetime] NOT NULL,
	[DenNgay] [datetime] NULL,
	[MoTa] [nvarchar](4000) NULL,
	[HieuLuc] [bit] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NULL,
 CONSTRAINT [PK__DichVuGi__3214EC0713C1C6FF] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DichVuKhamBenh]    Script Date: 3/23/2020 4:47:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DichVuKhamBenh](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ma] [nvarchar](50) NOT NULL,
	[MaTT37] [nvarchar](50) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[KhoaId] [bigint] NOT NULL,
	[HangBenhVien] [int] NOT NULL,
	[NgoaiQuyDinhBHYT] [bit] NULL,
	[MoTa] [nvarchar](4000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__DichVuKh__3214EC070BE1C321] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DichVuKhamBenhBenhVien]    Script Date: 3/23/2020 4:47:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DichVuKhamBenhBenhVien](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DichVuKhamBenhId] [bigint] NOT NULL,
	[KhoaPhongId] [bigint] NOT NULL,
	[CoUuDai] [bit] NULL,
	[MoTa] [nvarchar](4000) NULL,
	[HieuLuc] [bit] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__DichVuKh__3214EC0793DF5D4D] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DichVuKhamBenhBenhVienGiaBaoHiem]    Script Date: 3/23/2020 4:47:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DichVuKhamBenhBenhVienGiaBaoHiem](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DichVuKhamBenhBenhVienId] [bigint] NOT NULL,
	[Gia] [decimal](15, 2) NOT NULL,
	[TuNgay] [datetime] NOT NULL,
	[DenNgay] [datetime] NULL,
	[TiLeBaoHiemThanhToan] [int] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DichVuKhamBenhBenhVienGiaBenhVien]    Script Date: 3/23/2020 4:47:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DichVuKhamBenhBenhVienGiaBenhVien](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DichVuKhamBenhBenhVienId] [bigint] NOT NULL,
	[NhomGiaDichVuKhamBenhBenhVienId] [bigint] NOT NULL,
	[Gia] [decimal](15, 2) NOT NULL,
	[TuNgay] [datetime] NOT NULL,
	[DenNgay] [datetime] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DichVuKhamBenhThongTinGia]    Script Date: 3/23/2020 4:47:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DichVuKhamBenhThongTinGia](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DichVuKhamBenhId] [bigint] NOT NULL,
	[Gia] [decimal](15, 2) NOT NULL,
	[TuNgay] [datetime] NOT NULL,
	[DenNgay] [datetime] NULL,
	[MoTa] [nvarchar](4000) NULL,
	[HieuLuc] [bit] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NULL,
 CONSTRAINT [PK__DichVuKh__3214EC0784C61111] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DichVuKyThuat]    Script Date: 3/23/2020 4:47:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DichVuKyThuat](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[TenTiengAnh] [nvarchar](250) NULL,
	[Ma] [nvarchar](50) NOT NULL,
	[Ma4350] [nvarchar](50) NULL,
	[MaGia] [nvarchar](50) NOT NULL,
	[TenGia] [nvarchar](250) NULL,
	[NhomChiPhi] [int] NOT NULL,
	[NhomDichVuKyThuatId] [bigint] NOT NULL,
	[LoaiPhauThuatThuThuat] [int] NULL,
	[NgoaiQuyDinhBHYT] [bit] NULL,
	[MoTa] [nvarchar](4000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__DichVuKy__3214EC0756CBFEA6] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DichVuKyThuatBenhVien]    Script Date: 3/23/2020 4:47:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DichVuKyThuatBenhVien](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DichVuKyThuatId] [bigint] NOT NULL,
	[KhoaPhongId] [bigint] NULL,
	[NgayBatDau] [datetime] NOT NULL,
	[ThongTu] [nvarchar](100) NULL,
	[QuyetDinh] [nvarchar](100) NULL,
	[NoiBanHanh] [nvarchar](100) NULL,
	[SoMayThucHien] [int] NULL,
	[SoCanBoChuyenMon] [int] NULL,
	[ThoiGianThucHien] [int] NULL,
	[SoCaChoPhep] [int] NULL,
	[CoUuDai] [bit] NULL,
	[DieuKienBaoHiemThanhToan] [nvarchar](4000) NULL,
	[MoTa] [nvarchar](4000) NULL,
	[HieuLuc] [bit] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__DichVuKy__3214EC07DA609539] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DichVuKyThuatBenhVienGiaBaoHiem]    Script Date: 3/23/2020 4:47:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DichVuKyThuatBenhVienGiaBaoHiem](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DichVuKyThuatBenhVienId] [bigint] NOT NULL,
	[Gia] [decimal](15, 2) NOT NULL,
	[TuNgay] [datetime] NOT NULL,
	[DenNgay] [datetime] NULL,
	[TiLeBaoHiemThanhToan] [int] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DichVuKyThuatBenhVienGiaBenhVien]    Script Date: 3/23/2020 4:47:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DichVuKyThuatBenhVienGiaBenhVien](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DichVuKyThuatBenhVienId] [bigint] NOT NULL,
	[NhomGiaDichVuKyThuatBenhVienId] [bigint] NOT NULL,
	[Gia] [decimal](15, 2) NOT NULL,
	[TuNgay] [datetime] NOT NULL,
	[DenNgay] [datetime] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DichVuKyThuatThongTinGia]    Script Date: 3/23/2020 4:47:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DichVuKyThuatThongTinGia](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DichVuKyThuatId] [bigint] NOT NULL,
	[HangBenhVien] [int] NULL,
	[Gia] [decimal](15, 2) NOT NULL,
	[TuNgay] [datetime] NOT NULL,
	[DenNgay] [datetime] NULL,
	[ThongTu] [nvarchar](100) NULL,
	[QuyetDinh] [nvarchar](100) NULL,
	[MoTa] [nvarchar](4000) NULL,
	[HieuLuc] [bit] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NULL,
 CONSTRAINT [PK__DichVuKy__3214EC07AE79AA3C] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DinhMucDuocPhamTonKho]    Script Date: 3/23/2020 4:47:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DinhMucDuocPhamTonKho](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DuocPhamBenhVienId] [bigint] NOT NULL,
	[KhoDuocPhamId] [bigint] NOT NULL,
	[TonToiThieu] [int] NULL,
	[TonToiDa] [int] NULL,
	[SoNgayTruocKhiHetHan] [int] NULL,
	[MoTa] [nvarchar](2000) NULL,
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
/****** Object:  Table [dbo].[DoiTuongUuDai]    Script Date: 3/23/2020 4:47:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DoiTuongUuDai](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[MoTa] [nvarchar](2000) NULL,
	[IsDisabled] [bit] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DoiTuongUuDaiDichVuKhamBenhBenhVien]    Script Date: 3/23/2020 4:47:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DoiTuongUuDaiDichVuKhamBenhBenhVien](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DoiTuongUuDaiId] [bigint] NOT NULL,
	[DichVuKhamBenhBenhVienId] [bigint] NOT NULL,
	[TiLeUuDai] [int] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DoiTuongUuDaiDichVuKyThuatBenhVien]    Script Date: 3/23/2020 4:47:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DoiTuongUuDaiDichVuKyThuatBenhVien](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DoiTuongUuDaiId] [bigint] NOT NULL,
	[DichVuKyThuatBenhVienId] [bigint] NOT NULL,
	[TiLeUuDai] [int] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DoiTuongUuTienKhamChuaBenh]    Script Date: 3/23/2020 4:47:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DoiTuongUuTienKhamChuaBenh](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[TenVietTat] [nvarchar](50) NOT NULL,
	[ThuTuUuTien] [int] NOT NULL,
	[MoTa] [nvarchar](2000) NULL,
	[IsDisabled] [bit] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DonThuocThanhToan]    Script Date: 3/23/2020 4:47:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DonThuocThanhToan](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[YeuCauKhamBenhDonThuocId] [bigint] NULL,
	[YeuCauKhamBenhId] [bigint] NULL,
	[YeuCauTiepNhanId] [bigint] NULL,
	[BenhNhanId] [bigint] NULL,
	[LoaiDonThuoc] [int] NOT NULL,
	[TrangThai] [int] NOT NULL,
	[TrangThaiThanhToan] [int] NOT NULL,
	[NhanVienHuyThanhToanId] [bigint] NULL,
	[LyDoHuyThanhToan] [nvarchar](1000) NULL,
	[NoiCapThuocId] [bigint] NULL,
	[NhanVienCapThuocId] [bigint] NULL,
	[ThoiDiemCapThuoc] [datetime] NULL,
	[GhiChu] [nvarchar](1000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__YeuCauKh__3214EC0711A22A6B] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DonThuocThanhToanChiTiet]    Script Date: 3/23/2020 4:47:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DonThuocThanhToanChiTiet](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DonThuocThanhToanId] [bigint] NOT NULL,
	[YeuCauKhamBenhDonThuocChiTietId] [bigint] NULL,
	[DuocPhamId] [bigint] NOT NULL,
	[XuatKhoDuocPhamChiTietViTriId] [bigint] NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[TenTiengAnh] [nvarchar](250) NULL,
	[SoDangKy] [nvarchar](50) NOT NULL,
	[STTHoatChat] [int] NULL,
	[NhomChiPhi] [int] NOT NULL,
	[MaHoatChat] [nvarchar](20) NOT NULL,
	[HoatChat] [nvarchar](500) NOT NULL,
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
	[HopDongThauDuocPhamId] [bigint] NULL,
	[NhaThauId] [bigint] NULL,
	[SoHopDongThau] [nvarchar](50) NULL,
	[SoQuyetDinhThau] [nvarchar](50) NULL,
	[LoaiThau] [int] NULL,
	[LoaiThuocThau] [int] NULL,
	[NhomThau] [nvarchar](50) NULL,
	[GoiThau] [nvarchar](2) NULL,
	[NamThau] [int] NULL,
	[Gia] [decimal](15, 2) NULL,
	[SoLuong] [float] NOT NULL,
	[SoNgayDung] [int] NULL,
	[DungSang] [float] NULL,
	[DungTrua] [float] NULL,
	[DungChieu] [float] NULL,
	[DungToi] [float] NULL,
	[SoTienBenhNhanDaChi] [decimal](15, 2) NULL,
	[DuocHuongBaoHiem] [bit] NOT NULL,
	[BaoHiemChiTra] [bit] NULL,
	[ThoiDiemDuyetBaoHiem] [datetime] NULL,
	[NhanVienDuyetBaoHiemId] [bigint] NULL,
	[GiaBaoHiemThanhToan] [decimal](15, 2) NULL,
	[GhiChu] [nvarchar](1000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__YeuCauKh__3214EC0775073EC5] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DonViHanhChinh]    Script Date: 3/23/2020 4:47:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DonViHanhChinh](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ma] [nvarchar](50) NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[CapHanhChinh] [int] NOT NULL,
	[TenDonViHanhChinh] [nvarchar](50) NOT NULL,
	[VungDiaLy] [int] NULL,
	[TenVietTat] [nvarchar](50) NULL,
	[TrucThuocDonViHanhChinhId] [bigint] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__DonViHan__3214EC0711E97D9B] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DonViTinh]    Script Date: 3/23/2020 4:47:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DonViTinh](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ma] [nvarchar](20) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[MoTa] [nvarchar](2000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__DonViTin__3214EC07207B6CC1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DuocPham]    Script Date: 3/23/2020 4:47:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DuocPham](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[TenTiengAnh] [nvarchar](250) NULL,
	[SoDangKy] [nvarchar](100) NOT NULL,
	[STTHoatChat] [int] NULL,
	[MaHoatChat] [nvarchar](20) NOT NULL,
	[HoatChat] [nvarchar](500) NOT NULL,
	[LoaiThuocHoacHoatChat] [int] NOT NULL,
	[NhaSanXuat] [nvarchar](250) NULL,
	[NuocSanXuat] [nvarchar](250) NULL,
	[DuongDungId] [bigint] NOT NULL,
	[HamLuong] [nvarchar](500) NULL,
	[QuyCach] [nvarchar](250) NULL,
	[TieuChuan] [nvarchar](50) NULL,
	[DangBaoChe] [nvarchar](250) NULL,
	[DonViTinhId] [bigint] NOT NULL,
	[DonViTinhThamKhao] [nvarchar](100) NULL,
	[HuongDan] [nvarchar](4000) NULL,
	[MoTa] [nvarchar](4000) NULL,
	[ChiDinh] [nvarchar](4000) NULL,
	[ChongChiDinh] [nvarchar](4000) NULL,
	[LieuLuongCachDung] [nvarchar](4000) NULL,
	[TacDungPhu] [nvarchar](4000) NULL,
	[ChuYDePhong] [nvarchar](4000) NULL,
	[IsDisabled] [bit] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__DuocPham__3214EC078BB9F10C] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DuocPhamBenhVien]    Script Date: 3/23/2020 4:47:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DuocPhamBenhVien](
	[Id] [bigint] NOT NULL,
	[DieuKienBaoHiemThanhToan] [nvarchar](4000) NULL,
	[HieuLuc] [bit] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__DuocPham__3214EC079562ACE6] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DuocPhamBenhVienGiaBaoHiem]    Script Date: 3/23/2020 4:47:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DuocPhamBenhVienGiaBaoHiem](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DuocPhamBenhVienId] [bigint] NOT NULL,
	[Gia] [decimal](15, 2) NOT NULL,
	[TuNgay] [datetime] NOT NULL,
	[DenNgay] [datetime] NULL,
	[TiLeBaoHiemThanhToan] [int] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DuongDung]    Script Date: 3/23/2020 4:47:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DuongDung](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ma] [nvarchar](50) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[MoTa] [nvarchar](250) NULL,
	[IsDisabled] [bit] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__DuongDun__3214EC0725C0933C] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DuTruDuocPham]    Script Date: 3/23/2020 4:47:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DuTruDuocPham](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[KhoaPhongId] [bigint] NULL,
	[NhanVienLapDuTruId] [bigint] NULL,
	[DuocPhamId] [bigint] NOT NULL,
	[SoLuong] [int] NULL,
	[Mota] [nvarchar](2000) NULL,
	[DuyetYeuCau] [bit] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DuyetBaoHiem]    Script Date: 3/23/2020 4:47:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DuyetBaoHiem](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ThoiDiemDuyetBaoHiem] [datetime] NOT NULL,
	[NhanVienDuyetBaoHiemId] [bigint] NOT NULL,
	[NoiDuyetBaoHiemId] [bigint] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__DuyetBao__3214EC07D6E5F6CB] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DuyetBaoHiemChiTiet]    Script Date: 3/23/2020 4:47:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DuyetBaoHiemChiTiet](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DuyetBaoHiemId] [bigint] NOT NULL,
	[YeuCauKhamBenhId] [bigint] NULL,
	[YeuCauDichVuKyThuatId] [bigint] NULL,
	[YeuCauDuocPhamBenhVienId] [bigint] NULL,
	[YeuCauVatTuBenhVienId] [bigint] NULL,
	[YeuCauDichVuGiuongBenhVienId] [bigint] NULL,
	[DonThuocThanhToanChiTietId] [bigint] NULL,
	[GiaBaoHiemThanhToan] [decimal](15, 2) NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GiayChuyenVien]    Script Date: 3/23/2020 4:47:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GiayChuyenVien](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ma] [nvarchar](50) NULL,
	[Ten] [nvarchar](256) NOT NULL,
	[TenGuid] [nvarchar](256) NOT NULL,
	[KichThuoc] [bigint] NOT NULL,
	[DuongDan] [nvarchar](500) NOT NULL,
	[LoaiTapTin] [int] NOT NULL,
	[MoTa] [nvarchar](2000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GiayMienCungChiTra]    Script Date: 3/23/2020 4:47:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GiayMienCungChiTra](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ma] [nvarchar](50) NULL,
	[Ten] [nvarchar](256) NOT NULL,
	[TenGuid] [nvarchar](256) NOT NULL,
	[KichThuoc] [bigint] NOT NULL,
	[DuongDan] [nvarchar](500) NOT NULL,
	[LoaiTapTin] [int] NOT NULL,
	[MoTa] [nvarchar](2000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GoiDichVu]    Script Date: 3/23/2020 4:47:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GoiDichVu](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[LoaiGoiDichVu] [int] NOT NULL,
	[Ten] [nvarchar](200) NOT NULL,
	[CoChietKhau] [bit] NOT NULL,
	[ChiPhiGoiDichVu] [bigint] NULL,
	[NhanVienTaoGoiId] [bigint] NOT NULL,
	[NgayBatDau] [datetime] NOT NULL,
	[NgayKetThuc] [datetime] NULL,
	[MoTa] [nvarchar](4000) NULL,
	[IsDisabled] [bit] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__GoiDichV__3214EC074F130BF6] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GoiDichVuChiTietDichVuGiuong]    Script Date: 3/23/2020 4:47:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GoiDichVuChiTietDichVuGiuong](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[GoiDichVuId] [bigint] NOT NULL,
	[DichVuGiuongBenhVienId] [bigint] NOT NULL,
	[NhomGiaDichVuGiuongBenhVienId] [bigint] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GoiDichVuChiTietDichVuKhamBenh]    Script Date: 3/23/2020 4:47:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GoiDichVuChiTietDichVuKhamBenh](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[GoiDichVuId] [bigint] NOT NULL,
	[DichVuKhamBenhBenhVienId] [bigint] NOT NULL,
	[NhomGiaDichVuKhamBenhBenhVienId] [bigint] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__GoiDichV__3214EC07D4601036] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GoiDichVuChiTietDichVuKyThuat]    Script Date: 3/23/2020 4:47:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GoiDichVuChiTietDichVuKyThuat](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[GoiDichVuId] [bigint] NOT NULL,
	[DichVuKyThuatBenhVienId] [bigint] NOT NULL,
	[NhomGiaDichVuKyThuatBenhVienId] [bigint] NOT NULL,
	[SoLan] [int] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__GoiDichV__3214EC07FBDDB84B] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GoiDichVuChiTietDuocPham]    Script Date: 3/23/2020 4:47:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GoiDichVuChiTietDuocPham](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[GoiDichVuId] [bigint] NOT NULL,
	[DuocPhamBenhVienId] [bigint] NOT NULL,
	[SoLuong] [float] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__GoiDichV__3214EC079D29FF31] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GoiDichVuChiTietVatTu]    Script Date: 3/23/2020 4:47:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GoiDichVuChiTietVatTu](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[GoiDichVuId] [bigint] NOT NULL,
	[VatTuBenhVienId] [bigint] NOT NULL,
	[SoLuong] [float] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__GoiDichV__3214EC076C813016] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HamGuiHoSoWatching]    Script Date: 3/23/2020 4:47:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HamGuiHoSoWatching](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TimeSend] [datetime] NULL,
	[DataJson] [nvarchar](max) NULL,
	[XMLJson] [nvarchar](max) NULL,
	[XMLError] [nvarchar](max) NULL,
	[APIError] [nvarchar](max) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_HamGuiHoSoWatching] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HinhThucDen]    Script Date: 3/23/2020 4:47:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HinhThucDen](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[MoTa] [nvarchar](2000) NULL,
	[IsDisabled] [bit] NULL,
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
/****** Object:  Table [dbo].[HoatDongNhanVien]    Script Date: 3/23/2020 4:47:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HoatDongNhanVien](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[PhongBenhVienId] [bigint] NOT NULL,
	[NhanVienId] [bigint] NOT NULL,
	[ThoiDiemBatDau] [datetime] NOT NULL,
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
/****** Object:  Table [dbo].[HocViHocHam]    Script Date: 3/23/2020 4:47:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HocViHocHam](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[Ma] [nvarchar](50) NULL,
	[MoTa] [nvarchar](2000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__HocHamHo__3214EC07F1756321] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HopDongThauDuocPham]    Script Date: 3/23/2020 4:47:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HopDongThauDuocPham](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[NhaThauId] [bigint] NOT NULL,
	[SoHopDong] [nvarchar](50) NOT NULL,
	[SoQuyetDinh] [nvarchar](50) NOT NULL,
	[CongBo] [datetime] NOT NULL,
	[NgayKy] [datetime] NOT NULL,
	[NgayHieuLuc] [datetime] NOT NULL,
	[NgayHetHan] [datetime] NOT NULL,
	[LoaiThau] [int] NOT NULL,
	[LoaiThuocThau] [int] NOT NULL,
	[NhomThau] [nvarchar](50) NOT NULL,
	[GoiThau] [nvarchar](2) NOT NULL,
	[Nam] [int] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__HopDongT__3214EC07886AF1D5] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HopDongThauDuocPhamChiTiet]    Script Date: 3/23/2020 4:47:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HopDongThauDuocPhamChiTiet](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[HopDongThauDuocPhamId] [bigint] NOT NULL,
	[DuocPhamId] [bigint] NOT NULL,
	[Gia] [decimal](15, 2) NOT NULL,
	[SoLuong] [float] NOT NULL,
	[SoLuongDaCap] [float] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__HopDongT__3214EC076966428F] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ICD]    Script Date: 3/23/2020 4:47:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ICD](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[LoaiICDId] [bigint] NOT NULL,
	[Ma] [nvarchar](20) NOT NULL,
	[TenTiengViet] [nvarchar](250) NOT NULL,
	[TenTiengAnh] [nvarchar](250) NULL,
	[ManTinh] [bit] NULL,
	[GioiTinh] [int] NULL,
	[BenhThuongGap] [bit] NULL,
	[BenhNam] [bit] NULL,
	[KhongBaoHiem] [bit] NULL,
	[NgoaiDinhSuat] [bit] NULL,
	[KhoaId] [bigint] NULL,
	[MoTa] [nvarchar](1000) NULL,
	[ChuanDoanLamSan] [nvarchar](max) NULL,
	[ThongTinThamKhaoChoBenhNhan] [nvarchar](max) NULL,
	[TenGoiKhac] [nvarchar](1000) NULL,
	[HieuLuc] [bit] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
	[MaChiTiet] [nvarchar](20) NULL,
 CONSTRAINT [PK__ICD__3214EC07A3DE0B12] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ICDDoiTuongBenhNhan]    Script Date: 3/23/2020 4:47:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ICDDoiTuongBenhNhan](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[GhiChu] [nvarchar](250) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ICDDoiTuongBenhNhanChiTiet]    Script Date: 3/23/2020 4:47:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ICDDoiTuongBenhNhanChiTiet](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ICDId] [bigint] NOT NULL,
	[ICDDoiTuongBenhNhanId] [bigint] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KetQuaChuanDoanHinhAnh]    Script Date: 3/23/2020 4:47:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KetQuaChuanDoanHinhAnh](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ChuanDoanHinhAnhId] [bigint] NOT NULL,
	[YeuCauDichVuKyThuatId] [bigint] NOT NULL,
	[MaMay] [nvarchar](50) NULL,
	[KetQua] [nvarchar](max) NULL,
	[MoTa] [nvarchar](4000) NULL,
	[KetLuan] [nvarchar](4000) NULL,
	[ThoiDiemKetLuan] [datetime] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__KetQuaCh__3214EC07D38FB44B] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KetQuaSinhHieu]    Script Date: 3/23/2020 4:47:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KetQuaSinhHieu](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[YeuCauTiepNhanId] [bigint] NOT NULL,
	[NhipTim] [int] NULL,
	[NhipTho] [int] NULL,
	[ThanNhiet] [float] NULL,
	[HuyetApTamThu] [int] NULL,
	[HuyetApTamTruong] [int] NULL,
	[ChieuCao] [float] NULL,
	[CanNang] [float] NULL,
	[BMI] [float] NULL,
	[NoiThucHienId] [bigint] NULL,
	[NhanVienThucHienId] [bigint] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KetQuaXetNghiem]    Script Date: 3/23/2020 4:47:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KetQuaXetNghiem](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ChiSoXetNghiemId] [bigint] NOT NULL,
	[YeuCauDichVuKyThuatId] [bigint] NOT NULL,
	[MaMay] [nvarchar](50) NULL,
	[KetQua] [nvarchar](250) NULL,
	[MoTa] [nvarchar](4000) NULL,
	[KetLuan] [nvarchar](4000) NULL,
	[ThoiDiemKetLuan] [datetime] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__KetQuaXe__3214EC07142FC125] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Khoa]    Script Date: 3/23/2020 4:47:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Khoa](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[Ma] [nvarchar](50) NOT NULL,
	[MoTa] [nvarchar](2000) NULL,
	[IsDisabled] [bit] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__Khoa__3214EC079C2EBD05] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KhoaPhong]    Script Date: 3/23/2020 4:47:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KhoaPhong](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[Ma] [nvarchar](50) NOT NULL,
	[LoaiKhoaPhong] [int] NOT NULL,
	[CoKhamNgoaiTru] [bit] NULL,
	[SoTienThuTamUng] [bigint] NULL,
	[IsDisabled] [bit] NULL,
	[MoTa] [nvarchar](2000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__KhoaPhon__3214EC07038F0452] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KhoaPhongChuyenKhoa]    Script Date: 3/23/2020 4:47:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KhoaPhongChuyenKhoa](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[KhoaPhongId] [bigint] NOT NULL,
	[KhoaId] [bigint] NOT NULL,
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
/****** Object:  Table [dbo].[KhoaPhongNhanVien]    Script Date: 3/23/2020 4:47:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KhoaPhongNhanVien](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[KhoaPhongId] [bigint] NOT NULL,
	[NhanVienId] [bigint] NOT NULL,
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
/****** Object:  Table [dbo].[KhoDuocPham]    Script Date: 3/23/2020 4:47:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KhoDuocPham](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](50) NOT NULL,
	[LoaiKho] [int] NOT NULL,
	[KhoaPhongId] [bigint] NULL,
	[IsDefault] [bit] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__KhoDuocP__3214EC0742E75424] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KhoDuocPhamViTri]    Script Date: 3/23/2020 4:47:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KhoDuocPhamViTri](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[KhoDuocPhamId] [bigint] NOT NULL,
	[Ten] [nvarchar](100) NOT NULL,
	[MoTa] [nvarchar](2000) NULL,
	[IsDisabled] [bit] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__KhoDuocP__3214EC0793229E8E] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LichPhanCongNgoaiTru]    Script Date: 3/23/2020 4:47:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LichPhanCongNgoaiTru](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[PhongNgoaiTruId] [bigint] NOT NULL,
	[NhanVienId] [bigint] NOT NULL,
	[NgayPhanCong] [datetime] NOT NULL,
	[LoaiThoiGianPhanCong] [int] NOT NULL,
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
/****** Object:  Table [dbo].[LichSuEmail]    Script Date: 3/23/2020 4:47:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LichSuEmail](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[MaDoiTuong] [nvarchar](20) NULL,
	[IdDoiTuong] [bigint] NULL,
	[TieuDe] [nvarchar](100) NULL,
	[GoiDen] [nvarchar](50) NOT NULL,
	[NoiDung] [nvarchar](2000) NOT NULL,
	[TapTinDinhKem] [nvarchar](200) NULL,
	[TrangThai] [int] NOT NULL,
	[NgayGui] [datetime] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_LichSuEmail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LichSuHoatDongNhanVien]    Script Date: 3/23/2020 4:47:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LichSuHoatDongNhanVien](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[PhongBenhVienId] [bigint] NOT NULL,
	[NhanVienId] [bigint] NOT NULL,
	[ThoiDiemBatDau] [datetime] NOT NULL,
	[ThoiDiemKetThuc] [datetime] NULL,
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
/****** Object:  Table [dbo].[LichSuSMS]    Script Date: 3/23/2020 4:47:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LichSuSMS](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[GoiDen] [nvarchar](20) NOT NULL,
	[NoiDung] [nvarchar](250) NOT NULL,
	[TrangThai] [int] NOT NULL,
	[NgayGui] [datetime] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NULL,
 CONSTRAINT [PK_LichSuSMS] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LichSuThongBao]    Script Date: 3/23/2020 4:47:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LichSuThongBao](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[GoiDen] [nvarchar](200) NOT NULL,
	[NoiDung] [nvarchar](250) NOT NULL,
	[TrangThai] [int] NOT NULL,
	[NgayGui] [datetime] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NULL,
 CONSTRAINT [PK_LichSuThongBao] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LoaiBenhVien]    Script Date: 3/23/2020 4:47:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LoaiBenhVien](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[MoTa] [nvarchar](2000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__LoaiBenh__3214EC07AC067043] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LoaiICD]    Script Date: 3/23/2020 4:47:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LoaiICD](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[NhomICDId] [bigint] NOT NULL,
	[Ma] [nvarchar](20) NOT NULL,
	[TenTiengViet] [nvarchar](250) NOT NULL,
	[TenTiengAnh] [nvarchar](250) NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LoaiPhongBenhNoiTru]    Script Date: 3/23/2020 4:47:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LoaiPhongBenhNoiTru](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[MoTa] [nvarchar](2000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__LoaiPhon__3214EC079A22054F] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LocaleStringResource]    Script Date: 3/23/2020 4:47:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LocaleStringResource](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ResourceName] [nvarchar](200) NOT NULL,
	[ResourceValue] [nvarchar](max) NOT NULL,
	[Language] [int] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_LocaleStringResource] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LyDoKhamBenh]    Script Date: 3/23/2020 4:47:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LyDoKhamBenh](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[TenVietTat] [nvarchar](50) NOT NULL,
	[MoTa] [nvarchar](2000) NULL,
	[IsDisabled] [bit] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__LyDoKham__3214EC07DBA91F47] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MauVaChePham]    Script Date: 3/23/2020 4:47:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MauVaChePham](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ma] [nvarchar](20) NOT NULL,
	[Ten] [nvarchar](500) NOT NULL,
	[PhanLoaiMau] [int] NOT NULL,
	[TheTich] [bigint] NOT NULL,
	[GiaTriToiDa] [bigint] NOT NULL,
	[GhiChu] [nvarchar](500) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__MauVaChe__3214EC0702BD9C67] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MessagingTemplate]    Script Date: 3/23/2020 4:47:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MessagingTemplate](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[MessagingType] [int] NOT NULL,
	[Title] [nvarchar](500) NULL,
	[Body] [nvarchar](max) NOT NULL,
	[Link] [nvarchar](500) NULL,
	[MessagePriority] [int] NOT NULL,
	[Language] [int] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[IsDisabled] [bit] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__Messagin__3214EC0736675CAA] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NgheNghiep]    Script Date: 3/23/2020 4:47:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NgheNghiep](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[TenVietTat] [nvarchar](50) NOT NULL,
	[MoTa] [nvarchar](2000) NULL,
	[IsDisabled] [bit] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__NgheNghi__3214EC079EB6E26E] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NhanVien]    Script Date: 3/23/2020 4:47:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NhanVien](
	[Id] [bigint] NOT NULL,
	[QuyenHan] [nvarchar](50) NULL,
	[GhiChu] [nvarchar](2000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
	[HocHamHocViId] [bigint] NULL,
	[PhamViHanhNgheId] [bigint] NULL,
	[ChucDanhId] [bigint] NULL,
	[MaChungChiHanhNghe] [nvarchar](50) NULL,
	[VanBangChuyenMonId] [bigint] NULL,
	[NgayCapChungChiHanhNghe] [datetime] NULL,
	[NoiCapChungChiHanhNghe] [nvarchar](200) NULL,
	[NgayKyHopDong] [datetime] NULL,
	[NgayHetHopDong] [datetime] NULL,
 CONSTRAINT [PK_NhanVien] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NhanVienRole]    Script Date: 3/23/2020 4:47:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NhanVienRole](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[NhanVienId] [bigint] NOT NULL,
	[RoleId] [bigint] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NhapKhoDuocPham]    Script Date: 3/23/2020 4:47:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NhapKhoDuocPham](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[KhoDuocPhamId] [bigint] NOT NULL,
	[SoChungTu] [nvarchar](50) NOT NULL,
	[LoaiNhapKho] [int] NOT NULL,
	[XuatKhoDuocPhamId] [bigint] NULL,
	[TenNguoiGiao] [nvarchar](100) NULL,
	[NguoiGiaoId] [bigint] NULL,
	[NguoiNhapId] [bigint] NOT NULL,
	[DaHet] [bit] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
	[LoaiNguoiGiao] [int] NOT NULL,
	[NgayNhap] [datetime] NOT NULL,
 CONSTRAINT [PK__NhapKhoD__3214EC07BC19C317] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NhapKhoDuocPhamChiTiet]    Script Date: 3/23/2020 4:47:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NhapKhoDuocPhamChiTiet](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[NhapKhoDuocPhamId] [bigint] NOT NULL,
	[DuocPhamBenhVienId] [bigint] NOT NULL,
	[HopDongThauDuocPhamId] [bigint] NOT NULL,
	[Solo] [nvarchar](50) NOT NULL,
	[DatChatLuong] [bit] NOT NULL,
	[HanSuDung] [datetime] NOT NULL,
	[SoLuongNhap] [float] NOT NULL,
	[DonGiaNhap] [decimal](15, 2) NOT NULL,
	[DonGiaBan] [decimal](15, 2) NULL,
	[VAT] [int] NULL,
	[ChietKhau] [int] NULL,
	[MaVach] [nvarchar](100) NULL,
	[KhoDuocPhamViTriId] [bigint] NULL,
	[SoLuongDaXuat] [float] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
	[NgayNhap] [datetime] NOT NULL,
 CONSTRAINT [PK__NhapKhoD__3214EC072D98D4AD] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NhaSanXuat]    Script Date: 3/23/2020 4:47:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NhaSanXuat](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ma] [nvarchar](20) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__NhaSanXu__3214EC07E50D81B1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NhaSanXuatTheoQuocGia]    Script Date: 3/23/2020 4:47:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NhaSanXuatTheoQuocGia](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[NhaSanXuatId] [bigint] NOT NULL,
	[DiaChi] [nvarchar](500) NOT NULL,
	[QuocGiaId] [bigint] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NhaThau]    Script Date: 3/23/2020 4:47:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NhaThau](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[DiaChi] [nvarchar](250) NOT NULL,
	[MaSoThue] [nvarchar](20) NOT NULL,
	[TaiKhoanNganHang] [nvarchar](20) NOT NULL,
	[NguoiDaiDien] [nvarchar](100) NOT NULL,
	[NguoiLienHe] [nvarchar](100) NOT NULL,
	[SoDienThoaiLienHe] [nvarchar](20) NOT NULL,
	[EmailLienHe] [nvarchar](200) NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NhatKyHeThong]    Script Date: 3/23/2020 4:47:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NhatKyHeThong](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[HoatDong] [int] NOT NULL,
	[MaDoiTuong] [nvarchar](20) NULL,
	[IdDoiTuong] [bigint] NULL,
	[NoiDung] [nvarchar](2000) NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_NhatKyHeThong] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NhomChucDanh]    Script Date: 3/23/2020 4:47:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NhomChucDanh](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[Ma] [nvarchar](50) NOT NULL,
	[MoTa] [nvarchar](2000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__NhomChuc__3214EC0760C7FFC0] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NhomDichVuKyThuat]    Script Date: 3/23/2020 4:47:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NhomDichVuKyThuat](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[Ma] [nvarchar](10) NOT NULL,
	[NhomDichVuKyThuatChaId] [bigint] NULL,
	[CapNhom] [int] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__NhomDich__3214EC07451C0C62] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NhomGiaDichVuGiuongBenhVien]    Script Date: 3/23/2020 4:47:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NhomGiaDichVuGiuongBenhVien](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NhomGiaDichVuKhamBenhBenhVien]    Script Date: 3/23/2020 4:47:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NhomGiaDichVuKhamBenhBenhVien](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NhomGiaDichVuKyThuatBenhVien]    Script Date: 3/23/2020 4:47:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NhomGiaDichVuKyThuatBenhVien](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NhomICD]    Script Date: 3/23/2020 4:47:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NhomICD](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ChuongICDId] [bigint] NOT NULL,
	[Ma] [nvarchar](20) NOT NULL,
	[TenTiengViet] [nvarchar](250) NOT NULL,
	[TenTiengAnh] [nvarchar](250) NOT NULL,
	[NhomICDChaId] [bigint] NULL,
	[CapNhomICD] [int] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__NhomICD__3214EC074B626BE1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NhomThuoc]    Script Date: 3/23/2020 4:47:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NhomThuoc](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[NhomChaId] [bigint] NULL,
	[CapNhom] [int] NOT NULL,
	[LoaiThuocHoacHoatChat] [int] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__NhomThuo__3214EC07DF1B2880] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NhomVatTu]    Script Date: 3/23/2020 4:47:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NhomVatTu](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ma] [nvarchar](20) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[NhomVatTuChaId] [bigint] NULL,
	[CapNhom] [int] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NoiGioiThieu]    Script Date: 3/23/2020 4:47:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NoiGioiThieu](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[MoTa] [nvarchar](2000) NULL,
	[IsDisabled] [bit] NULL,
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
/****** Object:  Table [dbo].[PhamViHanhNghe]    Script Date: 3/23/2020 4:47:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PhamViHanhNghe](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[Ma] [nvarchar](50) NOT NULL,
	[MoTa] [nvarchar](2000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__PhamViHa__3214EC0730E0B7BD] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PhongBenhVien]    Script Date: 3/23/2020 4:47:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PhongBenhVien](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[KhoaPhongId] [bigint] NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[Ma] [nvarchar](50) NOT NULL,
	[IsDisabled] [bit] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__PhongNgo__3214EC07F07E3DA0] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PhongBenhVienHangDoi]    Script Date: 3/23/2020 4:47:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PhongBenhVienHangDoi](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[PhongBenhVienId] [bigint] NOT NULL,
	[YeuCauTiepNhanId] [bigint] NOT NULL,
	[YeuCauKhamBenhId] [bigint] NULL,
	[YeuCauDichVuKyThuatId] [bigint] NULL,
	[LoaiHangDoi] [int] NOT NULL,
	[TrangThai] [int] NOT NULL,
	[SoThuTu] [int] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__PhongBen__3214EC07636EE260] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PhongBenhVienNhomGiaDichVuKhamBenh]    Script Date: 3/23/2020 4:47:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PhongBenhVienNhomGiaDichVuKhamBenh](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[PhongBenhVienId] [bigint] NOT NULL,
	[NhomGiaDichVuKhamBenhBenhVienId] [bigint] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PhongBenhVienNhomGiaDichVuKyThuat]    Script Date: 3/23/2020 4:47:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PhongBenhVienNhomGiaDichVuKyThuat](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[PhongBenhVienId] [bigint] NOT NULL,
	[NhomGiaDichVuKyThuatBenhVienId] [bigint] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[QuanHeNhanThan]    Script Date: 3/23/2020 4:47:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QuanHeNhanThan](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[TenVietTat] [nvarchar](50) NOT NULL,
	[MoTa] [nvarchar](2000) NULL,
	[IsDisabled] [bit] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__QuanHeNh__3214EC07E2F39D03] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[QueuedCloudMessaging]    Script Date: 3/23/2020 4:47:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QueuedCloudMessaging](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ToUserIds] [nvarchar](2000) NULL,
	[ToMessagingUserGroupIds] [nvarchar](2000) NULL,
	[SentUserIds] [nvarchar](2000) NULL,
	[SentMessagingUserGroupIds] [nvarchar](2000) NULL,
	[MessagingType] [int] NOT NULL,
	[Title] [nvarchar](100) NULL,
	[Body] [nvarchar](2000) NOT NULL,
	[Data] [nvarchar](4000) NULL,
	[Link] [nvarchar](500) NULL,
	[DontSendBeforeDate] [datetime] NULL,
	[SentTries] [int] NOT NULL,
	[SentDoneOn] [datetime] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__QueuedCl__3214EC07B818DC72] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[QueuedEmail]    Script Date: 3/23/2020 4:47:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QueuedEmail](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[To] [nvarchar](250) NOT NULL,
	[Subject] [nvarchar](1000) NULL,
	[Body] [nvarchar](max) NULL,
	[AttachmentFile] [nvarchar](max) NULL,
	[DontSendBeforeDate] [datetime] NULL,
	[SentTries] [int] NOT NULL,
	[SentOn] [datetime] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[QueuedSms]    Script Date: 3/23/2020 4:47:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QueuedSms](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[To] [nvarchar](250) NOT NULL,
	[Body] [nvarchar](max) NULL,
	[DontSendBeforeDate] [datetime] NULL,
	[SentTries] [int] NOT NULL,
	[SentOn] [datetime] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[QuocGia]    Script Date: 3/23/2020 4:47:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QuocGia](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ma] [nvarchar](20) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[TenVietTat] [nvarchar](50) NOT NULL,
	[QuocTich] [nvarchar](250) NOT NULL,
	[MaDienThoaiQuocTe] [nvarchar](100) NULL,
	[ThuDo] [nvarchar](50) NOT NULL,
	[ChauLuc] [int] NOT NULL,
	[IsDisabled] [bit] NULL,
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
/****** Object:  Table [dbo].[Role]    Script Date: 3/23/2020 4:47:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[UserType] [int] NOT NULL,
	[IsDefault] [bit] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RoleFunction]    Script Date: 3/23/2020 4:47:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoleFunction](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[RoleId] [bigint] NOT NULL,
	[SecurityOperation] [int] NOT NULL,
	[DocumentType] [int] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_RoleFunction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TaiKhoanBenhNhan]    Script Date: 3/23/2020 4:47:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaiKhoanBenhNhan](
	[Id] [bigint] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__TaiKhoan__3214EC078A4905BB] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TaiKhoanBenhNhanChi]    Script Date: 3/23/2020 4:47:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaiKhoanBenhNhanChi](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TaiKhoanBenhNhanId] [bigint] NOT NULL,
	[LoaiChiTienBenhNhan] [int] NOT NULL,
	[TienChiPhi] [decimal](15, 2) NULL,
	[TienMat] [decimal](15, 2) NULL,
	[ChuyenKhoan] [decimal](15, 2) NULL,
	[NoiDungChi] [nvarchar](1000) NULL,
	[NgayChi] [datetime] NOT NULL,
	[SoQuyen] [int] NULL,
	[SoPhieu] [int] NULL,
	[TaiKhoanBenhNhanThuId] [bigint] NULL,
	[YeuCauKhamBenhId] [bigint] NULL,
	[YeuCauDichVuKyThuatId] [bigint] NULL,
	[YeuCauDuocPhamBenhVienId] [bigint] NULL,
	[YeuCauVatTuBenhVienId] [bigint] NULL,
	[YeuCauDichVuGiuongBenhVienId] [bigint] NULL,
	[YeuCauGoiDichVuId] [bigint] NULL,
	[DonThuocThanhToanId] [bigint] NULL,
	[YeuCauTiepNhanId] [bigint] NOT NULL,
	[NhanVienThucHienId] [bigint] NULL,
	[NoiThucHienId] [bigint] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__TaiKhoan__3214EC0793C145A9] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TaiKhoanBenhNhanThu]    Script Date: 3/23/2020 4:47:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaiKhoanBenhNhanThu](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TaiKhoanBenhNhanId] [bigint] NOT NULL,
	[YeuCauTiepNhanId] [bigint] NOT NULL,
	[LoaiThuTienBenhNhan] [int] NOT NULL,
	[TienMat] [decimal](15, 2) NULL,
	[ChuyenKhoan] [decimal](15, 2) NULL,
	[POS] [decimal](15, 2) NULL,
	[NoiDungThu] [nvarchar](1000) NULL,
	[NgayThu] [datetime] NOT NULL,
	[SoQuyen] [int] NULL,
	[SoPhieu] [int] NULL,
	[HoanTraYeuCauKhamBenhId] [bigint] NULL,
	[HoanTraYeuCauDichVuKyThuatId] [bigint] NULL,
	[HoanTraYeuCauDuocPhamBenhVienId] [bigint] NULL,
	[HoanTraYeuCauVatTuBenhVienId] [bigint] NULL,
	[HoanTraYeuCauDichVuGiuongBenhVienId] [bigint] NULL,
	[HoanTraYeuCauGoiDichVuId] [bigint] NULL,
	[HoanTraDonThuocThanhToanId] [bigint] NULL,
	[NhanVienThucHienId] [bigint] NOT NULL,
	[NoiThucHienId] [bigint] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__TaiKhoan__3214EC07B61F9328] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Template]    Script Date: 3/23/2020 4:47:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Template](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Title] [nvarchar](500) NULL,
	[Body] [nvarchar](max) NOT NULL,
	[TemplateType] [int] NOT NULL,
	[Language] [int] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Version] [int] NOT NULL,
	[IsDisabled] [bit] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_Template] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TemplateKhamBenhTheoKhoa]    Script Date: 3/23/2020 4:47:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TemplateKhamBenhTheoKhoa](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[KhoaPhongId] [bigint] NOT NULL,
	[Ten] [nvarchar](100) NOT NULL,
	[TieuDe] [nvarchar](500) NULL,
	[ComponentDynamics] [nvarchar](max) NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ThuocHoacHoatChat]    Script Date: 3/23/2020 4:47:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ThuocHoacHoatChat](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[STTHoatChat] [int] NULL,
	[STTThuoc] [int] NULL,
	[Ma] [nvarchar](20) NOT NULL,
	[MaATC] [nvarchar](20) NULL,
	[Ten] [nvarchar](400) NOT NULL,
	[DuongDungId] [bigint] NULL,
	[HoiChan] [bit] NOT NULL,
	[TyLeBaoHiemThanhToan] [bigint] NOT NULL,
	[CoDieuKienThanhToan] [bit] NOT NULL,
	[MoTa] [nvarchar](1000) NULL,
	[NhomChiPhi] [int] NULL,
	[BenhVienHang1] [bit] NULL,
	[BenhVienHang2] [bit] NULL,
	[BenhVienHang3] [bit] NULL,
	[BenhVienHang4] [bit] NULL,
	[NhomThuocId] [bigint] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__ThuocTan__3214EC07E25BC264] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ToaThuocMau]    Script Date: 3/23/2020 4:47:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ToaThuocMau](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](200) NULL,
	[ICDId] [bigint] NULL,
	[TrieuChungId] [bigint] NULL,
	[ChuanDoanId] [bigint] NULL,
	[BacSiKeToaId] [bigint] NOT NULL,
	[GhiChu] [nvarchar](1000) NULL,
	[IsDisabled] [bit] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__ToaThuoc__3214EC07A54DDB18] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ToaThuocMauChiTiet]    Script Date: 3/23/2020 4:47:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ToaThuocMauChiTiet](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ToaThuocMauId] [bigint] NOT NULL,
	[DuocPhamId] [bigint] NOT NULL,
	[SoLuong] [float] NOT NULL,
	[SoNgayDung] [int] NULL,
	[DungSang] [float] NULL,
	[DungTrua] [float] NULL,
	[DungChieu] [float] NULL,
	[DungToi] [float] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TrieuChung]    Script Date: 3/23/2020 4:47:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TrieuChung](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[TrieuChungChaId] [bigint] NULL,
	[CapNhom] [int] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TrieuChungDanhMucChuanDoan]    Script Date: 3/23/2020 4:47:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TrieuChungDanhMucChuanDoan](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TrieuChungId] [bigint] NOT NULL,
	[DanhMucChuanDoanId] [bigint] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 3/23/2020 4:47:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Avatar] [nvarchar](max) NULL,
	[Password] [nvarchar](100) NULL,
	[HoTen] [nvarchar](100) NULL,
	[SoDienThoai] [varchar](20) NOT NULL,
	[Email] [varchar](200) NULL,
	[SoChungMinhThu] [nvarchar](12) NULL,
	[DiaChi] [nvarchar](200) NULL,
	[NgaySinh] [datetime] NULL,
	[GioiTinh] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[PassCode] [nvarchar](50) NULL,
	[ExpiredCodeDate] [datetime] NULL,
	[IsDefault] [bit] NULL,
	[Region] [int] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserMessagingToken]    Script Date: 3/23/2020 4:47:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserMessagingToken](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [bigint] NOT NULL,
	[MessagingToken] [nvarchar](200) NOT NULL,
	[DeviceType] [int] NOT NULL,
	[LastAccess] [datetime] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserMessagingTokenSubscribe]    Script Date: 3/23/2020 4:47:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserMessagingTokenSubscribe](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserMessagingTokenId] [bigint] NOT NULL,
	[Topic] [nvarchar](100) NOT NULL,
	[IsSubscribed] [bit] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VanBangChuyenMon]    Script Date: 3/23/2020 4:47:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VanBangChuyenMon](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ma] [nvarchar](20) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[TenVietTat] [nvarchar](50) NOT NULL,
	[MoTa] [nvarchar](2000) NULL,
	[IsDisabled] [bit] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_TrinhDoChuyenMon] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VatTu]    Script Date: 3/23/2020 4:47:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VatTu](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ma] [nvarchar](50) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[NhomVatTuId] [bigint] NOT NULL,
	[DonViTinh] [nvarchar](50) NULL,
	[TyLeBaoHiemThanhToan] [int] NOT NULL,
	[QuyCach] [nvarchar](250) NULL,
	[NhaSanXuat] [nvarchar](250) NULL,
	[NuocSanXuat] [nvarchar](250) NULL,
	[MoTa] [nvarchar](4000) NULL,
	[IsDisabled] [bit] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__VatTu__3214EC07FB0FCA47] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VatTuBenhVien]    Script Date: 3/23/2020 4:47:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VatTuBenhVien](
	[Id] [bigint] NOT NULL,
	[BaoHiemChiTra] [bit] NOT NULL,
	[TiLeBaoHiemThanhToan] [int] NOT NULL,
	[HieuLuc] [bit] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[XuatKhoDuocPham]    Script Date: 3/23/2020 4:47:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[XuatKhoDuocPham](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[KhoDuocPhamXuatId] [bigint] NOT NULL,
	[KhoDuocPhamNhapId] [bigint] NULL,
	[SoPhieu] [nvarchar](50) NOT NULL,
	[LoaiXuatKho] [int] NOT NULL,
	[LyDoXuatKho] [nvarchar](1000) NOT NULL,
	[TenNguoiNhan] [nvarchar](100) NULL,
	[NguoiNhanId] [bigint] NULL,
	[NguoiXuatId] [bigint] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
	[LoaiNguoiNhan] [int] NOT NULL,
	[NgayXuat] [datetime] NOT NULL,
 CONSTRAINT [PK__XuatKhoD__3214EC07E8286CA2] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[XuatKhoDuocPhamChiTiet]    Script Date: 3/23/2020 4:47:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[XuatKhoDuocPhamChiTiet](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[XuatKhoDuocPhamId] [bigint] NOT NULL,
	[DuocPhamBenhVienId] [bigint] NOT NULL,
	[DatChatLuong] [bit] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
	[NgayXuat] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[XuatKhoDuocPhamChiTietViTri]    Script Date: 3/23/2020 4:47:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[XuatKhoDuocPhamChiTietViTri](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[XuatKhoDuocPhamChiTietId] [bigint] NOT NULL,
	[NhapKhoDuocPhamChiTietId] [bigint] NOT NULL,
	[SoLuongXuat] [float] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
	[NgayXuat] [datetime] NOT NULL,
 CONSTRAINT [PK__XuatKhoD__3214EC075255D995] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[YeuCauDichVuGiuongBenhVien]    Script Date: 3/23/2020 4:47:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[YeuCauDichVuGiuongBenhVien](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[YeuCauTiepNhanId] [bigint] NOT NULL,
	[YeuCauKhamBenhId] [bigint] NULL,
	[YeuCauDichVuKyThuatId] [bigint] NULL,
	[DichVuGiuongBenhVienId] [bigint] NOT NULL,
	[NhomGiaDichVuGiuongBenhVienId] [bigint] NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[Ma] [nvarchar](50) NOT NULL,
	[MaTT37] [nvarchar](50) NOT NULL,
	[LoaiGiuong] [int] NOT NULL,
	[MoTa] [nvarchar](4000) NULL,
	[KhongTinhPhi] [bit] NULL,
	[Gia] [decimal](15, 2) NULL,
	[YeuCauGoiDichVuId] [bigint] NULL,
	[NhanVienChiDinhId] [bigint] NOT NULL,
	[NoiChiDinhId] [bigint] NOT NULL,
	[ThoiDiemChiDinh] [datetime] NOT NULL,
	[NoiThucHienId] [bigint] NULL,
	[MaGiuong] [nvarchar](20) NULL,
	[ThoiDiemBatDauSuDung] [datetime] NULL,
	[ThoiDiemKetThucSuDung] [datetime] NULL,
	[DuocHuongBaoHiem] [bit] NOT NULL,
	[BaoHiemChiTra] [bit] NULL,
	[ThoiDiemDuyetBaoHiem] [datetime] NULL,
	[NhanVienDuyetBaoHiemId] [bigint] NULL,
	[GiaBaoHiemThanhToan] [decimal](15, 2) NULL,
	[TrangThai] [int] NOT NULL,
	[SoTienBenhNhanDaChi] [decimal](15, 2) NULL,
	[TrangThaiThanhToan] [int] NOT NULL,
	[NhanVienHuyThanhToanId] [bigint] NULL,
	[LyDoHuyThanhToan] [nvarchar](1000) NULL,
	[GhiChu] [nvarchar](1000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__YeuCauDi__3214EC07D3829114] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[YeuCauDichVuKyThuat]    Script Date: 3/23/2020 4:47:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[YeuCauDichVuKyThuat](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[YeuCauTiepNhanId] [bigint] NOT NULL,
	[YeuCauKhamBenhId] [bigint] NULL,
	[LoaiDichVuKyThuat] [int] NOT NULL,
	[DichVuKyThuatBenhVienId] [bigint] NOT NULL,
	[NhomGiaDichVuKyThuatBenhVienId] [bigint] NOT NULL,
	[MaDichVu] [nvarchar](50) NOT NULL,
	[MaGiaDichVu] [nvarchar](50) NOT NULL,
	[Ma4350DichVu] [nvarchar](50) NULL,
	[TenDichVu] [nvarchar](250) NOT NULL,
	[TenTiengAnhDichVu] [nvarchar](250) NULL,
	[TenGiaDichVu] [nvarchar](250) NULL,
	[NhomChiPhi] [int] NOT NULL,
	[LoaiPhauThuatThuThuat] [int] NULL,
	[Gia] [decimal](15, 2) NOT NULL,
	[TiLeUuDai] [int] NULL,
	[YeuCauGoiDichVuId] [bigint] NULL,
	[SoLan] [int] NOT NULL,
	[DuocHuongBaoHiem] [bit] NOT NULL,
	[BaoHiemChiTra] [bit] NULL,
	[ThoiDiemDuyetBaoHiem] [datetime] NULL,
	[NhanVienDuyetBaoHiemId] [bigint] NULL,
	[GiaBaoHiemThanhToan] [decimal](15, 2) NULL,
	[ThongTu] [nvarchar](100) NULL,
	[QuyetDinh] [nvarchar](100) NULL,
	[NoiBanHanh] [nvarchar](100) NULL,
	[MoTa] [nvarchar](4000) NULL,
	[TrangThai] [int] NOT NULL,
	[SoTienBenhNhanDaChi] [decimal](15, 2) NULL,
	[TrangThaiThanhToan] [int] NOT NULL,
	[NhanVienHuyThanhToanId] [bigint] NULL,
	[LyDoHuyThanhToan] [nvarchar](1000) NULL,
	[NhanVienChiDinhId] [bigint] NOT NULL,
	[NoiChiDinhId] [bigint] NULL,
	[ThoiDiemChiDinh] [datetime] NOT NULL,
	[ThoiDiemDangKy] [datetime] NOT NULL,
	[NoiThucHienId] [bigint] NULL,
	[MaGiuong] [nvarchar](20) NULL,
	[ThoiDiemThucHien] [datetime] NULL,
	[ThoiDiemHoanThanh] [datetime] NULL,
	[NhanVienThucHienId] [bigint] NULL,
	[KetLuan] [nvarchar](4000) NULL,
	[NhanVienKetLuanId] [bigint] NULL,
	[DeNghi] [nvarchar](1000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__YeuCauKh__3214EC07087F373B] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[YeuCauDuocPhamBenhVien]    Script Date: 3/23/2020 4:47:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[YeuCauDuocPhamBenhVien](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[YeuCauTiepNhanId] [bigint] NOT NULL,
	[YeuCauKhamBenhId] [bigint] NULL,
	[YeuCauDichVuKyThuatId] [bigint] NULL,
	[DuocPhamBenhVienId] [bigint] NOT NULL,
	[XuatKhoDuocPhamChiTietViTriId] [bigint] NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[TenTiengAnh] [nvarchar](250) NULL,
	[SoDangKy] [nvarchar](50) NOT NULL,
	[STTHoatChat] [int] NULL,
	[MaHoatChat] [nvarchar](20) NOT NULL,
	[HoatChat] [nvarchar](500) NOT NULL,
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
	[HopDongThauDuocPhamId] [bigint] NULL,
	[NhaThauId] [bigint] NULL,
	[SoHopDongThau] [nvarchar](50) NULL,
	[SoQuyetDinhThau] [nvarchar](50) NULL,
	[LoaiThau] [int] NULL,
	[LoaiThuocThau] [int] NULL,
	[NhomThau] [nvarchar](50) NULL,
	[GoiThau] [nvarchar](2) NULL,
	[NamThau] [int] NULL,
	[KhongTinhPhi] [bit] NULL,
	[Gia] [decimal](15, 2) NULL,
	[YeuCauGoiDichVuId] [bigint] NULL,
	[SoLuong] [float] NOT NULL,
	[NhanVienChiDinhId] [bigint] NOT NULL,
	[NoiChiDinhId] [bigint] NOT NULL,
	[ThoiDiemChiDinh] [datetime] NOT NULL,
	[NoiCapThuocId] [bigint] NULL,
	[NhanVienCapThuocId] [bigint] NULL,
	[ThoiDiemCapThuoc] [datetime] NULL,
	[DuocHuongBaoHiem] [bit] NOT NULL,
	[BaoHiemChiTra] [bit] NULL,
	[ThoiDiemDuyetBaoHiem] [datetime] NULL,
	[NhanVienDuyetBaoHiemId] [bigint] NULL,
	[GiaBaoHiemThanhToan] [decimal](15, 2) NULL,
	[DaCapThuoc] [bit] NOT NULL,
	[TrangThai] [int] NOT NULL,
	[SoTienBenhNhanDaChi] [decimal](15, 2) NULL,
	[TrangThaiThanhToan] [int] NOT NULL,
	[NhanVienHuyThanhToanId] [bigint] NULL,
	[LyDoHuyThanhToan] [nvarchar](1000) NULL,
	[GhiChu] [nvarchar](1000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__YeuCauKh__3214EC07DB510D30] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[YeuCauGoiDichVu]    Script Date: 3/23/2020 4:47:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[YeuCauGoiDichVu](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[YeuCauTiepNhanId] [bigint] NOT NULL,
	[YeuCauKhamBenhId] [bigint] NULL,
	[GoiDichVuId] [bigint] NOT NULL,
	[LoaiGoiDichVu] [int] NOT NULL,
	[Ten] [nvarchar](200) NOT NULL,
	[CoChietKhau] [bit] NOT NULL,
	[ChiPhiGoiDichVu] [bigint] NOT NULL,
	[MoTa] [nvarchar](4000) NULL,
	[NhanVienChiDinhId] [bigint] NOT NULL,
	[NoiChiDinhId] [bigint] NOT NULL,
	[ThoiDiemChiDinh] [datetime] NOT NULL,
	[TrangThai] [int] NOT NULL,
	[SoTienBenhNhanDaChi] [decimal](15, 2) NULL,
	[TrangThaiThanhToan] [int] NOT NULL,
	[GhiChu] [nvarchar](1000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__YeuCauGo__3214EC0701A51CF4] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[YeuCauKhamBenh]    Script Date: 3/23/2020 4:47:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[YeuCauKhamBenh](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[YeuCauTiepNhanId] [bigint] NOT NULL,
	[DichVuKhamBenhBenhVienId] [bigint] NOT NULL,
	[NhomGiaDichVuKhamBenhBenhVienId] [bigint] NOT NULL,
	[YeuCauKhamBenhTruocId] [bigint] NULL,
	[MaDichVu] [nvarchar](50) NOT NULL,
	[MaDichVuTT37] [nvarchar](50) NOT NULL,
	[TenDichVu] [nvarchar](250) NOT NULL,
	[Gia] [decimal](15, 2) NOT NULL,
	[TiLeUuDai] [int] NULL,
	[YeuCauGoiDichVuId] [bigint] NULL,
	[MoTa] [nvarchar](4000) NULL,
	[DuocHuongBaoHiem] [bit] NOT NULL,
	[BaoHiemChiTra] [bit] NULL,
	[ThoiDiemDuyetBaoHiem] [datetime] NULL,
	[NhanVienDuyetBaoHiemId] [bigint] NULL,
	[GiaBaoHiemThanhToan] [decimal](15, 2) NULL,
	[TrangThai] [int] NOT NULL,
	[SoTienBenhNhanDaChi] [decimal](15, 2) NULL,
	[TrangThaiThanhToan] [int] NOT NULL,
	[NhanVienHuyThanhToanId] [bigint] NULL,
	[LyDoHuyThanhToan] [nvarchar](1000) NULL,
	[KhongTinhPhi] [bit] NULL,
	[NhanVienChiDinhId] [bigint] NOT NULL,
	[NoiChiDinhId] [bigint] NULL,
	[ThoiDiemChiDinh] [datetime] NOT NULL,
	[ThoiDiemDangKy] [datetime] NOT NULL,
	[NoiDangKyId] [bigint] NULL,
	[BacSiDangKyId] [bigint] NULL,
	[ThoiDiemThucHien] [datetime] NULL,
	[NoiThucHienId] [bigint] NULL,
	[BacSiThucHienId] [bigint] NULL,
	[BenhSu] [nvarchar](4000) NULL,
	[ThongTinKhamTheoKhoa] [nvarchar](max) NULL,
	[ICDChinhId] [bigint] NULL,
	[TenBenh] [nvarchar](2000) NULL,
	[NoiKetLuanId] [bigint] NULL,
	[BacSiKetLuanId] [bigint] NULL,
	[ThoiDiemHoanThanh] [datetime] NULL,
	[KetQuaDieuTri] [int] NULL,
	[TinhTrangRaVien] [int] NULL,
	[HuongDieuTri] [nvarchar](2000) NULL,
	[CoKhamChuyenKhoaTiepTheo] [bit] NULL,
	[CoKeToa] [bit] NULL,
	[CoTaiKham] [bit] NULL,
	[NgayTaiKham] [datetime] NULL,
	[GhiChuTaiKham] [nvarchar](2000) NULL,
	[QuayLaiYeuCauKhamBenhTruoc] [bit] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__YeuCauKh__3214EC07873C82DE] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[YeuCauKhamBenhChuanDoan]    Script Date: 3/23/2020 4:47:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[YeuCauKhamBenhChuanDoan](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[YeuCauKhamBenhId] [bigint] NOT NULL,
	[ChuanDoanId] [bigint] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[YeuCauKhamBenhDonThuoc]    Script Date: 3/23/2020 4:47:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[YeuCauKhamBenhDonThuoc](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[YeuCauKhamBenhId] [bigint] NOT NULL,
	[ToaThuocMauId] [bigint] NULL,
	[LoaiDonThuoc] [int] NOT NULL,
	[TrangThai] [int] NOT NULL,
	[BacSiKeDonId] [bigint] NOT NULL,
	[NoiKeDonId] [bigint] NOT NULL,
	[ThoiDiemKeDon] [datetime] NOT NULL,
	[GhiChu] [nvarchar](1000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__YeuCauKh__3214EC07B3AF3DD4] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[YeuCauKhamBenhDonThuocChiTiet]    Script Date: 3/23/2020 4:47:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[YeuCauKhamBenhDonThuocChiTiet](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[YeuCauKhamBenhDonThuocId] [bigint] NOT NULL,
	[DuocPhamId] [bigint] NOT NULL,
	[LaDuocPhamBenhVien] [bit] NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[TenTiengAnh] [nvarchar](250) NULL,
	[SoDangKy] [nvarchar](50) NOT NULL,
	[STTHoatChat] [int] NULL,
	[MaHoatChat] [nvarchar](20) NOT NULL,
	[HoatChat] [nvarchar](500) NOT NULL,
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
	[SoNgayDung] [int] NULL,
	[DungSang] [float] NULL,
	[DungTrua] [float] NULL,
	[DungChieu] [float] NULL,
	[DungToi] [float] NULL,
	[DuocHuongBaoHiem] [bit] NOT NULL,
	[BenhNhanMuaNgoai] [bit] NOT NULL,
	[GhiChu] [nvarchar](1000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__YeuCauKh__3214EC07B00E6575] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[YeuCauKhamBenhICDKhac]    Script Date: 3/23/2020 4:47:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[YeuCauKhamBenhICDKhac](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[YeuCauKhamBenhId] [bigint] NOT NULL,
	[ICDId] [bigint] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[YeuCauKhamBenhLichSuTrangThai]    Script Date: 3/23/2020 4:47:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[YeuCauKhamBenhLichSuTrangThai](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[YeuCauKhamBenhId] [bigint] NOT NULL,
	[TrangThaiYeuCauKhamBenh] [int] NOT NULL,
	[MoTa] [nvarchar](4000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[YeuCauKhamBenhTrieuChung]    Script Date: 3/23/2020 4:47:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[YeuCauKhamBenhTrieuChung](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[YeuCauKhamBenhId] [bigint] NOT NULL,
	[TrieuChungId] [bigint] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[YeuCauTiepNhan]    Script Date: 3/23/2020 4:47:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[YeuCauTiepNhan](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[BenhNhanId] [bigint] NULL,
	[HoTen] [nvarchar](100) NULL,
	[NgaySinh] [int] NULL,
	[ThangSinh] [int] NULL,
	[NamSinh] [int] NULL,
	[SoChungMinhThu] [nvarchar](12) NULL,
	[GioiTinh] [int] NULL,
	[NhomMau] [int] NULL,
	[NgheNghiepId] [bigint] NULL,
	[NoiLamViec] [nvarchar](300) NULL,
	[QuocTichId] [bigint] NULL,
	[DanTocId] [bigint] NULL,
	[DiaChi] [nvarchar](200) NULL,
	[PhuongXaId] [bigint] NULL,
	[QuanHuyenId] [bigint] NULL,
	[TinhThanhId] [bigint] NULL,
	[SoDienThoai] [nvarchar](12) NULL,
	[Email] [nvarchar](200) NULL,
	[NoiGioiThieuId] [bigint] NULL,
	[HinhThucDenId] [bigint] NULL,
	[DuocUuDai] [bit] NULL,
	[DoiTuongUuDaiId] [bigint] NULL,
	[CongTyUuDaiId] [bigint] NULL,
	[NguoiLienHeHoTen] [nvarchar](100) NULL,
	[NguoiLienHeQuanHeNhanThanId] [bigint] NULL,
	[NguoiLienHeSoDienThoai] [nvarchar](12) NULL,
	[NguoiLienHeEmail] [nvarchar](200) NULL,
	[NguoiLienHeDiaChi] [nvarchar](200) NULL,
	[NguoiLienHePhuongXaId] [bigint] NULL,
	[NguoiLienHeQuanHuyenId] [bigint] NULL,
	[NguoiLienHeTinhThanhId] [bigint] NULL,
	[CoBHYT] [bit] NULL,
	[BHYTMaSoThe] [nvarchar](20) NULL,
	[BHYTMucHuong] [int] NULL,
	[BHYTMaDKBD] [nvarchar](20) NULL,
	[BHYTNgayHieuLuc] [datetime] NULL,
	[BHYTNgayHetHan] [datetime] NULL,
	[BHYTDiaChi] [nvarchar](200) NULL,
	[BHYTCoQuanBHXH] [nvarchar](200) NULL,
	[BHYTNgayDu5Nam] [datetime] NULL,
	[BHYTNgayDuocMienCungChiTra] [datetime] NULL,
	[BHYTMaKhuVuc] [nvarchar](5) NULL,
	[BHYTDuocMienCungChiTra] [bit] NULL,
	[BHYTGiayMienCungChiTraId] [bigint] NULL,
	[CoBHTN] [bit] NULL,
	[LoaiYeuCauTiepNhan] [int] NOT NULL,
	[MaYeuCauTiepNhan] [nvarchar](50) NULL,
	[ThoiDiemTiepNhan] [datetime] NOT NULL,
	[NhanVienTiepNhanId] [bigint] NULL,
	[NoiTiepNhanId] [bigint] NULL,
	[LyDoTiepNhan] [int] NULL,
	[LyDoVaoVien] [int] NULL,
	[LaTaiKham] [bit] NULL,
	[TrieuChungTiepNhan] [nvarchar](200) NULL,
	[LoaiTaiNan] [int] NULL,
	[DuocChuyenVien] [bit] NULL,
	[GiayChuyenVienId] [bigint] NULL,
	[ThoiGianChuyenVien] [datetime] NULL,
	[NoiChuyenId] [bigint] NULL,
	[SoChuyenTuyen] [nvarchar](20) NULL,
	[TuyenChuyen] [int] NULL,
	[LyDoChuyen] [nvarchar](2000) NULL,
	[DoiTuongUuTienKhamChuaBenhId] [bigint] NULL,
	[KetQuaDieuTri] [int] NULL,
	[TinhTrangRaVien] [int] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
	[TrangThaiYeuCauTiepNhan] [int] NOT NULL,
	[ThoiDiemCapNhatTrangThai] [datetime] NOT NULL,
	[TinhTrangThe] [int] NULL,
	[IsCheckedBHYT] [bit] NULL,
 CONSTRAINT [PK__YeuCauKh__3214EC07140A722A] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[YeuCauTiepNhanCongTyBaoHiemTuNhan]    Script Date: 3/23/2020 4:47:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[YeuCauTiepNhanCongTyBaoHiemTuNhan](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[YeuCauTiepNhanId] [bigint] NOT NULL,
	[CongTyBaoHiemTuNhanId] [bigint] NOT NULL,
	[MaSoThe] [nvarchar](20) NULL,
	[DiaChi] [nvarchar](200) NULL,
	[SoDienThoai] [nvarchar](20) NULL,
	[NgayHieuLuc] [datetime] NULL,
	[NgayHetHan] [datetime] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[YeuCauTiepNhanLichSuTrangThai]    Script Date: 3/23/2020 4:47:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[YeuCauTiepNhanLichSuTrangThai](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[YeuCauTiepNhanId] [bigint] NOT NULL,
	[TrangThaiYeuCauTiepNhan] [int] NOT NULL,
	[MoTa] [nvarchar](4000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[YeuCauTiepNhanThuocDangSuDung]    Script Date: 3/23/2020 4:47:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[YeuCauTiepNhanThuocDangSuDung](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[YeuCauTiepNhanId] [bigint] NOT NULL,
	[DuocPhamId] [bigint] NOT NULL,
	[SoNgayDaSuDung] [int] NULL,
	[LieuDung] [nvarchar](1000) NULL,
	[DuocKeTheoDon] [bit] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[YeuCauVatTuBenhVien]    Script Date: 3/23/2020 4:47:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[YeuCauVatTuBenhVien](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[YeuCauTiepNhanId] [bigint] NOT NULL,
	[YeuCauKhamBenhId] [bigint] NULL,
	[YeuCauDichVuKyThuatId] [bigint] NULL,
	[VatTuBenhVienId] [bigint] NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[Ma] [nvarchar](50) NOT NULL,
	[NhomVatTuId] [bigint] NOT NULL,
	[DonViTinh] [nvarchar](50) NULL,
	[NhaSanXuat] [nvarchar](250) NULL,
	[NuocSanXuat] [nvarchar](250) NULL,
	[QuyCach] [nvarchar](250) NULL,
	[TieuChuan] [nvarchar](50) NULL,
	[MoTa] [nvarchar](4000) NULL,
	[KhongTinhPhi] [bit] NULL,
	[Gia] [decimal](15, 2) NULL,
	[YeuCauGoiDichVuId] [bigint] NULL,
	[SoLuong] [float] NOT NULL,
	[NhanVienChiDinhId] [bigint] NOT NULL,
	[NoiChiDinhId] [bigint] NOT NULL,
	[ThoiDiemChiDinh] [datetime] NOT NULL,
	[NoiCapVatTuId] [bigint] NULL,
	[NhanVienCapVatTuId] [bigint] NULL,
	[ThoiDiemCapVatTu] [datetime] NULL,
	[DuocHuongBaoHiem] [bit] NOT NULL,
	[BaoHiemChiTra] [bit] NULL,
	[ThoiDiemDuyetBaoHiem] [datetime] NULL,
	[NhanVienDuyetBaoHiemId] [bigint] NULL,
	[GiaBaoHiemThanhToan] [decimal](15, 2) NULL,
	[DaCapVatTu] [bit] NOT NULL,
	[TrangThai] [int] NOT NULL,
	[SoTienBenhNhanDaChi] [decimal](15, 2) NULL,
	[TrangThaiThanhToan] [int] NOT NULL,
	[NhanVienHuyThanhToanId] [bigint] NULL,
	[LyDoHuyThanhToan] [nvarchar](1000) NULL,
	[GhiChu] [nvarchar](1000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__YeuCauKh__3214EC07FEA0896C] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_Region]  DEFAULT ((1)) FOR [Region]
GO
ALTER TABLE [dbo].[ADR]  WITH CHECK ADD  CONSTRAINT [FK__ADR__ThuocTanDuo__336AA144] FOREIGN KEY([ThuocHoacHoatChat1Id])
REFERENCES [dbo].[ThuocHoacHoatChat] ([Id])
GO
ALTER TABLE [dbo].[ADR] CHECK CONSTRAINT [FK__ADR__ThuocTanDuo__336AA144]
GO
ALTER TABLE [dbo].[ADR]  WITH CHECK ADD  CONSTRAINT [FK__ADR__ThuocTanDuo__345EC57D] FOREIGN KEY([ThuocHoacHoatChat2Id])
REFERENCES [dbo].[ThuocHoacHoatChat] ([Id])
GO
ALTER TABLE [dbo].[ADR] CHECK CONSTRAINT [FK__ADR__ThuocTanDuo__345EC57D]
GO
ALTER TABLE [dbo].[AuditColumn]  WITH CHECK ADD  CONSTRAINT [FK_AuditColumn_AuditTable] FOREIGN KEY([AuditTableId])
REFERENCES [dbo].[AuditTable] ([Id])
GO
ALTER TABLE [dbo].[AuditColumn] CHECK CONSTRAINT [FK_AuditColumn_AuditTable]
GO
ALTER TABLE [dbo].[BenhNhan]  WITH CHECK ADD  CONSTRAINT [FK_BenhNhan_DanToc] FOREIGN KEY([DanTocId])
REFERENCES [dbo].[DanToc] ([Id])
GO
ALTER TABLE [dbo].[BenhNhan] CHECK CONSTRAINT [FK_BenhNhan_DanToc]
GO
ALTER TABLE [dbo].[BenhNhan]  WITH CHECK ADD  CONSTRAINT [FK_BenhNhan_DonViHanhChinh] FOREIGN KEY([PhuongXaId])
REFERENCES [dbo].[DonViHanhChinh] ([Id])
GO
ALTER TABLE [dbo].[BenhNhan] CHECK CONSTRAINT [FK_BenhNhan_DonViHanhChinh]
GO
ALTER TABLE [dbo].[BenhNhan]  WITH CHECK ADD  CONSTRAINT [FK_BenhNhan_DonViHanhChinh1] FOREIGN KEY([QuanHuyenId])
REFERENCES [dbo].[DonViHanhChinh] ([Id])
GO
ALTER TABLE [dbo].[BenhNhan] CHECK CONSTRAINT [FK_BenhNhan_DonViHanhChinh1]
GO
ALTER TABLE [dbo].[BenhNhan]  WITH CHECK ADD  CONSTRAINT [FK_BenhNhan_DonViHanhChinh2] FOREIGN KEY([TinhThanhId])
REFERENCES [dbo].[DonViHanhChinh] ([Id])
GO
ALTER TABLE [dbo].[BenhNhan] CHECK CONSTRAINT [FK_BenhNhan_DonViHanhChinh2]
GO
ALTER TABLE [dbo].[BenhNhan]  WITH CHECK ADD  CONSTRAINT [FK_BenhNhan_DonViHanhChinh3] FOREIGN KEY([NguoiLienHePhuongXaId])
REFERENCES [dbo].[DonViHanhChinh] ([Id])
GO
ALTER TABLE [dbo].[BenhNhan] CHECK CONSTRAINT [FK_BenhNhan_DonViHanhChinh3]
GO
ALTER TABLE [dbo].[BenhNhan]  WITH CHECK ADD  CONSTRAINT [FK_BenhNhan_DonViHanhChinh4] FOREIGN KEY([NguoiLienHeQuanHuyenId])
REFERENCES [dbo].[DonViHanhChinh] ([Id])
GO
ALTER TABLE [dbo].[BenhNhan] CHECK CONSTRAINT [FK_BenhNhan_DonViHanhChinh4]
GO
ALTER TABLE [dbo].[BenhNhan]  WITH CHECK ADD  CONSTRAINT [FK_BenhNhan_DonViHanhChinh5] FOREIGN KEY([NguoiLienHeTinhThanhId])
REFERENCES [dbo].[DonViHanhChinh] ([Id])
GO
ALTER TABLE [dbo].[BenhNhan] CHECK CONSTRAINT [FK_BenhNhan_DonViHanhChinh5]
GO
ALTER TABLE [dbo].[BenhNhan]  WITH CHECK ADD  CONSTRAINT [FK_BenhNhan_GiayMienCungChiTra] FOREIGN KEY([BHYTGiayMienCungChiTraId])
REFERENCES [dbo].[GiayMienCungChiTra] ([Id])
GO
ALTER TABLE [dbo].[BenhNhan] CHECK CONSTRAINT [FK_BenhNhan_GiayMienCungChiTra]
GO
ALTER TABLE [dbo].[BenhNhan]  WITH CHECK ADD  CONSTRAINT [FK_BenhNhan_NgheNghiep] FOREIGN KEY([NgheNghiepId])
REFERENCES [dbo].[NgheNghiep] ([Id])
GO
ALTER TABLE [dbo].[BenhNhan] CHECK CONSTRAINT [FK_BenhNhan_NgheNghiep]
GO
ALTER TABLE [dbo].[BenhNhan]  WITH CHECK ADD  CONSTRAINT [FK_BenhNhan_QuanHeNhanThan] FOREIGN KEY([NguoiLienHeQuanHeNhanThanId])
REFERENCES [dbo].[QuanHeNhanThan] ([Id])
GO
ALTER TABLE [dbo].[BenhNhan] CHECK CONSTRAINT [FK_BenhNhan_QuanHeNhanThan]
GO
ALTER TABLE [dbo].[BenhNhan]  WITH CHECK ADD  CONSTRAINT [FK_BenhNhan_QuocGia] FOREIGN KEY([QuocTichId])
REFERENCES [dbo].[QuocGia] ([Id])
GO
ALTER TABLE [dbo].[BenhNhan] CHECK CONSTRAINT [FK_BenhNhan_QuocGia]
GO
ALTER TABLE [dbo].[BenhNhanCongTyBaoHiemTuNhan]  WITH CHECK ADD  CONSTRAINT [FK_BenhNhanCongTyBaoHiemTuNhan_BenhNhan] FOREIGN KEY([BenhNhanId])
REFERENCES [dbo].[BenhNhan] ([Id])
GO
ALTER TABLE [dbo].[BenhNhanCongTyBaoHiemTuNhan] CHECK CONSTRAINT [FK_BenhNhanCongTyBaoHiemTuNhan_BenhNhan]
GO
ALTER TABLE [dbo].[BenhNhanCongTyBaoHiemTuNhan]  WITH CHECK ADD  CONSTRAINT [FK_BenhNhanCongTyBaoHiemTuNhan_CongTyBaoHiemTuNhan] FOREIGN KEY([CongTyBaoHiemTuNhanId])
REFERENCES [dbo].[CongTyBaoHiemTuNhan] ([Id])
GO
ALTER TABLE [dbo].[BenhNhanCongTyBaoHiemTuNhan] CHECK CONSTRAINT [FK_BenhNhanCongTyBaoHiemTuNhan_CongTyBaoHiemTuNhan]
GO
ALTER TABLE [dbo].[BenhNhanDiUngThuoc]  WITH CHECK ADD  CONSTRAINT [FK_BenhNhanDiUngThuoc_BenhNhan] FOREIGN KEY([BenhNhanId])
REFERENCES [dbo].[BenhNhan] ([Id])
GO
ALTER TABLE [dbo].[BenhNhanDiUngThuoc] CHECK CONSTRAINT [FK_BenhNhanDiUngThuoc_BenhNhan]
GO
ALTER TABLE [dbo].[BenhNhanTienSuBenh]  WITH CHECK ADD  CONSTRAINT [FK_BenhNhanTienSuBenh_BenhNhan] FOREIGN KEY([BenhNhanId])
REFERENCES [dbo].[BenhNhan] ([Id])
GO
ALTER TABLE [dbo].[BenhNhanTienSuBenh] CHECK CONSTRAINT [FK_BenhNhanTienSuBenh_BenhNhan]
GO
ALTER TABLE [dbo].[BenhNhanTienSuBenh]  WITH CHECK ADD  CONSTRAINT [FK_BenhNhanTienSuBenh_ICD] FOREIGN KEY([ICDId])
REFERENCES [dbo].[ICD] ([Id])
GO
ALTER TABLE [dbo].[BenhNhanTienSuBenh] CHECK CONSTRAINT [FK_BenhNhanTienSuBenh_ICD]
GO
ALTER TABLE [dbo].[BenhVien]  WITH CHECK ADD  CONSTRAINT [FK__BenhVien__DonViH__2BFE89A6] FOREIGN KEY([DonViHanhChinhId])
REFERENCES [dbo].[DonViHanhChinh] ([Id])
GO
ALTER TABLE [dbo].[BenhVien] CHECK CONSTRAINT [FK__BenhVien__DonViH__2BFE89A6]
GO
ALTER TABLE [dbo].[BenhVien]  WITH CHECK ADD  CONSTRAINT [FK__BenhVien__LoaiBe__2A164134] FOREIGN KEY([LoaiBenhVienId])
REFERENCES [dbo].[LoaiBenhVien] ([Id])
GO
ALTER TABLE [dbo].[BenhVien] CHECK CONSTRAINT [FK__BenhVien__LoaiBe__2A164134]
GO
ALTER TABLE [dbo].[CauHinhTheoThoiGianChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_CauHinhTheoThoiGianChiTiet_CauHinhTheoThoiGian] FOREIGN KEY([CauHinhTheoThoiGianId])
REFERENCES [dbo].[CauHinhTheoThoiGian] ([Id])
GO
ALTER TABLE [dbo].[CauHinhTheoThoiGianChiTiet] CHECK CONSTRAINT [FK_CauHinhTheoThoiGianChiTiet_CauHinhTheoThoiGian]
GO
ALTER TABLE [dbo].[ChuanDoan]  WITH CHECK ADD FOREIGN KEY([DanhMucChuanDoanId])
REFERENCES [dbo].[DanhMucChuanDoan] ([Id])
GO
ALTER TABLE [dbo].[ChuanDoan]  WITH CHECK ADD FOREIGN KEY([DanhMucChuanDoanId])
REFERENCES [dbo].[DanhMucChuanDoan] ([Id])
GO
ALTER TABLE [dbo].[ChuanDoan]  WITH CHECK ADD FOREIGN KEY([DanhMucChuanDoanId])
REFERENCES [dbo].[DanhMucChuanDoan] ([Id])
GO
ALTER TABLE [dbo].[ChuanDoan]  WITH CHECK ADD FOREIGN KEY([DanhMucChuanDoanId])
REFERENCES [dbo].[DanhMucChuanDoan] ([Id])
GO
ALTER TABLE [dbo].[ChuanDoanLienKetICD]  WITH CHECK ADD FOREIGN KEY([ChuanDoanId])
REFERENCES [dbo].[ChuanDoan] ([Id])
GO
ALTER TABLE [dbo].[ChuanDoanLienKetICD]  WITH CHECK ADD FOREIGN KEY([ChuanDoanId])
REFERENCES [dbo].[ChuanDoan] ([Id])
GO
ALTER TABLE [dbo].[ChuanDoanLienKetICD]  WITH CHECK ADD FOREIGN KEY([ChuanDoanId])
REFERENCES [dbo].[ChuanDoan] ([Id])
GO
ALTER TABLE [dbo].[ChuanDoanLienKetICD]  WITH CHECK ADD FOREIGN KEY([ChuanDoanId])
REFERENCES [dbo].[ChuanDoan] ([Id])
GO
ALTER TABLE [dbo].[ChuanDoanLienKetICD]  WITH CHECK ADD  CONSTRAINT [FK__DRGICDChi__ICDId__4A8310C6] FOREIGN KEY([ICDId])
REFERENCES [dbo].[ICD] ([Id])
GO
ALTER TABLE [dbo].[ChuanDoanLienKetICD] CHECK CONSTRAINT [FK__DRGICDChi__ICDId__4A8310C6]
GO
ALTER TABLE [dbo].[CongTyBaoHiemTuNhanCongNo]  WITH CHECK ADD  CONSTRAINT [FK_CongTyBaoHiemTuNhanCongNo_CongTyBaoHiemTuNhan] FOREIGN KEY([CongTyBaoHiemTuNhanId])
REFERENCES [dbo].[CongTyBaoHiemTuNhan] ([Id])
GO
ALTER TABLE [dbo].[CongTyBaoHiemTuNhanCongNo] CHECK CONSTRAINT [FK_CongTyBaoHiemTuNhanCongNo_CongTyBaoHiemTuNhan]
GO
ALTER TABLE [dbo].[CongTyBaoHiemTuNhanCongNo]  WITH CHECK ADD  CONSTRAINT [FK_CongTyBaoHiemTuNhanCongNo_TaiKhoanBenhNhanThu] FOREIGN KEY([TaiKhoanBenhNhanThuId])
REFERENCES [dbo].[TaiKhoanBenhNhanThu] ([Id])
GO
ALTER TABLE [dbo].[CongTyBaoHiemTuNhanCongNo] CHECK CONSTRAINT [FK_CongTyBaoHiemTuNhanCongNo_TaiKhoanBenhNhanThu]
GO
ALTER TABLE [dbo].[DichVuGiuong]  WITH CHECK ADD  CONSTRAINT [FK_DichVuGiuong_Khoa] FOREIGN KEY([KhoaId])
REFERENCES [dbo].[Khoa] ([Id])
GO
ALTER TABLE [dbo].[DichVuGiuong] CHECK CONSTRAINT [FK_DichVuGiuong_Khoa]
GO
ALTER TABLE [dbo].[DichVuGiuongBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_DichVuGiuongBenhVien_DichVuGiuong] FOREIGN KEY([DichVuGiuongId])
REFERENCES [dbo].[DichVuGiuong] ([Id])
GO
ALTER TABLE [dbo].[DichVuGiuongBenhVien] CHECK CONSTRAINT [FK_DichVuGiuongBenhVien_DichVuGiuong]
GO
ALTER TABLE [dbo].[DichVuGiuongBenhVienGiaBaoHiem]  WITH CHECK ADD  CONSTRAINT [FK_DichVuGiuongBenhVienGiaBaoHiem_DichVuGiuongBenhVien] FOREIGN KEY([DichVuGiuongBenhVienId])
REFERENCES [dbo].[DichVuGiuongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[DichVuGiuongBenhVienGiaBaoHiem] CHECK CONSTRAINT [FK_DichVuGiuongBenhVienGiaBaoHiem_DichVuGiuongBenhVien]
GO
ALTER TABLE [dbo].[DichVuGiuongBenhVienGiaBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_DichVuGiuongBenhVienGiaBenhVien_DichVuGiuongBenhVien] FOREIGN KEY([DichVuGiuongBenhVienId])
REFERENCES [dbo].[DichVuGiuongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[DichVuGiuongBenhVienGiaBenhVien] CHECK CONSTRAINT [FK_DichVuGiuongBenhVienGiaBenhVien_DichVuGiuongBenhVien]
GO
ALTER TABLE [dbo].[DichVuGiuongBenhVienGiaBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_DichVuGiuongBenhVienGiaBenhVien_NhomGiaDichVuGiuongBenhVien] FOREIGN KEY([NhomGiaDichVuGiuongBenhVienId])
REFERENCES [dbo].[NhomGiaDichVuGiuongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[DichVuGiuongBenhVienGiaBenhVien] CHECK CONSTRAINT [FK_DichVuGiuongBenhVienGiaBenhVien_NhomGiaDichVuGiuongBenhVien]
GO
ALTER TABLE [dbo].[DichVuGiuongThongTinGia]  WITH CHECK ADD  CONSTRAINT [FK_DichVuGiuongThongTinGia_DichVuGiuong] FOREIGN KEY([DichVuGiuongId])
REFERENCES [dbo].[DichVuGiuong] ([Id])
GO
ALTER TABLE [dbo].[DichVuGiuongThongTinGia] CHECK CONSTRAINT [FK_DichVuGiuongThongTinGia_DichVuGiuong]
GO
ALTER TABLE [dbo].[DichVuKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK__DichVuKhamBenh__Khoa__2A164134] FOREIGN KEY([KhoaId])
REFERENCES [dbo].[Khoa] ([Id])
GO
ALTER TABLE [dbo].[DichVuKhamBenh] CHECK CONSTRAINT [FK__DichVuKhamBenh__Khoa__2A164134]
GO
ALTER TABLE [dbo].[DichVuKhamBenhBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_DichVuKhamBenhBenhVien_DichVuKhamBenh] FOREIGN KEY([DichVuKhamBenhId])
REFERENCES [dbo].[DichVuKhamBenh] ([Id])
GO
ALTER TABLE [dbo].[DichVuKhamBenhBenhVien] CHECK CONSTRAINT [FK_DichVuKhamBenhBenhVien_DichVuKhamBenh]
GO
ALTER TABLE [dbo].[DichVuKhamBenhBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_DichVuKhamBenhBenhVien_KhoaPhong] FOREIGN KEY([KhoaPhongId])
REFERENCES [dbo].[KhoaPhong] ([Id])
GO
ALTER TABLE [dbo].[DichVuKhamBenhBenhVien] CHECK CONSTRAINT [FK_DichVuKhamBenhBenhVien_KhoaPhong]
GO
ALTER TABLE [dbo].[DichVuKhamBenhBenhVienGiaBaoHiem]  WITH CHECK ADD  CONSTRAINT [FK_DichVuKhamBenhBenhVienGiaBaoHiem_DichVuKhamBenhBenhVien] FOREIGN KEY([DichVuKhamBenhBenhVienId])
REFERENCES [dbo].[DichVuKhamBenhBenhVien] ([Id])
GO
ALTER TABLE [dbo].[DichVuKhamBenhBenhVienGiaBaoHiem] CHECK CONSTRAINT [FK_DichVuKhamBenhBenhVienGiaBaoHiem_DichVuKhamBenhBenhVien]
GO
ALTER TABLE [dbo].[DichVuKhamBenhBenhVienGiaBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_DichVuKhamBenhBenhVienGiaBenhVien_DichVuKhamBenhBenhVien] FOREIGN KEY([DichVuKhamBenhBenhVienId])
REFERENCES [dbo].[DichVuKhamBenhBenhVien] ([Id])
GO
ALTER TABLE [dbo].[DichVuKhamBenhBenhVienGiaBenhVien] CHECK CONSTRAINT [FK_DichVuKhamBenhBenhVienGiaBenhVien_DichVuKhamBenhBenhVien]
GO
ALTER TABLE [dbo].[DichVuKhamBenhBenhVienGiaBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_DichVuKhamBenhBenhVienGiaBenhVien_NhomGiaDichVuKhamBenhBenhVien] FOREIGN KEY([NhomGiaDichVuKhamBenhBenhVienId])
REFERENCES [dbo].[NhomGiaDichVuKhamBenhBenhVien] ([Id])
GO
ALTER TABLE [dbo].[DichVuKhamBenhBenhVienGiaBenhVien] CHECK CONSTRAINT [FK_DichVuKhamBenhBenhVienGiaBenhVien_NhomGiaDichVuKhamBenhBenhVien]
GO
ALTER TABLE [dbo].[DichVuKhamBenhThongTinGia]  WITH CHECK ADD  CONSTRAINT [FK_DichVuKhamBenhThongTinGia_DichVuKhamBenh] FOREIGN KEY([DichVuKhamBenhId])
REFERENCES [dbo].[DichVuKhamBenh] ([Id])
GO
ALTER TABLE [dbo].[DichVuKhamBenhThongTinGia] CHECK CONSTRAINT [FK_DichVuKhamBenhThongTinGia_DichVuKhamBenh]
GO
ALTER TABLE [dbo].[DichVuKyThuat]  WITH CHECK ADD  CONSTRAINT [FK_DichVuKyThuat_NhomDichVuKyThuat] FOREIGN KEY([NhomDichVuKyThuatId])
REFERENCES [dbo].[NhomDichVuKyThuat] ([Id])
GO
ALTER TABLE [dbo].[DichVuKyThuat] CHECK CONSTRAINT [FK_DichVuKyThuat_NhomDichVuKyThuat]
GO
ALTER TABLE [dbo].[DichVuKyThuatBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_DichVuKyThuatBenhVien_DichVuKyThuat] FOREIGN KEY([DichVuKyThuatId])
REFERENCES [dbo].[DichVuKyThuat] ([Id])
GO
ALTER TABLE [dbo].[DichVuKyThuatBenhVien] CHECK CONSTRAINT [FK_DichVuKyThuatBenhVien_DichVuKyThuat]
GO
ALTER TABLE [dbo].[DichVuKyThuatBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_DichVuKyThuatBenhVien_KhoaPhong] FOREIGN KEY([KhoaPhongId])
REFERENCES [dbo].[KhoaPhong] ([Id])
GO
ALTER TABLE [dbo].[DichVuKyThuatBenhVien] CHECK CONSTRAINT [FK_DichVuKyThuatBenhVien_KhoaPhong]
GO
ALTER TABLE [dbo].[DichVuKyThuatBenhVienGiaBaoHiem]  WITH CHECK ADD  CONSTRAINT [FK_DichVuKyThuatBenhVienGiaBaoHiem_DichVuKyThuatBenhVien] FOREIGN KEY([DichVuKyThuatBenhVienId])
REFERENCES [dbo].[DichVuKyThuatBenhVien] ([Id])
GO
ALTER TABLE [dbo].[DichVuKyThuatBenhVienGiaBaoHiem] CHECK CONSTRAINT [FK_DichVuKyThuatBenhVienGiaBaoHiem_DichVuKyThuatBenhVien]
GO
ALTER TABLE [dbo].[DichVuKyThuatBenhVienGiaBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_DichVuKyThuatBenhVienGiaBenhVien_DichVuKyThuatBenhVien] FOREIGN KEY([DichVuKyThuatBenhVienId])
REFERENCES [dbo].[DichVuKyThuatBenhVien] ([Id])
GO
ALTER TABLE [dbo].[DichVuKyThuatBenhVienGiaBenhVien] CHECK CONSTRAINT [FK_DichVuKyThuatBenhVienGiaBenhVien_DichVuKyThuatBenhVien]
GO
ALTER TABLE [dbo].[DichVuKyThuatBenhVienGiaBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_DichVuKyThuatBenhVienGiaBenhVien_NhomGiaDichVuKyThuatBenhVien] FOREIGN KEY([NhomGiaDichVuKyThuatBenhVienId])
REFERENCES [dbo].[NhomGiaDichVuKyThuatBenhVien] ([Id])
GO
ALTER TABLE [dbo].[DichVuKyThuatBenhVienGiaBenhVien] CHECK CONSTRAINT [FK_DichVuKyThuatBenhVienGiaBenhVien_NhomGiaDichVuKyThuatBenhVien]
GO
ALTER TABLE [dbo].[DichVuKyThuatThongTinGia]  WITH CHECK ADD  CONSTRAINT [FK_DichVuKyThuatThongTinGia_DichVuKyThuat] FOREIGN KEY([DichVuKyThuatId])
REFERENCES [dbo].[DichVuKyThuat] ([Id])
GO
ALTER TABLE [dbo].[DichVuKyThuatThongTinGia] CHECK CONSTRAINT [FK_DichVuKyThuatThongTinGia_DichVuKyThuat]
GO
ALTER TABLE [dbo].[DinhMucDuocPhamTonKho]  WITH CHECK ADD  CONSTRAINT [FK_DinhMucDuocPhamTonKho_DuocPhamBenhVien] FOREIGN KEY([DuocPhamBenhVienId])
REFERENCES [dbo].[DuocPhamBenhVien] ([Id])
GO
ALTER TABLE [dbo].[DinhMucDuocPhamTonKho] CHECK CONSTRAINT [FK_DinhMucDuocPhamTonKho_DuocPhamBenhVien]
GO
ALTER TABLE [dbo].[DinhMucDuocPhamTonKho]  WITH CHECK ADD  CONSTRAINT [FK_DinhMucDuocPhamTonKho_KhoDuocPham] FOREIGN KEY([KhoDuocPhamId])
REFERENCES [dbo].[KhoDuocPham] ([Id])
GO
ALTER TABLE [dbo].[DinhMucDuocPhamTonKho] CHECK CONSTRAINT [FK_DinhMucDuocPhamTonKho_KhoDuocPham]
GO
ALTER TABLE [dbo].[DoiTuongUuDaiDichVuKhamBenhBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_DoiTuongUuDaiDichVuKhamBenhBenhVien_DichVuKhamBenhBenhVien] FOREIGN KEY([DichVuKhamBenhBenhVienId])
REFERENCES [dbo].[DichVuKhamBenhBenhVien] ([Id])
GO
ALTER TABLE [dbo].[DoiTuongUuDaiDichVuKhamBenhBenhVien] CHECK CONSTRAINT [FK_DoiTuongUuDaiDichVuKhamBenhBenhVien_DichVuKhamBenhBenhVien]
GO
ALTER TABLE [dbo].[DoiTuongUuDaiDichVuKhamBenhBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_DoiTuongUuDaiDichVuKhamBenhBenhVien_DoiTuongUuDai] FOREIGN KEY([DoiTuongUuDaiId])
REFERENCES [dbo].[DoiTuongUuDai] ([Id])
GO
ALTER TABLE [dbo].[DoiTuongUuDaiDichVuKhamBenhBenhVien] CHECK CONSTRAINT [FK_DoiTuongUuDaiDichVuKhamBenhBenhVien_DoiTuongUuDai]
GO
ALTER TABLE [dbo].[DoiTuongUuDaiDichVuKyThuatBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_DoiTuongUuDaiDichVuKyThuatBenhVien_DichVuKyThuatBenhVien] FOREIGN KEY([DichVuKyThuatBenhVienId])
REFERENCES [dbo].[DichVuKyThuatBenhVien] ([Id])
GO
ALTER TABLE [dbo].[DoiTuongUuDaiDichVuKyThuatBenhVien] CHECK CONSTRAINT [FK_DoiTuongUuDaiDichVuKyThuatBenhVien_DichVuKyThuatBenhVien]
GO
ALTER TABLE [dbo].[DoiTuongUuDaiDichVuKyThuatBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_DoiTuongUuDaiDichVuKyThuatBenhVien_DoiTuongUuDai] FOREIGN KEY([DoiTuongUuDaiId])
REFERENCES [dbo].[DoiTuongUuDai] ([Id])
GO
ALTER TABLE [dbo].[DoiTuongUuDaiDichVuKyThuatBenhVien] CHECK CONSTRAINT [FK_DoiTuongUuDaiDichVuKyThuatBenhVien_DoiTuongUuDai]
GO
ALTER TABLE [dbo].[DonThuocThanhToan]  WITH CHECK ADD  CONSTRAINT [FK_DonThuocThanhToan_BenhNhan] FOREIGN KEY([BenhNhanId])
REFERENCES [dbo].[BenhNhan] ([Id])
GO
ALTER TABLE [dbo].[DonThuocThanhToan] CHECK CONSTRAINT [FK_DonThuocThanhToan_BenhNhan]
GO
ALTER TABLE [dbo].[DonThuocThanhToan]  WITH CHECK ADD  CONSTRAINT [FK_DonThuocThanhToan_YeuCauTiepNhan] FOREIGN KEY([YeuCauTiepNhanId])
REFERENCES [dbo].[YeuCauTiepNhan] ([Id])
GO
ALTER TABLE [dbo].[DonThuocThanhToan] CHECK CONSTRAINT [FK_DonThuocThanhToan_YeuCauTiepNhan]
GO
ALTER TABLE [dbo].[DonThuocThanhToan]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhDonThuocThanhToan_NhanVien] FOREIGN KEY([NhanVienHuyThanhToanId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[DonThuocThanhToan] CHECK CONSTRAINT [FK_YeuCauKhamBenhDonThuocThanhToan_NhanVien]
GO
ALTER TABLE [dbo].[DonThuocThanhToan]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhDonThuocThanhToan_YeuCauKhamBenh] FOREIGN KEY([YeuCauKhamBenhId])
REFERENCES [dbo].[YeuCauKhamBenh] ([Id])
GO
ALTER TABLE [dbo].[DonThuocThanhToan] CHECK CONSTRAINT [FK_YeuCauKhamBenhDonThuocThanhToan_YeuCauKhamBenh]
GO
ALTER TABLE [dbo].[DonThuocThanhToan]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhDonThuocThanhToan_YeuCauKhamBenhDonThuoc] FOREIGN KEY([YeuCauKhamBenhDonThuocId])
REFERENCES [dbo].[YeuCauKhamBenhDonThuoc] ([Id])
GO
ALTER TABLE [dbo].[DonThuocThanhToan] CHECK CONSTRAINT [FK_YeuCauKhamBenhDonThuocThanhToan_YeuCauKhamBenhDonThuoc]
GO
ALTER TABLE [dbo].[DonThuocThanhToanChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_DonThuocThanhToanChiTiet_DonViTinh] FOREIGN KEY([DonViTinhId])
REFERENCES [dbo].[DonViTinh] ([Id])
GO
ALTER TABLE [dbo].[DonThuocThanhToanChiTiet] CHECK CONSTRAINT [FK_DonThuocThanhToanChiTiet_DonViTinh]
GO
ALTER TABLE [dbo].[DonThuocThanhToanChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_DonThuocThanhToanChiTiet_DuongDung] FOREIGN KEY([DuongDungId])
REFERENCES [dbo].[DuongDung] ([Id])
GO
ALTER TABLE [dbo].[DonThuocThanhToanChiTiet] CHECK CONSTRAINT [FK_DonThuocThanhToanChiTiet_DuongDung]
GO
ALTER TABLE [dbo].[DonThuocThanhToanChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_DonThuocThanhToanChiTiet_XuatKhoDuocPhamChiTietViTri] FOREIGN KEY([XuatKhoDuocPhamChiTietViTriId])
REFERENCES [dbo].[XuatKhoDuocPhamChiTietViTri] ([Id])
GO
ALTER TABLE [dbo].[DonThuocThanhToanChiTiet] CHECK CONSTRAINT [FK_DonThuocThanhToanChiTiet_XuatKhoDuocPhamChiTietViTri]
GO
ALTER TABLE [dbo].[DonThuocThanhToanChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhDonThuocThanhToanChiTiet_DuocPham] FOREIGN KEY([DuocPhamId])
REFERENCES [dbo].[DuocPham] ([Id])
GO
ALTER TABLE [dbo].[DonThuocThanhToanChiTiet] CHECK CONSTRAINT [FK_YeuCauKhamBenhDonThuocThanhToanChiTiet_DuocPham]
GO
ALTER TABLE [dbo].[DonThuocThanhToanChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhDonThuocThanhToanChiTiet_YeuCauKhamBenhDonThuocThanhToan] FOREIGN KEY([DonThuocThanhToanId])
REFERENCES [dbo].[DonThuocThanhToan] ([Id])
GO
ALTER TABLE [dbo].[DonThuocThanhToanChiTiet] CHECK CONSTRAINT [FK_YeuCauKhamBenhDonThuocThanhToanChiTiet_YeuCauKhamBenhDonThuocThanhToan]
GO
ALTER TABLE [dbo].[DuocPham]  WITH CHECK ADD  CONSTRAINT [FK_DuocPham_DonViTinh] FOREIGN KEY([DonViTinhId])
REFERENCES [dbo].[DonViTinh] ([Id])
GO
ALTER TABLE [dbo].[DuocPham] CHECK CONSTRAINT [FK_DuocPham_DonViTinh]
GO
ALTER TABLE [dbo].[DuocPham]  WITH CHECK ADD  CONSTRAINT [FK_DuocPham_DuongDung] FOREIGN KEY([DuongDungId])
REFERENCES [dbo].[DuongDung] ([Id])
GO
ALTER TABLE [dbo].[DuocPham] CHECK CONSTRAINT [FK_DuocPham_DuongDung]
GO
ALTER TABLE [dbo].[DuocPhamBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_DuocPhamBenhVien_DuocPham] FOREIGN KEY([Id])
REFERENCES [dbo].[DuocPham] ([Id])
GO
ALTER TABLE [dbo].[DuocPhamBenhVien] CHECK CONSTRAINT [FK_DuocPhamBenhVien_DuocPham]
GO
ALTER TABLE [dbo].[DuocPhamBenhVienGiaBaoHiem]  WITH CHECK ADD  CONSTRAINT [FK_DuocPhamBenhVienGiaBaoHiem_DuocPhamBenhVien] FOREIGN KEY([DuocPhamBenhVienId])
REFERENCES [dbo].[DuocPhamBenhVien] ([Id])
GO
ALTER TABLE [dbo].[DuocPhamBenhVienGiaBaoHiem] CHECK CONSTRAINT [FK_DuocPhamBenhVienGiaBaoHiem_DuocPhamBenhVien]
GO
ALTER TABLE [dbo].[DuTruDuocPham]  WITH CHECK ADD  CONSTRAINT [FK_DuTruDuocPham_DuocPham] FOREIGN KEY([DuocPhamId])
REFERENCES [dbo].[DuocPham] ([Id])
GO
ALTER TABLE [dbo].[DuTruDuocPham] CHECK CONSTRAINT [FK_DuTruDuocPham_DuocPham]
GO
ALTER TABLE [dbo].[DuTruDuocPham]  WITH CHECK ADD  CONSTRAINT [FK_DuTruDuocPham_KhoaPhong] FOREIGN KEY([KhoaPhongId])
REFERENCES [dbo].[KhoaPhong] ([Id])
GO
ALTER TABLE [dbo].[DuTruDuocPham] CHECK CONSTRAINT [FK_DuTruDuocPham_KhoaPhong]
GO
ALTER TABLE [dbo].[DuTruDuocPham]  WITH CHECK ADD  CONSTRAINT [FK_DuTruDuocPham_NhanVien] FOREIGN KEY([NhanVienLapDuTruId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[DuTruDuocPham] CHECK CONSTRAINT [FK_DuTruDuocPham_NhanVien]
GO
ALTER TABLE [dbo].[DuyetBaoHiem]  WITH CHECK ADD  CONSTRAINT [FK_DuyetBaoHiem_NhanVien] FOREIGN KEY([NhanVienDuyetBaoHiemId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[DuyetBaoHiem] CHECK CONSTRAINT [FK_DuyetBaoHiem_NhanVien]
GO
ALTER TABLE [dbo].[DuyetBaoHiem]  WITH CHECK ADD  CONSTRAINT [FK_DuyetBaoHiem_PhongBenhVien] FOREIGN KEY([NoiDuyetBaoHiemId])
REFERENCES [dbo].[PhongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[DuyetBaoHiem] CHECK CONSTRAINT [FK_DuyetBaoHiem_PhongBenhVien]
GO
ALTER TABLE [dbo].[DuyetBaoHiemChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_DuyetBaoHiemChiTiet_DuyetBaoHiem] FOREIGN KEY([DuyetBaoHiemId])
REFERENCES [dbo].[DuyetBaoHiem] ([Id])
GO
ALTER TABLE [dbo].[DuyetBaoHiemChiTiet] CHECK CONSTRAINT [FK_DuyetBaoHiemChiTiet_DuyetBaoHiem]
GO
ALTER TABLE [dbo].[DuyetBaoHiemChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_DuyetBaoHiemChiTiet_YeuCauDichVuGiuongBenhVien] FOREIGN KEY([YeuCauDichVuGiuongBenhVienId])
REFERENCES [dbo].[YeuCauDichVuGiuongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[DuyetBaoHiemChiTiet] CHECK CONSTRAINT [FK_DuyetBaoHiemChiTiet_YeuCauDichVuGiuongBenhVien]
GO
ALTER TABLE [dbo].[DuyetBaoHiemChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_DuyetBaoHiemChiTiet_YeuCauDichVuKyThuat] FOREIGN KEY([YeuCauDichVuKyThuatId])
REFERENCES [dbo].[YeuCauDichVuKyThuat] ([Id])
GO
ALTER TABLE [dbo].[DuyetBaoHiemChiTiet] CHECK CONSTRAINT [FK_DuyetBaoHiemChiTiet_YeuCauDichVuKyThuat]
GO
ALTER TABLE [dbo].[DuyetBaoHiemChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_DuyetBaoHiemChiTiet_YeuCauDuocPhamBenhVien] FOREIGN KEY([YeuCauDuocPhamBenhVienId])
REFERENCES [dbo].[YeuCauDuocPhamBenhVien] ([Id])
GO
ALTER TABLE [dbo].[DuyetBaoHiemChiTiet] CHECK CONSTRAINT [FK_DuyetBaoHiemChiTiet_YeuCauDuocPhamBenhVien]
GO
ALTER TABLE [dbo].[DuyetBaoHiemChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_DuyetBaoHiemChiTiet_YeuCauKhamBenh] FOREIGN KEY([YeuCauKhamBenhId])
REFERENCES [dbo].[YeuCauKhamBenh] ([Id])
GO
ALTER TABLE [dbo].[DuyetBaoHiemChiTiet] CHECK CONSTRAINT [FK_DuyetBaoHiemChiTiet_YeuCauKhamBenh]
GO
ALTER TABLE [dbo].[DuyetBaoHiemChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_DuyetBaoHiemChiTiet_YeuCauKhamBenhDonThuocThanhToanChiTiet] FOREIGN KEY([DonThuocThanhToanChiTietId])
REFERENCES [dbo].[DonThuocThanhToanChiTiet] ([Id])
GO
ALTER TABLE [dbo].[DuyetBaoHiemChiTiet] CHECK CONSTRAINT [FK_DuyetBaoHiemChiTiet_YeuCauKhamBenhDonThuocThanhToanChiTiet]
GO
ALTER TABLE [dbo].[DuyetBaoHiemChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_DuyetBaoHiemChiTiet_YeuCauVatTuBenhVien] FOREIGN KEY([YeuCauVatTuBenhVienId])
REFERENCES [dbo].[YeuCauVatTuBenhVien] ([Id])
GO
ALTER TABLE [dbo].[DuyetBaoHiemChiTiet] CHECK CONSTRAINT [FK_DuyetBaoHiemChiTiet_YeuCauVatTuBenhVien]
GO
ALTER TABLE [dbo].[GoiDichVu]  WITH CHECK ADD  CONSTRAINT [FK_GoiDichVu_NhanVien] FOREIGN KEY([NhanVienTaoGoiId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[GoiDichVu] CHECK CONSTRAINT [FK_GoiDichVu_NhanVien]
GO
ALTER TABLE [dbo].[GoiDichVuChiTietDichVuGiuong]  WITH CHECK ADD  CONSTRAINT [FK_GoiDichVuChiTietDichVuGiuong_DichVuGiuongBenhVien] FOREIGN KEY([DichVuGiuongBenhVienId])
REFERENCES [dbo].[DichVuGiuongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[GoiDichVuChiTietDichVuGiuong] CHECK CONSTRAINT [FK_GoiDichVuChiTietDichVuGiuong_DichVuGiuongBenhVien]
GO
ALTER TABLE [dbo].[GoiDichVuChiTietDichVuGiuong]  WITH CHECK ADD  CONSTRAINT [FK_GoiDichVuChiTietDichVuGiuong_GoiDichVu] FOREIGN KEY([GoiDichVuId])
REFERENCES [dbo].[GoiDichVu] ([Id])
GO
ALTER TABLE [dbo].[GoiDichVuChiTietDichVuGiuong] CHECK CONSTRAINT [FK_GoiDichVuChiTietDichVuGiuong_GoiDichVu]
GO
ALTER TABLE [dbo].[GoiDichVuChiTietDichVuGiuong]  WITH CHECK ADD  CONSTRAINT [FK_GoiDichVuChiTietDichVuGiuong_NhomGiaDichVuGiuongBenhVien] FOREIGN KEY([NhomGiaDichVuGiuongBenhVienId])
REFERENCES [dbo].[NhomGiaDichVuGiuongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[GoiDichVuChiTietDichVuGiuong] CHECK CONSTRAINT [FK_GoiDichVuChiTietDichVuGiuong_NhomGiaDichVuGiuongBenhVien]
GO
ALTER TABLE [dbo].[GoiDichVuChiTietDichVuKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_GoiDichVuChiTietDichVuKhamBenh_DichVuKhamBenhBenhVien] FOREIGN KEY([DichVuKhamBenhBenhVienId])
REFERENCES [dbo].[DichVuKhamBenhBenhVien] ([Id])
GO
ALTER TABLE [dbo].[GoiDichVuChiTietDichVuKhamBenh] CHECK CONSTRAINT [FK_GoiDichVuChiTietDichVuKhamBenh_DichVuKhamBenhBenhVien]
GO
ALTER TABLE [dbo].[GoiDichVuChiTietDichVuKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_GoiDichVuChiTietDichVuKhamBenh_GoiDichVu] FOREIGN KEY([GoiDichVuId])
REFERENCES [dbo].[GoiDichVu] ([Id])
GO
ALTER TABLE [dbo].[GoiDichVuChiTietDichVuKhamBenh] CHECK CONSTRAINT [FK_GoiDichVuChiTietDichVuKhamBenh_GoiDichVu]
GO
ALTER TABLE [dbo].[GoiDichVuChiTietDichVuKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_GoiDichVuChiTietDichVuKhamBenh_NhomGiaDichVuKhamBenhBenhVien] FOREIGN KEY([NhomGiaDichVuKhamBenhBenhVienId])
REFERENCES [dbo].[NhomGiaDichVuKhamBenhBenhVien] ([Id])
GO
ALTER TABLE [dbo].[GoiDichVuChiTietDichVuKhamBenh] CHECK CONSTRAINT [FK_GoiDichVuChiTietDichVuKhamBenh_NhomGiaDichVuKhamBenhBenhVien]
GO
ALTER TABLE [dbo].[GoiDichVuChiTietDichVuKyThuat]  WITH CHECK ADD  CONSTRAINT [FK_GoiDichVuChiTietDichVuKyThuat_DichVuKyThuatBenhVien] FOREIGN KEY([DichVuKyThuatBenhVienId])
REFERENCES [dbo].[DichVuKyThuatBenhVien] ([Id])
GO
ALTER TABLE [dbo].[GoiDichVuChiTietDichVuKyThuat] CHECK CONSTRAINT [FK_GoiDichVuChiTietDichVuKyThuat_DichVuKyThuatBenhVien]
GO
ALTER TABLE [dbo].[GoiDichVuChiTietDichVuKyThuat]  WITH CHECK ADD  CONSTRAINT [FK_GoiDichVuChiTietDichVuKyThuat_GoiDichVu] FOREIGN KEY([GoiDichVuId])
REFERENCES [dbo].[GoiDichVu] ([Id])
GO
ALTER TABLE [dbo].[GoiDichVuChiTietDichVuKyThuat] CHECK CONSTRAINT [FK_GoiDichVuChiTietDichVuKyThuat_GoiDichVu]
GO
ALTER TABLE [dbo].[GoiDichVuChiTietDichVuKyThuat]  WITH CHECK ADD  CONSTRAINT [FK_GoiDichVuChiTietDichVuKyThuat_NhomGiaDichVuKyThuatBenhVien] FOREIGN KEY([NhomGiaDichVuKyThuatBenhVienId])
REFERENCES [dbo].[NhomGiaDichVuKyThuatBenhVien] ([Id])
GO
ALTER TABLE [dbo].[GoiDichVuChiTietDichVuKyThuat] CHECK CONSTRAINT [FK_GoiDichVuChiTietDichVuKyThuat_NhomGiaDichVuKyThuatBenhVien]
GO
ALTER TABLE [dbo].[GoiDichVuChiTietDuocPham]  WITH CHECK ADD  CONSTRAINT [FK_GoiDichVuChiTietDuocPham_DuocPhamBenhVien] FOREIGN KEY([DuocPhamBenhVienId])
REFERENCES [dbo].[DuocPhamBenhVien] ([Id])
GO
ALTER TABLE [dbo].[GoiDichVuChiTietDuocPham] CHECK CONSTRAINT [FK_GoiDichVuChiTietDuocPham_DuocPhamBenhVien]
GO
ALTER TABLE [dbo].[GoiDichVuChiTietDuocPham]  WITH CHECK ADD  CONSTRAINT [FK_GoiDichVuChiTietDuocPham_GoiDichVu] FOREIGN KEY([GoiDichVuId])
REFERENCES [dbo].[GoiDichVu] ([Id])
GO
ALTER TABLE [dbo].[GoiDichVuChiTietDuocPham] CHECK CONSTRAINT [FK_GoiDichVuChiTietDuocPham_GoiDichVu]
GO
ALTER TABLE [dbo].[GoiDichVuChiTietVatTu]  WITH CHECK ADD  CONSTRAINT [FK_GoiDichVuChiTietVatTu_GoiDichVu] FOREIGN KEY([GoiDichVuId])
REFERENCES [dbo].[GoiDichVu] ([Id])
GO
ALTER TABLE [dbo].[GoiDichVuChiTietVatTu] CHECK CONSTRAINT [FK_GoiDichVuChiTietVatTu_GoiDichVu]
GO
ALTER TABLE [dbo].[GoiDichVuChiTietVatTu]  WITH CHECK ADD  CONSTRAINT [FK_GoiDichVuChiTietVatTu_VatTuBenhVien] FOREIGN KEY([VatTuBenhVienId])
REFERENCES [dbo].[VatTuBenhVien] ([Id])
GO
ALTER TABLE [dbo].[GoiDichVuChiTietVatTu] CHECK CONSTRAINT [FK_GoiDichVuChiTietVatTu_VatTuBenhVien]
GO
ALTER TABLE [dbo].[HoatDongNhanVien]  WITH CHECK ADD  CONSTRAINT [FK_HoatDongNhanVien_NhanVien] FOREIGN KEY([NhanVienId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[HoatDongNhanVien] CHECK CONSTRAINT [FK_HoatDongNhanVien_NhanVien]
GO
ALTER TABLE [dbo].[HoatDongNhanVien]  WITH CHECK ADD  CONSTRAINT [FK_HoatDongNhanVien_PhongBenhVien] FOREIGN KEY([PhongBenhVienId])
REFERENCES [dbo].[PhongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[HoatDongNhanVien] CHECK CONSTRAINT [FK_HoatDongNhanVien_PhongBenhVien]
GO
ALTER TABLE [dbo].[HopDongThauDuocPham]  WITH CHECK ADD  CONSTRAINT [FK_HopDongThauDuocPham_NhaThau] FOREIGN KEY([NhaThauId])
REFERENCES [dbo].[NhaThau] ([Id])
GO
ALTER TABLE [dbo].[HopDongThauDuocPham] CHECK CONSTRAINT [FK_HopDongThauDuocPham_NhaThau]
GO
ALTER TABLE [dbo].[HopDongThauDuocPhamChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_HopDongThauDuocPhamChiTiet_DuocPham] FOREIGN KEY([DuocPhamId])
REFERENCES [dbo].[DuocPham] ([Id])
GO
ALTER TABLE [dbo].[HopDongThauDuocPhamChiTiet] CHECK CONSTRAINT [FK_HopDongThauDuocPhamChiTiet_DuocPham]
GO
ALTER TABLE [dbo].[HopDongThauDuocPhamChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_HopDongThauDuocPhamChiTiet_HopDongThauDuocPham] FOREIGN KEY([HopDongThauDuocPhamId])
REFERENCES [dbo].[HopDongThauDuocPham] ([Id])
GO
ALTER TABLE [dbo].[HopDongThauDuocPhamChiTiet] CHECK CONSTRAINT [FK_HopDongThauDuocPhamChiTiet_HopDongThauDuocPham]
GO
ALTER TABLE [dbo].[ICD]  WITH CHECK ADD  CONSTRAINT [FK__ICD__LoaiICDId__41EDCAC5] FOREIGN KEY([LoaiICDId])
REFERENCES [dbo].[LoaiICD] ([Id])
GO
ALTER TABLE [dbo].[ICD] CHECK CONSTRAINT [FK__ICD__LoaiICDId__41EDCAC5]
GO
ALTER TABLE [dbo].[ICDDoiTuongBenhNhanChiTiet]  WITH CHECK ADD FOREIGN KEY([ICDDoiTuongBenhNhanId])
REFERENCES [dbo].[ICDDoiTuongBenhNhan] ([Id])
GO
ALTER TABLE [dbo].[ICDDoiTuongBenhNhanChiTiet]  WITH CHECK ADD FOREIGN KEY([ICDDoiTuongBenhNhanId])
REFERENCES [dbo].[ICDDoiTuongBenhNhan] ([Id])
GO
ALTER TABLE [dbo].[ICDDoiTuongBenhNhanChiTiet]  WITH CHECK ADD FOREIGN KEY([ICDDoiTuongBenhNhanId])
REFERENCES [dbo].[ICDDoiTuongBenhNhan] ([Id])
GO
ALTER TABLE [dbo].[ICDDoiTuongBenhNhanChiTiet]  WITH CHECK ADD FOREIGN KEY([ICDDoiTuongBenhNhanId])
REFERENCES [dbo].[ICDDoiTuongBenhNhan] ([Id])
GO
ALTER TABLE [dbo].[ICDDoiTuongBenhNhanChiTiet]  WITH CHECK ADD  CONSTRAINT [FK__ICDDoiTuo__ICDId__489AC854] FOREIGN KEY([ICDId])
REFERENCES [dbo].[ICD] ([Id])
GO
ALTER TABLE [dbo].[ICDDoiTuongBenhNhanChiTiet] CHECK CONSTRAINT [FK__ICDDoiTuo__ICDId__489AC854]
GO
ALTER TABLE [dbo].[KetQuaChuanDoanHinhAnh]  WITH CHECK ADD  CONSTRAINT [FK_KetQuaChuanDoanHinhAnh_ChuanDoanHinhAnh] FOREIGN KEY([ChuanDoanHinhAnhId])
REFERENCES [dbo].[ChuanDoanHinhAnh] ([Id])
GO
ALTER TABLE [dbo].[KetQuaChuanDoanHinhAnh] CHECK CONSTRAINT [FK_KetQuaChuanDoanHinhAnh_ChuanDoanHinhAnh]
GO
ALTER TABLE [dbo].[KetQuaChuanDoanHinhAnh]  WITH CHECK ADD  CONSTRAINT [FK_KetQuaChuanDoanHinhAnh_YeuCauKhamBenhDichVuKyThuat] FOREIGN KEY([YeuCauDichVuKyThuatId])
REFERENCES [dbo].[YeuCauDichVuKyThuat] ([Id])
GO
ALTER TABLE [dbo].[KetQuaChuanDoanHinhAnh] CHECK CONSTRAINT [FK_KetQuaChuanDoanHinhAnh_YeuCauKhamBenhDichVuKyThuat]
GO
ALTER TABLE [dbo].[KetQuaSinhHieu]  WITH CHECK ADD  CONSTRAINT [FK_KetQuaSinhHieu_NhanVien] FOREIGN KEY([NhanVienThucHienId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[KetQuaSinhHieu] CHECK CONSTRAINT [FK_KetQuaSinhHieu_NhanVien]
GO
ALTER TABLE [dbo].[KetQuaSinhHieu]  WITH CHECK ADD  CONSTRAINT [FK_KetQuaSinhHieu_PhongBenhVien] FOREIGN KEY([NoiThucHienId])
REFERENCES [dbo].[PhongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[KetQuaSinhHieu] CHECK CONSTRAINT [FK_KetQuaSinhHieu_PhongBenhVien]
GO
ALTER TABLE [dbo].[KetQuaSinhHieu]  WITH CHECK ADD  CONSTRAINT [FK_KetQuaSinhHieu_YeuCauTiepNhan] FOREIGN KEY([YeuCauTiepNhanId])
REFERENCES [dbo].[YeuCauTiepNhan] ([Id])
GO
ALTER TABLE [dbo].[KetQuaSinhHieu] CHECK CONSTRAINT [FK_KetQuaSinhHieu_YeuCauTiepNhan]
GO
ALTER TABLE [dbo].[KetQuaXetNghiem]  WITH CHECK ADD  CONSTRAINT [FK_KetQuaXetNghiem_ChiSoXetNghiem] FOREIGN KEY([ChiSoXetNghiemId])
REFERENCES [dbo].[ChiSoXetNghiem] ([Id])
GO
ALTER TABLE [dbo].[KetQuaXetNghiem] CHECK CONSTRAINT [FK_KetQuaXetNghiem_ChiSoXetNghiem]
GO
ALTER TABLE [dbo].[KetQuaXetNghiem]  WITH CHECK ADD  CONSTRAINT [FK_KetQuaXetNghiem_YeuCauKhamBenhDichVuKyThuat] FOREIGN KEY([YeuCauDichVuKyThuatId])
REFERENCES [dbo].[YeuCauDichVuKyThuat] ([Id])
GO
ALTER TABLE [dbo].[KetQuaXetNghiem] CHECK CONSTRAINT [FK_KetQuaXetNghiem_YeuCauKhamBenhDichVuKyThuat]
GO
ALTER TABLE [dbo].[KhoaPhongChuyenKhoa]  WITH CHECK ADD  CONSTRAINT [FK_KhoaPhongChuyenKhoa_Khoa] FOREIGN KEY([KhoaId])
REFERENCES [dbo].[Khoa] ([Id])
GO
ALTER TABLE [dbo].[KhoaPhongChuyenKhoa] CHECK CONSTRAINT [FK_KhoaPhongChuyenKhoa_Khoa]
GO
ALTER TABLE [dbo].[KhoaPhongChuyenKhoa]  WITH CHECK ADD  CONSTRAINT [FK_KhoaPhongChuyenKhoa_KhoaPhong] FOREIGN KEY([KhoaPhongId])
REFERENCES [dbo].[KhoaPhong] ([Id])
GO
ALTER TABLE [dbo].[KhoaPhongChuyenKhoa] CHECK CONSTRAINT [FK_KhoaPhongChuyenKhoa_KhoaPhong]
GO
ALTER TABLE [dbo].[KhoaPhongNhanVien]  WITH CHECK ADD  CONSTRAINT [FK__KhoaPhongNhanVien__KhoaPhong__2A164134] FOREIGN KEY([KhoaPhongId])
REFERENCES [dbo].[KhoaPhong] ([Id])
GO
ALTER TABLE [dbo].[KhoaPhongNhanVien] CHECK CONSTRAINT [FK__KhoaPhongNhanVien__KhoaPhong__2A164134]
GO
ALTER TABLE [dbo].[KhoaPhongNhanVien]  WITH CHECK ADD  CONSTRAINT [FK__KhoaPhongNhanVien__NhanVien__2A164134] FOREIGN KEY([NhanVienId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[KhoaPhongNhanVien] CHECK CONSTRAINT [FK__KhoaPhongNhanVien__NhanVien__2A164134]
GO
ALTER TABLE [dbo].[KhoDuocPham]  WITH CHECK ADD  CONSTRAINT [FK_KhoDuocPham_KhoaPhong] FOREIGN KEY([KhoaPhongId])
REFERENCES [dbo].[KhoaPhong] ([Id])
GO
ALTER TABLE [dbo].[KhoDuocPham] CHECK CONSTRAINT [FK_KhoDuocPham_KhoaPhong]
GO
ALTER TABLE [dbo].[KhoDuocPhamViTri]  WITH CHECK ADD  CONSTRAINT [FK_KhoDuocPhamViTri_KhoDuocPham] FOREIGN KEY([KhoDuocPhamId])
REFERENCES [dbo].[KhoDuocPham] ([Id])
GO
ALTER TABLE [dbo].[KhoDuocPhamViTri] CHECK CONSTRAINT [FK_KhoDuocPhamViTri_KhoDuocPham]
GO
ALTER TABLE [dbo].[LichPhanCongNgoaiTru]  WITH CHECK ADD  CONSTRAINT [FK__LichPhanCongNgoaiTru__NhanVien__2A164134] FOREIGN KEY([NhanVienId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[LichPhanCongNgoaiTru] CHECK CONSTRAINT [FK__LichPhanCongNgoaiTru__NhanVien__2A164134]
GO
ALTER TABLE [dbo].[LichPhanCongNgoaiTru]  WITH CHECK ADD  CONSTRAINT [FK__LichPhanCongNgoaiTru__PhongNgoaiTru__2A164134] FOREIGN KEY([PhongNgoaiTruId])
REFERENCES [dbo].[PhongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[LichPhanCongNgoaiTru] CHECK CONSTRAINT [FK__LichPhanCongNgoaiTru__PhongNgoaiTru__2A164134]
GO
ALTER TABLE [dbo].[LichSuHoatDongNhanVien]  WITH CHECK ADD  CONSTRAINT [FK_LichSuHoatDongNhanVien_NhanVien] FOREIGN KEY([NhanVienId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[LichSuHoatDongNhanVien] CHECK CONSTRAINT [FK_LichSuHoatDongNhanVien_NhanVien]
GO
ALTER TABLE [dbo].[LichSuHoatDongNhanVien]  WITH CHECK ADD  CONSTRAINT [FK_LichSuHoatDongNhanVien_PhongBenhVien] FOREIGN KEY([PhongBenhVienId])
REFERENCES [dbo].[PhongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[LichSuHoatDongNhanVien] CHECK CONSTRAINT [FK_LichSuHoatDongNhanVien_PhongBenhVien]
GO
ALTER TABLE [dbo].[LoaiICD]  WITH CHECK ADD  CONSTRAINT [FK__LoaiICD__NhomICD__4589517F] FOREIGN KEY([NhomICDId])
REFERENCES [dbo].[NhomICD] ([Id])
GO
ALTER TABLE [dbo].[LoaiICD] CHECK CONSTRAINT [FK__LoaiICD__NhomICD__4589517F]
GO
ALTER TABLE [dbo].[NhanVien]  WITH CHECK ADD  CONSTRAINT [FK_NhanVien_User] FOREIGN KEY([Id])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[NhanVien] CHECK CONSTRAINT [FK_NhanVien_User]
GO
ALTER TABLE [dbo].[NhanVienRole]  WITH CHECK ADD  CONSTRAINT [FK_NhanVienRole_NhanVien] FOREIGN KEY([NhanVienId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[NhanVienRole] CHECK CONSTRAINT [FK_NhanVienRole_NhanVien]
GO
ALTER TABLE [dbo].[NhanVienRole]  WITH CHECK ADD  CONSTRAINT [FK_NhanVienRole_Role] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
GO
ALTER TABLE [dbo].[NhanVienRole] CHECK CONSTRAINT [FK_NhanVienRole_Role]
GO
ALTER TABLE [dbo].[NhapKhoDuocPham]  WITH CHECK ADD  CONSTRAINT [FK_NhapKhoDuocPham_KhoDuocPham] FOREIGN KEY([KhoDuocPhamId])
REFERENCES [dbo].[KhoDuocPham] ([Id])
GO
ALTER TABLE [dbo].[NhapKhoDuocPham] CHECK CONSTRAINT [FK_NhapKhoDuocPham_KhoDuocPham]
GO
ALTER TABLE [dbo].[NhapKhoDuocPham]  WITH CHECK ADD  CONSTRAINT [FK_NhapKhoDuocPham_NhanVien] FOREIGN KEY([NguoiGiaoId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[NhapKhoDuocPham] CHECK CONSTRAINT [FK_NhapKhoDuocPham_NhanVien]
GO
ALTER TABLE [dbo].[NhapKhoDuocPham]  WITH CHECK ADD  CONSTRAINT [FK_NhapKhoDuocPham_NhanVien1] FOREIGN KEY([NguoiNhapId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[NhapKhoDuocPham] CHECK CONSTRAINT [FK_NhapKhoDuocPham_NhanVien1]
GO
ALTER TABLE [dbo].[NhapKhoDuocPham]  WITH CHECK ADD  CONSTRAINT [FK_NhapKhoDuocPham_XuatKhoDuocPham] FOREIGN KEY([XuatKhoDuocPhamId])
REFERENCES [dbo].[XuatKhoDuocPham] ([Id])
GO
ALTER TABLE [dbo].[NhapKhoDuocPham] CHECK CONSTRAINT [FK_NhapKhoDuocPham_XuatKhoDuocPham]
GO
ALTER TABLE [dbo].[NhapKhoDuocPhamChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_NhapKhoDuocPhamChiTiet_DuocPhamBenhVien] FOREIGN KEY([DuocPhamBenhVienId])
REFERENCES [dbo].[DuocPhamBenhVien] ([Id])
GO
ALTER TABLE [dbo].[NhapKhoDuocPhamChiTiet] CHECK CONSTRAINT [FK_NhapKhoDuocPhamChiTiet_DuocPhamBenhVien]
GO
ALTER TABLE [dbo].[NhapKhoDuocPhamChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_NhapKhoDuocPhamChiTiet_HopDongThauDuocPham] FOREIGN KEY([HopDongThauDuocPhamId])
REFERENCES [dbo].[HopDongThauDuocPham] ([Id])
GO
ALTER TABLE [dbo].[NhapKhoDuocPhamChiTiet] CHECK CONSTRAINT [FK_NhapKhoDuocPhamChiTiet_HopDongThauDuocPham]
GO
ALTER TABLE [dbo].[NhapKhoDuocPhamChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_NhapKhoDuocPhamChiTiet_KhoDuocPhamViTri] FOREIGN KEY([KhoDuocPhamViTriId])
REFERENCES [dbo].[KhoDuocPhamViTri] ([Id])
GO
ALTER TABLE [dbo].[NhapKhoDuocPhamChiTiet] CHECK CONSTRAINT [FK_NhapKhoDuocPhamChiTiet_KhoDuocPhamViTri]
GO
ALTER TABLE [dbo].[NhapKhoDuocPhamChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_NhapKhoDuocPhamChiTiet_NhapKhoDuocPham] FOREIGN KEY([NhapKhoDuocPhamId])
REFERENCES [dbo].[NhapKhoDuocPham] ([Id])
GO
ALTER TABLE [dbo].[NhapKhoDuocPhamChiTiet] CHECK CONSTRAINT [FK_NhapKhoDuocPhamChiTiet_NhapKhoDuocPham]
GO
ALTER TABLE [dbo].[NhaSanXuatTheoQuocGia]  WITH CHECK ADD  CONSTRAINT [FK__NhaSanXua__NhaSa__047AA831] FOREIGN KEY([NhaSanXuatId])
REFERENCES [dbo].[NhaSanXuat] ([Id])
GO
ALTER TABLE [dbo].[NhaSanXuatTheoQuocGia] CHECK CONSTRAINT [FK__NhaSanXua__NhaSa__047AA831]
GO
ALTER TABLE [dbo].[NhaSanXuatTheoQuocGia]  WITH CHECK ADD FOREIGN KEY([QuocGiaId])
REFERENCES [dbo].[QuocGia] ([Id])
GO
ALTER TABLE [dbo].[NhaSanXuatTheoQuocGia]  WITH CHECK ADD FOREIGN KEY([QuocGiaId])
REFERENCES [dbo].[QuocGia] ([Id])
GO
ALTER TABLE [dbo].[NhaSanXuatTheoQuocGia]  WITH CHECK ADD FOREIGN KEY([QuocGiaId])
REFERENCES [dbo].[QuocGia] ([Id])
GO
ALTER TABLE [dbo].[NhaSanXuatTheoQuocGia]  WITH CHECK ADD FOREIGN KEY([QuocGiaId])
REFERENCES [dbo].[QuocGia] ([Id])
GO
ALTER TABLE [dbo].[NhomICD]  WITH CHECK ADD FOREIGN KEY([ChuongICDId])
REFERENCES [dbo].[ChuongICD] ([Id])
GO
ALTER TABLE [dbo].[NhomICD]  WITH CHECK ADD FOREIGN KEY([ChuongICDId])
REFERENCES [dbo].[ChuongICD] ([Id])
GO
ALTER TABLE [dbo].[NhomICD]  WITH CHECK ADD FOREIGN KEY([ChuongICDId])
REFERENCES [dbo].[ChuongICD] ([Id])
GO
ALTER TABLE [dbo].[NhomICD]  WITH CHECK ADD FOREIGN KEY([ChuongICDId])
REFERENCES [dbo].[ChuongICD] ([Id])
GO
ALTER TABLE [dbo].[NhomICD]  WITH CHECK ADD  CONSTRAINT [FK__NhomICD__NhomICD__31B762FC] FOREIGN KEY([NhomICDChaId])
REFERENCES [dbo].[NhomICD] ([Id])
GO
ALTER TABLE [dbo].[NhomICD] CHECK CONSTRAINT [FK__NhomICD__NhomICD__31B762FC]
GO
ALTER TABLE [dbo].[NhomThuoc]  WITH CHECK ADD  CONSTRAINT [FK__NhomThuoc__NhomT__19AACF41] FOREIGN KEY([NhomChaId])
REFERENCES [dbo].[NhomThuoc] ([Id])
GO
ALTER TABLE [dbo].[NhomThuoc] CHECK CONSTRAINT [FK__NhomThuoc__NhomT__19AACF41]
GO
ALTER TABLE [dbo].[NhomThuoc]  WITH CHECK ADD  CONSTRAINT [FK_NhomThuoc_NhomThuoc] FOREIGN KEY([NhomChaId])
REFERENCES [dbo].[NhomThuoc] ([Id])
GO
ALTER TABLE [dbo].[NhomThuoc] CHECK CONSTRAINT [FK_NhomThuoc_NhomThuoc]
GO
ALTER TABLE [dbo].[NhomVatTu]  WITH CHECK ADD  CONSTRAINT [FK_NhomVatTu_NhomVatTu] FOREIGN KEY([NhomVatTuChaId])
REFERENCES [dbo].[NhomVatTu] ([Id])
GO
ALTER TABLE [dbo].[NhomVatTu] CHECK CONSTRAINT [FK_NhomVatTu_NhomVatTu]
GO
ALTER TABLE [dbo].[PhongBenhVien]  WITH CHECK ADD  CONSTRAINT [FK__PhongNgoaiTru__KhoaPhong__2A164134] FOREIGN KEY([KhoaPhongId])
REFERENCES [dbo].[KhoaPhong] ([Id])
GO
ALTER TABLE [dbo].[PhongBenhVien] CHECK CONSTRAINT [FK__PhongNgoaiTru__KhoaPhong__2A164134]
GO
ALTER TABLE [dbo].[PhongBenhVienHangDoi]  WITH CHECK ADD  CONSTRAINT [FK_PhongBenhVienHangDoi_PhongBenhVien] FOREIGN KEY([PhongBenhVienId])
REFERENCES [dbo].[PhongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[PhongBenhVienHangDoi] CHECK CONSTRAINT [FK_PhongBenhVienHangDoi_PhongBenhVien]
GO
ALTER TABLE [dbo].[PhongBenhVienHangDoi]  WITH CHECK ADD  CONSTRAINT [FK_PhongBenhVienHangDoi_YeuCauDichVuKyThuat] FOREIGN KEY([YeuCauDichVuKyThuatId])
REFERENCES [dbo].[YeuCauDichVuKyThuat] ([Id])
GO
ALTER TABLE [dbo].[PhongBenhVienHangDoi] CHECK CONSTRAINT [FK_PhongBenhVienHangDoi_YeuCauDichVuKyThuat]
GO
ALTER TABLE [dbo].[PhongBenhVienHangDoi]  WITH CHECK ADD  CONSTRAINT [FK_PhongBenhVienHangDoi_YeuCauKhamBenh] FOREIGN KEY([YeuCauKhamBenhId])
REFERENCES [dbo].[YeuCauKhamBenh] ([Id])
GO
ALTER TABLE [dbo].[PhongBenhVienHangDoi] CHECK CONSTRAINT [FK_PhongBenhVienHangDoi_YeuCauKhamBenh]
GO
ALTER TABLE [dbo].[PhongBenhVienHangDoi]  WITH CHECK ADD  CONSTRAINT [FK_PhongBenhVienHangDoi_YeuCauTiepNhan] FOREIGN KEY([YeuCauTiepNhanId])
REFERENCES [dbo].[YeuCauTiepNhan] ([Id])
GO
ALTER TABLE [dbo].[PhongBenhVienHangDoi] CHECK CONSTRAINT [FK_PhongBenhVienHangDoi_YeuCauTiepNhan]
GO
ALTER TABLE [dbo].[PhongBenhVienNhomGiaDichVuKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_PhongBenhVienNhomGiaDichVuKhamBenh_NhomGiaDichVuKhamBenhBenhVien] FOREIGN KEY([NhomGiaDichVuKhamBenhBenhVienId])
REFERENCES [dbo].[NhomGiaDichVuKhamBenhBenhVien] ([Id])
GO
ALTER TABLE [dbo].[PhongBenhVienNhomGiaDichVuKhamBenh] CHECK CONSTRAINT [FK_PhongBenhVienNhomGiaDichVuKhamBenh_NhomGiaDichVuKhamBenhBenhVien]
GO
ALTER TABLE [dbo].[PhongBenhVienNhomGiaDichVuKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_PhongBenhVienNhomGiaDichVuKhamBenh_PhongBenhVien] FOREIGN KEY([PhongBenhVienId])
REFERENCES [dbo].[PhongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[PhongBenhVienNhomGiaDichVuKhamBenh] CHECK CONSTRAINT [FK_PhongBenhVienNhomGiaDichVuKhamBenh_PhongBenhVien]
GO
ALTER TABLE [dbo].[PhongBenhVienNhomGiaDichVuKyThuat]  WITH CHECK ADD  CONSTRAINT [FK_PhongBenhVienNhomGiaDichVuKyThuat_NhomGiaDichVuKyThuatBenhVien] FOREIGN KEY([NhomGiaDichVuKyThuatBenhVienId])
REFERENCES [dbo].[NhomGiaDichVuKyThuatBenhVien] ([Id])
GO
ALTER TABLE [dbo].[PhongBenhVienNhomGiaDichVuKyThuat] CHECK CONSTRAINT [FK_PhongBenhVienNhomGiaDichVuKyThuat_NhomGiaDichVuKyThuatBenhVien]
GO
ALTER TABLE [dbo].[PhongBenhVienNhomGiaDichVuKyThuat]  WITH CHECK ADD  CONSTRAINT [FK_PhongBenhVienNhomGiaDichVuKyThuat_PhongBenhVien] FOREIGN KEY([PhongBenhVienId])
REFERENCES [dbo].[PhongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[PhongBenhVienNhomGiaDichVuKyThuat] CHECK CONSTRAINT [FK_PhongBenhVienNhomGiaDichVuKyThuat_PhongBenhVien]
GO
ALTER TABLE [dbo].[RoleFunction]  WITH CHECK ADD  CONSTRAINT [FK_RoleFunction_Role] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
GO
ALTER TABLE [dbo].[RoleFunction] CHECK CONSTRAINT [FK_RoleFunction_Role]
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhan]  WITH CHECK ADD  CONSTRAINT [FK_TaiKhoanBenhNhan_BenhNhan1] FOREIGN KEY([Id])
REFERENCES [dbo].[BenhNhan] ([Id])
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhan] CHECK CONSTRAINT [FK_TaiKhoanBenhNhan_BenhNhan1]
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanChi]  WITH CHECK ADD  CONSTRAINT [FK_TaiKhoanBenhNhanChi_NhanVien] FOREIGN KEY([NhanVienThucHienId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanChi] CHECK CONSTRAINT [FK_TaiKhoanBenhNhanChi_NhanVien]
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanChi]  WITH CHECK ADD  CONSTRAINT [FK_TaiKhoanBenhNhanChi_PhongBenhVien] FOREIGN KEY([NoiThucHienId])
REFERENCES [dbo].[PhongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanChi] CHECK CONSTRAINT [FK_TaiKhoanBenhNhanChi_PhongBenhVien]
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanChi]  WITH CHECK ADD  CONSTRAINT [FK_TaiKhoanBenhNhanChi_TaiKhoanBenhNhan] FOREIGN KEY([TaiKhoanBenhNhanId])
REFERENCES [dbo].[TaiKhoanBenhNhan] ([Id])
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanChi] CHECK CONSTRAINT [FK_TaiKhoanBenhNhanChi_TaiKhoanBenhNhan]
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanChi]  WITH CHECK ADD  CONSTRAINT [FK_TaiKhoanBenhNhanChi_TaiKhoanBenhNhanThu] FOREIGN KEY([TaiKhoanBenhNhanThuId])
REFERENCES [dbo].[TaiKhoanBenhNhanThu] ([Id])
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanChi] CHECK CONSTRAINT [FK_TaiKhoanBenhNhanChi_TaiKhoanBenhNhanThu]
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanChi]  WITH CHECK ADD  CONSTRAINT [FK_TaiKhoanBenhNhanChi_YeuCauDichVuGiuongBenhVien] FOREIGN KEY([YeuCauDichVuGiuongBenhVienId])
REFERENCES [dbo].[YeuCauDichVuGiuongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanChi] CHECK CONSTRAINT [FK_TaiKhoanBenhNhanChi_YeuCauDichVuGiuongBenhVien]
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanChi]  WITH CHECK ADD  CONSTRAINT [FK_TaiKhoanBenhNhanChi_YeuCauDichVuKyThuat] FOREIGN KEY([YeuCauDichVuKyThuatId])
REFERENCES [dbo].[YeuCauDichVuKyThuat] ([Id])
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanChi] CHECK CONSTRAINT [FK_TaiKhoanBenhNhanChi_YeuCauDichVuKyThuat]
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanChi]  WITH CHECK ADD  CONSTRAINT [FK_TaiKhoanBenhNhanChi_YeuCauDuocPhamBenhVien] FOREIGN KEY([YeuCauDuocPhamBenhVienId])
REFERENCES [dbo].[YeuCauDuocPhamBenhVien] ([Id])
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanChi] CHECK CONSTRAINT [FK_TaiKhoanBenhNhanChi_YeuCauDuocPhamBenhVien]
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanChi]  WITH CHECK ADD  CONSTRAINT [FK_TaiKhoanBenhNhanChi_YeuCauGoiDichVu] FOREIGN KEY([YeuCauGoiDichVuId])
REFERENCES [dbo].[YeuCauGoiDichVu] ([Id])
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanChi] CHECK CONSTRAINT [FK_TaiKhoanBenhNhanChi_YeuCauGoiDichVu]
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanChi]  WITH CHECK ADD  CONSTRAINT [FK_TaiKhoanBenhNhanChi_YeuCauKhamBenh] FOREIGN KEY([YeuCauKhamBenhId])
REFERENCES [dbo].[YeuCauKhamBenh] ([Id])
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanChi] CHECK CONSTRAINT [FK_TaiKhoanBenhNhanChi_YeuCauKhamBenh]
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanChi]  WITH CHECK ADD  CONSTRAINT [FK_TaiKhoanBenhNhanChi_YeuCauKhamBenhDonThuocThanhToan] FOREIGN KEY([DonThuocThanhToanId])
REFERENCES [dbo].[DonThuocThanhToan] ([Id])
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanChi] CHECK CONSTRAINT [FK_TaiKhoanBenhNhanChi_YeuCauKhamBenhDonThuocThanhToan]
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanChi]  WITH CHECK ADD  CONSTRAINT [FK_TaiKhoanBenhNhanChi_YeuCauTiepNhan] FOREIGN KEY([YeuCauTiepNhanId])
REFERENCES [dbo].[YeuCauTiepNhan] ([Id])
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanChi] CHECK CONSTRAINT [FK_TaiKhoanBenhNhanChi_YeuCauTiepNhan]
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanChi]  WITH CHECK ADD  CONSTRAINT [FK_TaiKhoanBenhNhanChi_YeuCauVatTuBenhVien] FOREIGN KEY([YeuCauVatTuBenhVienId])
REFERENCES [dbo].[YeuCauVatTuBenhVien] ([Id])
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanChi] CHECK CONSTRAINT [FK_TaiKhoanBenhNhanChi_YeuCauVatTuBenhVien]
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanThu]  WITH CHECK ADD  CONSTRAINT [FK_TaiKhoanBenhNhanThu_TaiKhoanBenhNhan] FOREIGN KEY([TaiKhoanBenhNhanId])
REFERENCES [dbo].[TaiKhoanBenhNhan] ([Id])
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanThu] CHECK CONSTRAINT [FK_TaiKhoanBenhNhanThu_TaiKhoanBenhNhan]
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanThu]  WITH CHECK ADD  CONSTRAINT [FK_TaiKhoanBenhNhanThu_YeuCauDichVuGiuongBenhVien] FOREIGN KEY([HoanTraYeuCauDichVuGiuongBenhVienId])
REFERENCES [dbo].[YeuCauDichVuGiuongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanThu] CHECK CONSTRAINT [FK_TaiKhoanBenhNhanThu_YeuCauDichVuGiuongBenhVien]
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanThu]  WITH CHECK ADD  CONSTRAINT [FK_TaiKhoanBenhNhanThu_YeuCauDichVuKyThuat] FOREIGN KEY([HoanTraYeuCauDichVuKyThuatId])
REFERENCES [dbo].[YeuCauDichVuKyThuat] ([Id])
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanThu] CHECK CONSTRAINT [FK_TaiKhoanBenhNhanThu_YeuCauDichVuKyThuat]
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanThu]  WITH CHECK ADD  CONSTRAINT [FK_TaiKhoanBenhNhanThu_YeuCauDuocPhamBenhVien] FOREIGN KEY([HoanTraYeuCauDuocPhamBenhVienId])
REFERENCES [dbo].[YeuCauDuocPhamBenhVien] ([Id])
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanThu] CHECK CONSTRAINT [FK_TaiKhoanBenhNhanThu_YeuCauDuocPhamBenhVien]
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanThu]  WITH CHECK ADD  CONSTRAINT [FK_TaiKhoanBenhNhanThu_YeuCauGoiDichVu] FOREIGN KEY([HoanTraYeuCauGoiDichVuId])
REFERENCES [dbo].[YeuCauGoiDichVu] ([Id])
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanThu] CHECK CONSTRAINT [FK_TaiKhoanBenhNhanThu_YeuCauGoiDichVu]
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanThu]  WITH CHECK ADD  CONSTRAINT [FK_TaiKhoanBenhNhanThu_YeuCauKhamBenh] FOREIGN KEY([HoanTraYeuCauKhamBenhId])
REFERENCES [dbo].[YeuCauKhamBenh] ([Id])
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanThu] CHECK CONSTRAINT [FK_TaiKhoanBenhNhanThu_YeuCauKhamBenh]
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanThu]  WITH CHECK ADD  CONSTRAINT [FK_TaiKhoanBenhNhanThu_YeuCauKhamBenhDonThuocThanhToan] FOREIGN KEY([HoanTraDonThuocThanhToanId])
REFERENCES [dbo].[DonThuocThanhToan] ([Id])
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanThu] CHECK CONSTRAINT [FK_TaiKhoanBenhNhanThu_YeuCauKhamBenhDonThuocThanhToan]
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanThu]  WITH CHECK ADD  CONSTRAINT [FK_TaiKhoanBenhNhanThu_YeuCauVatTuBenhVien] FOREIGN KEY([HoanTraYeuCauVatTuBenhVienId])
REFERENCES [dbo].[YeuCauVatTuBenhVien] ([Id])
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanThu] CHECK CONSTRAINT [FK_TaiKhoanBenhNhanThu_YeuCauVatTuBenhVien]
GO
ALTER TABLE [dbo].[ThuocHoacHoatChat]  WITH CHECK ADD  CONSTRAINT [FK__ThuocTanD__Duong__3A179ED3] FOREIGN KEY([DuongDungId])
REFERENCES [dbo].[DuongDung] ([Id])
GO
ALTER TABLE [dbo].[ThuocHoacHoatChat] CHECK CONSTRAINT [FK__ThuocTanD__Duong__3A179ED3]
GO
ALTER TABLE [dbo].[ThuocHoacHoatChat]  WITH CHECK ADD  CONSTRAINT [FK__ThuocTanD__NhomT__1E6F845E] FOREIGN KEY([NhomThuocId])
REFERENCES [dbo].[NhomThuoc] ([Id])
GO
ALTER TABLE [dbo].[ThuocHoacHoatChat] CHECK CONSTRAINT [FK__ThuocTanD__NhomT__1E6F845E]
GO
ALTER TABLE [dbo].[ToaThuocMau]  WITH CHECK ADD  CONSTRAINT [FK_ToaThuocMau_ChuanDoan] FOREIGN KEY([ChuanDoanId])
REFERENCES [dbo].[ChuanDoan] ([Id])
GO
ALTER TABLE [dbo].[ToaThuocMau] CHECK CONSTRAINT [FK_ToaThuocMau_ChuanDoan]
GO
ALTER TABLE [dbo].[ToaThuocMau]  WITH CHECK ADD  CONSTRAINT [FK_ToaThuocMau_ICD] FOREIGN KEY([ICDId])
REFERENCES [dbo].[ICD] ([Id])
GO
ALTER TABLE [dbo].[ToaThuocMau] CHECK CONSTRAINT [FK_ToaThuocMau_ICD]
GO
ALTER TABLE [dbo].[ToaThuocMau]  WITH CHECK ADD  CONSTRAINT [FK_ToaThuocMau_NhanVien] FOREIGN KEY([BacSiKeToaId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[ToaThuocMau] CHECK CONSTRAINT [FK_ToaThuocMau_NhanVien]
GO
ALTER TABLE [dbo].[ToaThuocMau]  WITH CHECK ADD  CONSTRAINT [FK_ToaThuocMau_TrieuChung] FOREIGN KEY([TrieuChungId])
REFERENCES [dbo].[TrieuChung] ([Id])
GO
ALTER TABLE [dbo].[ToaThuocMau] CHECK CONSTRAINT [FK_ToaThuocMau_TrieuChung]
GO
ALTER TABLE [dbo].[ToaThuocMauChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_ToaThuocMauChiTiet_DuocPham] FOREIGN KEY([DuocPhamId])
REFERENCES [dbo].[DuocPham] ([Id])
GO
ALTER TABLE [dbo].[ToaThuocMauChiTiet] CHECK CONSTRAINT [FK_ToaThuocMauChiTiet_DuocPham]
GO
ALTER TABLE [dbo].[ToaThuocMauChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_ToaThuocMauChiTiet_ToaThuocMau] FOREIGN KEY([ToaThuocMauId])
REFERENCES [dbo].[ToaThuocMau] ([Id])
GO
ALTER TABLE [dbo].[ToaThuocMauChiTiet] CHECK CONSTRAINT [FK_ToaThuocMauChiTiet_ToaThuocMau]
GO
ALTER TABLE [dbo].[TrieuChungDanhMucChuanDoan]  WITH CHECK ADD  CONSTRAINT [FK_TrieuChungDanhMucChuanDoan_DanhMucChuanDoan] FOREIGN KEY([DanhMucChuanDoanId])
REFERENCES [dbo].[DanhMucChuanDoan] ([Id])
GO
ALTER TABLE [dbo].[TrieuChungDanhMucChuanDoan] CHECK CONSTRAINT [FK_TrieuChungDanhMucChuanDoan_DanhMucChuanDoan]
GO
ALTER TABLE [dbo].[TrieuChungDanhMucChuanDoan]  WITH CHECK ADD  CONSTRAINT [FK_TrieuChungDanhMucChuanDoan_TrieuChung] FOREIGN KEY([TrieuChungId])
REFERENCES [dbo].[TrieuChung] ([Id])
GO
ALTER TABLE [dbo].[TrieuChungDanhMucChuanDoan] CHECK CONSTRAINT [FK_TrieuChungDanhMucChuanDoan_TrieuChung]
GO
ALTER TABLE [dbo].[UserMessagingToken]  WITH CHECK ADD  CONSTRAINT [FK_UserMessagingToken_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[UserMessagingToken] CHECK CONSTRAINT [FK_UserMessagingToken_User]
GO
ALTER TABLE [dbo].[UserMessagingTokenSubscribe]  WITH CHECK ADD  CONSTRAINT [FK_UserMessagingTokenSubscribe_UserMessagingToken] FOREIGN KEY([UserMessagingTokenId])
REFERENCES [dbo].[UserMessagingToken] ([Id])
GO
ALTER TABLE [dbo].[UserMessagingTokenSubscribe] CHECK CONSTRAINT [FK_UserMessagingTokenSubscribe_UserMessagingToken]
GO
ALTER TABLE [dbo].[VatTu]  WITH CHECK ADD  CONSTRAINT [FK_VatTu_NhomVatTu] FOREIGN KEY([NhomVatTuId])
REFERENCES [dbo].[NhomVatTu] ([Id])
GO
ALTER TABLE [dbo].[VatTu] CHECK CONSTRAINT [FK_VatTu_NhomVatTu]
GO
ALTER TABLE [dbo].[VatTuBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_VatTuBenhVien_VatTu] FOREIGN KEY([Id])
REFERENCES [dbo].[VatTu] ([Id])
GO
ALTER TABLE [dbo].[VatTuBenhVien] CHECK CONSTRAINT [FK_VatTuBenhVien_VatTu]
GO
ALTER TABLE [dbo].[XuatKhoDuocPham]  WITH CHECK ADD  CONSTRAINT [FK_XuatKhoDuocPham_KhoDuocPham] FOREIGN KEY([KhoDuocPhamXuatId])
REFERENCES [dbo].[KhoDuocPham] ([Id])
GO
ALTER TABLE [dbo].[XuatKhoDuocPham] CHECK CONSTRAINT [FK_XuatKhoDuocPham_KhoDuocPham]
GO
ALTER TABLE [dbo].[XuatKhoDuocPham]  WITH CHECK ADD  CONSTRAINT [FK_XuatKhoDuocPham_KhoDuocPham1] FOREIGN KEY([KhoDuocPhamNhapId])
REFERENCES [dbo].[KhoDuocPham] ([Id])
GO
ALTER TABLE [dbo].[XuatKhoDuocPham] CHECK CONSTRAINT [FK_XuatKhoDuocPham_KhoDuocPham1]
GO
ALTER TABLE [dbo].[XuatKhoDuocPham]  WITH CHECK ADD  CONSTRAINT [FK_XuatKhoDuocPham_NhanVien] FOREIGN KEY([NguoiNhanId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[XuatKhoDuocPham] CHECK CONSTRAINT [FK_XuatKhoDuocPham_NhanVien]
GO
ALTER TABLE [dbo].[XuatKhoDuocPham]  WITH CHECK ADD  CONSTRAINT [FK_XuatKhoDuocPham_NhanVien1] FOREIGN KEY([NguoiXuatId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[XuatKhoDuocPham] CHECK CONSTRAINT [FK_XuatKhoDuocPham_NhanVien1]
GO
ALTER TABLE [dbo].[XuatKhoDuocPhamChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_XuatKhoDuocPhamChiTiet_DuocPhamBenhVien] FOREIGN KEY([DuocPhamBenhVienId])
REFERENCES [dbo].[DuocPhamBenhVien] ([Id])
GO
ALTER TABLE [dbo].[XuatKhoDuocPhamChiTiet] CHECK CONSTRAINT [FK_XuatKhoDuocPhamChiTiet_DuocPhamBenhVien]
GO
ALTER TABLE [dbo].[XuatKhoDuocPhamChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_XuatKhoDuocPhamChiTiet_XuatKhoDuocPham] FOREIGN KEY([XuatKhoDuocPhamId])
REFERENCES [dbo].[XuatKhoDuocPham] ([Id])
GO
ALTER TABLE [dbo].[XuatKhoDuocPhamChiTiet] CHECK CONSTRAINT [FK_XuatKhoDuocPhamChiTiet_XuatKhoDuocPham]
GO
ALTER TABLE [dbo].[XuatKhoDuocPhamChiTietViTri]  WITH CHECK ADD  CONSTRAINT [FK_XuatKhoDuocPhamChiTietViTri_NhapKhoDuocPhamChiTiet] FOREIGN KEY([NhapKhoDuocPhamChiTietId])
REFERENCES [dbo].[NhapKhoDuocPhamChiTiet] ([Id])
GO
ALTER TABLE [dbo].[XuatKhoDuocPhamChiTietViTri] CHECK CONSTRAINT [FK_XuatKhoDuocPhamChiTietViTri_NhapKhoDuocPhamChiTiet]
GO
ALTER TABLE [dbo].[XuatKhoDuocPhamChiTietViTri]  WITH CHECK ADD  CONSTRAINT [FK_XuatKhoDuocPhamChiTietViTri_XuatKhoDuocPhamChiTiet] FOREIGN KEY([XuatKhoDuocPhamChiTietId])
REFERENCES [dbo].[XuatKhoDuocPhamChiTiet] ([Id])
GO
ALTER TABLE [dbo].[XuatKhoDuocPhamChiTietViTri] CHECK CONSTRAINT [FK_XuatKhoDuocPhamChiTietViTri_XuatKhoDuocPhamChiTiet]
GO
ALTER TABLE [dbo].[YeuCauDichVuGiuongBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDichVuGiuongBenhVien_DichVuGiuongBenhVien] FOREIGN KEY([DichVuGiuongBenhVienId])
REFERENCES [dbo].[DichVuGiuongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauDichVuGiuongBenhVien] CHECK CONSTRAINT [FK_YeuCauDichVuGiuongBenhVien_DichVuGiuongBenhVien]
GO
ALTER TABLE [dbo].[YeuCauDichVuGiuongBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDichVuGiuongBenhVien_NhanVien] FOREIGN KEY([NhanVienChiDinhId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauDichVuGiuongBenhVien] CHECK CONSTRAINT [FK_YeuCauDichVuGiuongBenhVien_NhanVien]
GO
ALTER TABLE [dbo].[YeuCauDichVuGiuongBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDichVuGiuongBenhVien_NhanVien1] FOREIGN KEY([NhanVienDuyetBaoHiemId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauDichVuGiuongBenhVien] CHECK CONSTRAINT [FK_YeuCauDichVuGiuongBenhVien_NhanVien1]
GO
ALTER TABLE [dbo].[YeuCauDichVuGiuongBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDichVuGiuongBenhVien_NhanVien2] FOREIGN KEY([NhanVienHuyThanhToanId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauDichVuGiuongBenhVien] CHECK CONSTRAINT [FK_YeuCauDichVuGiuongBenhVien_NhanVien2]
GO
ALTER TABLE [dbo].[YeuCauDichVuGiuongBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDichVuGiuongBenhVien_NhomGiaDichVuGiuongBenhVien] FOREIGN KEY([NhomGiaDichVuGiuongBenhVienId])
REFERENCES [dbo].[NhomGiaDichVuGiuongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauDichVuGiuongBenhVien] CHECK CONSTRAINT [FK_YeuCauDichVuGiuongBenhVien_NhomGiaDichVuGiuongBenhVien]
GO
ALTER TABLE [dbo].[YeuCauDichVuGiuongBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDichVuGiuongBenhVien_PhongBenhVien] FOREIGN KEY([NoiChiDinhId])
REFERENCES [dbo].[PhongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauDichVuGiuongBenhVien] CHECK CONSTRAINT [FK_YeuCauDichVuGiuongBenhVien_PhongBenhVien]
GO
ALTER TABLE [dbo].[YeuCauDichVuGiuongBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDichVuGiuongBenhVien_PhongBenhVien1] FOREIGN KEY([NoiThucHienId])
REFERENCES [dbo].[PhongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauDichVuGiuongBenhVien] CHECK CONSTRAINT [FK_YeuCauDichVuGiuongBenhVien_PhongBenhVien1]
GO
ALTER TABLE [dbo].[YeuCauDichVuGiuongBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDichVuGiuongBenhVien_YeuCauDichVuKyThuat] FOREIGN KEY([YeuCauDichVuKyThuatId])
REFERENCES [dbo].[YeuCauDichVuKyThuat] ([Id])
GO
ALTER TABLE [dbo].[YeuCauDichVuGiuongBenhVien] CHECK CONSTRAINT [FK_YeuCauDichVuGiuongBenhVien_YeuCauDichVuKyThuat]
GO
ALTER TABLE [dbo].[YeuCauDichVuGiuongBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDichVuGiuongBenhVien_YeuCauGoiDichVu] FOREIGN KEY([YeuCauGoiDichVuId])
REFERENCES [dbo].[YeuCauGoiDichVu] ([Id])
GO
ALTER TABLE [dbo].[YeuCauDichVuGiuongBenhVien] CHECK CONSTRAINT [FK_YeuCauDichVuGiuongBenhVien_YeuCauGoiDichVu]
GO
ALTER TABLE [dbo].[YeuCauDichVuGiuongBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDichVuGiuongBenhVien_YeuCauKhamBenh] FOREIGN KEY([YeuCauKhamBenhId])
REFERENCES [dbo].[YeuCauKhamBenh] ([Id])
GO
ALTER TABLE [dbo].[YeuCauDichVuGiuongBenhVien] CHECK CONSTRAINT [FK_YeuCauDichVuGiuongBenhVien_YeuCauKhamBenh]
GO
ALTER TABLE [dbo].[YeuCauDichVuGiuongBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDichVuGiuongBenhVien_YeuCauTiepNhan] FOREIGN KEY([YeuCauTiepNhanId])
REFERENCES [dbo].[YeuCauTiepNhan] ([Id])
GO
ALTER TABLE [dbo].[YeuCauDichVuGiuongBenhVien] CHECK CONSTRAINT [FK_YeuCauDichVuGiuongBenhVien_YeuCauTiepNhan]
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDichVuKyThuat_NhanVien] FOREIGN KEY([NhanVienDuyetBaoHiemId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat] CHECK CONSTRAINT [FK_YeuCauDichVuKyThuat_NhanVien]
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDichVuKyThuat_NhanVien1] FOREIGN KEY([NhanVienChiDinhId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat] CHECK CONSTRAINT [FK_YeuCauDichVuKyThuat_NhanVien1]
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDichVuKyThuat_NhanVien2] FOREIGN KEY([NhanVienHuyThanhToanId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat] CHECK CONSTRAINT [FK_YeuCauDichVuKyThuat_NhanVien2]
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDichVuKyThuat_PhongBenhVien] FOREIGN KEY([NoiChiDinhId])
REFERENCES [dbo].[PhongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat] CHECK CONSTRAINT [FK_YeuCauDichVuKyThuat_PhongBenhVien]
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDichVuKyThuat_YeuCauGoiDichVu] FOREIGN KEY([YeuCauGoiDichVuId])
REFERENCES [dbo].[YeuCauGoiDichVu] ([Id])
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat] CHECK CONSTRAINT [FK_YeuCauDichVuKyThuat_YeuCauGoiDichVu]
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDichVuKyThuat_YeuCauKhamBenh] FOREIGN KEY([YeuCauKhamBenhId])
REFERENCES [dbo].[YeuCauKhamBenh] ([Id])
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat] CHECK CONSTRAINT [FK_YeuCauDichVuKyThuat_YeuCauKhamBenh]
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhDichVuKyThuat_DichVuKyThuatBenhVien] FOREIGN KEY([DichVuKyThuatBenhVienId])
REFERENCES [dbo].[DichVuKyThuatBenhVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat] CHECK CONSTRAINT [FK_YeuCauKhamBenhDichVuKyThuat_DichVuKyThuatBenhVien]
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhDichVuKyThuat_NhanVien1] FOREIGN KEY([NhanVienThucHienId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat] CHECK CONSTRAINT [FK_YeuCauKhamBenhDichVuKyThuat_NhanVien1]
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhDichVuKyThuat_NhanVien2] FOREIGN KEY([NhanVienKetLuanId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat] CHECK CONSTRAINT [FK_YeuCauKhamBenhDichVuKyThuat_NhanVien2]
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhDichVuKyThuat_PhongBenhVien1] FOREIGN KEY([NoiThucHienId])
REFERENCES [dbo].[PhongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat] CHECK CONSTRAINT [FK_YeuCauKhamBenhDichVuKyThuat_PhongBenhVien1]
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhDichVuKyThuat_YeuCauKhamBenh] FOREIGN KEY([YeuCauTiepNhanId])
REFERENCES [dbo].[YeuCauTiepNhan] ([Id])
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat] CHECK CONSTRAINT [FK_YeuCauKhamBenhDichVuKyThuat_YeuCauKhamBenh]
GO
ALTER TABLE [dbo].[YeuCauDuocPhamBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDuocPhamBenhVien_XuatKhoDuocPhamChiTietViTri] FOREIGN KEY([XuatKhoDuocPhamChiTietViTriId])
REFERENCES [dbo].[XuatKhoDuocPhamChiTietViTri] ([Id])
GO
ALTER TABLE [dbo].[YeuCauDuocPhamBenhVien] CHECK CONSTRAINT [FK_YeuCauDuocPhamBenhVien_XuatKhoDuocPhamChiTietViTri]
GO
ALTER TABLE [dbo].[YeuCauGoiDichVu]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauGoiDichVu_GoiDichVu] FOREIGN KEY([GoiDichVuId])
REFERENCES [dbo].[GoiDichVu] ([Id])
GO
ALTER TABLE [dbo].[YeuCauGoiDichVu] CHECK CONSTRAINT [FK_YeuCauGoiDichVu_GoiDichVu]
GO
ALTER TABLE [dbo].[YeuCauGoiDichVu]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauGoiDichVu_YeuCauKhamBenh] FOREIGN KEY([YeuCauKhamBenhId])
REFERENCES [dbo].[YeuCauKhamBenh] ([Id])
GO
ALTER TABLE [dbo].[YeuCauGoiDichVu] CHECK CONSTRAINT [FK_YeuCauGoiDichVu_YeuCauKhamBenh]
GO
ALTER TABLE [dbo].[YeuCauGoiDichVu]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauGoiDichVu_YeuCauTiepNhan] FOREIGN KEY([YeuCauTiepNhanId])
REFERENCES [dbo].[YeuCauTiepNhan] ([Id])
GO
ALTER TABLE [dbo].[YeuCauGoiDichVu] CHECK CONSTRAINT [FK_YeuCauGoiDichVu_YeuCauTiepNhan]
GO
ALTER TABLE [dbo].[YeuCauKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenh_NhanVien] FOREIGN KEY([NhanVienDuyetBaoHiemId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauKhamBenh] CHECK CONSTRAINT [FK_YeuCauKhamBenh_NhanVien]
GO
ALTER TABLE [dbo].[YeuCauKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenh_NhanVien1] FOREIGN KEY([NhanVienChiDinhId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauKhamBenh] CHECK CONSTRAINT [FK_YeuCauKhamBenh_NhanVien1]
GO
ALTER TABLE [dbo].[YeuCauKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenh_NhanVien3] FOREIGN KEY([NhanVienHuyThanhToanId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauKhamBenh] CHECK CONSTRAINT [FK_YeuCauKhamBenh_NhanVien3]
GO
ALTER TABLE [dbo].[YeuCauKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenh_NhomGiaDichVuKhamBenhBenhVien] FOREIGN KEY([NhomGiaDichVuKhamBenhBenhVienId])
REFERENCES [dbo].[NhomGiaDichVuKhamBenhBenhVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauKhamBenh] CHECK CONSTRAINT [FK_YeuCauKhamBenh_NhomGiaDichVuKhamBenhBenhVien]
GO
ALTER TABLE [dbo].[YeuCauKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenh_PhongBenhVien] FOREIGN KEY([NoiChiDinhId])
REFERENCES [dbo].[PhongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauKhamBenh] CHECK CONSTRAINT [FK_YeuCauKhamBenh_PhongBenhVien]
GO
ALTER TABLE [dbo].[YeuCauKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenh_YeuCauGoiDichVu] FOREIGN KEY([YeuCauGoiDichVuId])
REFERENCES [dbo].[YeuCauGoiDichVu] ([Id])
GO
ALTER TABLE [dbo].[YeuCauKhamBenh] CHECK CONSTRAINT [FK_YeuCauKhamBenh_YeuCauGoiDichVu]
GO
ALTER TABLE [dbo].[YeuCauKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenh_YeuCauKhamBenh] FOREIGN KEY([YeuCauKhamBenhTruocId])
REFERENCES [dbo].[YeuCauKhamBenh] ([Id])
GO
ALTER TABLE [dbo].[YeuCauKhamBenh] CHECK CONSTRAINT [FK_YeuCauKhamBenh_YeuCauKhamBenh]
GO
ALTER TABLE [dbo].[YeuCauKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhDichVuKhamBenh_DichVuKhamBenhBenhVien] FOREIGN KEY([DichVuKhamBenhBenhVienId])
REFERENCES [dbo].[DichVuKhamBenhBenhVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauKhamBenh] CHECK CONSTRAINT [FK_YeuCauKhamBenhDichVuKhamBenh_DichVuKhamBenhBenhVien]
GO
ALTER TABLE [dbo].[YeuCauKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhDichVuKhamBenh_NhanVien1] FOREIGN KEY([BacSiDangKyId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauKhamBenh] CHECK CONSTRAINT [FK_YeuCauKhamBenhDichVuKhamBenh_NhanVien1]
GO
ALTER TABLE [dbo].[YeuCauKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhDichVuKhamBenh_NhanVien2] FOREIGN KEY([BacSiThucHienId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauKhamBenh] CHECK CONSTRAINT [FK_YeuCauKhamBenhDichVuKhamBenh_NhanVien2]
GO
ALTER TABLE [dbo].[YeuCauKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhDichVuKhamBenh_PhongBenhVien1] FOREIGN KEY([NoiDangKyId])
REFERENCES [dbo].[PhongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauKhamBenh] CHECK CONSTRAINT [FK_YeuCauKhamBenhDichVuKhamBenh_PhongBenhVien1]
GO
ALTER TABLE [dbo].[YeuCauKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhDichVuKhamBenh_PhongBenhVien2] FOREIGN KEY([NoiThucHienId])
REFERENCES [dbo].[PhongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauKhamBenh] CHECK CONSTRAINT [FK_YeuCauKhamBenhDichVuKhamBenh_PhongBenhVien2]
GO
ALTER TABLE [dbo].[YeuCauKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhDichVuKhamBenh_YeuCauKhamBenh] FOREIGN KEY([YeuCauTiepNhanId])
REFERENCES [dbo].[YeuCauTiepNhan] ([Id])
GO
ALTER TABLE [dbo].[YeuCauKhamBenh] CHECK CONSTRAINT [FK_YeuCauKhamBenhDichVuKhamBenh_YeuCauKhamBenh]
GO
ALTER TABLE [dbo].[YeuCauKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauTiepNhanDichVuKhamBenh_ICD] FOREIGN KEY([ICDChinhId])
REFERENCES [dbo].[ICD] ([Id])
GO
ALTER TABLE [dbo].[YeuCauKhamBenh] CHECK CONSTRAINT [FK_YeuCauTiepNhanDichVuKhamBenh_ICD]
GO
ALTER TABLE [dbo].[YeuCauKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauTiepNhanDichVuKhamBenh_NhanVien] FOREIGN KEY([BacSiKetLuanId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauKhamBenh] CHECK CONSTRAINT [FK_YeuCauTiepNhanDichVuKhamBenh_NhanVien]
GO
ALTER TABLE [dbo].[YeuCauKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauTiepNhanDichVuKhamBenh_PhongBenhVien] FOREIGN KEY([NoiKetLuanId])
REFERENCES [dbo].[PhongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauKhamBenh] CHECK CONSTRAINT [FK_YeuCauTiepNhanDichVuKhamBenh_PhongBenhVien]
GO
ALTER TABLE [dbo].[YeuCauKhamBenhChuanDoan]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhChuanDoan_DRG] FOREIGN KEY([ChuanDoanId])
REFERENCES [dbo].[ChuanDoan] ([Id])
GO
ALTER TABLE [dbo].[YeuCauKhamBenhChuanDoan] CHECK CONSTRAINT [FK_YeuCauKhamBenhChuanDoan_DRG]
GO
ALTER TABLE [dbo].[YeuCauKhamBenhChuanDoan]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhChuanDoan_YeuCauTiepNhanDichVuKhamBenh] FOREIGN KEY([YeuCauKhamBenhId])
REFERENCES [dbo].[YeuCauKhamBenh] ([Id])
GO
ALTER TABLE [dbo].[YeuCauKhamBenhChuanDoan] CHECK CONSTRAINT [FK_YeuCauKhamBenhChuanDoan_YeuCauTiepNhanDichVuKhamBenh]
GO
ALTER TABLE [dbo].[YeuCauKhamBenhDonThuoc]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhDonThuoc_NhanVien] FOREIGN KEY([BacSiKeDonId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauKhamBenhDonThuoc] CHECK CONSTRAINT [FK_YeuCauKhamBenhDonThuoc_NhanVien]
GO
ALTER TABLE [dbo].[YeuCauKhamBenhDonThuoc]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhDonThuoc_PhongBenhVien] FOREIGN KEY([NoiKeDonId])
REFERENCES [dbo].[PhongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauKhamBenhDonThuoc] CHECK CONSTRAINT [FK_YeuCauKhamBenhDonThuoc_PhongBenhVien]
GO
ALTER TABLE [dbo].[YeuCauKhamBenhDonThuoc]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhDonThuoc_ToaThuocMau] FOREIGN KEY([ToaThuocMauId])
REFERENCES [dbo].[ToaThuocMau] ([Id])
GO
ALTER TABLE [dbo].[YeuCauKhamBenhDonThuoc] CHECK CONSTRAINT [FK_YeuCauKhamBenhDonThuoc_ToaThuocMau]
GO
ALTER TABLE [dbo].[YeuCauKhamBenhDonThuoc]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauTiepNhanDonThuoc_YeuCauTiepNhanDichVuKhamBenh] FOREIGN KEY([YeuCauKhamBenhId])
REFERENCES [dbo].[YeuCauKhamBenh] ([Id])
GO
ALTER TABLE [dbo].[YeuCauKhamBenhDonThuoc] CHECK CONSTRAINT [FK_YeuCauTiepNhanDonThuoc_YeuCauTiepNhanDichVuKhamBenh]
GO
ALTER TABLE [dbo].[YeuCauKhamBenhDonThuocChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhDonThuocChiTiet_DonViTinh] FOREIGN KEY([DonViTinhId])
REFERENCES [dbo].[DonViTinh] ([Id])
GO
ALTER TABLE [dbo].[YeuCauKhamBenhDonThuocChiTiet] CHECK CONSTRAINT [FK_YeuCauKhamBenhDonThuocChiTiet_DonViTinh]
GO
ALTER TABLE [dbo].[YeuCauKhamBenhDonThuocChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhDonThuocChiTiet_DuocPham] FOREIGN KEY([DuocPhamId])
REFERENCES [dbo].[DuocPham] ([Id])
GO
ALTER TABLE [dbo].[YeuCauKhamBenhDonThuocChiTiet] CHECK CONSTRAINT [FK_YeuCauKhamBenhDonThuocChiTiet_DuocPham]
GO
ALTER TABLE [dbo].[YeuCauKhamBenhDonThuocChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhDonThuocChiTiet_DuongDung] FOREIGN KEY([DuongDungId])
REFERENCES [dbo].[DuongDung] ([Id])
GO
ALTER TABLE [dbo].[YeuCauKhamBenhDonThuocChiTiet] CHECK CONSTRAINT [FK_YeuCauKhamBenhDonThuocChiTiet_DuongDung]
GO
ALTER TABLE [dbo].[YeuCauKhamBenhDonThuocChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhDonThuocChiTiet_YeuCauKhamBenhDonThuoc] FOREIGN KEY([YeuCauKhamBenhDonThuocId])
REFERENCES [dbo].[YeuCauKhamBenhDonThuoc] ([Id])
GO
ALTER TABLE [dbo].[YeuCauKhamBenhDonThuocChiTiet] CHECK CONSTRAINT [FK_YeuCauKhamBenhDonThuocChiTiet_YeuCauKhamBenhDonThuoc]
GO
ALTER TABLE [dbo].[YeuCauKhamBenhICDKhac]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhICDKhac_ICD] FOREIGN KEY([ICDId])
REFERENCES [dbo].[ICD] ([Id])
GO
ALTER TABLE [dbo].[YeuCauKhamBenhICDKhac] CHECK CONSTRAINT [FK_YeuCauKhamBenhICDKhac_ICD]
GO
ALTER TABLE [dbo].[YeuCauKhamBenhICDKhac]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhICDKhac_YeuCauTiepNhanDichVuKhamBenh] FOREIGN KEY([YeuCauKhamBenhId])
REFERENCES [dbo].[YeuCauKhamBenh] ([Id])
GO
ALTER TABLE [dbo].[YeuCauKhamBenhICDKhac] CHECK CONSTRAINT [FK_YeuCauKhamBenhICDKhac_YeuCauTiepNhanDichVuKhamBenh]
GO
ALTER TABLE [dbo].[YeuCauKhamBenhLichSuTrangThai]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhLichSuTrangThai_YeuCauKhamBenh1] FOREIGN KEY([YeuCauKhamBenhId])
REFERENCES [dbo].[YeuCauKhamBenh] ([Id])
GO
ALTER TABLE [dbo].[YeuCauKhamBenhLichSuTrangThai] CHECK CONSTRAINT [FK_YeuCauKhamBenhLichSuTrangThai_YeuCauKhamBenh1]
GO
ALTER TABLE [dbo].[YeuCauKhamBenhTrieuChung]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhTrieuChung_TrieuChung] FOREIGN KEY([TrieuChungId])
REFERENCES [dbo].[TrieuChung] ([Id])
GO
ALTER TABLE [dbo].[YeuCauKhamBenhTrieuChung] CHECK CONSTRAINT [FK_YeuCauKhamBenhTrieuChung_TrieuChung]
GO
ALTER TABLE [dbo].[YeuCauKhamBenhTrieuChung]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhTrieuChung_YeuCauTiepNhanDichVuKhamBenh] FOREIGN KEY([YeuCauKhamBenhId])
REFERENCES [dbo].[YeuCauKhamBenh] ([Id])
GO
ALTER TABLE [dbo].[YeuCauKhamBenhTrieuChung] CHECK CONSTRAINT [FK_YeuCauKhamBenhTrieuChung_YeuCauTiepNhanDichVuKhamBenh]
GO
ALTER TABLE [dbo].[YeuCauTiepNhan]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenh_BenhNhan] FOREIGN KEY([BenhNhanId])
REFERENCES [dbo].[BenhNhan] ([Id])
GO
ALTER TABLE [dbo].[YeuCauTiepNhan] CHECK CONSTRAINT [FK_YeuCauKhamBenh_BenhNhan]
GO
ALTER TABLE [dbo].[YeuCauTiepNhan]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenh_DanToc] FOREIGN KEY([DanTocId])
REFERENCES [dbo].[DanToc] ([Id])
GO
ALTER TABLE [dbo].[YeuCauTiepNhan] CHECK CONSTRAINT [FK_YeuCauKhamBenh_DanToc]
GO
ALTER TABLE [dbo].[YeuCauTiepNhan]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenh_DoiTuongUuTienKhamChuaBenh] FOREIGN KEY([DoiTuongUuTienKhamChuaBenhId])
REFERENCES [dbo].[DoiTuongUuTienKhamChuaBenh] ([Id])
GO
ALTER TABLE [dbo].[YeuCauTiepNhan] CHECK CONSTRAINT [FK_YeuCauKhamBenh_DoiTuongUuTienKhamChuaBenh]
GO
ALTER TABLE [dbo].[YeuCauTiepNhan]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenh_DonViHanhChinh] FOREIGN KEY([PhuongXaId])
REFERENCES [dbo].[DonViHanhChinh] ([Id])
GO
ALTER TABLE [dbo].[YeuCauTiepNhan] CHECK CONSTRAINT [FK_YeuCauKhamBenh_DonViHanhChinh]
GO
ALTER TABLE [dbo].[YeuCauTiepNhan]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenh_DonViHanhChinh1] FOREIGN KEY([QuanHuyenId])
REFERENCES [dbo].[DonViHanhChinh] ([Id])
GO
ALTER TABLE [dbo].[YeuCauTiepNhan] CHECK CONSTRAINT [FK_YeuCauKhamBenh_DonViHanhChinh1]
GO
ALTER TABLE [dbo].[YeuCauTiepNhan]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenh_DonViHanhChinh2] FOREIGN KEY([TinhThanhId])
REFERENCES [dbo].[DonViHanhChinh] ([Id])
GO
ALTER TABLE [dbo].[YeuCauTiepNhan] CHECK CONSTRAINT [FK_YeuCauKhamBenh_DonViHanhChinh2]
GO
ALTER TABLE [dbo].[YeuCauTiepNhan]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenh_DonViHanhChinh3] FOREIGN KEY([NguoiLienHePhuongXaId])
REFERENCES [dbo].[DonViHanhChinh] ([Id])
GO
ALTER TABLE [dbo].[YeuCauTiepNhan] CHECK CONSTRAINT [FK_YeuCauKhamBenh_DonViHanhChinh3]
GO
ALTER TABLE [dbo].[YeuCauTiepNhan]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenh_DonViHanhChinh4] FOREIGN KEY([NguoiLienHeQuanHuyenId])
REFERENCES [dbo].[DonViHanhChinh] ([Id])
GO
ALTER TABLE [dbo].[YeuCauTiepNhan] CHECK CONSTRAINT [FK_YeuCauKhamBenh_DonViHanhChinh4]
GO
ALTER TABLE [dbo].[YeuCauTiepNhan]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenh_DonViHanhChinh5] FOREIGN KEY([NguoiLienHeTinhThanhId])
REFERENCES [dbo].[DonViHanhChinh] ([Id])
GO
ALTER TABLE [dbo].[YeuCauTiepNhan] CHECK CONSTRAINT [FK_YeuCauKhamBenh_DonViHanhChinh5]
GO
ALTER TABLE [dbo].[YeuCauTiepNhan]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenh_GiayChuyenVien] FOREIGN KEY([GiayChuyenVienId])
REFERENCES [dbo].[GiayChuyenVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauTiepNhan] CHECK CONSTRAINT [FK_YeuCauKhamBenh_GiayChuyenVien]
GO
ALTER TABLE [dbo].[YeuCauTiepNhan]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenh_NgheNghiep] FOREIGN KEY([NgheNghiepId])
REFERENCES [dbo].[NgheNghiep] ([Id])
GO
ALTER TABLE [dbo].[YeuCauTiepNhan] CHECK CONSTRAINT [FK_YeuCauKhamBenh_NgheNghiep]
GO
ALTER TABLE [dbo].[YeuCauTiepNhan]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenh_NhanVien2] FOREIGN KEY([NhanVienTiepNhanId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauTiepNhan] CHECK CONSTRAINT [FK_YeuCauKhamBenh_NhanVien2]
GO
ALTER TABLE [dbo].[YeuCauTiepNhan]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenh_PhongBenhVien2] FOREIGN KEY([NoiTiepNhanId])
REFERENCES [dbo].[PhongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauTiepNhan] CHECK CONSTRAINT [FK_YeuCauKhamBenh_PhongBenhVien2]
GO
ALTER TABLE [dbo].[YeuCauTiepNhan]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenh_QuanHeNhanThan] FOREIGN KEY([NguoiLienHeQuanHeNhanThanId])
REFERENCES [dbo].[QuanHeNhanThan] ([Id])
GO
ALTER TABLE [dbo].[YeuCauTiepNhan] CHECK CONSTRAINT [FK_YeuCauKhamBenh_QuanHeNhanThan]
GO
ALTER TABLE [dbo].[YeuCauTiepNhan]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenh_QuocGia] FOREIGN KEY([QuocTichId])
REFERENCES [dbo].[QuocGia] ([Id])
GO
ALTER TABLE [dbo].[YeuCauTiepNhan] CHECK CONSTRAINT [FK_YeuCauKhamBenh_QuocGia]
GO
ALTER TABLE [dbo].[YeuCauTiepNhan]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauTiepNhan_CongTyUuDai] FOREIGN KEY([CongTyUuDaiId])
REFERENCES [dbo].[CongTyUuDai] ([Id])
GO
ALTER TABLE [dbo].[YeuCauTiepNhan] CHECK CONSTRAINT [FK_YeuCauTiepNhan_CongTyUuDai]
GO
ALTER TABLE [dbo].[YeuCauTiepNhan]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauTiepNhan_DoiTuongUuDai] FOREIGN KEY([DoiTuongUuDaiId])
REFERENCES [dbo].[DoiTuongUuDai] ([Id])
GO
ALTER TABLE [dbo].[YeuCauTiepNhan] CHECK CONSTRAINT [FK_YeuCauTiepNhan_DoiTuongUuDai]
GO
ALTER TABLE [dbo].[YeuCauTiepNhan]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauTiepNhan_GiayMienCungChiTra] FOREIGN KEY([BHYTGiayMienCungChiTraId])
REFERENCES [dbo].[GiayMienCungChiTra] ([Id])
GO
ALTER TABLE [dbo].[YeuCauTiepNhan] CHECK CONSTRAINT [FK_YeuCauTiepNhan_GiayMienCungChiTra]
GO
ALTER TABLE [dbo].[YeuCauTiepNhan]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauTiepNhan_HinhThucDen] FOREIGN KEY([HinhThucDenId])
REFERENCES [dbo].[HinhThucDen] ([Id])
GO
ALTER TABLE [dbo].[YeuCauTiepNhan] CHECK CONSTRAINT [FK_YeuCauTiepNhan_HinhThucDen]
GO
ALTER TABLE [dbo].[YeuCauTiepNhan]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauTiepNhan_NoiGioiThieu] FOREIGN KEY([NoiGioiThieuId])
REFERENCES [dbo].[NoiGioiThieu] ([Id])
GO
ALTER TABLE [dbo].[YeuCauTiepNhan] CHECK CONSTRAINT [FK_YeuCauTiepNhan_NoiGioiThieu]
GO
ALTER TABLE [dbo].[YeuCauTiepNhanCongTyBaoHiemTuNhan]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauTiepNhanCongTyBaoHiemTuNhan_CongTyBaoHiemTuNhan] FOREIGN KEY([CongTyBaoHiemTuNhanId])
REFERENCES [dbo].[CongTyBaoHiemTuNhan] ([Id])
GO
ALTER TABLE [dbo].[YeuCauTiepNhanCongTyBaoHiemTuNhan] CHECK CONSTRAINT [FK_YeuCauTiepNhanCongTyBaoHiemTuNhan_CongTyBaoHiemTuNhan]
GO
ALTER TABLE [dbo].[YeuCauTiepNhanCongTyBaoHiemTuNhan]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauTiepNhanCongTyBaoHiemTuNhan_YeuCauTiepNhan] FOREIGN KEY([YeuCauTiepNhanId])
REFERENCES [dbo].[YeuCauTiepNhan] ([Id])
GO
ALTER TABLE [dbo].[YeuCauTiepNhanCongTyBaoHiemTuNhan] CHECK CONSTRAINT [FK_YeuCauTiepNhanCongTyBaoHiemTuNhan_YeuCauTiepNhan]
GO
ALTER TABLE [dbo].[YeuCauTiepNhanLichSuTrangThai]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhLichSuTrangThai_YeuCauKhamBenh] FOREIGN KEY([YeuCauTiepNhanId])
REFERENCES [dbo].[YeuCauTiepNhan] ([Id])
GO
ALTER TABLE [dbo].[YeuCauTiepNhanLichSuTrangThai] CHECK CONSTRAINT [FK_YeuCauKhamBenhLichSuTrangThai_YeuCauKhamBenh]
GO
ALTER TABLE [dbo].[YeuCauTiepNhanThuocDangSuDung]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauTiepNhanThuocDangSuDung_DuocPham] FOREIGN KEY([DuocPhamId])
REFERENCES [dbo].[DuocPham] ([Id])
GO
ALTER TABLE [dbo].[YeuCauTiepNhanThuocDangSuDung] CHECK CONSTRAINT [FK_YeuCauTiepNhanThuocDangSuDung_DuocPham]
GO
ALTER TABLE [dbo].[YeuCauTiepNhanThuocDangSuDung]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauTiepNhanThuocDangSuDung_YeuCauTiepNhan] FOREIGN KEY([YeuCauTiepNhanId])
REFERENCES [dbo].[YeuCauTiepNhan] ([Id])
GO
ALTER TABLE [dbo].[YeuCauTiepNhanThuocDangSuDung] CHECK CONSTRAINT [FK_YeuCauTiepNhanThuocDangSuDung_YeuCauTiepNhan]
GO
ALTER TABLE [dbo].[YeuCauVatTuBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauVatTuBenhVien_NhanVien] FOREIGN KEY([NhanVienChiDinhId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauVatTuBenhVien] CHECK CONSTRAINT [FK_YeuCauVatTuBenhVien_NhanVien]
GO
ALTER TABLE [dbo].[YeuCauVatTuBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauVatTuBenhVien_NhanVien1] FOREIGN KEY([NhanVienCapVatTuId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauVatTuBenhVien] CHECK CONSTRAINT [FK_YeuCauVatTuBenhVien_NhanVien1]
GO
ALTER TABLE [dbo].[YeuCauVatTuBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauVatTuBenhVien_NhanVien2] FOREIGN KEY([NhanVienHuyThanhToanId])
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauVatTuBenhVien] CHECK CONSTRAINT [FK_YeuCauVatTuBenhVien_NhanVien2]
GO
ALTER TABLE [dbo].[YeuCauVatTuBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauVatTuBenhVien_PhongBenhVien] FOREIGN KEY([NoiChiDinhId])
REFERENCES [dbo].[PhongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauVatTuBenhVien] CHECK CONSTRAINT [FK_YeuCauVatTuBenhVien_PhongBenhVien]
GO
ALTER TABLE [dbo].[YeuCauVatTuBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauVatTuBenhVien_PhongBenhVien1] FOREIGN KEY([NoiCapVatTuId])
REFERENCES [dbo].[PhongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauVatTuBenhVien] CHECK CONSTRAINT [FK_YeuCauVatTuBenhVien_PhongBenhVien1]
GO
ALTER TABLE [dbo].[YeuCauVatTuBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauVatTuBenhVien_VatTuBenhVien] FOREIGN KEY([VatTuBenhVienId])
REFERENCES [dbo].[VatTuBenhVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauVatTuBenhVien] CHECK CONSTRAINT [FK_YeuCauVatTuBenhVien_VatTuBenhVien]
GO
ALTER TABLE [dbo].[YeuCauVatTuBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauVatTuBenhVien_YeuCauDichVuKyThuat] FOREIGN KEY([YeuCauDichVuKyThuatId])
REFERENCES [dbo].[YeuCauDichVuKyThuat] ([Id])
GO
ALTER TABLE [dbo].[YeuCauVatTuBenhVien] CHECK CONSTRAINT [FK_YeuCauVatTuBenhVien_YeuCauDichVuKyThuat]
GO
ALTER TABLE [dbo].[YeuCauVatTuBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauVatTuBenhVien_YeuCauGoiDichVu] FOREIGN KEY([YeuCauGoiDichVuId])
REFERENCES [dbo].[YeuCauGoiDichVu] ([Id])
GO
ALTER TABLE [dbo].[YeuCauVatTuBenhVien] CHECK CONSTRAINT [FK_YeuCauVatTuBenhVien_YeuCauGoiDichVu]
GO
ALTER TABLE [dbo].[YeuCauVatTuBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauVatTuBenhVien_YeuCauKhamBenh] FOREIGN KEY([YeuCauKhamBenhId])
REFERENCES [dbo].[YeuCauKhamBenh] ([Id])
GO
ALTER TABLE [dbo].[YeuCauVatTuBenhVien] CHECK CONSTRAINT [FK_YeuCauVatTuBenhVien_YeuCauKhamBenh]
GO
ALTER TABLE [dbo].[YeuCauVatTuBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauVatTuBenhVien_YeuCauTiepNhan] FOREIGN KEY([YeuCauTiepNhanId])
REFERENCES [dbo].[YeuCauTiepNhan] ([Id])
GO
ALTER TABLE [dbo].[YeuCauVatTuBenhVien] CHECK CONSTRAINT [FK_YeuCauVatTuBenhVien_YeuCauTiepNhan]
GO
/****** Object:  StoredProcedure [dbo].[import_dich_vu_benh_vien]    Script Date: 3/23/2020 4:47:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Ang.Ngoc
-- Create date: 03/03/2020
-- Description:	import dịch vụ bệnh viện (kỹ thuật - giường - khám bệnh)
-- Cần cập nhật DB: thêm col phạm vi BHYT cho các table DỊCH VỤ
-- =============================================
CREATE PROCEDURE [dbo].[import_dich_vu_benh_vien] 
	@dataXML XML = NULL
AS
BEGIN
	SET NOCOUNT ON;

	declare @data table(
		Id int identity(1,1),
		TenChuong NVARCHAR(250),
		MaBH NVARCHAR(50),
		TenTT13 NVARCHAR(250),
		DVT NVARCHAR(50),
		GiaBH BIGINT,
		GiaBv BIGINT
	);

	INSERT INTO @data
	        (
				TenChuong,
				MaBH,
				TenTT13,
				GiaBH,
				GiaBv
	        )
	SELECT 
		d.value('(TenChuong)[1]', 'nvarchar(max)') AS TenChuong ,
		d.value('(MaBH)[1]', 'nvarchar(max)') AS MaBH,
		d.value('(TenTT13)[1]', 'nvarchar(max)') AS TenTT13,
		d.value('(GiaBH)[1]', 'nvarchar(max)') AS GiaBH,
		d.value('(GiaBv)[1]', 'nvarchar(max)') AS GiaBv
	FROM @dataXML.nodes('NewDataSet/icdRow') n(d)

	declare @total int = (select count(1) from @data), @idx int = 1;

	declare 
		@TenChuong NVARCHAR(250),
		@MaBH NVARCHAR(50),
		@TenTT13 NVARCHAR(250),
		@GiaBH BIGINT,
		@GiaBv BIGINT,
		@IdDichVuTemp BIGINT = NULL,
		@IdBenhVienTemp BIGINT = NULL,
		@KhoaIdTemp BIGINT = NULL,
		@KhoaPhongIdTemp BIGINT = NULL,
		@NgayBatDau DATETIME = '01/01/2020',
		@HieuLuc BIT = 1,

		@CreatedById int = 1,
		@LastUserId int = 1,
		@LastTime datetime = GETDATE(),
		@CreatedOn datetime = GETDATE()

BEGIN TRY
	BEGIN TRANSACTION
		WHILE (@idx <= @total)
		BEGIN
			-- lấy dữ liệu theo hàng
			SELECT  @TenChuong = TenChuong,
					@MaBH = LTRIM(RTRIM(MaBH)),
					@TenTT13 = LTRIM(RTRIM(TenTT13)),
					@GiaBH = GiaBH,
					@GiaBv = GiaBv
			FROM @data
			WHERE Id = @idx;

			if @MaBH IS NOT NULL AND @MaBH <> ''
			BEGIN
				-- kiểm tra insert dịch vụ kỹ thuật
				IF (SELECT COUNT(1) FROM DichVuKyThuat WHERE Ma = @MaBH) > 0
				BEGIN
					SELECT @IdDichVuTemp = Id FROM DichVuKyThuat WHERE Ma = @MaBH

					IF (SELECT COUNT(1) FROM DichVuKyThuatBenhVien where DichVuKyThuatId = @IdDichVuTemp) = 0
					BEGIN
						INSERT INTO DichVuKyThuatBenhVien(DichVuKyThuatId, NgayBatDau, HieuLuc, CreatedById, LastUserId, LastTime, CreatedOn)
						VALUES(@IdDichVuTemp, @NgayBatDau, @HieuLuc, @CreatedById, @LastUserId, @LastTime, @CreatedOn)

						SET @IdBenhVienTemp = IDENT_CURRENT('DichVuKyThuatBenhVien')

						IF @GiaBH IS NOT NULL AND @GiaBH <> ''
						BEGIN
							INSERT INTO DichVuKyThuatBenhVienGiaBaoHiem(DichVuKyThuatBenhVienId, TiLeBaoHiemThanhToan, Gia, TuNgay, CreatedById, LastUserId, LastTime, CreatedOn)
							VALUES(@IdBenhVienTemp, 100, @GiaBH, @NgayBatDau, @CreatedById, @LastUserId, @LastTime, @CreatedOn)
						END
					END
					ELSE
						SELECT @IdBenhVienTemp = Id FROM DichVuKyThuatBenhVien where DichVuKyThuatId = @IdDichVuTemp
					
					IF @GiaBv IS NOT NULL AND @GiaBv <> ''
					BEGIN
						INSERT INTO DichVuKyThuatBenhVienGiaBenhVien(DichVuKyThuatBenhVienId, NhomGiaDichVuKyThuatBenhVienId, Gia, TuNgay, CreatedById, LastUserId, LastTime, CreatedOn)
						VALUES(@IdBenhVienTemp, 1, @GiaBv, @NgayBatDau, @CreatedById, @LastUserId, @LastTime, @CreatedOn)
					END
				END

				-- kiểm tra insert dịch vụ khám bệnh
				ELSE IF (SELECT COUNT(1) FROM DichVuKhamBenh WHERE Ma = @MaBH) > 0
				BEGIN
					SELECT @IdDichVuTemp = Id, @KhoaIdTemp = KhoaId FROM DichVuKhamBenh WHERE Ma = @MaBH

					IF @MaBH in (N'14.1898',N'16.1898',N'15.1898',N'05.1898')
					BEGIN
						SET @KhoaPhongIdTemp = 1
					END
					ELSE
					BEGIN
						SELECT TOP 1 @KhoaPhongIdTemp = KhoaPhongId FROM KhoaPhongChuyenKhoa WHERE KhoaId = @KhoaIdTemp
					END
					
					INSERT INTO DichVuKhamBenhBenhVien(DichVuKhamBenhId, KhoaPhongId, HieuLuc, CreatedById, LastUserId, LastTime, CreatedOn)
					VALUES(@IdDichVuTemp, @KhoaPhongIdTemp, @HieuLuc, @CreatedById, @LastUserId, @LastTime, @CreatedOn)

					SET @IdBenhVienTemp = IDENT_CURRENT('DichVuKhamBenhBenhVien')
					IF @GiaBv IS NOT NULL AND @GiaBv <> ''
					BEGIN
						INSERT INTO DichVuKhamBenhBenhVienGiaBenhVien(DichVuKhamBenhBenhVienId, NhomGiaDichVuKhamBenhBenhVienId, Gia, TuNgay, CreatedById, LastUserId, LastTime, CreatedOn)
						VALUES(@IdBenhVienTemp, 1, @GiaBv, @NgayBatDau, @CreatedById, @LastUserId, @LastTime, @CreatedOn)
					END


					IF @GiaBH IS NOT NULL AND @GiaBH <> ''
					BEGIN
						INSERT INTO DichVuKhamBenhBenhVienGiaBaoHiem(DichVuKhamBenhBenhVienId, TiLeBaoHiemThanhToan, Gia, TuNgay, CreatedById, LastUserId, LastTime, CreatedOn)
						VALUES(@IdBenhVienTemp, 100, @GiaBH, @NgayBatDau, @CreatedById, @LastUserId, @LastTime, @CreatedOn)
					END
				END

				-- kiểm tra insert dịch vụ giường
				--ELSE IF (SELECT COUNT(1) FROM DichVuGiuong WHERE Ma = @MaBH) > 0
				--BEGIN
				--	SELECT @IdDichVuTemp = Id, @KhoaIdTemp = KhoaId FROM DichVuGiuong WHERE Ma = @MaBH
				--	SELECT TOP 1 @KhoaPhongIdTemp = KhoaPhongId FROM KhoaPhongChuyenKhoa WHERE KhoaId = @KhoaIdTemp

				--	INSERT INTO DichVuGiuongBenhVien(DichVuGiuongId, KhoaPhongId, LoaiGiuong, HieuLuc, CreatedById, LastUserId, LastTime, CreatedOn)
				--	VALUES(@IdDichVuTemp, @KhoaPhongIdTemp, 1, @HieuLuc, @CreatedById, @LastUserId, @LastTime, @CreatedOn)

				--	SET @IdBenhVienTemp = IDENT_CURRENT('DichVuGiuongBenhVien')
				--	IF @GiaBv IS NOT NULL AND @GiaBv <> ''
				--	BEGIN
				--		INSERT INTO DichVuGiuongBenhVienGiaBenhVien(DichVuGiuongBenhVienId, NhomGiaDichVuGiuongBenhVienId, Gia, TuNgay, CreatedById, LastUserId, LastTime, CreatedOn)
				--		VALUES(@IdBenhVienTemp, 1, @GiaBv, @NgayBatDau, @CreatedById, @LastUserId, @LastTime, @CreatedOn)
				--	END

				--	-- cần kiểm tra lại nhóm giá dịch vụ và tỷ lệ bảo hiểm thanh toán

				--	IF @GiaBH IS NOT NULL AND @GiaBH <> ''
				--	BEGIN
				--		INSERT INTO DichVuGiuongBenhVienGiaBaoHiem(DichVuGiuongBenhVienId, TiLeBaoHiemThanhToan, Gia, TuNgay, CreatedById, LastUserId, LastTime, CreatedOn)
				--		VALUES(@IdBenhVienTemp, 100, @GiaBH, @NgayBatDau, @CreatedById, @LastUserId, @LastTime, @CreatedOn)
				--	END
				--END


				-- kiểm tra insert dịch vụ kỹ thuật của bệnh viện
				--ELSE
				--BEGIN
				--	INSERT INTO DichVuKhamBenh(Ma, MaTT37, Ten, KhoaId, HangBenhVien, NgoaiQuyDinhBHYT, CreatedById, LastUserId, LastTime, CreatedOn)
				--	VALUES(@MaBH, N'', @TenTT13, 51, 3, 1, @CreatedById, @LastUserId, @LastTime, @CreatedOn)

				--	SELECT @IdDichVuTemp = Id, @KhoaIdTemp = KhoaId FROM DichVuKhamBenh WHERE Ma = @MaBH
				--	SELECT TOP 1 @KhoaPhongIdTemp = KhoaPhongId FROM KhoaPhongChuyenKhoa WHERE KhoaId = @KhoaIdTemp

				--	INSERT INTO DichVuKhamBenhBenhVien(DichVuKhamBenhId, KhoaPhongId, HieuLuc, CreatedById, LastUserId, LastTime, CreatedOn)
				--	VALUES(@IdDichVuTemp, @KhoaPhongIdTemp, @HieuLuc, @CreatedById, @LastUserId, @LastTime, @CreatedOn)

				--	SET @IdBenhVienTemp = IDENT_CURRENT('DichVuKhamBenhBenhVien')
				--	IF @GiaBv IS NOT NULL AND @GiaBv <> ''
				--	BEGIN
				--		INSERT INTO DichVuKhamBenhBenhVienGiaBenhVien(DichVuKhamBenhBenhVienId, NhomGiaDichVuKhamBenhBenhVienId, Gia, TuNgay, CreatedById, LastUserId, LastTime, CreatedOn)
				--		VALUES(@IdBenhVienTemp, 1, @GiaBv, @NgayBatDau, @CreatedById, @LastUserId, @LastTime, @CreatedOn)
				--	END
				--END
			END

			SELECT	@idx = @idx + 1,
					@IdDichVuTemp = NULL,
					@IdBenhVienTemp = NULL,
					@KhoaIdTemp = NULL,
					@KhoaPhongIdTemp = NULL
		END
		COMMIT TRANSACTION;

	END TRY 
	BEGIN CATCH
		ROLLBACK TRANSACTION;
	END CATCH

	SELECT @idx
END
GO
/****** Object:  StoredProcedure [dbo].[import_dich_vu_giuong_benh_vien]    Script Date: 3/23/2020 4:47:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Ang.Ngoc
-- Create date: 03/03/2020
-- Description:	import dịch vụ bệnh viện (kỹ thuật - giường - khám bệnh)
-- Cần cập nhật DB: thêm col phạm vi BHYT cho các table DỊCH VỤ
-- =============================================
CREATE PROCEDURE [dbo].[import_dich_vu_giuong_benh_vien] 
	@dataXML XML = NULL
AS
BEGIN
	SET NOCOUNT ON;

	declare @data table(
		Id int identity(1,1),
		Ma NVARCHAR(50),
		Ten NVARCHAR(250),
		MaTT37 NVARCHAR(250),
		Loai NVARCHAR(50),
		GiaBH BIGINT,
		GiaBv BIGINT,
		MoTa NVARCHAR(250)
	);

	INSERT INTO @data
	        (
				Ma,
				Ten,
				MaTT37,
				Loai,
				GiaBH,
				GiaBv,
				MoTa
	        )
	SELECT 
		d.value('(Ma)[1]', 'nvarchar(max)') AS Ma ,
		d.value('(Ten)[1]', 'nvarchar(max)') AS Ten,
		d.value('(MaTT37)[1]', 'nvarchar(max)') AS MaTT37,
		d.value('(Loai)[1]', 'nvarchar(max)') AS Loai,
		d.value('(GiaBH)[1]', 'nvarchar(max)') AS GiaBH,
		d.value('(GiaBV)[1]', 'nvarchar(max)') AS GiaBv,
		d.value('(MoTa)[1]', 'nvarchar(max)') AS MoTa
	FROM @dataXML.nodes('NewDataSet/icdRow') n(d)

	declare @total int = (select count(1) from @data), @idx int = 1;

	declare 
		@Ma NVARCHAR(250) = NULL,
		@Ten NVARCHAR(50) = NULL,
		@MaTT37 NVARCHAR(250) = NULL,
		@GiaBH BIGINT = NULL,
		@GiaBv BIGINT = NULL,
		@MoTa NVARCHAR(250) = NULL,
		@Loai INT = NULL,
		@IdDichVuTemp BIGINT = NULL,
		@IdBenhVienTemp BIGINT = NULL,
		@KhoaIdTemp BIGINT = NULL,
		@KhoaPhongIdTemp BIGINT = NULL,
		@NgayBatDau DATETIME = '01/01/2020',
		@HieuLuc BIT = 1,

		@CreatedById int = 1,
		@LastUserId int = 1,
		@LastTime datetime = GETDATE(),
		@CreatedOn datetime = GETDATE()

BEGIN TRY
	BEGIN TRANSACTION
		WHILE (@idx <= @total)
		BEGIN
			-- lấy dữ liệu theo hàng
			SELECT  @Ma =Ma,
					@Ten =Ten,
					@MaTT37 = MaTT37,
					@Loai = CASE Loai WHEN N'Nội trú' THEN 1
									  ELSE 3 END,
					@GiaBH = GiaBH,
					@GiaBv = GiaBv,
					@MoTa = MoTa
			FROM @data
			WHERE Id = @idx;

			if @Ma IS NOT NULL AND @Ma <> ''
			BEGIN

				-- kiểm tra insert dịch vụ giường
				IF (SELECT COUNT(1) FROM DichVuGiuong WHERE Ma = @Ma) > 0
				BEGIN
					SELECT @IdDichVuTemp = Id, @KhoaIdTemp = KhoaId FROM DichVuGiuong WHERE Ma = @Ma
					SELECT TOP 1 @KhoaPhongIdTemp = KhoaPhongId FROM KhoaPhongChuyenKhoa WHERE KhoaId = @KhoaIdTemp

					INSERT INTO DichVuGiuongBenhVien(DichVuGiuongId, KhoaPhongId, LoaiGiuong, HieuLuc, CreatedById, LastUserId, LastTime, CreatedOn)
					VALUES(@IdDichVuTemp, @KhoaPhongIdTemp, @Loai, @HieuLuc, @CreatedById, @LastUserId, @LastTime, @CreatedOn)

					SET @IdBenhVienTemp = IDENT_CURRENT('DichVuGiuongBenhVien')
					IF @GiaBv IS NOT NULL AND @GiaBv <> ''
					BEGIN
						INSERT INTO DichVuGiuongBenhVienGiaBenhVien(DichVuGiuongBenhVienId, NhomGiaDichVuGiuongBenhVienId, Gia, TuNgay, CreatedById, LastUserId, LastTime, CreatedOn)
						VALUES(@IdBenhVienTemp, 1, @GiaBv, @NgayBatDau, @CreatedById, @LastUserId, @LastTime, @CreatedOn)
					END

					-- cần kiểm tra lại nhóm giá dịch vụ và tỷ lệ bảo hiểm thanh toán

					IF @GiaBH IS NOT NULL AND @GiaBH <> ''
					BEGIN
						INSERT INTO DichVuGiuongBenhVienGiaBaoHiem(DichVuGiuongBenhVienId, TiLeBaoHiemThanhToan, Gia, TuNgay, CreatedById, LastUserId, LastTime, CreatedOn)
						VALUES(@IdBenhVienTemp, 100, @GiaBH, @NgayBatDau, @CreatedById, @LastUserId, @LastTime, @CreatedOn)
					END
				END
			END

			SELECT	@idx = @idx + 1,
					@Loai = NULL
		END
		COMMIT TRANSACTION;

	END TRY 
	BEGIN CATCH
		ROLLBACK TRANSACTION;
	END CATCH

	SELECT @idx
END
GO
/****** Object:  StoredProcedure [dbo].[import_NhanVien]    Script Date: 3/23/2020 4:47:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Anh.Ngoc
-- Create date: 03/03/2020
-- Description:	import thông tin nhân viên - liên quan
-- =============================================
CREATE PROCEDURE [dbo].[import_NhanVien] 
	@dataXML XML = NULL
AS
BEGIN
	SET NOCOUNT ON;

	declare @data table(
		Id int identity(1,1),
		HoTen NVARCHAR(100),
		SDT NVARCHAR(20),
		Email NVARCHAR(200),
		CMT NVARCHAR(12),
		DiaChi NVARCHAR(200),
		NgaySinh NVARCHAR(50),
		GioiTinh NVARCHAR(50),
		ChucDanhId BIGINT,
		MaCCHN NVARCHAR(50),
		VBCM NVARCHAR(250),
		NgayCapCCHN NVARCHAR(50),
		NoiCapCCHN NVARCHAR(200),
		Khoa NVARCHAR(50),
		NgayKyHopDong NVARCHAR(50),
		NgayHetHopDong NVARCHAR(50)
	);

	INSERT INTO @data
	        (
				HoTen,
				SDT,
				Email,
				CMT,
				DiaChi ,
				NgaySinh,
				GioiTinh,
				ChucDanhId,
				MaCCHN,
				VBCM,
				NgayCapCCHN,
				NoiCapCCHN,
				Khoa,
				NgayKyHopDong ,
				NgayHetHopDong
	        )
	SELECT 
		d.value('(HoTen)[1]', 'nvarchar(max)') AS HoTen ,
		d.value('(SDT)[1]', 'nvarchar(max)') AS SDT,
		d.value('(Email)[1]', 'nvarchar(max)') AS Email,
		d.value('(CMT)[1]', 'nvarchar(max)') AS CMT,
		d.value('(DiaChi)[1]', 'nvarchar(max)') AS DiaChi,
		d.value('(NgaySinh)[1]', 'nvarchar(max)') AS NgaySinh ,
		d.value('(GioiTinh)[1]', 'nvarchar(max)') AS GioiTinh,
		d.value('(ChucDanhId)[1]', 'bigint') AS ChucDanhId,
		d.value('(MaCCHN)[1]', 'nvarchar(max)') AS MaCCHN,
		d.value('(VanBangChuyenMon)[1]', 'nvarchar(max)') AS VBCM,
		d.value('(NgayCapCCHN)[1]', 'nvarchar(max)') AS NgayCapCCHN,
		d.value('(NoiCapCCHN)[1]', 'nvarchar(max)') AS NoiCapCCHN,
		d.value('(Khoa)[1]', 'nvarchar(max)') AS Khoa,
		d.value('(NgayKyHopDong)[1]', 'nvarchar(max)') AS NgayKyHopDong,
		d.value('(NgayHetHopDong)[1]', 'nvarchar(max)') AS NgayHetHopDong
	FROM @dataXML.nodes('NewDataSet/icdRow') n(d)

	declare @total int = (select count(1) from @data), @idx int = 1;

	declare 
		@HoTen NVARCHAR(100) = NULL,
		@SDT NVARCHAR(20)  = NULL,
		@Email NVARCHAR(200)  = NULL,
		@CMT NVARCHAR(12) = NULL,
		@DiaChi NVARCHAR(200) = NULL,
		@NgaySinh DATETIME = NULL,
		@GioiTinh INT = NULL,
		@ChucDanhId BIGINT = NULL,
		@MaCCHN NVARCHAR(50) = NULL,
		@VBCM NVARCHAR(250) = NULL,
		@NgayCapCCHN DATETIME = NULL,
		@NoiCapCCHN NVARCHAR(200) = NULL,
		@Khoa NVARCHAR(50) = NULL,
		@NgayKyHopDong DATETIME = NULL,
		@NgayHetHopDong DATETIME = NULL,
		@UserId BIGINT = NULL,
		@KhoaId BIGINT = NULL,
		@Khoa2Id BIGINT = NULL,
		@VBCMId BIGINT = NULL,

		@CreatedById int = 1,
		@LastUserId int = 1,
		@LastTime datetime = GETDATE(),
		@CreatedOn datetime = GETDATE()

BEGIN TRY
	BEGIN TRANSACTION
		WHILE (@idx <= @total)
		BEGIN
			-- lấy dữ liệu theo hàng
			SELECT  @HoTen = HoTen,
				@SDT = REPLACE(REPLACE(LTRIM(RTRIM(SDT)), '.', ''), ' ', ''),
				@Email = REPLACE(LTRIM(RTRIM(Email)), ';', ''),
				@CMT = REPLACE(CMT, ';', ''),
				@DiaChi = DiaChi ,
				@NgaySinh = CONVERT(DATETIME, NgaySinh),
				@GioiTinh = CASE GioiTinh WHEN N'Nam' THEN 1
										  WHEN N'Nữ' THEN 2
										  ELSE 1 END,
				@ChucDanhId = ChucDanhId,
				@MaCCHN = LTRIM(RTRIM(REPLACE(MaCCHN, ';', ''))),
				@VBCM = LTRIM(RTRIM(REPLACE(VBCM, ';', ''))),
				@NgayCapCCHN = CONVERT(DATETIME, REPLACE(NgayCapCCHN, ';', '')),
				@NoiCapCCHN = REPLACE(NoiCapCCHN, ';', ''),
				@Khoa  = LTRIM(RTRIM(Khoa)),
				@NgayKyHopDong = CONVERT(DATETIME, NgayKyHopDong),
				@NgayHetHopDong = CASE NgayHetHopDong WHEN '' THEN NULL
													  WHEN NULL THEN NULL	
													  ELSE CONVERT(DATETIME, NgayHetHopDong) END
			FROM @data
			WHERE Id = @idx;

			if @SDT IS NOT NULL AND @SDT <> ''
			BEGIN

				-- thêm user
				INSERT INTO [User](HoTen, SoDienThoai, Email, NgaySinh, SoChungMinhThu, DiaChi, GioiTinh, IsActive, Region, CreatedById, LastUserId, LastTime, CreatedOn)
				VALUES(@HoTen, @SDT, @Email, @NgaySinh, @CMT, @DiaChi, @GioiTinh, 1, 1, @CreatedById, @LastUserId, @LastTime, @CreatedOn)

				SET @UserId = IDENT_CURRENT('User')

				-- kierm tra văn bằng chuyên môn
				IF @VBCM IS NOT NULL AND @VBCM <> ''
				BEGIN
					IF (SELECT COUNT(1) FROM VanBangChuyenMon WHERE Ten = @VBCM) = 0
					BEGIN
						INSERT INTO VanBangChuyenMon(Ma, Ten, TenVietTat, IsDisabled, CreatedById, LastUserId, LastTime, CreatedOn)
						VALUES(SUBSTRING(REPLACE(@VBCM, ' ', ''), 1, 20), @VBCM, SUBSTRING(REPLACE(@VBCM, ' ', ''), 1, 50), 0, @CreatedById, @LastUserId, @LastTime, @CreatedOn)
					
						SET @VBCMId = IDENT_CURRENT('VanBangChuyenMon')
					END
					ELSE
					BEGIN
						SELECT @VBCMId = Id FROM VanBangChuyenMon WHERE Ten = @VBCM
					END
				END

				-- thểm nhân viên
				INSERT INTO NhanVien(Id, ChucDanhId, CreatedById, LastUserId, LastTime, CreatedOn, MaChungChiHanhNghe, NgayCapChungChiHanhNghe, NoiCapChungChiHanhNghe, VanBangChuyenMonId)
				VALUES(@UserId, @ChucDanhId, @CreatedById, @LastUserId, @LastTime, @CreatedOn, @MaCCHN, @NgayCapCCHN, @NoiCapCCHN, @VBCMId)


				-- thêm role nhân viên
				INSERT INTO NhanVienRole(NhanVienId, RoleId, CreatedById, LastUserId, LastTime, CreatedOn)
				VALUES(@UserId, 1, @CreatedById, @LastUserId, @LastTime, @CreatedOn)

				-- thêm nhân viên vào khoa phòng
				IF @Khoa IS NOT NULL AND @Khoa <> ''
				BEGIN
					IF CHARINDEX(';', @Khoa) > 0
					BEGIN
						SELECT @KhoaId = Id FROM KhoaPhong WHERE Ma = SUBSTRING(@Khoa,1, CHARINDEX(';',@Khoa) - 1)
						INSERT INTO KhoaPhongNhanVien(KhoaPhongId, NhanVienId, CreatedById, LastUserId, LastTime, CreatedOn)
						VALUES(@KhoaId, @UserId, @CreatedById, @LastUserId, @LastTime, @CreatedOn)

						SELECT @Khoa2Id = Id FROM KhoaPhong WHERE Ma = SUBSTRING(@Khoa, CHARINDEX(';',@Khoa) + 1, LEN(@Khoa))
						INSERT INTO KhoaPhongNhanVien(KhoaPhongId, NhanVienId, CreatedById, LastUserId, LastTime, CreatedOn)
						VALUES(@Khoa2Id, @UserId, @CreatedById, @LastUserId, @LastTime, @CreatedOn)
					END
					ELSE
					BEGIN
						SELECT @KhoaId = Id FROM KhoaPhong WHERE Ma = @Khoa

						INSERT INTO KhoaPhongNhanVien(KhoaPhongId, NhanVienId, CreatedById, LastUserId, LastTime, CreatedOn)
						VALUES(@KhoaId, @UserId, @CreatedById, @LastUserId, @LastTime, @CreatedOn)
					END
				END
			END

			SELECT	@idx = @idx + 1,
					@KhoaId = NULL,
					@Khoa2Id = NULL,
					@VBCMId = NULL
		END
		COMMIT TRANSACTION;

	END TRY 
	BEGIN CATCH
		ROLLBACK TRANSACTION;
	END CATCH

	SELECT @idx
END
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Hạng Đặc Biệt=1,
Hạng 1=2,
Hạng 2=3,
Hạng 3=4,
Hạng 4=5,
Chưa Xếp Hạng=6' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'BenhVien', @level2type=N'COLUMN',@level2name=N'HangBenhVien'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Tuyến 1=1,
Tuyến 2=2,
Tuyến 3=3,
Tuyến 4=4,
Chưa Phân Tuyến=5' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'BenhVien', @level2type=N'COLUMN',@level2name=N'TuyenChuyenMonKyThuat'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Tân dược=1,
Chế phẩm=2,
Vị thuốc=3,
Phóng xạ=4,
Tân dược tự bào chế=5,
Chế phẩm YHCT tự bào chế=6' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DuocPham', @level2type=N'COLUMN',@level2name=N'LoaiThuocHoacHoatChat'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Thầu tập trung=1,
Thầu riêng=2,
Tự bào chế=3' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HopDongThauDuocPham', @level2type=N'COLUMN',@level2name=N'LoaiThau'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Tổng hợp=1,
Tân dược=2,
Chế phẩm=3,
Vị thuốc=4,
Phóng xạ=5' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HopDongThauDuocPham', @level2type=N'COLUMN',@level2name=N'LoaiThuocThau'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Kho tổng=1,
Kho nhánh=2,
Kho tại khoa=3' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'KhoDuocPham', @level2type=N'COLUMN',@level2name=N'LoaiKho'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'đơn vị mặc định là (ml)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MauVaChePham', @level2type=N'COLUMN',@level2name=N'TheTich'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Nhập từ nhà cung cấp=1,
Nhập từ kho khác=2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'NhapKhoDuocPham', @level2type=N'COLUMN',@level2name=N'LoaiNhapKho'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Internal=1,External=2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'User', @level2type=N'COLUMN',@level2name=N'Region'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Xuất qua kho khác=1,
Xuất trả nhà cung cấp=2,
Xuất cho bệnh nhân=3,
Xuất hủy=4' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'XuatKhoDuocPham', @level2type=N'COLUMN',@level2name=N'LoaiXuatKho'
GO
USE [master]
GO
ALTER DATABASE [BvBacHa] SET  READ_WRITE 
GO
