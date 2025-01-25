CREATE TABLE [dbo].[LyDoTiepNhan](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[LyDoTiepNhanChaId] [bigint] NULL,
	[CapNhom] [int] NOT NULL,
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

ALTER TABLE [dbo].[LyDoTiepNhan]  WITH CHECK ADD  CONSTRAINT [FK_LyDoTiepNhan_LyDoTiepNhan] FOREIGN KEY([LyDoTiepNhanChaId])
REFERENCES [dbo].[LyDoTiepNhan] ([Id])
GO

ALTER TABLE [dbo].[LyDoTiepNhan] CHECK CONSTRAINT [FK_LyDoTiepNhan_LyDoTiepNhan]
GO

CREATE TABLE [dbo].[NguoiGioiThieu](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[HoTen] [nvarchar](100) NOT NULL,
	[SoDienThoai] [nvarchar](12) NULL,
	[NhanVienQuanLyId] [bigint] NOT NULL,
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

ALTER TABLE [dbo].[NguoiGioiThieu]  WITH CHECK ADD  CONSTRAINT [FK_NguoiGioiThieu_NhanVien] FOREIGN KEY([NhanVienQuanLyId])
REFERENCES [dbo].[NhanVien] ([Id])
GO

ALTER TABLE [dbo].[NguoiGioiThieu] CHECK CONSTRAINT [FK_NguoiGioiThieu_NhanVien]
GO
CREATE TABLE [dbo].[YeuCauTiepNhanLichSuKiemTraTheBHYT](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[MaUserKiemTra] [nvarchar](20) NULL,
	[ThoiGianKiemTra] [datetime] NULL,
	[ThongBao] [nvarchar](2000) NULL,
	[MaLoi] [nvarchar](20) NULL,
	[YeuCauTiepNhanId] [bigint] NOT NULL,
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

ALTER TABLE [dbo].[YeuCauTiepNhanLichSuKiemTraTheBHYT]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauTiepNhanLichSuKiemTraTheBHYT_YeuCauTiepNhan] FOREIGN KEY([YeuCauTiepNhanId])
REFERENCES [dbo].[YeuCauTiepNhan] ([Id])
GO

ALTER TABLE [dbo].[YeuCauTiepNhanLichSuKiemTraTheBHYT] CHECK CONSTRAINT [FK_YeuCauTiepNhanLichSuKiemTraTheBHYT_YeuCauTiepNhan]
GO
CREATE TABLE [dbo].[YeuCauTiepNhanLichSuKhamBHYT](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[MaTheBHYT] [nvarchar](20) NULL,
	[TenBenh] [nvarchar](200) NULL,
	[NgayVao] [datetime] NULL,
	[NgayRa] [datetime] NULL,
	[MaCSKCB] [nvarchar](20) NULL,
	[TinhTrangRaVien] [int] NULL,
	[LyDoVaoVien] [int] NULL,
	[KetQuaDieuTri] [int] NULL,
	[YeuCauTiepNhanId] [bigint] NOT NULL,
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

ALTER TABLE [dbo].[YeuCauTiepNhanLichSuKhamBHYT]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauTiepNhanLichSuKhamBHYT_YeuCauTiepNhan] FOREIGN KEY([YeuCauTiepNhanId])
REFERENCES [dbo].[YeuCauTiepNhan] ([Id])
GO

ALTER TABLE [dbo].[YeuCauTiepNhanLichSuKhamBHYT] CHECK CONSTRAINT [FK_YeuCauTiepNhanLichSuKhamBHYT_YeuCauTiepNhan]
GO

ALTER TABLE [YeuCauTiepNhan]
ADD 
	[TuNhap] bit NULL,
	[NhanVienDuyetMienGiamThemId] [bigint] NULL,
	[NguoiGioiThieuId] [bigint] NULL;
GO

Update CauHinh
Set [Value] = '0.1.7' where [Name] = 'CauHinhHeThong.DatabaseVesion'