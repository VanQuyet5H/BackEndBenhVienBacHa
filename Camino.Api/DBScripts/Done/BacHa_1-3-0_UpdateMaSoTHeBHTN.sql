ALTER TABLE BenhNhanCongTyBaoHiemTuNhan ALTER COLUMN MaSoThe nvarchar(30) NULL;

ALTER TABLE YeuCauTiepNhanCongTyBaoHiemTuNhan ALTER COLUMN MaSoThe nvarchar(30) NULL;
GO

Update dbo.CauHinh
Set [Value] = '1.3.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'