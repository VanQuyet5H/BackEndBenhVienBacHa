CREATE TABLE [dbo].[GoiKhamSucKhoeChung](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ma] [nvarchar](50) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[GoiChung] [bit] NULL,
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
CREATE TABLE [dbo].[GoiKhamSucKhoeChungDichVuDichVuKyThuat](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[GoiKhamSucKhoeChungId] [bigint] NOT NULL,
	[DichVuKyThuatBenhVienId] [bigint] NOT NULL,
	[NhomGiaDichVuKyThuatBenhVienId] [bigint] NOT NULL,
	[DonGiaBenhVien] [decimal](15, 2) NOT NULL,
	[DonGiaUuDai] [decimal](15, 2) NOT NULL,
	[DonGiaChuaUuDai] [decimal](15, 2) NOT NULL,
	[GioiTinhNam] [bit] NOT NULL,
	[GioiTinhNu] [bit] NOT NULL,
	[CoMangThai] [bit] NOT NULL,
	[KhongMangThai] [bit] NOT NULL,
	[ChuaLapGiaDinh] [bit] NOT NULL,
	[DaLapGiaDinh] [bit] NOT NULL,
	[SoTuoiTu] [int] NULL,
	[SoTuoiDen] [int] NULL,
	[SoLan] [int] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__GoiKhamSucKhoeChungDichVuDichVuKyThuat] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE TABLE [dbo].[GoiKhamSucKhoeChungDichVuKhamBenh](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[GoiKhamSucKhoeChungId] [bigint] NOT NULL,
	[DichVuKhamBenhBenhVienId] [bigint] NOT NULL,
	[ChuyenKhoaKhamSucKhoe] [int] NOT NULL,
	[NhomGiaDichVuKhamBenhBenhVienId] [bigint] NOT NULL,
	[DonGiaBenhVien] [decimal](15, 2) NOT NULL,
	[DonGiaUuDai] [decimal](15, 2) NOT NULL,
	[DonGiaChuaUuDai] [decimal](15, 2) NOT NULL,
	[GioiTinhNam] [bit] NOT NULL,
	[GioiTinhNu] [bit] NOT NULL,
	[CoMangThai] [bit] NOT NULL,
	[KhongMangThai] [bit] NOT NULL,
	[ChuaLapGiaDinh] [bit] NOT NULL,
	[DaLapGiaDinh] [bit] NOT NULL,
	[SoTuoiTu] [int] NULL,
	[SoTuoiDen] [int] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__GoiKhamSucKhoeChungDichVuKhamBenh] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE TABLE [dbo].[GoiKhamSucKhoeChungNoiThucHien](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[GoiKhamSucKhoeChungDichVuKhamBenhId] [bigint] NULL,
	[GoiKhamSucKhoeChungDichVuDichVuKyThuatId] [bigint] NULL,
	[PhongBenhVienId] [bigint] NOT NULL,
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
ALTER TABLE [dbo].[GoiKhamSucKhoeChungDichVuDichVuKyThuat]  WITH CHECK ADD  CONSTRAINT [FK_GoiKhamSucKhoeChungDichVuDichVuKyThuat_DichVuKyThuatBenhVien] FOREIGN KEY([DichVuKyThuatBenhVienId])
REFERENCES [dbo].[DichVuKyThuatBenhVien] ([Id])
GO
ALTER TABLE [dbo].[GoiKhamSucKhoeChungDichVuDichVuKyThuat] CHECK CONSTRAINT [FK_GoiKhamSucKhoeChungDichVuDichVuKyThuat_DichVuKyThuatBenhVien]
GO
ALTER TABLE [dbo].[GoiKhamSucKhoeChungDichVuDichVuKyThuat]  WITH CHECK ADD  CONSTRAINT [FK_GoiKhamSucKhoeChungDichVuDichVuKyThuat_GoiKhamSucKhoeChung] FOREIGN KEY([GoiKhamSucKhoeChungId])
REFERENCES [dbo].[GoiKhamSucKhoeChung] ([Id])
GO
ALTER TABLE [dbo].[GoiKhamSucKhoeChungDichVuDichVuKyThuat] CHECK CONSTRAINT [FK_GoiKhamSucKhoeChungDichVuDichVuKyThuat_GoiKhamSucKhoeChung]
GO
ALTER TABLE [dbo].[GoiKhamSucKhoeChungDichVuDichVuKyThuat]  WITH CHECK ADD  CONSTRAINT [FK_GoiKhamSucKhoeChungDichVuDichVuKyThuat_NhomGiaDichVuKyThuatBenhVien] FOREIGN KEY([NhomGiaDichVuKyThuatBenhVienId])
REFERENCES [dbo].[NhomGiaDichVuKyThuatBenhVien] ([Id])
GO
ALTER TABLE [dbo].[GoiKhamSucKhoeChungDichVuDichVuKyThuat] CHECK CONSTRAINT [FK_GoiKhamSucKhoeChungDichVuDichVuKyThuat_NhomGiaDichVuKyThuatBenhVien]
GO
ALTER TABLE [dbo].[GoiKhamSucKhoeChungDichVuKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_GoiKhamSucKhoeChungDichVuKhamBenh_DichVuKhamBenhBenhVien] FOREIGN KEY([DichVuKhamBenhBenhVienId])
REFERENCES [dbo].[DichVuKhamBenhBenhVien] ([Id])
GO
ALTER TABLE [dbo].[GoiKhamSucKhoeChungDichVuKhamBenh] CHECK CONSTRAINT [FK_GoiKhamSucKhoeChungDichVuKhamBenh_DichVuKhamBenhBenhVien]
GO
ALTER TABLE [dbo].[GoiKhamSucKhoeChungDichVuKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_GoiKhamSucKhoeChungDichVuKhamBenh_GoiKhamSucKhoeChung] FOREIGN KEY([GoiKhamSucKhoeChungId])
REFERENCES [dbo].[GoiKhamSucKhoeChung] ([Id])
GO
ALTER TABLE [dbo].[GoiKhamSucKhoeChungDichVuKhamBenh] CHECK CONSTRAINT [FK_GoiKhamSucKhoeChungDichVuKhamBenh_GoiKhamSucKhoeChung]
GO
ALTER TABLE [dbo].[GoiKhamSucKhoeChungDichVuKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_GoiKhamSucKhoeChungDichVuKhamBenh_NhomGiaDichVuKhamBenhBenhVien] FOREIGN KEY([NhomGiaDichVuKhamBenhBenhVienId])
REFERENCES [dbo].[NhomGiaDichVuKhamBenhBenhVien] ([Id])
GO
ALTER TABLE [dbo].[GoiKhamSucKhoeChungDichVuKhamBenh] CHECK CONSTRAINT [FK_GoiKhamSucKhoeChungDichVuKhamBenh_NhomGiaDichVuKhamBenhBenhVien]
GO
ALTER TABLE [dbo].[GoiKhamSucKhoeChungNoiThucHien]  WITH CHECK ADD  CONSTRAINT [FK_GoiKhamSucKhoeChungNoiThucHien_GoiKhamSucKhoeChungDichVuDichVuKyThuat] FOREIGN KEY([GoiKhamSucKhoeChungDichVuDichVuKyThuatId])
REFERENCES [dbo].[GoiKhamSucKhoeChungDichVuDichVuKyThuat] ([Id])
GO
ALTER TABLE [dbo].[GoiKhamSucKhoeChungNoiThucHien] CHECK CONSTRAINT [FK_GoiKhamSucKhoeChungNoiThucHien_GoiKhamSucKhoeChungDichVuDichVuKyThuat]
GO
ALTER TABLE [dbo].[GoiKhamSucKhoeChungNoiThucHien]  WITH CHECK ADD  CONSTRAINT [FK_GoiKhamSucKhoeChungNoiThucHien_GoiKhamSucKhoeChungDichVuKhamBenh] FOREIGN KEY([GoiKhamSucKhoeChungDichVuKhamBenhId])
REFERENCES [dbo].[GoiKhamSucKhoeChungDichVuKhamBenh] ([Id])
GO
ALTER TABLE [dbo].[GoiKhamSucKhoeChungNoiThucHien] CHECK CONSTRAINT [FK_GoiKhamSucKhoeChungNoiThucHien_GoiKhamSucKhoeChungDichVuKhamBenh]
GO
ALTER TABLE [dbo].[GoiKhamSucKhoeChungNoiThucHien]  WITH CHECK ADD  CONSTRAINT [FK_GoiKhamSucKhoeChungNoiThucHien_PhongBenhVien] FOREIGN KEY([PhongBenhVienId])
REFERENCES [dbo].[PhongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[GoiKhamSucKhoeChungNoiThucHien] CHECK CONSTRAINT [FK_GoiKhamSucKhoeChungNoiThucHien_PhongBenhVien]
GO

Update dbo.CauHinh
Set [Value] = '3.2.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'
