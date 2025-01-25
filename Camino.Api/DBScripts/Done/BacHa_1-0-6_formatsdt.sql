Alter TABLE [CongTyBaoHiemTuNhan] Add SoDienThoaiDisplay AS ([dbo].[LayFormatSoDienThoai]([SoDienThoai]))

Update CauHinh
Set [Value] = '1.0.6' where [Name] = 'CauHinhHeThong.DatabaseVesion'