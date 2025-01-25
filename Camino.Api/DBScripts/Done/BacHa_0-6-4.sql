
Alter TABLE YeuCauTiepNhan Add SoDienThoaiDisplay AS ([dbo].[LayFormatSoDienThoai]([SoDienThoai]))

Update dbo.CauHinh
Set [Value] = '0.6.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'