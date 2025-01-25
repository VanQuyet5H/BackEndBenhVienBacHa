ALTER TABLE DichVuKyThuatBenhVien ADD TenKyThuat NVARCHAR(250) NULL
UPDATE CauHinh
Set [Value] = '1.9.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'