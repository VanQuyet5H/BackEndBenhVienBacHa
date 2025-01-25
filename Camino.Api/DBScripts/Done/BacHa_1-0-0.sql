ALTER TABLE dbo.YeuCauLinhDuocPhamChiTiet
   ADD SoLuongDaLinhBu FLOAT NULL;

ALTER TABLE dbo.YeuCauLinhDuocPhamChiTiet
   ADD SoLuongCanBu FLOAT NULL;


ALTER TABLE dbo.YeuCauLinhVatTuChiTiet
   ADD SoLuongDaLinhBu FLOAT NULL;

ALTER TABLE dbo.YeuCauLinhVatTuChiTiet
   ADD SoLuongCanBu FLOAT NULL;

Update CauHinh
Set [Value] = '1.0.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'

