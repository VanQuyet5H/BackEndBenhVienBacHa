
-- Câu lệnh dùng để kiểm tra data đang bị lỗi data
--select tc.YeuCauGoiDichVuId, tc.TrangThaiThanhToan, tc.ThoiDiemChiDinh, tc.NoiTruPhieuDieuTriId, sl.NoiTruPhieuDieuTriId, 
--		tc.NoiTruPhieuDieuTriId, tc.YeuCauDichVuKyThuatKhamSangLocTiemChungId, * 
--from YeuCauDichVuKyThuat tc
--left join YeuCauDichVuKyThuat sl on tc.YeuCauDichVuKyThuatKhamSangLocTiemChungId = sl.Id
--where tc.YeuCauDichVuKyThuatKhamSangLocTiemChungId 
--	in (select Id from YeuCauDichVuKyThuat where NhomDichVuBenhVienId = 300 and TrangThai <> 4 and NoiTruPhieuDieuTriId IS NOT NULL)
--and tc.NoiTruPhieuDieuTriId is null


-- Câu lệnh dùng để update
UPDATE tc
SET tc.NoiTruPhieuDieuTriId = sl.NoiTruPhieuDieuTriId
FROM YeuCauDichVuKyThuat tc
LEFT JOIN YeuCauDichVuKyThuat sl on tc.YeuCauDichVuKyThuatKhamSangLocTiemChungId = sl.Id
WHERE tc.YeuCauDichVuKyThuatKhamSangLocTiemChungId 
	in (select Id from YeuCauDichVuKyThuat where NhomDichVuBenhVienId = 300 and TrangThai <> 4 and NoiTruPhieuDieuTriId IS NOT NULL)
and tc.NoiTruPhieuDieuTriId is null
GO

Update dbo.CauHinh
Set [Value] = '3.1.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'