ALTER TABLE YeuCauTiepNhan
ADD LyDoHuyNhapVien NVARCHAR(2000) NULL
GO

ALTER TABLE YeuCauDichVuGiuongBenhVien
ADD TenGiuong NVARCHAR(250) NULL
GO

ALTER TABLE GiuongBenh
ADD Ma NVARCHAR(50) NULL
GO

UPDATE GiuongBenh
SET Ma = Id + ''
GO

ALTER TABLE GiuongBenh
ALTER COLUMN Ma NVARCHAR(50) NOT NULL
GO

Update dbo.CauHinh
Set [Value] = '1.5.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'