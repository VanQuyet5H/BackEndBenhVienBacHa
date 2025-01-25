ALTER TABLE dbo.BenhVien Add SoDienThoaiDisplay AS ([dbo].[LayFormatSoDienThoai]([SoDienThoaiLanhDao]))
ALTER TABLE dbo.NhaThau Add SoDienThoaiDisplay AS ([dbo].[LayFormatSoDienThoai]([SoDienThoaiLienHe]))

Update dbo.CauHinh
Set [Value] = '0.6.7' where [Name] = 'CauHinhHeThong.DatabaseVesion'