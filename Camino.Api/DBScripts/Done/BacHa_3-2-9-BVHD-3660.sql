ALTER TABLE YeuCauTraDuocPhamTuBenhNhan
ADD ThoiDiemHoanTraTongHopTuNgay DateTime Null
ALTER TABLE YeuCauTraDuocPhamTuBenhNhan
ADD ThoiDiemHoanTraTongHopDenNgay DateTime  Null

ALTER TABLE YeuCauTraVatTuTuBenhNhan
ADD ThoiDiemHoanTraTongHopTuNgay DateTime Null
ALTER TABLE YeuCauTraVatTuTuBenhNhan
ADD ThoiDiemHoanTraTongHopDenNgay DateTime  Null

Update dbo.CauHinh
Set [Value] = '3.2.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'