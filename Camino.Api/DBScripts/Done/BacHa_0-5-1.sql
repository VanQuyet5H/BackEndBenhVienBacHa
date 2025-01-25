ALTER TABLE [PhongBenhVienHangDoi]
DROP COLUMN [LoaiHangDoi];

GO
Update CauHinh
Set [Value] = '0.5.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'