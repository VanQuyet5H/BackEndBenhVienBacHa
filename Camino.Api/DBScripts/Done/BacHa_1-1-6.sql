

ALTER TABLE dbo.KyDuTruMuaDuocPhamVatTu
   ADD NgayBatDauLap DATETIME  NOT NULL;

ALTER TABLE dbo.KyDuTruMuaDuocPhamVatTu
   ADD NgayKetThucLap DATETIME  NOT NULL;

 

Update CauHinh
Set [Value] = '1.1.6' where [Name] = 'CauHinhHeThong.DatabaseVesion'


