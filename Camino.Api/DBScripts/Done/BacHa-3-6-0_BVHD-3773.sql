ALTER TABLE YeuCauKhamBenh
ADD STTChuyenVien BIGINT NULL

Go
Update dbo.CauHinh
Set [Value] = '3.6.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'