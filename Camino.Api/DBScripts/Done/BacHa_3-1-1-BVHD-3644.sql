ALTER TABLE KetQuaXetNghiemChiTiet
  ALTER COLUMN GiaTriDuyet NVARCHAR(500) NULL;

Update dbo.CauHinh
Set [Value] = '3.1.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'