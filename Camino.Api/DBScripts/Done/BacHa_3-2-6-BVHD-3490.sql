CREATE TABLE [dbo].[DichVuKyThuatBenhVienDinhMucDuocPhamVatTu](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DichVuKyThuatBenhVienId] [bigint] NOT NULL,
	[DuocPhamBenhVienId] [bigint] NULL,
	[VatTuBenhVienId] [bigint] NULL,
	[SoLuong] [float] NOT NULL,
	[KhongTinhPhi] [bit] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_DichVuKyThuatBenhVienDinhMucDuocPhamVatTu] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[DichVuKyThuatBenhVienDinhMucDuocPhamVatTu]  WITH CHECK ADD  CONSTRAINT [FK_DichVuKyThuatBenhVienDinhMucDuocPhamVatTu_DichVuKyThuatBenhVien] FOREIGN KEY([DichVuKyThuatBenhVienId])
REFERENCES [dbo].[DichVuKyThuatBenhVien] ([Id])
GO
ALTER TABLE [dbo].[DichVuKyThuatBenhVienDinhMucDuocPhamVatTu] CHECK CONSTRAINT [FK_DichVuKyThuatBenhVienDinhMucDuocPhamVatTu_DichVuKyThuatBenhVien]
GO
ALTER TABLE [dbo].[DichVuKyThuatBenhVienDinhMucDuocPhamVatTu]  WITH CHECK ADD  CONSTRAINT [FK_DichVuKyThuatBenhVienDinhMucDuocPhamVatTu_DuocPhamBenhVien] FOREIGN KEY([DuocPhamBenhVienId])
REFERENCES [dbo].[DuocPhamBenhVien] ([Id])
GO
ALTER TABLE [dbo].[DichVuKyThuatBenhVienDinhMucDuocPhamVatTu] CHECK CONSTRAINT [FK_DichVuKyThuatBenhVienDinhMucDuocPhamVatTu_DuocPhamBenhVien]
GO
ALTER TABLE [dbo].[DichVuKyThuatBenhVienDinhMucDuocPhamVatTu]  WITH CHECK ADD  CONSTRAINT [FK_DichVuKyThuatBenhVienDinhMucDuocPhamVatTu_VatTuBenhVien] FOREIGN KEY([VatTuBenhVienId])
REFERENCES [dbo].[VatTuBenhVien] ([Id])
GO
ALTER TABLE [dbo].[DichVuKyThuatBenhVienDinhMucDuocPhamVatTu] CHECK CONSTRAINT [FK_DichVuKyThuatBenhVienDinhMucDuocPhamVatTu_VatTuBenhVien]
GO

Update dbo.CauHinh
Set [Value] = '3.2.6' where [Name] = 'CauHinhHeThong.DatabaseVesion'
