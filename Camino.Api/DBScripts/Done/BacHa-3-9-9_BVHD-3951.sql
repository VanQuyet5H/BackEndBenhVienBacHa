ALTER TABLE TaiKhoanBenhNhanThu ADD GhiChu NVARCHAR(1000) NULL

Update dbo.CauHinh
Set [Value] = '3.4.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'