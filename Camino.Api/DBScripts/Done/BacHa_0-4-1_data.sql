update DichVuKhamBenhBenhVien
set DichVuKhamBenhId = (select top 1 id from DichVuKhamBenh kb where kb.MaChung = Ma)
where DichVuKhamBenhId IS NULL

update DichVuGiuongBenhVien
set DichVuGiuongId = (select top 1 id from DichVuGiuong g where g.MaChung = Ma)
where DichVuGiuongId IS NULL

update DichVuKyThuatBenhVien
set DichVuKyThuatId = (select top 1 id from DichVuKyThuat kt where kt.MaChung = Ma)
where DichVuKyThuatId IS NULL

delete from DichVuGiuongBenhVienGiaBaoHiem 
where DichVuGiuongBenhVienId in (select Id from DichVuGiuongBenhVien where DichVuGiuongId IS NULL)

delete from DichVuKhamBenhBenhVienGiaBaoHiem 
where DichVuKhamBenhBenhVienId in (select Id from DichVuKhamBenhBenhVien where DichVuKhamBenhId IS NULL)

delete from DichVuKyThuatBenhVienGiaBaoHiem 
where DichVuKyThuatBenhVienId in (select Id from DichVuKyThuatBenhVien where DichVuKyThuatId IS NULL)