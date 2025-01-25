ALTER TABLE dbo.[YeuCauLinhDuocPham]
ADD [NoiYeuCauId] [bigint] NULL
GO
ALTER TABLE dbo.[YeuCauLinhVatTu]
ADD [NoiYeuCauId] [bigint] NULL
GO
ALTER TABLE dbo.[YeuCauLinhVatTuChiTiet]
ADD [SoLuongCoTheXuat] [float] NULL
GO
Update CauHinh
Set [Value] = '0.8.6' where [Name] = 'CauHinhHeThong.DatabaseVesion'