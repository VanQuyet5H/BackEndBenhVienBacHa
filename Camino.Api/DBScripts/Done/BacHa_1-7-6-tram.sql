alter table YeuCauDichVuKyThuatTuongTrinhPTTT add ThoiGianBatDauGayMe datetime null;
alter table YeuCauDichVuKyThuatTuongTrinhPTTT add ThoiDiemKetThucPhauThuat datetime null;
GO
Update CauHinh
Set [Value] = '1.7.6' where [Name] = 'CauHinhHeThong.DatabaseVesion'