ALTER TABLE dbo.[XuatKhoDuocPham]
DROP COLUMN [SoPhieu]
GO
ALTER TABLE dbo.[XuatKhoDuocPham]
ADD [SoPhieu]  AS (concat('PX',right(datepart(year,[CreatedOn]),(2)),right('000000'+ltrim(str([Id])),(6))))
GO
ALTER TABLE dbo.[XuatKhoVatTu]
DROP COLUMN [SoPhieu]
GO
ALTER TABLE dbo.[XuatKhoVatTu]
ADD [SoPhieu]  AS (concat('PX',right(datepart(year,[CreatedOn]),(2)),right('000000'+ltrim(str([Id])),(6))))
GO
ALTER TABLE dbo.[YeuCauLinhDuocPham]
ADD [SoPhieu]  AS (concat('PL',right(datepart(year,[CreatedOn]),(2)),right('000000'+ltrim(str([Id])),(6))))
GO
ALTER TABLE dbo.[YeuCauLinhVatTu]
ADD [SoPhieu]  AS (concat('PL',right(datepart(year,[CreatedOn]),(2)),right('000000'+ltrim(str([Id])),(6))))
GO
ALTER TABLE dbo.[YeuCauDuocPhamBenhVien]
ADD [GiaiDoanPhauThuat] int NULL;
GO
ALTER TABLE dbo.[YeuCauVatTuBenhVien]
ADD [GiaiDoanPhauThuat] int NULL;
GO
Update CauHinh
Set [Value] = '0.7.7' where [Name] = 'CauHinhHeThong.DatabaseVesion'







