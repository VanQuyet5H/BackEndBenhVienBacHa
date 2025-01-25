ALTER TABLE ToaThuocMauChiTiet
ADD 
	ThoiGianDungSang int NULL,
	ThoiGianDungTrua int NULL,
	ThoiGianDungChieu int NULL,
	ThoiGianDungToi int NULL;
	
Go
Update CauHinh
Set [Value] = '0.0.6' where [Name] = 'CauHinhHeThong.DatabaseVesion'