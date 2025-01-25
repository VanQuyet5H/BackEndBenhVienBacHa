ALTER TABLE dbo.[NhapKhoDuocPham]
ADD [SoPhieu]  AS (concat('PN',right(datepart(year,[CreatedOn]),(2)),right('000000'+ltrim(str([Id])),(6))))
GO

ALTER TABLE dbo.[NhapKhoVatTu]
ADD [SoPhieu]  AS (concat('PN',right(datepart(year,[CreatedOn]),(2)),right('000000'+ltrim(str([Id])),(6))))
GO

Update CauHinh
Set [Value] = '0.8.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'







