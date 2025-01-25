CREATE TABLE [dbo].[TaiKhoanBenhNhanHuyDichVu](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TaiKhoanBenhNhanId] [bigint] NOT NULL,
	[YeuCauTiepNhanId] [bigint] NOT NULL,	
	[LyDoHuy] [nvarchar](1000) NULL,
	[NgayHuy] [datetime] NOT NULL,
	[NhanVienThucHienId] [bigint] NOT NULL,
	[NoiThucHienId] [bigint] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TaiKhoanBenhNhanHuyDichVu] WITH CHECK ADD  CONSTRAINT [FK_TaiKhoanBenhNhanHuyDichVu_TaiKhoanBenhNhan] FOREIGN KEY([TaiKhoanBenhNhanId])
REFERENCES [dbo].[TaiKhoanBenhNhan] ([Id])
GO

ALTER TABLE [dbo].[TaiKhoanBenhNhanHuyDichVu] CHECK CONSTRAINT [FK_TaiKhoanBenhNhanHuyDichVu_TaiKhoanBenhNhan]
GO

ALTER TABLE [dbo].[TaiKhoanBenhNhanHuyDichVu] WITH CHECK ADD  CONSTRAINT [FK_TaiKhoanBenhNhanHuyDichVu_YeuCauTiepNhan] FOREIGN KEY([YeuCauTiepNhanId])
REFERENCES [dbo].[YeuCauTiepNhan] ([Id])
GO

ALTER TABLE [dbo].[TaiKhoanBenhNhanHuyDichVu] CHECK CONSTRAINT [FK_TaiKhoanBenhNhanHuyDichVu_YeuCauTiepNhan]
GO

ALTER TABLE [TaiKhoanBenhNhanThu]
ADD 	
	[TaiKhoanBenhNhanHuyDichVuId] bigint NULL;
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanThu]  WITH CHECK ADD  CONSTRAINT [FK_TaiKhoanBenhNhanThu_TaiKhoanBenhNhanHuyDichVu] FOREIGN KEY([TaiKhoanBenhNhanHuyDichVuId])
REFERENCES [dbo].[TaiKhoanBenhNhanHuyDichVu] ([Id])
GO

ALTER TABLE [dbo].[TaiKhoanBenhNhanThu] CHECK CONSTRAINT [FK_TaiKhoanBenhNhanThu_TaiKhoanBenhNhanHuyDichVu]
GO

CREATE TABLE [dbo].[NhomDichVuBenhVien](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ma] [nvarchar](50) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[MoTa] [nvarchar](2000) NULL,
	[IsDefault] [bit] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

Update CauHinh
Set [Value] = '0.2.7' where [Name] = 'CauHinhHeThong.DatabaseVesion'