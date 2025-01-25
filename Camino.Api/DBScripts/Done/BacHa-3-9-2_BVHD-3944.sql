ALTER TABLE GoiKhamSucKhoeDichVuDichVuKyThuat ADD DonGiaThucTe decimal(15, 2) default 0 not null
ALTER TABLE GoiKhamSucKhoeDichVuKhamBenh ADD DonGiaThucTe decimal(15, 2) default 0 not null
ALTER TABLE GoiKhamSucKhoeChungDichVuKhamBenh ADD DonGiaThucTe decimal(15, 2) default 0 not null
ALTER TABLE GoiKhamSucKhoeChungDichVuDichVuKyThuat ADD DonGiaThucTe decimal(15, 2) default 0 not null
ALTER TABLE HopDongKhamSucKhoe ADD GiaTriThucTe decimal(15, 2) null
ALTER TABLE HopDongKhamSucKhoe ALTER COLUMN NgayKetThuc datetime NULL

Update dbo.CauHinh
Set [Value] = '3.4.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'