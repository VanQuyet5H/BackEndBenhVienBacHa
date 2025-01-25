ALTER TABLE NoiTruChiDinhDuocPham
ADD LoaiNoiChiDinh INT NULL;
GO

ALTER TABLE YeuCauVatTuBenhVien
ADD LoaiNoiChiDinh INT NULL;
GO

select * from YeuCauDuocPhamBenhVien where YeuCauDichVuKyThuatId is null and YeuCauTiepNhanId in (select Id from YeuCauTiepNhan where LoaiYeuCauTiepNhan=2);
update b set b.LoaiNoiChiDinh=1 
from YeuCauDuocPhamBenhVien a inner join NoiTruChiDinhDuocPham b on a.NoiTruChiDinhDuocPhamId=b.Id
 where a.YeuCauDichVuKyThuatId is null and a.YeuCauTiepNhanId in (select Id from YeuCauTiepNhan where LoaiYeuCauTiepNhan=2);
 
update a set a.LoaiNoiChiDinh=1 
from YeuCauVatTuBenhVien a inner join YeuCauDichVuKyThuat b on a.YeuCauDichVuKyThuatId=b.Id
where YeuCauDichVuKyThuatId is not null and a.YeuCauTiepNhanId in (select Id from YeuCauTiepNhan where LoaiYeuCauTiepNhan=2)
and b.DichVuKyThuatBenhVienId not in (select id from DichVuKyThuatBenhVien where NhomDichVuBenhVienId in (select Id from  NhomDichVuBenhVien where Ten like N'%Phẫu thuật%' or Ten like N'%Thủ thuật%'));
Update dbo.CauHinh
Set [Value] = '2.3.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'