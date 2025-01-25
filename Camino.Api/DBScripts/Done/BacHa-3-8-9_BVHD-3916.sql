ALTER TABLE NoiTruPhieuDieuTri ADD GhiChuCanLamSang NVARCHAR(MAX) NULL
GO
Update dbo.CauHinh
Set [Value] = '3.8.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'