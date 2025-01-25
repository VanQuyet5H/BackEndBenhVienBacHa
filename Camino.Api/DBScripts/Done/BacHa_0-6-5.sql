
ALTER TABLE dbo.NoiGioiThieu Add SoDienThoaiDisplay AS ([dbo].[LayFormatSoDienThoai]([SoDienThoai]))

ALTER TABLE dbo.NguoiGioiThieu Add SoDienThoaiDisplay AS ([dbo].[LayFormatSoDienThoai]([SoDienThoai]))

Update dbo.CauHinh
Set [Value] = '0.6.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'