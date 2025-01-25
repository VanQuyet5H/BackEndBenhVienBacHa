ALTER TABLE [dbo].[TaiKhoanBenhNhanThu]  
ADD  CONSTRAINT [FK_TaiKhoanBenhNhanThu_YeuCauTiepNhan] FOREIGN KEY([YeuCauTiepNhanId])
REFERENCES [dbo].[YeuCauTiepNhan] ([Id])
GO

Update dbo.CauHinh
Set [Value] = '1.5.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'