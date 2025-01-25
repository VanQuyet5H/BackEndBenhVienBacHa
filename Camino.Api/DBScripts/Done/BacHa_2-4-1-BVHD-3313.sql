ALTER TABLE [YeuCauDichVuGiuongBenhVienChiPhiBenhVien]
ALTER COLUMN [GiuongBenhId] [bigint] NULL;

ALTER TABLE [YeuCauDichVuGiuongBenhVienChiPhiBHYT]
ALTER COLUMN [GiuongBenhId] [bigint] NULL;

GO
Update dbo.CauHinh
Set [Value] = '2.4.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'