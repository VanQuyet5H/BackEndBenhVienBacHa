-- cập nhật trạng thái các YC dược phẩm (trực tiếp) đã xuất mà chưa chuyển trạng thái qua đã thực hiện
UPDATE yc
set TrangThai = 2
from YeuCauDuocPhamBenhVien yc
left join XuatKhoDuocPhamChiTiet x on yc.XuatKhoDuocPhamChiTietId = x.Id
where YeuCauLinhDuocPhamId in (select id from YeuCauLinhDuocPham where loaiphieulinh = 3)
and yc.TrangThai = 1
and yc.XuatKhoDuocPhamChiTietId is not null
and x.XuatKhoDuocPhamId is not null
GO

-- cập nhật trạng thái các YC vật tư (trực tiếp) đã xuất mà chưa chuyển trạng thái qua đã thực hiện
UPDATE yc
set TrangThai = 2
from YeuCauVatTuBenhVien yc
left join XuatKhoVatTuChiTiet x on yc.XuatKhoVatTuChiTietId = x.Id
where YeuCauLinhVatTuId in (select id from YeuCauLinhVatTu where loaiphieulinh = 3 and DuocDuyet = 1)
and yc.TrangThai = 1
and yc.XuatKhoVatTuChiTietId is not null
and x.XuatKhoVatTuId is not null

Go
Update dbo.CauHinh
Set [Value] = '2.4.7' where [Name] = 'CauHinhHeThong.DatabaseVesion'