ALTER TABLE YeuCauDichVuKyThuatTiemChung
ALTER COLUMN HamLuong NVARCHAR(1000)

Update dbo.CauHinh
Set [Value] = '2.9.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'