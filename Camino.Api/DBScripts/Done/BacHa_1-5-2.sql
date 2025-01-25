ALTER TABLE [dbo].[TaiKhoanBenhNhanChi]
	ADD 
		[NoiHuyId]                                     BIGINT          NULL,
		[LyDoHuy]                                      NVARCHAR(1000)  NULL,
		CONSTRAINT [FK_TaiKhoanBenhNhanChi_NoiHuy] FOREIGN KEY ([NoiHuyId]) REFERENCES [dbo].[PhongBenhVien] ([Id]);
		
GO
ALTER TABLE [dbo].[YeuCauDichVuGiuongBenhVienChiPhiBenhVien]
	ADD 
		[GhiChuMienGiamThem]                                      NVARCHAR(1000)  NULL;
		
GO
Update dbo.CauHinh
Set [Value] = '1.5.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'


