ALTER TABLE [YeuCauTiepNhan]
ADD LyDoTiepNhanId bigint NULL;
GO

ALTER TABLE [YeuCauTiepNhan]
DROP COLUMN LyDoTiepNhan;
GO

ALTER TABLE [dbo].[YeuCauTiepNhan]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauTiepNhan_LyDoTiepNhan] FOREIGN KEY([LyDoTiepNhanId])
REFERENCES [dbo].[LyDoTiepNhan] ([Id])
GO

Update CauHinh
Set [Value] = '0.3.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'
