ALTER TABLE YeuCauKhamBenh ADD GhiChuCanLamSang NVARCHAR(4000) NULL
GO
UPDATE CauHinh
Set [Value] = '1.9.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'