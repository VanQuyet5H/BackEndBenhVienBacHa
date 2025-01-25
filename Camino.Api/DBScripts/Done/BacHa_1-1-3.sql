ALTER TABLE GachNo
ALTER COLUMN NgayHopDong DATETIME NULL
GO
EXEC sp_RENAME 'GachNo.SoHopDong', 'SoHoaDon', 'COLUMN';
GO
EXEC sp_RENAME 'GachNo.NgayHopDong', 'NgayHoaDon', 'COLUMN';

GO
Update CauHinh
Set [Value] = '1.1.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'