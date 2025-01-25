ALTER TABLE dbo.YeuCauDichVuKyThuat
  ADD NhanVienThucHien2Id BIGINT NULL;
GO

ALTER TABLE [dbo].[YeuCauDichVuKyThuat]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDichVuKyThuat_NhanVienThucHien2] FOREIGN KEY(NhanVienThucHien2Id)
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat] CHECK CONSTRAINT [FK_YeuCauDichVuKyThuat_NhanVienThucHien2]
GO

ALTER TABLE dbo.YeuCauDichVuKyThuat
  ADD NhanVienKetLuan2Id BIGINT NULL;
GO

ALTER TABLE [dbo].[YeuCauDichVuKyThuat]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDichVuKyThuat_NhanVienKetLuan2] FOREIGN KEY(NhanVienKetLuan2Id)
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat] CHECK CONSTRAINT [FK_YeuCauDichVuKyThuat_NhanVienKetLuan2]
GO

ALTER TABLE dbo.YeuCauDichVuKyThuat
  ADD GhiChuKetQuaCLS NVARCHAR(1000) NULL;
GO


Update dbo.CauHinh
Set [Value] = '2.2.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'