ALTER TABLE dbo.[BenhNhanCongTyBaoHiemTuNhan] ALTER COLUMN [SoDienThoai] NVARCHAR (100) NULL;

Update CauHinh
Set [Value] = '0.8.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'