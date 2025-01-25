ALTER TABLE dbo.HopDongThauDuocPham
   ADD HeThongTuPhatSinh BIT NULL;

ALTER TABLE dbo.HopDongThauVatTu
   ADD HeThongTuPhatSinh BIT NULL;


ALTER TABLE dbo.YeuCauKhamBenhDonThuoc
   ALTER COLUMN GhiChu NVARCHAR(MAX) NULL ;

Update CauHinh
Set [Value] = '1.0.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'
