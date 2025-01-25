ALTER TABLE [dbo].[CongTyBaoHiemTuNhanCongNo]
	ALTER COLUMN [TaiKhoanBenhNhanThuId]                      BIGINT          NULL;
	
GO
ALTER TABLE [dbo].[MienGiamChiPhi]
	ALTER COLUMN [TaiKhoanBenhNhanThuId]                      BIGINT          NULL;
	
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanThu]
	ADD 
		[SoPhieuHienThi]                                    NVARCHAR (20)   NULL,
		[DaHuy]                                             BIT             NULL,
		[NhanVienHuyId]                                     BIGINT          NULL,
		[NgayHuy]                                           DATETIME        NULL,
		[NoiHuyId]                                          BIGINT          NULL,
		[LyDoHuy]                                           NVARCHAR (1000) NULL,
		[PhieuHoanUngId]                                    BIGINT          NULL,
		[TamUng]                                            DECIMAL (15, 2) NULL,
		[DaThuNo]                                           BIT             NULL,
		[ThuNoPhieuThuId]                                   BIGINT          NULL,
		CONSTRAINT [FK_TaiKhoanBenhNhanThu_NhanVien] FOREIGN KEY ([NhanVienHuyId]) REFERENCES [dbo].[NhanVien] ([Id]),
		CONSTRAINT [FK_TaiKhoanBenhNhanThu_TaiKhoanBenhNhanChi] FOREIGN KEY ([PhieuHoanUngId]) REFERENCES [dbo].[TaiKhoanBenhNhanChi] ([Id]),
		CONSTRAINT [FK_TaiKhoanBenhNhanThu_TaiKhoanBenhNhanThu] FOREIGN KEY ([ThuNoPhieuThuId]) REFERENCES [dbo].[TaiKhoanBenhNhanThu] ([Id]);
		
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanChi]
	ADD 
		[SoPhieuHienThi]                             NVARCHAR (20)   NULL,
		[Gia]                                        DECIMAL (15, 2) NULL,
		[SoLuong]                                    FLOAT (53)      NULL,
		[DonGiaBaoHiem]                              DECIMAL (15, 2) NULL,
		[MucHuongBaoHiem]                            INT             NULL,
		[TiLeBaoHiemThanhToan]                       INT             NULL,
		[SoTienBaoHiemTuNhanChiTra]                  DECIMAL (15, 2) NULL,
		[SoTienMienGiam]                             DECIMAL (15, 2) NULL,
		[DaHuy]                                      BIT             NULL,
		[PhieuThanhToanChiPhiId]                     BIGINT          NULL,
		CONSTRAINT [FK_TaiKhoanBenhNhanChi_TaiKhoanBenhNhanChi1] FOREIGN KEY ([PhieuThanhToanChiPhiId]) REFERENCES [dbo].[TaiKhoanBenhNhanChi] ([Id]);
		
GO
ALTER TABLE [dbo].[YeuCauDichVuGiuongBenhVienChiPhiBHYT]
	ADD 
		[ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVienId]	BIGINT          NULL,
		CONSTRAINT [FK_YeuCauDichVuGiuongBenhVienChiPhiBHYT_YeuCauDichVuGiuongBenhVienChiPhiBenhVien] FOREIGN KEY ([ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVienId]) REFERENCES [dbo].[YeuCauDichVuGiuongBenhVienChiPhiBenhVien] ([Id]);

GO
Update dbo.CauHinh
Set [Value] = '1.4.8' where [Name] = 'CauHinhHeThong.DatabaseVesion'