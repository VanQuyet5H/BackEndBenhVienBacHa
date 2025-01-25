ALTER TABLE [dbo].[TaiKhoanBenhNhanThu]
	ADD 
		[DaThuHoi]                                             BIT             NULL,
		[NhanVienThuHoiId]                                     BIGINT          NULL,
		[NgayThuHoi]                                           DATETIME        NULL,		
		CONSTRAINT [FK_TaiKhoanBenhNhanThu_NhanVienThuHoi] FOREIGN KEY ([NhanVienThuHoiId]) REFERENCES [dbo].[NhanVien] ([Id]);
		
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanChi]
	ADD 
		[DaThuHoi]                                             BIT             NULL,
		[NhanVienThuHoiId]                                     BIGINT          NULL,
		[NgayThuHoi]                                           DATETIME        NULL,
		[NhanVienHuyId] BIGINT NULL,
		[NgayHuy] DATETIME NULL,
		CONSTRAINT [FK_TaiKhoanBenhNhanChi_NhanVienThuHoi] FOREIGN KEY ([NhanVienThuHoiId]) REFERENCES [dbo].[NhanVien] ([Id]);
		
GO
Update dbo.CauHinh
Set [Value] = '1.5.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'


