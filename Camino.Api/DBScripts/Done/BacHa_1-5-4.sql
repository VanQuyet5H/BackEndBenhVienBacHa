ALTER TABLE [dbo].[DuocPhamVaVatTuBenhVien] DROP CONSTRAINT [FK_DuocPhamVaVatTuBenhVien_DuocPhamBenhVien]
GO
ALTER TABLE [dbo].[DuocPhamVaVatTuBenhVien] DROP CONSTRAINT [FK_DuocPhamVaVatTuBenhVien_VatTuBenhVien]
GO
Update dbo.CauHinh
Set [Value] = '1.5.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'