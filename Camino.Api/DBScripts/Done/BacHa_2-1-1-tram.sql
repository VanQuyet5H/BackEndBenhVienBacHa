alter table DichVuKyThuatBenhVien ADD DichVuCoKetQuaLau bit null;
GO
UPDATE CauHinh
Set [Value] = '2.1.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'