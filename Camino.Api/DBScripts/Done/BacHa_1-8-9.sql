ALTER TABLE NhapKhoMauChiTiet
ADD KetQuaXetNghiemKhac NVARCHAR(MAX) NULL
GO
ALTER TABLE NhapKhoMau
ALTER COLUMN NgayHoaDon DATETIME NULL
GO
ALTER TABLE NhapKhoMau
ALTER COLUMN SoHoaDon NVARCHAR(50) NULL

UPDATE CauHinh
Set [Value] = '1.8.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'