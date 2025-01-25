ALTER TABLE [YeuCauKhamBenh]
ADD KetQuaXetNghiemCLS NVARCHAR(2000) NULL
GO

ALTER TABLE [YeuCauKhamBenh]
ADD PhuongPhapTrongDieuTri NVARCHAR(2000) NULL
Go

Update dbo.CauHinh
Set [Value] = '3.3.8' where [Name] = 'CauHinhHeThong.DatabaseVesion'
