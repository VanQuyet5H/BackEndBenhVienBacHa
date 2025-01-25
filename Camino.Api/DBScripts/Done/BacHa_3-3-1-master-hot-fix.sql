
DECLARE @NhomKhamThaiId BIGINT = NULL

SELECT TOP 1 @NhomKhamThaiId = id FROM DichVuKhamBenhBenhVien WHERE Ten = N'Khám thai'
Update CauHinh SET Value = ISNULL(Value, N'') + CAST(@NhomKhamThaiId as NVARCHAR) + N';' WHERE Name = N'CauHinhBaoCao.NhomKhamPhuSan'

GO
Update dbo.CauHinh
Set [Value] = '3.3.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'