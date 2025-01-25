ALTER TABLE YeuCauDichVuKyThuat ADD LyDoHuyDichVu NVARCHAR(2000) NULL
ALTER TABLE YeuCauDichVuKyThuat ADD NhanVienHuyDichVuId BIGINT NULL
GO
UPDATE CauHinh
Set [Value] = '1.9.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'