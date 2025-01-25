ALTER TABLE [dbo].[YeuCauTraDuocPham]
	add [SoPhieu]  AS (concat('PH',right(datepart(year,[CreatedOn]),(2)),right('000000'+ltrim(str([Id])),(6))))
GO
ALTER TABLE [dbo].[YeuCauTraVatTu]
	add [SoPhieu]  AS (concat('PH',right(datepart(year,[CreatedOn]),(2)),right('000000'+ltrim(str([Id])),(6))))
GO
ALTER TABLE [dbo].[YeuCauTraDuocPhamChiTiet]
ADD	[DuocPhamBenhVienId] [bigint] NOT NULL,
	[SoLuongTra] [float] NOT NULL,
	[HopDongThauDuocPhamId] [bigint] NOT NULL,
	[LaDuocPhamBHYT] [bit] NOT NULL,
	[DuocPhamBenhVienPhanNhomId] [bigint] NOT NULL,
	[NgayNhapVaoBenhVien] [datetime] NOT NULL,
	[Solo] [nvarchar](50) NOT NULL,
	[HanSuDung] [datetime] NOT NULL,
	[DonGiaNhap] [decimal](15, 2) NOT NULL,
	[TiLeTheoThapGia] [int] NOT NULL,
	[VAT] [int] NOT NULL,
	[MaVach] [nvarchar](100) NULL,
	[KhoViTriId] [bigint] NULL;
GO
ALTER TABLE [dbo].[YeuCauTraDuocPhamChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauTraDuocPhamChiTiet_DuocPhamBenhVien] FOREIGN KEY([DuocPhamBenhVienId])
REFERENCES [dbo].[DuocPhamBenhVien] ([Id])
GO

ALTER TABLE [dbo].[YeuCauTraDuocPhamChiTiet] CHECK CONSTRAINT [FK_YeuCauTraDuocPhamChiTiet_DuocPhamBenhVien]
GO

ALTER TABLE [dbo].[YeuCauTraDuocPhamChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauTraDuocPhamChiTiet_DuocPhamBenhVienPhanNhom] FOREIGN KEY([DuocPhamBenhVienPhanNhomId])
REFERENCES [dbo].[DuocPhamBenhVienPhanNhom] ([Id])
GO

ALTER TABLE [dbo].[YeuCauTraDuocPhamChiTiet] CHECK CONSTRAINT [FK_YeuCauTraDuocPhamChiTiet_DuocPhamBenhVienPhanNhom]
GO

ALTER TABLE [dbo].[YeuCauTraDuocPhamChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauTraDuocPhamChiTiet_HopDongThauDuocPham] FOREIGN KEY([HopDongThauDuocPhamId])
REFERENCES [dbo].[HopDongThauDuocPham] ([Id])
GO

ALTER TABLE [dbo].[YeuCauTraDuocPhamChiTiet] CHECK CONSTRAINT [FK_YeuCauTraDuocPhamChiTiet_HopDongThauDuocPham]
GO

ALTER TABLE [dbo].[YeuCauTraDuocPhamChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauTraDuocPhamChiTiet_KhoViTri] FOREIGN KEY([KhoViTriId])
REFERENCES [dbo].[KhoViTri] ([Id])
GO

ALTER TABLE [dbo].[YeuCauTraDuocPhamChiTiet] CHECK CONSTRAINT [FK_YeuCauTraDuocPhamChiTiet_KhoViTri]
GO

ALTER TABLE [dbo].[YeuCauTraVatTuChiTiet]
ADD	[VatTuBenhVienId] [bigint] NOT NULL,
	[SoLuongTra] [float] NOT NULL,
	[HopDongThauVatTuId] [bigint] NOT NULL,
	[LaVatTuBHYT] [bit] NOT NULL,
	[NgayNhapVaoBenhVien] [datetime] NOT NULL,
	[Solo] [nvarchar](50) NOT NULL,
	[HanSuDung] [datetime] NOT NULL,
	[DonGiaNhap] [decimal](15, 2) NOT NULL,
	[TiLeTheoThapGia] [int] NOT NULL,
	[VAT] [int] NOT NULL,
	[MaVach] [nvarchar](100) NULL,
	[KhoViTriId] [bigint] NULL;
GO

ALTER TABLE [dbo].[YeuCauTraVatTuChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauTraVatTuChiTiet_VatTuBenhVien] FOREIGN KEY([VatTuBenhVienId])
REFERENCES [dbo].[VatTuBenhVien] ([Id])
GO

ALTER TABLE [dbo].[YeuCauTraVatTuChiTiet] CHECK CONSTRAINT [FK_YeuCauTraVatTuChiTiet_VatTuBenhVien]
GO

ALTER TABLE [dbo].[YeuCauTraVatTuChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauTraVatTuChiTiet_HopDongThauVatTu] FOREIGN KEY([HopDongThauVatTuId])
REFERENCES [dbo].[HopDongThauVatTu] ([Id])
GO

ALTER TABLE [dbo].[YeuCauTraVatTuChiTiet] CHECK CONSTRAINT [FK_YeuCauTraVatTuChiTiet_HopDongThauVatTu]
GO

ALTER TABLE [dbo].[YeuCauTraVatTuChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauTraVatTuChiTiet_KhoViTri] FOREIGN KEY([KhoViTriId])
REFERENCES [dbo].[KhoViTri] ([Id])
GO

ALTER TABLE [dbo].[YeuCauTraVatTuChiTiet] CHECK CONSTRAINT [FK_YeuCauTraVatTuChiTiet_KhoViTri]
GO
ALTER TABLE [dbo].[YeuCauTraDuocPhamChiTiet]
ALTER COLUMN [XuatKhoDuocPhamChiTietViTriId] [bigint] NULL
GO
ALTER TABLE [dbo].[YeuCauTraVatTuChiTiet]
ALTER COLUMN [XuatKhoVatTuChiTietViTriId] [bigint] NULL
GO

Update CauHinh
Set [Value] = '0.9.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'
