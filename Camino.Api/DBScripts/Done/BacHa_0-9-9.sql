ALTER TABLE [dbo].[MienGiamChiPhi] ADD [MaTheVoucher]                NVARCHAR (25)             NULL
ALTER TABLE [dbo].[MienGiamChiPhi] ADD [DoiTuongUuDaiId]                         BIGINT             NULL
GO
ALTER TABLE [dbo].[MienGiamChiPhi] ADD CONSTRAINT [FK_MienGiamChiPhi_DoiTuongUuDai] FOREIGN KEY ([DoiTuongUuDaiId]) REFERENCES [dbo].[DoiTuongUuDai] ([Id])
GO
Update CauHinh
Set [Value] = '0.9.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'