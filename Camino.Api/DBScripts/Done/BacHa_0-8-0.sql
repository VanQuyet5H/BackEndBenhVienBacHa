ALTER TABLE dbo.[CongTyBaoHiemTuNhan] ALTER COLUMN [SoDienThoai] NVARCHAR (100) NOT NULL;
GO
ALTER TABLE dbo.[YeuCauTiepNhanCongTyBaoHiemTuNhan] ALTER COLUMN [SoDienThoai] NVARCHAR (100) NULL;
GO
Update CauHinh
Set [Value] = '0.8.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'







