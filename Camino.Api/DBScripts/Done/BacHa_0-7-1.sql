ALTER TABLE DuocPham
ALTER COLUMN HoatChat NVARCHAR(550) NOT NULL

Update dbo.CauHinh
Set [Value] = '0.7.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'