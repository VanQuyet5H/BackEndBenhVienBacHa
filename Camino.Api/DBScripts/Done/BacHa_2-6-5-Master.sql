update [YeuCauDichVuKyThuat] set DataKetQuaCanLamSang=REPLACE(DataKetQuaCanLamSang,']}',CONCAT('],"NguoiLuuId":',NhanVienKetLuanId,',"NguoiLuuTen":"',(select top 1 HoTen from [User] where Id=NhanVienKetLuanId),'","ThoiDiemLuu":"',CONVERT(varchar,ThoiDiemThucHien,21),'"}'))
where DataKetQuaCanLamSang is not null and (LoaiDichVuKyThuat=3 or LoaiDichVuKyThuat=4)
Update dbo.CauHinh
Set [Value] = '2.6.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'