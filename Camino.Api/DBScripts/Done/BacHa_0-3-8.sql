ALTER TABLE HoatDongGiuongBenh
ADD TinhTrangBenhNhan int NULL
GO


Update CauHinh
Set [Value] = '0.3.8' where [Name] = 'CauHinhHeThong.DatabaseVesion'
