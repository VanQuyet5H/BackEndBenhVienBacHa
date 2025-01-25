ALTER TABLE dbo.YeuCauDichVuKyThuat
  ADD NhanVienHuyTrangThaiDaThucHienId BIGINT NULL;
GO

ALTER TABLE [dbo].[YeuCauDichVuKyThuat]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDichVuKyThuat_NhanVienHuyTrangThaiDaThucHien] FOREIGN KEY(NhanVienHuyTrangThaiDaThucHienId)
REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat] CHECK CONSTRAINT [FK_YeuCauDichVuKyThuat_NhanVienHuyTrangThaiDaThucHien]
GO

ALTER TABLE dbo.YeuCauDichVuKyThuat
  ADD LyDoHuyTrangThaiDaThucHien NVARCHAR(1000) NULL;
GO


Update dbo.CauHinh
Set [Value] = '2.2.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'