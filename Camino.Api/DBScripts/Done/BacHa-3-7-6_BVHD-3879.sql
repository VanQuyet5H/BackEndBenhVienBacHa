ALTER TABLE [dbo].[YeuCauTiepNhanDuLieuGuiCongBHYT]	
	ADD 
		[HinhThucKCB] int NULL,
		[NgayVao] datetime NULL,
		[NgayRa] datetime NULL
GO
Update dbo.CauHinh
Set [Value] = '3.6.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'