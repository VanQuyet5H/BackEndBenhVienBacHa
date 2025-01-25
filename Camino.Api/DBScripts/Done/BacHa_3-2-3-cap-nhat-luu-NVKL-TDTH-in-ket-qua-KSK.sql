ALTER TABLE YeuCauTiepNhan
ADD LoaiLuuInKetQuaKSK BIT NULL
GO
Update dbo.CauHinh
Set [Value] = '3.2.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'