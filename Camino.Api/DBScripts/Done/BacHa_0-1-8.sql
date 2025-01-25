ALTER TABLE [TaiKhoanBenhNhanThu]
ADD 
	[CongNo] decimal(15,2) NULL;
GO

Update CauHinh
Set [Value] = '0.1.8' where [Name] = 'CauHinhHeThong.DatabaseVesion'