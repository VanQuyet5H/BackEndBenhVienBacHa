ALTER TABLE dbo.YeuCauNhapKhoDuocPhamChiTiet
ADD GhiChu NVARCHAR(1000)
GO

ALTER TABLE dbo.YeuCauNhapKhoVatTuChiTiet
ADD GhiChu NVARCHAR(1000)
GO
Update dbo.CauHinh
Set [Value] = '2.9.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'