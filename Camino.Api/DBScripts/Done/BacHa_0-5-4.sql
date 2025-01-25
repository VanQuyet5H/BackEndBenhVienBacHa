
ALTER TABLE KhoaPhongNhanVien Add PhongBenhVienId bigint null;

ALTER TABLE [dbo].[KhoaPhongNhanVien]  WITH CHECK ADD  CONSTRAINT [FK_KhoaPhongNhanVien_PhongBenhVien] FOREIGN KEY([PhongBenhVienId])
REFERENCES [dbo].[PhongBenhVien] ([Id])
GO

ALTER TABLE [dbo].[KhoaPhongNhanVien] CHECK CONSTRAINT [FK_KhoaPhongNhanVien_PhongBenhVien]
GO

Update dbo.CauHinh
Set [Value] = '0.5.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'