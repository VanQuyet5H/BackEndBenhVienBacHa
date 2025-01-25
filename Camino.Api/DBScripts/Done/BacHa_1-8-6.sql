UPDATE NhomDichVuBenhVien
SET IsDefault = 0
WHERE Ma NOT IN (N'PTTT',N'XN',N'CĐHA',N'TDCN',N'SA')

GO
Update CauHinh
Set [Value] = '1.8.6' where [Name] = 'CauHinhHeThong.DatabaseVesion'