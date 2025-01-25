ALTER TABLE dbo.[DuocPhamBenhVien]
ADD [Ma]  AS (CASE WHEN [MaDuocPhamBenhVien] IS NULL THEN str([Id]) ELSE [MaDuocPhamBenhVien] END)
GO
ALTER TABLE dbo.[VatTuBenhVien]
ADD [Ma]  AS (CASE WHEN [MaVatTuBenhVien] IS NULL THEN str([Id]) ELSE [MaVatTuBenhVien] END)
GO
Update CauHinh
Set [Value] = '0.8.7' where [Name] = 'CauHinhHeThong.DatabaseVesion'