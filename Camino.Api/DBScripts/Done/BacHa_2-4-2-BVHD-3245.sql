CREATE TABLE [dbo].[YeuCauXuatKhoDuocPham]
(
[Id] [bigint] NOT NULL IDENTITY(1, 1),
[KhoXuatId] [bigint] NOT NULL,
[NguoiXuatId] [bigint] NOT NULL,
[NguoiNhanId] [bigint]  NULL,
[DuocDuyet] [bit] NULL,
[NgayDuyet] [datetime] NULL,
[NgayXuat] [datetime] NOT NULL,
[NhanVienDuyetId] [bigint] NULL,
[LyDoXuatKho] [nvarchar] (1000) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
[TraNCC] [bit] NULL,
[CreatedById] [bigint] NOT NULL,
[LastUserId] [bigint] NOT NULL,
[LastTime] [datetime] NOT NULL,
[CreatedOn] [datetime] NOT NULL,
[LastModified] [timestamp] NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[YeuCauXuatKhoDuocPham] ADD CONSTRAINT [PK_YeuCauXuatKhoDuocPham] PRIMARY KEY CLUSTERED ([Id]) ON [PRIMARY]
GO
ALTER TABLE [dbo].[YeuCauXuatKhoDuocPham] ADD CONSTRAINT [FK_YeuCauXuatKhoDuocPham_Kho] FOREIGN KEY ([KhoXuatId]) REFERENCES [dbo].[Kho] ([Id])
GO
ALTER TABLE [dbo].[YeuCauXuatKhoDuocPham] ADD CONSTRAINT [FK_YeuCauXuatKhoDuocPham_NhanVien] FOREIGN KEY ([NguoiXuatId]) REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauXuatKhoDuocPham] ADD CONSTRAINT [FK_YeuCauXuatKhoDuocPham_NhanVien1] FOREIGN KEY ([NguoiNhanId]) REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauXuatKhoDuocPham] ADD CONSTRAINT [FK_YeuCauXuatKhoDuocPham_NhanVien2] FOREIGN KEY ([NhanVienDuyetId]) REFERENCES [dbo].[NhanVien] ([Id])
GO


