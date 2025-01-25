ALTER TABLE YeuCauKhamBenh
ADD DuongThaiTuNgay DateTime NULL
GO

ALTER TABLE YeuCauKhamBenh
ADD DuongThaiDenNgay DateTime NULL
GO

ALTER TABLE YeuCauKhamBenh
ADD NghiHuongBHXHTuNgay DateTime NULL
GO

ALTER TABLE YeuCauKhamBenh
ADD NghiHuongBHXHDenNgay DateTime NULL
GO

ALTER TABLE YeuCauKhamBenh
ADD NghiHuongBHXHNguoiInId BIGINT NULL
GO

ALTER TABLE YeuCauKhamBenh
ADD NghiHuongBHXHNgayIn DateTime NULL
GO


Update dbo.CauHinh
Set [Value] = '3.3.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'