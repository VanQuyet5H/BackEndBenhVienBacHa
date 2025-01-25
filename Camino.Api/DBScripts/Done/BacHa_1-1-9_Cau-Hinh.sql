Update CauHinh
SET DataType = 10
WHERE Name in (N'CauHinhGachNo.TaiKhoan', N'CauHinhGachNo.SoTaiKhoanNganHang', N'CauHinhGachNo.LoaiThuChi') 
GO
Update dbo.CauHinh
Set [Value] = '1.1.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'