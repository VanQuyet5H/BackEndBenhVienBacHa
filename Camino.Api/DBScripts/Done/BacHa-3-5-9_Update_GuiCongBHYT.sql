ALter TABLE [dbo].[YeuCauTiepNhanDuLieuGuiCongBHYT]
	Add [CoGuiCong] [bit] NULL

Go
Update dbo.CauHinh
Set [Value] = '3.5.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'