ALTER TABLE YeuCauKhamBenh
ADD LoiDanCuaBacSi nvarchar(max) NULL;

Go
ALTER TABLE ICD
ADD LoiDanCuaBacSi nvarchar(max) NULL;

GO
Update CauHinh
Set [Value] = '0.0.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'