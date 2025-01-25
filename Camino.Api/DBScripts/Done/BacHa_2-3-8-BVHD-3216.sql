ALTER TABLE YeuCauLinhDuocPham
ADD ThoiDiemLinhTongHopTuNgay DateTime Null
ALTER TABLE YeuCauLinhDuocPham
ADD ThoiDiemLinhTongHopDenNgay DateTime  Null

ALTER TABLE YeuCauLinhVatTu
ADD ThoiDiemLinhTongHopTuNgay DateTime Null
ALTER TABLE YeuCauLinhVatTu
ADD ThoiDiemLinhTongHopDenNgay DateTime  Null
Update dbo.CauHinh
Set [Value] = '2.3.8' where [Name] = 'CauHinhHeThong.DatabaseVesion'