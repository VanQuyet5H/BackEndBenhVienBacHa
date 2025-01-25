ALTER TABLE [NoiGioiThieu]
ADD 
	[SoDienThoai] nvarchar(12) NULL,
	[DonVi] nvarchar(250) NULL,
	[NhanVienQuanLyId] [bigint] NULL;
GO
ALTER TABLE [dbo].[NoiGioiThieu]  WITH CHECK ADD  CONSTRAINT [FK_NoiGioiThieu_NhanVien] FOREIGN KEY([NhanVienQuanLyId])
REFERENCES [dbo].[NhanVien] ([Id])
GO

ALTER TABLE [dbo].[NoiGioiThieu] CHECK CONSTRAINT [FK_NoiGioiThieu_NhanVien]
GO

ALTER TABLE [HinhThucDen]
ADD 
	[IsDefault] bit NULL;
GO

UPDATE [HinhThucDen] set [IsDefault] = 0
UPDATE [HinhThucDen] set [IsDefault] = 1, [Ten] = N'Tự đến' WHERE [Id] = 1
UPDATE [HinhThucDen] set [IsDefault] = 1, [Ten] = N'Giới thiệu' WHERE [Id] = 2

ALTER TABLE [HinhThucDen]
ALTER COLUMN [IsDefault] bit NOT NULL;
GO

Go
ALTER TABLE [BenhNhan]
ALTER COLUMN HoTen NVARCHAR(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL

GO
Update CauHinh
Set [Value] = '0.2.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'