ALTER TABLE YeuCauKhamBenh ADD LyDoHuyDichVu NVARCHAR(2000) NULL
ALTER TABLE YeuCauKhamBenh ADD NhanVienHuyDichVuId BIGINT NULL
GO
UPDATE CauHinh
Set [Value] = '1.9.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'