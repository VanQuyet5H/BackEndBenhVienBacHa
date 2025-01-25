ALTER TABLE CongTyKhamSucKhoe ALTER COLUMN NguoiDaiDien NVARCHAR(100) NULL;
ALTER TABLE CongTyKhamSucKhoe ALTER COLUMN NguoiLienHe NVARCHAR(100) NULL;

Update dbo.CauHinh
Set [Value] = '2.3.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'