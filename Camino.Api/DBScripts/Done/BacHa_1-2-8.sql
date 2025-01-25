ALTER TABLE MauXetNghiem
ADD ThoiDiemXetKhongDat DATETIME NULL
GO

UPDATE dbo.CauHinh
Set [Value] = '1.2.8' where [Name] = 'CauHinhHeThong.DatabaseVesion'