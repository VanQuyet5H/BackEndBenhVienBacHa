ALTER TABLE CongTyKhamSucKhoe ALTER COLUMN SoTaiKhoanNganHang NVARCHAR(1000) NULL;

Update dbo.CauHinh
Set [Value] = '3.0.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'