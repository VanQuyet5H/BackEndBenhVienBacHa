ALTER TABLE DichVuKyThuatBenhVien 
ADD CoInKetQuaKemHinhAnh BIT NULL
Go
UPDATE DichVuKyThuatBenhVien
SET CoInKetQuaKemHinhAnh = 1
WHERE NhomDichVuBenhVienId IN (select Id from NhomDichVuBenhVien WHERE Ten LIKE N'nội soi%' OR Ten LIKE N'ĐO LOÃNG XƯƠNG')

GO
Update CauHinh
Set [Value] = '1.8.7' where [Name] = 'CauHinhHeThong.DatabaseVesion'