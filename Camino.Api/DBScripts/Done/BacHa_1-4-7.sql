ALTER TABLE YeuCauNhapVien
ADD YeuCauTiepNhanMeId BIGINT NULL
GO

ALTER TABLE YeuCauTiepNhan
ADD TenBanDau NVARCHAR(100) NULL
GO
ALTER TABLE YeuCauTiepNhan
ADD TenKhaiSinh NVARCHAR(100) NULL
GO
ALTER TABLE YeuCauTiepNhan
ADD GioSinh INT NULL
GO

ALTER TABLE YeuCauNhapVien
ADD CONSTRAINT FK_YeuCauNhapVienCon_YeuCauTiepNhanMe
FOREIGN KEY (YeuCauTiepNhanMeId) REFERENCES YeuCauTiepNhan(Id);
GO

Update dbo.CauHinh
Set [Value] = '1.4.7' where [Name] = 'CauHinhHeThong.DatabaseVesion'