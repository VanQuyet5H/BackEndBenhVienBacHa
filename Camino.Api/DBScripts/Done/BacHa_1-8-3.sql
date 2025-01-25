ALTER TABLE YeuCauTiepNhan ALTER COLUMN KSKPhanLoaiTheLuc INT NULL

GO
Update CauHinh
Set [Value] = '1.8.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'