BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.CongTyBaoHiemTuNhanCongNo
	DROP CONSTRAINT FK_CongTyBaoHiemTuNhanCongNo_YeuCauDichVuGiuongBenhVienChiPhiBenhVien
GO
ALTER TABLE dbo.MienGiamChiPhi
	DROP CONSTRAINT FK_MienGiamChiPhi_YeuCauDichVuGiuongBenhVienChiPhiBenhVien
GO
ALTER TABLE dbo.TaiKhoanBenhNhanChi
	DROP CONSTRAINT FK_TaiKhoanBenhNhanChi_YeuCauDichVuGiuongBenhVienChiPhiBenhVien
GO
ALTER TABLE dbo.YeuCauDichVuGiuongBenhVienChiPhiBenhVien SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.TaiKhoanBenhNhanChi ADD CONSTRAINT
	FK_TaiKhoanBenhNhanChi_YeuCauDichVuGiuongBenhVienChiPhiBenhVien FOREIGN KEY
	(
	YeuCauDichVuGiuongBenhVienChiPhiBenhVienId
	) REFERENCES dbo.YeuCauDichVuGiuongBenhVienChiPhiBenhVien
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  SET NULL 
	
GO
ALTER TABLE dbo.TaiKhoanBenhNhanChi SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.MienGiamChiPhi ADD CONSTRAINT
	FK_MienGiamChiPhi_YeuCauDichVuGiuongBenhVienChiPhiBenhVien FOREIGN KEY
	(
	YeuCauDichVuGiuongBenhVienChiPhiBenhVienId
	) REFERENCES dbo.YeuCauDichVuGiuongBenhVienChiPhiBenhVien
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE dbo.MienGiamChiPhi SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.CongTyBaoHiemTuNhanCongNo ADD CONSTRAINT
	FK_CongTyBaoHiemTuNhanCongNo_YeuCauDichVuGiuongBenhVienChiPhiBenhVien FOREIGN KEY
	(
	YeuCauDichVuGiuongBenhVienChiPhiBenhVienId
	) REFERENCES dbo.YeuCauDichVuGiuongBenhVienChiPhiBenhVien
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE dbo.CongTyBaoHiemTuNhanCongNo SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
Update dbo.CauHinh
Set [Value] = '2.3.7' where [Name] = 'CauHinhHeThong.DatabaseVesion'