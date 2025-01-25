ALTER TABLE YeuCauDichVuKyThuat
ADD ThoiDiemKetLuan DateTime NULL
GO

Update dbo.CauHinh
Set [Value] = '1.4.6' where [Name] = 'CauHinhHeThong.DatabaseVesion'