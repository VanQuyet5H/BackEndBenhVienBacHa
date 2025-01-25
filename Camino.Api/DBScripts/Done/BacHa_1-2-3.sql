ALTER TABLE [dbo].[PhienXetNghiem]
Drop column [BarCodeId]
ALTER TABLE [dbo].[PhienXetNghiem]
Add [BarCodeId]  AS (concat(right(datepart(year,[ThoiDiemBatDau]),(2)),right('00'+ltrim(str(datepart(month,[ThoiDiemBatDau]))),(2)),right('00'+ltrim(str(datepart(day,[ThoiDiemBatDau]))),(2)),right('0000'+ltrim(str([MaSo])),(4))))
GO
Update dbo.CauHinh
Set [Value] = '1.2.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'