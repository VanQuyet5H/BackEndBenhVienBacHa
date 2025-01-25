ALTER TABLE [dbo].[NoiTruBenhAn]
	ADD 
		[DanhSachChanDoanKemTheoRaVienICDId] [nvarchar](500) NULL,
		[DanhSachChanDoanKemTheoRaVienGhiChu] [nvarchar](4000) NULL
GO

Update dbo.CauHinh
Set [Value] = '1.5.7' where [Name] = 'CauHinhHeThong.DatabaseVesion'