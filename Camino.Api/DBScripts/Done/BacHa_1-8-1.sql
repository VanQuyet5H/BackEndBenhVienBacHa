ALTER TABLE DichVuKyThuatBenhVien ADD LoaiPhauThuatThuThuat NVARCHAR(50) NULL
GO
Update CauHinh
Set [Value] = '1.8.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'