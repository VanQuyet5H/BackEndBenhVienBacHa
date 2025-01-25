CREATE TABLE [dbo].[KetQuaNhomXetNghiem](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[YeuCauTiepNhanId] [bigint] NOT NULL,
	[NhomDichVuBenhVienId] [bigint] NOT NULL,
	[FileKetQuaCanLamSangId] [bigint] NULL,
	[KetLuan] [nvarchar](4000) NULL,
	[ThoiDiemKetLuan] [datetime] NULL,
	[NhanVienKetLuanId] [bigint] NULL,
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

ALTER TABLE [dbo].[KetQuaNhomXetNghiem]  WITH CHECK ADD  CONSTRAINT [FK_KetQuaNhomXetNghiem_YeuCauTiepNhan] FOREIGN KEY([YeuCauTiepNhanId])
REFERENCES [dbo].[YeuCauTiepNhan] ([Id])
GO

ALTER TABLE [dbo].[KetQuaNhomXetNghiem] CHECK CONSTRAINT [FK_KetQuaNhomXetNghiem_YeuCauTiepNhan]
GO

ALTER TABLE [dbo].[KetQuaNhomXetNghiem]  WITH CHECK ADD  CONSTRAINT [FK_KetQuaNhomXetNghiem_NhomDichVuBenhVien] FOREIGN KEY([NhomDichVuBenhVienId])
REFERENCES [dbo].[NhomDichVuBenhVien] ([Id])
GO

ALTER TABLE [dbo].[KetQuaNhomXetNghiem] CHECK CONSTRAINT [FK_KetQuaNhomXetNghiem_NhomDichVuBenhVien]
GO

ALTER TABLE [dbo].[KetQuaNhomXetNghiem]  WITH CHECK ADD  CONSTRAINT [FK_KetQuaNhomXetNghiem_FileKetQuaCanLamSang] FOREIGN KEY([FileKetQuaCanLamSangId])
REFERENCES [dbo].[FileKetQuaCanLamSang] ([Id])
GO

ALTER TABLE [dbo].[KetQuaNhomXetNghiem] CHECK CONSTRAINT [FK_KetQuaNhomXetNghiem_FileKetQuaCanLamSang]
GO

ALTER TABLE [dbo].[KetQuaNhomXetNghiem]  WITH CHECK ADD  CONSTRAINT [FK_KetQuaNhomXetNghiem_NhanVienKetLuan] FOREIGN KEY([NhanVienKetLuanId])
REFERENCES [dbo].[NhanVien] ([Id])
GO

ALTER TABLE [dbo].[KetQuaNhomXetNghiem] CHECK CONSTRAINT [FK_KetQuaNhomXetNghiem_NhanVienKetLuan]
GO

Update dbo.CauHinh
Set [Value] = '0.5.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'

