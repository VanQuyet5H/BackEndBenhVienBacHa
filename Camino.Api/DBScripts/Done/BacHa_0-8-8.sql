ALTER TABLE dbo.[DuocPhamBenhVien]
DROP Column [Ma]
GO
ALTER TABLE dbo.[DuocPhamBenhVien]
ADD [Ma]  AS (CASE WHEN [MaDuocPhamBenhVien] IS NULL THEN right('00000'+ltrim(str([Id])),(5)) ELSE [MaDuocPhamBenhVien] END)
GO
ALTER TABLE dbo.[VatTuBenhVien]
DROP Column [Ma]
GO
ALTER TABLE dbo.[VatTuBenhVien]
ADD [Ma]  AS (CASE WHEN [MaVatTuBenhVien] IS NULL THEN right('00000'+ltrim(str([Id])),(5)) ELSE [MaVatTuBenhVien] END)
GO
Update CauHinh
Set [Value] = '0.8.8' where [Name] = 'CauHinhHeThong.DatabaseVesion'