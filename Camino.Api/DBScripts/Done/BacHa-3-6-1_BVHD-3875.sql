CREATE TABLE [dbo].[DuocPhamBenhVienMayXetNghiem](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DuocPhamBenhVienId] [bigint] NOT NULL,
	[MayXetNghiemId] [bigint] NOT NULL,
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

ALTER TABLE [dbo].[DuocPhamBenhVienMayXetNghiem]  WITH CHECK ADD  CONSTRAINT [FK_DuocPhamBenhVienMayXetNghiem_DuocPhamBenhVien] FOREIGN KEY([DuocPhamBenhVienId])
REFERENCES [dbo].[DuocPhamBenhVien] ([Id])
GO

ALTER TABLE [dbo].[DuocPhamBenhVienMayXetNghiem] CHECK CONSTRAINT [FK_DuocPhamBenhVienMayXetNghiem_DuocPhamBenhVien]
GO

ALTER TABLE [dbo].[DuocPhamBenhVienMayXetNghiem]  WITH CHECK ADD  CONSTRAINT [FK_DuocPhamBenhVienMayXetNghiem_MayXetNghiem] FOREIGN KEY([MayXetNghiemId])
REFERENCES [dbo].[MayXetNghiem] ([Id])
GO

ALTER TABLE [dbo].[DuocPhamBenhVienMayXetNghiem] CHECK CONSTRAINT [FK_DuocPhamBenhVienMayXetNghiem_MayXetNghiem]
GO

ALTER TABLE [dbo].[DuocPhamBenhVien]
ADD 
	[LoaiDieuKienBaoQuanDuocPham] [int] NULL,
	[ThongTinDieuKienBaoQuanDuocPham] NVARCHAR(1000) NULL
GO

ALTER TABLE [dbo].[YeuCauLinhDuocPhamChiTiet]
ADD 
	[DanhSachMayXetNghiemId] NVARCHAR(200) NULL
GO

ALTER TABLE [dbo].[NhapKhoDuocPhamChiTiet]
ADD 
	[DanhSachMayXetNghiemId] NVARCHAR(200) NULL
GO

ALTER TABLE [dbo].[YeuCauXuatKhoDuocPhamChiTiet]
ADD 
	[MayXetNghiemId] [bigint] NULL
GO

ALTER TABLE [dbo].[XuatKhoDuocPhamChiTietViTri]
ADD 
	[MayXetNghiemId] [bigint] NULL
Go
Update dbo.CauHinh
Set [Value] = '3.6.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'