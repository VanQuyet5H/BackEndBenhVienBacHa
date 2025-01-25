alter table YeuCauDichVuKyThuat add KhongTinhPhi BIT null;

GO
Update CauHinh
Set [Value] = '1.7.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'