/****** Object:  Table [dbo].[GoiKhamSucKhoeChungDichVuKhamBenhNhanVien]    Script Date: 11/4/2021 8:17:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GoiKhamSucKhoeChungDichVuKhamBenhNhanVien](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[GoiKhamSucKhoeId] [bigint] NOT NULL,
	[GoiKhamSucKhoeDichVuKhamBenhId] [bigint] NOT NULL,
	[DichVuKhamBenhBenhVienId] [bigint] NOT NULL,
	[HopDongKhamSucKhoeNhanVienId] [bigint] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_GoiKhamSucKhoeChungDichVuKhamBenhNhanVien] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GoiKhamSucKhoeChungDichVuKyThuatNhanVien]    Script Date: 11/4/2021 8:17:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GoiKhamSucKhoeChungDichVuKyThuatNhanVien](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[GoiKhamSucKhoeId] [bigint] NOT NULL,
	[GoiKhamSucKhoeDichVuDichVuKyThuatId] [bigint] NOT NULL,
	[DichVuKyThuatBenhVienId] [bigint] NOT NULL,
	[HopDongKhamSucKhoeNhanVienId] [bigint] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_GoiKhamSucKhoeChungDichVuKyThuatNhanVien] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[GoiKhamSucKhoeChungDichVuKhamBenhNhanVien]  WITH CHECK ADD  CONSTRAINT [FK_GoiKhamSucKhoeChungDichVuKhamBenhNhanVien_DichVuKhamBenhBenhVien] FOREIGN KEY([DichVuKhamBenhBenhVienId])
REFERENCES [dbo].[DichVuKhamBenhBenhVien] ([Id])
GO
ALTER TABLE [dbo].[GoiKhamSucKhoeChungDichVuKhamBenhNhanVien] CHECK CONSTRAINT [FK_GoiKhamSucKhoeChungDichVuKhamBenhNhanVien_DichVuKhamBenhBenhVien]
GO
ALTER TABLE [dbo].[GoiKhamSucKhoeChungDichVuKhamBenhNhanVien]  WITH CHECK ADD  CONSTRAINT [FK_GoiKhamSucKhoeChungDichVuKhamBenhNhanVien_GoiKhamSucKhoe] FOREIGN KEY([GoiKhamSucKhoeId])
REFERENCES [dbo].[GoiKhamSucKhoe] ([Id])
GO
ALTER TABLE [dbo].[GoiKhamSucKhoeChungDichVuKhamBenhNhanVien] CHECK CONSTRAINT [FK_GoiKhamSucKhoeChungDichVuKhamBenhNhanVien_GoiKhamSucKhoe]
GO
ALTER TABLE [dbo].[GoiKhamSucKhoeChungDichVuKhamBenhNhanVien]  WITH CHECK ADD  CONSTRAINT [FK_GoiKhamSucKhoeChungDichVuKhamBenhNhanVien_GoiKhamSucKhoeDichVuKhamBenh] FOREIGN KEY([GoiKhamSucKhoeDichVuKhamBenhId])
REFERENCES [dbo].[GoiKhamSucKhoeDichVuKhamBenh] ([Id])
GO
ALTER TABLE [dbo].[GoiKhamSucKhoeChungDichVuKhamBenhNhanVien] CHECK CONSTRAINT [FK_GoiKhamSucKhoeChungDichVuKhamBenhNhanVien_GoiKhamSucKhoeDichVuKhamBenh]
GO
ALTER TABLE [dbo].[GoiKhamSucKhoeChungDichVuKhamBenhNhanVien]  WITH CHECK ADD  CONSTRAINT [FK_GoiKhamSucKhoeChungDichVuKhamBenhNhanVien_HopDongKhamSucKhoeNhanVien] FOREIGN KEY([HopDongKhamSucKhoeNhanVienId])
REFERENCES [dbo].[HopDongKhamSucKhoeNhanVien] ([Id])
GO
ALTER TABLE [dbo].[GoiKhamSucKhoeChungDichVuKhamBenhNhanVien] CHECK CONSTRAINT [FK_GoiKhamSucKhoeChungDichVuKhamBenhNhanVien_HopDongKhamSucKhoeNhanVien]
GO
ALTER TABLE [dbo].[GoiKhamSucKhoeChungDichVuKyThuatNhanVien]  WITH CHECK ADD  CONSTRAINT [FK_GoiKhamSucKhoeChungDichVuKyThuatNhanVien_DichVuKyThuatBenhVien] FOREIGN KEY([DichVuKyThuatBenhVienId])
REFERENCES [dbo].[DichVuKyThuatBenhVien] ([Id])
GO
ALTER TABLE [dbo].[GoiKhamSucKhoeChungDichVuKyThuatNhanVien] CHECK CONSTRAINT [FK_GoiKhamSucKhoeChungDichVuKyThuatNhanVien_DichVuKyThuatBenhVien]
GO
ALTER TABLE [dbo].[GoiKhamSucKhoeChungDichVuKyThuatNhanVien]  WITH CHECK ADD  CONSTRAINT [FK_GoiKhamSucKhoeChungDichVuKyThuatNhanVien_GoiKhamSucKhoe] FOREIGN KEY([GoiKhamSucKhoeId])
REFERENCES [dbo].[GoiKhamSucKhoe] ([Id])
GO
ALTER TABLE [dbo].[GoiKhamSucKhoeChungDichVuKyThuatNhanVien] CHECK CONSTRAINT [FK_GoiKhamSucKhoeChungDichVuKyThuatNhanVien_GoiKhamSucKhoe]
GO
ALTER TABLE [dbo].[GoiKhamSucKhoeChungDichVuKyThuatNhanVien]  WITH CHECK ADD  CONSTRAINT [FK_GoiKhamSucKhoeChungDichVuKyThuatNhanVien_GoiKhamSucKhoeDichVuDichVuKyThuat] FOREIGN KEY([GoiKhamSucKhoeDichVuDichVuKyThuatId])
REFERENCES [dbo].[GoiKhamSucKhoeDichVuDichVuKyThuat] ([Id])
GO
ALTER TABLE [dbo].[GoiKhamSucKhoeChungDichVuKyThuatNhanVien] CHECK CONSTRAINT [FK_GoiKhamSucKhoeChungDichVuKyThuatNhanVien_GoiKhamSucKhoeDichVuDichVuKyThuat]
GO
ALTER TABLE [dbo].[GoiKhamSucKhoeChungDichVuKyThuatNhanVien]  WITH CHECK ADD  CONSTRAINT [FK_GoiKhamSucKhoeChungDichVuKyThuatNhanVien_HopDongKhamSucKhoeNhanVien] FOREIGN KEY([HopDongKhamSucKhoeNhanVienId])
REFERENCES [dbo].[HopDongKhamSucKhoeNhanVien] ([Id])
GO
ALTER TABLE [dbo].[GoiKhamSucKhoeChungDichVuKyThuatNhanVien] CHECK CONSTRAINT [FK_GoiKhamSucKhoeChungDichVuKyThuatNhanVien_HopDongKhamSucKhoeNhanVien]
GO


Update dbo.CauHinh
Set [Value] = '3.0.7' where [Name] = 'CauHinhHeThong.DatabaseVesion'
