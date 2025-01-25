UPDATE dbo.DichVuKhamBenhBenhVien
SET     ChuyenKhoaKhamSucKhoe = 1
WHERE Id = 101
GO
UPDATE dbo.DichVuKhamBenhBenhVien
SET     ChuyenKhoaKhamSucKhoe = 2
WHERE Id = 102
GO
UPDATE dbo.DichVuKhamBenhBenhVien
SET     ChuyenKhoaKhamSucKhoe = 3
WHERE Id = 103
GO
UPDATE dbo.DichVuKhamBenhBenhVien
SET     ChuyenKhoaKhamSucKhoe = 4
WHERE Id = 105
GO
UPDATE dbo.DichVuKhamBenhBenhVien
SET     ChuyenKhoaKhamSucKhoe = 5
WHERE Id = 107
GO
UPDATE dbo.DichVuKhamBenhBenhVien
SET     ChuyenKhoaKhamSucKhoe = 6
WHERE Id = 106
GO
UPDATE dbo.DichVuKhamBenhBenhVien
SET     ChuyenKhoaKhamSucKhoe = 7
WHERE Id = 108
GO
UPDATE CauHinh
Set [Value] = '2.0.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'