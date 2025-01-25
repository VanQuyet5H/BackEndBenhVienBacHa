ALter TABLE [dbo].[YeuCauChayLaiXetNghiem] Add [LanThucHien] INT NULL
GO
UPDATE [dbo].[YeuCauChayLaiXetNghiem] SET [LanThucHien] = 1
GO
ALter TABLE [dbo].[YeuCauChayLaiXetNghiem] ALTER Column [LanThucHien] INT NOT NULL
GO
Update dbo.CauHinh
Set [Value] = '1.2.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'