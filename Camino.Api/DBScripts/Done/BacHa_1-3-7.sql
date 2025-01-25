ALTER TABLE NhapKhoMauChiTiet
ADD KetQuaXetNghiemMoiTruongMuoi INT NULL
GO

ALTER TABLE NhapKhoMauChiTiet
ADD KetQuaXetNghiem37oCKhangGlubulin INT NULL
GO

ALTER TABLE NhapKhoMauChiTiet
ADD KetQuaXetNghiemNAT INT NULL
GO

ALTER TABLE NhapKhoMauChiTiet
ADD NgayLamXetNghiemHoaHop DATETIME NULL
GO

ALTER TABLE NhapKhoMauChiTiet
ALTER COLUMN KetQuaPhanUngCheoOngI INT NULL
GO

ALTER TABLE NhapKhoMauChiTiet
ALTER COLUMN KetQuaPhanUngCheoOngII INT NULL
GO

Update dbo.CauHinh
Set [Value] = '1.3.7' where [Name] = 'CauHinhHeThong.DatabaseVesion'