ALTER TABLE YeuCauDichVuKyThuatTuongTrinhPTTT
ADD GhiChuCaPTTT NVARCHAR(4000) NULL

Go
Update dbo.CauHinh
Set [Value] = '3.6.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'