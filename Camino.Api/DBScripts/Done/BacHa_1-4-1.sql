ALTER TABLE [dbo].[YeuCauTraDuocPhamTuBenhNhanChiTiet]
	ALTER COLUMN [YeuCauTraDuocPhamTuBenhNhanId] bigint NULL
GO

ALTER TABLE [dbo].[YeuCauTraDuocPhamTuBenhNhanChiTiet]
	ADD [KhoTraId] bigint NOT NULL
GO
ALTER TABLE [dbo].[YeuCauTraDuocPhamTuBenhNhanChiTiet]
	ADD [TraVeTuTruc] bit NOT NULL
GO
ALTER TABLE [dbo].[YeuCauTraDuocPhamTuBenhNhanChiTiet]
	ADD [NgayYeuCau] datetime NOT NULL
GO
ALTER TABLE [dbo].[YeuCauTraDuocPhamTuBenhNhanChiTiet]
	ADD [NhanVienYeuCauId] bigint NOT NULL
GO


ALTER TABLE [dbo].[YeuCauTraDuocPhamTuBenhNhanChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauTraDuocPhamTuBenhNhanChiTiet_Kho] FOREIGN KEY([KhoTraId])
REFERENCES [dbo].[Kho] ([Id])
GO

ALTER TABLE [dbo].[YeuCauTraDuocPhamTuBenhNhanChiTiet] CHECK CONSTRAINT [FK_YeuCauTraDuocPhamTuBenhNhanChiTiet_Kho]
GO

ALTER TABLE [dbo].[YeuCauTraDuocPhamTuBenhNhanChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauTraDuocPhamTuBenhNhanChiTiet_NhanVien] FOREIGN KEY([NhanVienYeuCauId])
REFERENCES [dbo].[NhanVien] ([Id])
GO

ALTER TABLE [dbo].[YeuCauTraDuocPhamTuBenhNhanChiTiet] CHECK CONSTRAINT [FK_YeuCauTraDuocPhamTuBenhNhanChiTiet_NhanVien]
GO

ALTER TABLE [dbo].[YeuCauTraDuocPhamTuBenhNhan]
	ADD [KhoaHoanTraId] bigint NOT NULL
GO
ALTER TABLE [dbo].[YeuCauTraDuocPhamTuBenhNhan]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauTraDuocPhamTuBenhNhan_KhoaPhong] FOREIGN KEY([KhoaHoanTraId])
REFERENCES [dbo].[KhoaPhong] ([Id])
GO

ALTER TABLE [dbo].[YeuCauTraDuocPhamTuBenhNhan] CHECK CONSTRAINT [FK_YeuCauTraDuocPhamTuBenhNhan_KhoaPhong]
GO

ALTER TABLE [dbo].[YeuCauDuocPhamBenhVien]
	ADD [SoLuongDaTra] FLOAT (53) NULL
GO

ALTER TABLE [dbo].[YeuCauTraVatTuTuBenhNhanChiTiet]
	ALTER COLUMN [YeuCauTraVatTuTuBenhNhanId] bigint NULL
GO

ALTER TABLE [dbo].[YeuCauTraVatTuTuBenhNhanChiTiet]
	ADD [KhoTraId] bigint NOT NULL
GO
ALTER TABLE [dbo].[YeuCauTraVatTuTuBenhNhanChiTiet]
	ADD [TraVeTuTruc] bit NOT NULL
GO
ALTER TABLE [dbo].[YeuCauTraVatTuTuBenhNhanChiTiet]
	ADD [NgayYeuCau] datetime NOT NULL
GO
ALTER TABLE [dbo].[YeuCauTraVatTuTuBenhNhanChiTiet]
	ADD [NhanVienYeuCauId] bigint NOT NULL
GO


ALTER TABLE [dbo].[YeuCauTraVatTuTuBenhNhanChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauTraVatTuTuBenhNhanChiTiet_Kho] FOREIGN KEY([KhoTraId])
REFERENCES [dbo].[Kho] ([Id])
GO

ALTER TABLE [dbo].[YeuCauTraVatTuTuBenhNhanChiTiet] CHECK CONSTRAINT [FK_YeuCauTraVatTuTuBenhNhanChiTiet_Kho]
GO

ALTER TABLE [dbo].[YeuCauTraVatTuTuBenhNhanChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauTraVatTuTuBenhNhanChiTiet_NhanVien] FOREIGN KEY([NhanVienYeuCauId])
REFERENCES [dbo].[NhanVien] ([Id])
GO

ALTER TABLE [dbo].[YeuCauTraVatTuTuBenhNhanChiTiet] CHECK CONSTRAINT [FK_YeuCauTraVatTuTuBenhNhanChiTiet_NhanVien]
GO

ALTER TABLE [dbo].[YeuCauTraVatTuTuBenhNhan]
	ADD [KhoaHoanTraId] bigint NOT NULL
GO
ALTER TABLE [dbo].[YeuCauTraVatTuTuBenhNhan]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauTraVatTuTuBenhNhan_KhoaPhong] FOREIGN KEY([KhoaHoanTraId])
REFERENCES [dbo].[KhoaPhong] ([Id])
GO

ALTER TABLE [dbo].[YeuCauTraVatTuTuBenhNhan] CHECK CONSTRAINT [FK_YeuCauTraVatTuTuBenhNhan_KhoaPhong]
GO

ALTER TABLE [dbo].[YeuCauVatTuBenhVien]
	ADD [SoLuongDaTra] FLOAT (53) NULL
GO

Update dbo.CauHinh
Set [Value] = '1.4.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'