ALTER TABLE [dbo].[GachNo]
Add [TienThueHachToan] AS ISNULL(CAST(([TienHachToan] * [VAT]/100) as decimal(15,2)), 0);
GO
ALTER TABLE [dbo].[GachNo]
Add [TongTienHachToan] AS ISNULL(CAST((([TienHachToan] + ISNULL(CAST(([TienHachToan] * [VAT]/100) as decimal(15,2)), 0)) * [TyGia]) as decimal(15,2)), 0);
GO

Update CauHinh
Set [Value] = '1.1.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'