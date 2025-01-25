ALTER TABLE dbo.DuocPham
ADD DuocPhamCoDau  bit NULL
GO
Update dbo.CauHinh
Set [Value] = '3.0.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'