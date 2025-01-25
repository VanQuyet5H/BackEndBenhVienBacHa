CREATE TABLE [dbo].[DichVuKyThuatBenhVienNoiThucHienUuTien](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DichVuKyThuatBenhVienId] [bigint] NOT NULL,
	[PhongBenhVienId] [bigint] NOT NULL,
	[LoaiNoiThucHienUuTien] [int] NOT NULL,
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

ALTER TABLE [dbo].[DichVuKyThuatBenhVienNoiThucHienUuTien]  WITH CHECK ADD  CONSTRAINT [FK_DichVuKyThuatBenhVienNoiThucHienUuTien_DichVuKyThuatBenhVien] FOREIGN KEY([DichVuKyThuatBenhVienId])
REFERENCES [dbo].[DichVuKyThuatBenhVien] ([Id])
GO

ALTER TABLE [dbo].[DichVuKyThuatBenhVienNoiThucHienUuTien] CHECK CONSTRAINT [FK_DichVuKyThuatBenhVienNoiThucHienUuTien_DichVuKyThuatBenhVien]
GO

ALTER TABLE [dbo].[DichVuKyThuatBenhVienNoiThucHienUuTien]  WITH CHECK ADD  CONSTRAINT [FK_DichVuKyThuatBenhVienNoiThucHienUuTien_PhongBenhVien] FOREIGN KEY([PhongBenhVienId])
REFERENCES [dbo].[PhongBenhVien] ([Id])
GO

ALTER TABLE [dbo].[DichVuKyThuatBenhVienNoiThucHienUuTien] CHECK CONSTRAINT [FK_DichVuKyThuatBenhVienNoiThucHienUuTien_PhongBenhVien]
GO

Update dbo.CauHinh
Set [Value] = '0.6.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'