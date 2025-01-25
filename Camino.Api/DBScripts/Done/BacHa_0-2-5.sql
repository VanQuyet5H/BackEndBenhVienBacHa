ALTER TABLE [YeuCauKhamBenh]
ADD 
	[CoNhapVien] bit NULL,
	[KhoaPhongNhapVienId] bigint NULL,
	[LyDoNhapVien] nvarchar(1000) NULL,
	[CoChuyenVien] bit NULL,
	[BenhVienChuyenVienId] bigint NULL,
	[LyDoChuyenVien] nvarchar(1000) NULL,
	[CoTuVong] bit NULL;
GO
ALTER TABLE [dbo].[YeuCauKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenh_KhoaPhong] FOREIGN KEY([KhoaPhongNhapVienId])
REFERENCES [dbo].[KhoaPhong] ([Id])
GO

ALTER TABLE [dbo].[YeuCauKhamBenh] CHECK CONSTRAINT [FK_YeuCauKhamBenh_KhoaPhong]
GO

GO
ALTER TABLE [dbo].[YeuCauKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenh_BenhVien] FOREIGN KEY([BenhVienChuyenVienId])
REFERENCES [dbo].[BenhVien] ([Id])
GO

ALTER TABLE [dbo].[YeuCauKhamBenh] CHECK CONSTRAINT [FK_YeuCauKhamBenh_BenhVien]
GO

Update CauHinh
Set [Value] = '0.2.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'