ALTER TABLE YeuCauDuocPhamBenhVien
ADD NguoiDanhDauKhongBuId BIGINT NULL
GO
ALTER TABLE YeuCauDuocPhamBenhVien
ADD NgayDanhDauKhongBu DateTime NULL
GO
ALTER TABLE YeuCauVatTuBenhVien
ADD NguoiDanhDauKhongBuId BIGINT NULL
GO
ALTER TABLE YeuCauVatTuBenhVien
ADD NgayDanhDauKhongBu DateTime NULL
GO
Update dbo.CauHinh
Set [Value] = '3.0.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'