CREATE TABLE [dbo].[YeuCauXuatKhoDuocPhamChiTiet]
(
[Id] [bigint] NOT NULL IDENTITY(1, 1),
[YeuCauXuatKhoDuocPhamId] [bigint] NOT NULL,
[XuatKhoDuocPhamChiTietViTriId] [bigint] NULL,
[SoLuongXuat] [float] NOT NULL,
[DuocPhamBenhVienId] [bigint] NOT NULL,
[HopDongThauDuocPhamId] [bigint] NOT NULL,
[LaDuocPhamBHYT] [bit] NOT NULL,
[Solo] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AI NOT NULL,
[HanSuDung] [datetime] NOT NULL,
[NgayNhapVaoBenhVien] [datetime] NOT NULL,
[DonGiaNhap] [decimal] (15, 2) NOT NULL,
[TiLeTheoThapGia] [int] NOT NULL,
[VAT] [int] NOT NULL,
[MaVach] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
[TiLeBHYTThanhToan] [int] NULL,
[MaRef] [nvarchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
[CreatedById] [bigint] NOT NULL,
[LastUserId] [bigint] NOT NULL,
[LastTime] [datetime] NOT NULL,
[CreatedOn] [datetime] NOT NULL,
[LastModified] [timestamp] NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[YeuCauXuatKhoDuocPhamChiTiet] ADD CONSTRAINT [PK_YeuCauXuatKhoDuocPhamChiTiet] PRIMARY KEY CLUSTERED ([Id]) ON [PRIMARY]
GO
ALTER TABLE [dbo].[YeuCauXuatKhoDuocPhamChiTiet] ADD CONSTRAINT [FK_YeuCauXuatKhoDuocPhamChiTiet_DuocPhamBenhVien] FOREIGN KEY ([DuocPhamBenhVienId]) REFERENCES [dbo].[DuocPhamBenhVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauXuatKhoDuocPhamChiTiet] ADD CONSTRAINT [FK_YeuCauXuatKhoDuocPhamChiTiet_HopDongThauDuocPham] FOREIGN KEY ([HopDongThauDuocPhamId]) REFERENCES [dbo].[HopDongThauDuocPham] ([Id])
GO
ALTER TABLE [dbo].[YeuCauXuatKhoDuocPhamChiTiet] ADD CONSTRAINT [FK_YeuCauXuatKhoDuocPhamChiTiet_XuatKhoDuocPhamChiTietViTri] FOREIGN KEY ([XuatKhoDuocPhamChiTietViTriId]) REFERENCES [dbo].[XuatKhoDuocPhamChiTietViTri] ([Id])
GO
ALTER TABLE [dbo].[YeuCauXuatKhoDuocPhamChiTiet] ADD CONSTRAINT [FK_YeuCauXuatKhoDuocPhamChiTiet_YeuCauXuatKhoDuocPham] FOREIGN KEY ([YeuCauXuatKhoDuocPhamId]) REFERENCES [dbo].[YeuCauXuatKhoDuocPham] ([Id])
GO


CREATE TABLE [dbo].[YeuCauXuatKhoVatTu]
(
[Id] [bigint] NOT NULL IDENTITY(1, 1),
[KhoXuatId] [bigint] NOT NULL,
[NguoiXuatId] [bigint] NOT NULL,
[NguoiNhanId] [bigint]  NULL,
[DuocDuyet] [bit] NULL,
[NgayDuyet] [datetime] NULL,
[NgayXuat] [datetime] NOT NULL,
[NhanVienDuyetId] [bigint] NULL,
[LyDoXuatKho] [nvarchar] (1000) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
[TraNCC] [bit] NULL,
[CreatedById] [bigint] NOT NULL,
[LastUserId] [bigint] NOT NULL,
[LastTime] [datetime] NOT NULL,
[CreatedOn] [datetime] NOT NULL,
[LastModified] [timestamp] NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[YeuCauXuatKhoVatTu] ADD CONSTRAINT [PK_YeuCauXuatKhoVatTu] PRIMARY KEY CLUSTERED ([Id]) ON [PRIMARY]
GO
ALTER TABLE [dbo].[YeuCauXuatKhoVatTu] ADD CONSTRAINT [FK_YeuCauXuatKhoVatTu_Kho] FOREIGN KEY ([KhoXuatId]) REFERENCES [dbo].[Kho] ([Id])
GO
ALTER TABLE [dbo].[YeuCauXuatKhoVatTu] ADD CONSTRAINT [FK_YeuCauXuatKhoVatTu_NhanVien] FOREIGN KEY ([NguoiXuatId]) REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauXuatKhoVatTu] ADD CONSTRAINT [FK_YeuCauXuatKhoVatTu_NhanVien1] FOREIGN KEY ([NguoiNhanId]) REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauXuatKhoVatTu] ADD CONSTRAINT [FK_YeuCauXuatKhoVatTu_NhanVien2] FOREIGN KEY ([NhanVienDuyetId]) REFERENCES [dbo].[NhanVien] ([Id])
GO


CREATE TABLE [dbo].[YeuCauXuatKhoVatTuChiTiet]
(
[Id] [bigint] NOT NULL IDENTITY(1, 1),
[YeuCauXuatKhoVatTuId] [bigint] NOT NULL,
[XuatKhoVatTuChiTietViTriId] [bigint] NULL,
[SoLuongXuat] [float] NOT NULL,
[VatTuBenhVienId] [bigint] NOT NULL,
[HopDongThauVatTuId] [bigint] NOT NULL,
[LaVatTuBHYT] [bit] NOT NULL,
[Solo] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AI NOT NULL,
[HanSuDung] [datetime] NOT NULL,
[NgayNhapVaoBenhVien] [datetime] NOT NULL,
[DonGiaNhap] [decimal] (15, 2) NOT NULL,
[TiLeTheoThapGia] [int] NOT NULL,
[VAT] [int] NOT NULL,
[MaVach] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
[TiLeBHYTThanhToan] [int] NULL,
[MaRef] [nvarchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
[CreatedById] [bigint] NOT NULL,
[LastUserId] [bigint] NOT NULL,
[LastTime] [datetime] NOT NULL,
[CreatedOn] [datetime] NOT NULL,
[LastModified] [timestamp] NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[YeuCauXuatKhoVatTuChiTiet] ADD CONSTRAINT [PK_YeuCauXuatKhoVatTuChiTiet] PRIMARY KEY CLUSTERED ([Id]) ON [PRIMARY]
GO
ALTER TABLE [dbo].[YeuCauXuatKhoVatTuChiTiet] ADD CONSTRAINT [FK_YeuCauXuatKhoVatTuChiTiet_VatTuBenhVien] FOREIGN KEY ([VatTuBenhVienId]) REFERENCES [dbo].[VatTuBenhVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauXuatKhoVatTuChiTiet] ADD CONSTRAINT [FK_YeuCauXuatKhoVatTuChiTiet_HopDongThauVatTu] FOREIGN KEY ([HopDongThauVatTuId]) REFERENCES [dbo].[HopDongThauVatTu] ([Id])
GO
ALTER TABLE [dbo].[YeuCauXuatKhoVatTuChiTiet] ADD CONSTRAINT [FK_YeuCauXuatKhoVatTuChiTiet_XuatKhoVatTuChiTietViTri] FOREIGN KEY ([XuatKhoVatTuChiTietViTriId]) REFERENCES [dbo].[XuatKhoVatTuChiTietViTri] ([Id])
GO
ALTER TABLE [dbo].[YeuCauXuatKhoVatTuChiTiet] ADD CONSTRAINT [FK_YeuCauXuatKhoVatTuChiTiet_YeuCauXuatKhoVatTu] FOREIGN KEY ([YeuCauXuatKhoVatTuId]) REFERENCES [dbo].[YeuCauXuatKhoVatTu] ([Id])
GO

Update dbo.CauHinh
Set [Value] = '2.4.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'