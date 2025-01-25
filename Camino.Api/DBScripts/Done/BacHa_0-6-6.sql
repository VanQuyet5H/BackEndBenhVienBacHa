ALTER TABLE dbo.YeuCauKhamBenh
ADD KhongKeToa BIT NULL;

Update dbo.CauHinh
Set [Value] = '0.6.6' where [Name] = 'CauHinhHeThong.DatabaseVesion'