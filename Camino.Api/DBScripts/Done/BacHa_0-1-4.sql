ALTER TABLE [DuyetBaoHiem]
ADD 
	[YeuCauTiepNhanId] bigint NULL
GO

ALTER TABLE [dbo].[DuyetBaoHiem]  WITH CHECK ADD  CONSTRAINT [FK_DuyetBaoHiem_YeuCauTiepNhan] FOREIGN KEY([YeuCauTiepNhanId])
REFERENCES [dbo].[YeuCauTiepNhan] ([Id])
		
GO
Update CauHinh
Set [Value] = '0.1.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'