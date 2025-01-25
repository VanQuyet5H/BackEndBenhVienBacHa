
ALTER TABLE dbo.ToaThuocMauChiTiet
   ADD GhiChu [nvarchar] (1000) NULL;

Update CauHinh
Set [Value] = '1.0.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'