ALTER TABLE [dbo].[YeuCauGoiDichVu]
 ADD [NgungSuDung] [bit] NULL
GO

ALTER TABLE [dbo].[ChuongTrinhGoiDichVu]
 ADD [BenhNhanId] [bigint] NULL
GO

ALTER TABLE [dbo].[ChuongTrinhGoiDichVu]  WITH CHECK ADD  CONSTRAINT [FK_ChuongTrinhGoiDichVu_BenhNhan] FOREIGN KEY([BenhNhanId])
REFERENCES [dbo].[BenhNhan] ([Id])
GO

ALTER TABLE [dbo].[ChuongTrinhGoiDichVu] CHECK CONSTRAINT [FK_ChuongTrinhGoiDichVu_BenhNhan]
GO

Update dbo.CauHinh
Set [Value] = '3.1.8' where [Name] = 'CauHinhHeThong.DatabaseVesion'