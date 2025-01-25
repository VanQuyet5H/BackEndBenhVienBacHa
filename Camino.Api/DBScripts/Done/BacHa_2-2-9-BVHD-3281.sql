ALTER TABLE CongTyKhamSucKhoe ALTER COLUMN SoTaiKhoanNganHang NVARCHAR(1000) NULL;

Update dbo.CauHinh
Set [Value] = '2.2.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'