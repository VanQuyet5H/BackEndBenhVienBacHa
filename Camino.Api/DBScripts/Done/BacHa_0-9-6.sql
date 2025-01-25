ALTER TABLE [dbo].[YeuCauDuocPhamBenhVien] ADD [SoLuongDaLinhBu]           FLOAT      NULL
GO
ALTER TABLE [dbo].[YeuCauDuocPhamBenhVien] ADD [KhongLinhBu]               BIT             NULL
GO

ALTER TABLE [dbo].[YeuCauLinhDuocPhamChiTiet] ADD [YeuCauDuocPhamBenhVienId] BIGINT     NULL
GO

ALTER TABLE [dbo].[YeuCauLinhDuocPhamChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauLinhDuocPhamChiTiet_YeuCauDuocPhamBenhVien] FOREIGN KEY ([YeuCauDuocPhamBenhVienId]) REFERENCES [dbo].[YeuCauDuocPhamBenhVien] ([Id])
GO

ALTER TABLE [dbo].[YeuCauLinhDuocPhamChiTiet] CHECK CONSTRAINT [FK_YeuCauLinhDuocPhamChiTiet_YeuCauDuocPhamBenhVien]
GO

ALTER TABLE [dbo].[YeuCauVatTuBenhVien] ADD [SoLuongDaLinhBu]           FLOAT      NULL
GO
ALTER TABLE [dbo].[YeuCauVatTuBenhVien] ADD [KhongLinhBu]               BIT             NULL
GO

ALTER TABLE [dbo].[YeuCauLinhVatTuChiTiet] ADD [YeuCauVatTuBenhVienId] BIGINT     NULL
GO

ALTER TABLE [dbo].[YeuCauLinhVatTuChiTiet]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauLinhVatTuChiTiet_YeuCauVatTuBenhVien] FOREIGN KEY ([YeuCauVatTuBenhVienId]) REFERENCES [dbo].[YeuCauVatTuBenhVien] ([Id])
GO

ALTER TABLE [dbo].[YeuCauLinhVatTuChiTiet] CHECK CONSTRAINT [FK_YeuCauLinhVatTuChiTiet_YeuCauVatTuBenhVien]
GO

Update CauHinh
Set [Value] = '0.9.6' where [Name] = 'CauHinhHeThong.DatabaseVesion'