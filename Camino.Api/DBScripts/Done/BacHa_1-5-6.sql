ALTER TABLE [dbo].[YeuCauKhamBenh]
	ADD 
		[YeuCauTiepNhanTheBHYTId] [bigint] NULL,
		[MaSoTheBHYT] [nvarchar](20) NULL,
		CONSTRAINT [FK_YeuCauKhamBenh_YeuCauTiepNhanTheBHYT] FOREIGN KEY([YeuCauTiepNhanTheBHYTId]) REFERENCES [dbo].[YeuCauTiepNhanTheBHYT] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat]
	ADD 
		[YeuCauTiepNhanTheBHYTId] [bigint] NULL,
		[MaSoTheBHYT] [nvarchar](20) NULL,
		CONSTRAINT [FK_YeuCauDichVuKyThuat_YeuCauTiepNhanTheBHYT] FOREIGN KEY([YeuCauTiepNhanTheBHYTId]) REFERENCES [dbo].[YeuCauTiepNhanTheBHYT] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauDuocPhamBenhVien]
	ADD 
		[YeuCauTiepNhanTheBHYTId] [bigint] NULL,
		[MaSoTheBHYT] [nvarchar](20) NULL,
		CONSTRAINT [FK_YeuCauDuocPhamBenhVien_YeuCauTiepNhanTheBHYT] FOREIGN KEY([YeuCauTiepNhanTheBHYTId]) REFERENCES [dbo].[YeuCauTiepNhanTheBHYT] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauVatTuBenhVien]
	ADD 
		[YeuCauTiepNhanTheBHYTId] [bigint] NULL,
		[MaSoTheBHYT] [nvarchar](20) NULL,
		CONSTRAINT [FK_YeuCauVatTuBenhVien_YeuCauTiepNhanTheBHYT] FOREIGN KEY([YeuCauTiepNhanTheBHYTId]) REFERENCES [dbo].[YeuCauTiepNhanTheBHYT] ([Id]);
GO
ALTER TABLE [dbo].[DonThuocThanhToanChiTiet]
	ADD 
		[YeuCauTiepNhanTheBHYTId] [bigint] NULL,
		[MaSoTheBHYT] [nvarchar](20) NULL,
		CONSTRAINT [FK_DonThuocThanhToanChiTiet_YeuCauTiepNhanTheBHYT] FOREIGN KEY([YeuCauTiepNhanTheBHYTId]) REFERENCES [dbo].[YeuCauTiepNhanTheBHYT] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauTruyenMau]
	ADD 
		[YeuCauTiepNhanTheBHYTId] [bigint] NULL,
		[MaSoTheBHYT] [nvarchar](20) NULL,
		CONSTRAINT [FK_YeuCauTruyenMau_YeuCauTiepNhanTheBHYT] FOREIGN KEY([YeuCauTiepNhanTheBHYTId]) REFERENCES [dbo].[YeuCauTiepNhanTheBHYT] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauDichVuGiuongBenhVienChiPhiBHYT]
	ADD 
		[YeuCauTiepNhanTheBHYTId] [bigint] NULL,
		[MaSoTheBHYT] [nvarchar](20) NULL,
		CONSTRAINT [FK_YeuCauDichVuGiuongBenhVienChiPhiBHYT_YeuCauTiepNhanTheBHYT] FOREIGN KEY([YeuCauTiepNhanTheBHYTId]) REFERENCES [dbo].[YeuCauTiepNhanTheBHYT] ([Id]);
GO
ALTER TABLE [dbo].[DuyetBaoHiemChiTiet]
	ADD 
		[YeuCauTiepNhanTheBHYTId] [bigint] NULL,
		[MaSoTheBHYT] [nvarchar](20) NULL,
		CONSTRAINT [FK_DuyetBaoHiemChiTiet_YeuCauTiepNhanTheBHYT] FOREIGN KEY([YeuCauTiepNhanTheBHYTId]) REFERENCES [dbo].[YeuCauTiepNhanTheBHYT] ([Id]);
GO

Update dbo.CauHinh
Set [Value] = '1.5.6' where [Name] = 'CauHinhHeThong.DatabaseVesion'