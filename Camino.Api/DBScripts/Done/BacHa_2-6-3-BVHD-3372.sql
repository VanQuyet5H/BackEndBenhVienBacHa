-- cập nhật thời điểm hoàn thành is null
update YeuCauDichVuKyThuat 
set ThoiDiemHoanThanh = LastTime,
	LastTime = GETDATE()
where TrangThai = 3 and ThoiDiemHoanThanh is null

-- cập nhật nhân viên kết luận is null
update YeuCauDichVuKyThuat 
set NhanVienKetLuanId = LastUserId,
	LastTime = GETDATE()
where TrangThai = 3 and NhanVienKetLuanId is null

-- cập nhật thời điểm thực hiện is null
update YeuCauDichVuKyThuat 
set ThoiDiemThucHien = ThoiDiemHoanThanh,
	LastTime = GETDATE()
where TrangThai = 3 and ThoiDiemThucHien is null

-- cập nhật nhân viên thực hiện is null
update YeuCauDichVuKyThuat 
set NhanVienThucHienId = NhanVienKetLuanId,
	LastTime = GETDATE()
where TrangThai = 3 and NhanVienThucHienId is null

GO
Update dbo.CauHinh
Set [Value] = '2.6.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'
