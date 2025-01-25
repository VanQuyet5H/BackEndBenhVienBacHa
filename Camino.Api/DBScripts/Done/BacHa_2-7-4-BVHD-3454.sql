-- xóa file XML : DuocPhamBenhVienDaCapNhatMaCuoiCung

UPDATE DuocPhamBenhVienPhanNhom
SET KyHieuPhanNhom = N'B'
Where Ten = N'Thiết bị y tế'
GO
UPDATE DuocPhamBenhVienPhanNhom
SET KyHieuPhanNhom = N'Y'
Where Ten = N'Vật tư y tế'
GO
UPDATE DuocPhamBenhVienPhanNhom
SET KyHieuPhanNhom = N'V'
Where Ten = N'Vaccine' OR Ten = N'Vaccin'
Go
Update dbo.CauHinh
Set [Value] = '2.7.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'