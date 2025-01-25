sp_rename 'YeuCauDichVuKyThuatTuongTrinhPTTT.ICDTruocPhauThuat', 'ICDTruocPhauThuatId', 'COLUMN';
GO
sp_rename 'YeuCauDichVuKyThuatTuongTrinhPTTT.ICDSauPhauThuat', 'ICDSauPhauThuatId', 'COLUMN';

GO
Update CauHinh
Set [Value] = '0.3.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'