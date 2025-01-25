ALTER TABLE DuocPhamBenhVien
ADD TiLeThanhToanBHYT NVARCHAR(MAX) NULL
Go
ALTER TABLE VatTuBenhVien
ADD TiLeThanhToanBHYT NVARCHAR(MAX) NULL
Go
ALTER TABLE DichVuKyThuatBenhVien
ADD TiLeThanhToanBHYT NVARCHAR(MAX) NULL
Go
ALTER TABLE DichVuGiuongBenhVien
ADD TiLeThanhToanBHYT NVARCHAR(MAX) NULL


Go
Update dbo.CauHinh
Set [Value] = '3.8.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'