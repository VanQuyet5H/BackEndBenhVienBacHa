ALTER TABLE KhoDuocPhamViTri
ALTER COLUMN Ten nvarchar(120) NOT NULL;

Update CauHinh
Set [Value] = '0.0.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'