

ALTER TABLE dbo.[MauXetNghiem] 
ADD [ThoiDiemNhanMau] [datetime] NULL,
	[PhongNhanMauId] [bigint] NULL,
	[NhanVienNhanMauId] [bigint] NULL 
	
GO
ALTER TABLE dbo.[PhienXetNghiemChiTiet] 
ADD 
	[ThoiDiemLayMau] [datetime] NULL,
	[PhongLayMauId] [bigint] NULL,
	[NhanVienLayMauId] [bigint] NULL,
	[ThoiDiemNhanMau] [datetime] NULL,
	[PhongNhanMauId] [bigint] NULL,
	[NhanVienNhanMauId] [bigint] NULL
	
ALTER TABLE [dbo].[MauXetNghiem]  WITH CHECK ADD  CONSTRAINT [FK_MauXetNghiem_PhongBenhVien1] FOREIGN KEY([PhongNhanMauId])
REFERENCES [dbo].[PhongBenhVien] ([Id])
GO

ALTER TABLE [dbo].[MauXetNghiem] CHECK CONSTRAINT [FK_MauXetNghiem_PhongBenhVien1]
GO

ALTER TABLE [dbo].[MauXetNghiem]  WITH CHECK ADD  CONSTRAINT [FK_MauXetNghiem_NhanVien2] FOREIGN KEY([NhanVienNhanMauId])
REFERENCES [dbo].[NhanVien] ([Id])
GO

ALTER TABLE [dbo].[MauXetNghiem] CHECK CONSTRAINT [FK_MauXetNghiem_NhanVien2]
GO

ALTER TABLE [dbo].[PhienXetNghiemChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_PhienXetNghiemChiTiet_NhanVien1] FOREIGN KEY([NhanVienLayMauId])
REFERENCES [dbo].[NhanVien] ([Id])
GO

ALTER TABLE [dbo].[PhienXetNghiemChiTiet] CHECK CONSTRAINT [FK_PhienXetNghiemChiTiet_NhanVien1]
GO

ALTER TABLE [dbo].[PhienXetNghiemChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_PhienXetNghiemChiTiet_NhanVien2] FOREIGN KEY([NhanVienNhanMauId])
REFERENCES [dbo].[NhanVien] ([Id])
GO

ALTER TABLE [dbo].[PhienXetNghiemChiTiet] CHECK CONSTRAINT [FK_PhienXetNghiemChiTiet_NhanVien2]
GO

ALTER TABLE [dbo].[PhienXetNghiemChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_PhienXetNghiemChiTiet_PhongBenhVien] FOREIGN KEY([PhongLayMauId])
REFERENCES [dbo].[PhongBenhVien] ([Id])
GO

ALTER TABLE [dbo].[PhienXetNghiemChiTiet] CHECK CONSTRAINT [FK_PhienXetNghiemChiTiet_PhongBenhVien]
GO

ALTER TABLE [dbo].[PhienXetNghiemChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_PhienXetNghiemChiTiet_PhongBenhVien1] FOREIGN KEY([PhongNhanMauId])
REFERENCES [dbo].[PhongBenhVien] ([Id])
GO

ALTER TABLE [dbo].[PhienXetNghiemChiTiet] CHECK CONSTRAINT [FK_PhienXetNghiemChiTiet_PhongBenhVien1]
GO

GO
UPDATE CauHinh
Set [Value] = '2.1.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'