ALTER TABLE [LichSuEmail]
ALTER COLUMN [TieuDe] nvarchar(1000) NULL;
GO
ALTER TABLE [LichSuEmail]
ALTER COLUMN [GoiDen] nvarchar(250) NOT NULL;
GO
ALTER TABLE [LichSuEmail]
ALTER COLUMN [NoiDung] nvarchar(MAX) NOT NULL;
GO
ALTER TABLE [LichSuEmail]
ALTER COLUMN [TapTinDinhKem] nvarchar(MAX) NULL;
GO

Update CauHinh
Set [Value] = '0.4.6' where [Name] = 'CauHinhHeThong.DatabaseVesion'