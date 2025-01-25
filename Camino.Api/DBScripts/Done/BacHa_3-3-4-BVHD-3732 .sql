ALTER TABLE YeuCauKhamBenh
ADD DuongThaiNguoiInId BIGINT NULL
GO

ALTER TABLE YeuCauKhamBenh
ADD DuongThaiNgayIn DateTime NULL
GO

CREATE TABLE [dbo].[NhanVienChucVu](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[NhanVienId] [bigint] NOT NULL,
	[ChucVuId] [bigint] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_NhanVienChucVu] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[NhanVienChucVu]  WITH CHECK ADD  CONSTRAINT [FK_NhanVienChucVu_ChucVu] FOREIGN KEY([ChucVuId])
REFERENCES [dbo].[ChucVu] ([Id])
GO

ALTER TABLE [dbo].[NhanVienChucVu] CHECK CONSTRAINT [FK_NhanVienChucVu_ChucVu]
GO

ALTER TABLE [dbo].[NhanVienChucVu]  WITH CHECK ADD  CONSTRAINT [FK_NhanVienChucVu_NhanVien] FOREIGN KEY([NhanVienId])
REFERENCES [dbo].[NhanVien] ([Id])
GO

ALTER TABLE [dbo].[NhanVienChucVu] CHECK CONSTRAINT [FK_NhanVienChucVu_NhanVien]
GO

CREATE TABLE [dbo].[HoSoNhanVienFileDinhKem](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[NhanVienId] [bigint] NOT NULL,
	[Ma] [nvarchar](50) NULL,
	[Ten] [nvarchar](256) NOT NULL,
	[TenGuid] [nvarchar](256) NOT NULL,
	[KichThuoc] [bigint] NOT NULL,
	[DuongDan] [nvarchar](500) NOT NULL,
	[LoaiTapTin] [int] NOT NULL,
	[MoTa] [nvarchar](2000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_HoSoNhanVienFileDinhKem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[HoSoNhanVienFileDinhKem]  WITH CHECK ADD  CONSTRAINT [FK_HoSoNhanVienFileDinhKem_NhanVien] FOREIGN KEY([NhanVienId])
REFERENCES [dbo].[NhanVien] ([Id])
GO

ALTER TABLE [dbo].[HoSoNhanVienFileDinhKem] CHECK CONSTRAINT [FK_HoSoNhanVienFileDinhKem_NhanVien]
GO


Update dbo.CauHinh
Set [Value] = '3.3.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'