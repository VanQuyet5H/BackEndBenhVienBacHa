ALTER TABLE [dbo].[FileKetQuaCanLamSang] ADD [YeuCauDichVuKyThuatId] BIGINT          NULL
GO
ALTER TABLE [dbo].[FileKetQuaCanLamSang] ADD [KetQuaNhomXetNghiemId] BIGINT          NULL
GO

ALTER TABLE [dbo].[FileKetQuaCanLamSang]  WITH CHECK ADD  CONSTRAINT [FK_FileKetQuaCanLamSang_KetQuaNhomXetNghiem] FOREIGN KEY ([KetQuaNhomXetNghiemId]) REFERENCES [dbo].[KetQuaNhomXetNghiem] ([Id])
GO

ALTER TABLE [dbo].[FileKetQuaCanLamSang] CHECK CONSTRAINT [FK_FileKetQuaCanLamSang_KetQuaNhomXetNghiem]
GO

ALTER TABLE [dbo].[FileKetQuaCanLamSang]  WITH CHECK ADD  CONSTRAINT [FK_FileKetQuaCanLamSang_YeuCauDichVuKyThuat] FOREIGN KEY ([YeuCauDichVuKyThuatId]) REFERENCES [dbo].[YeuCauDichVuKyThuat] ([Id])
GO

ALTER TABLE [dbo].[FileKetQuaCanLamSang] CHECK CONSTRAINT [FK_FileKetQuaCanLamSang_YeuCauDichVuKyThuat]
GO

ALTER TABLE [dbo].[KetQuaNhomXetNghiem] DROP CONSTRAINT [FK_KetQuaNhomXetNghiem_FileKetQuaCanLamSang]
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat] DROP CONSTRAINT [FK_YeuCauDichVuKyThuat_FileKetQuaCanLamSang]
GO
Update CauHinh
Set [Value] = '0.9.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'