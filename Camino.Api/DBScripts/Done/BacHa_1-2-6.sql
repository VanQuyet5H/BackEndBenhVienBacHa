ALTER TABLE [dbo].[PhieuGoiMauXetNghiem]
	ADD SoPhieu AS (concat('PG',right(datepart(year,[CreatedOn]),(2)),right('000000'+ltrim(str([Id])),(6))))
GO

UPDATE dbo.CauHinh
Set [Value] = '1.2.6' where [Name] = 'CauHinhHeThong.DatabaseVesion'