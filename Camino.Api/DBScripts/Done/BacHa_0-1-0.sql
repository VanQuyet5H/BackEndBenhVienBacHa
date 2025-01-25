ALTER TABLE DichVuKhamBenhBenhVien
ALTER COLUMN KhoaPhongId BIGINT NULL

ALTER TABLE DichVuKhamBenh
ALTER COLUMN KhoaId BIGINT NULL

Update CauHinh
Set [Value] = '0.1.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'