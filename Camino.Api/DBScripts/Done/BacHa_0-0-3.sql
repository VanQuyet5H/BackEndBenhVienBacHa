ALTER TABLE BenhNhanTienSuBenh
DROP CONSTRAINT FK_BenhNhanTienSuBenh_ICD;

Go
ALTER TABLE BenhNhanTienSuBenh
DROP COLUMN ICDId;

Go
Delete from BenhNhanTienSuBenh
Go
ALTER TABLE BenhNhanTienSuBenh
ADD TenBenh nvarchar(200) NOT NULL;

Go
ALTER TABLE YeuCauKhamBenh
ADD GhiChuTrieuChungLamSang nvarchar(4000) NULL;

Go
ALTER TABLE YeuCauKhamBenhICDKhac
ADD GhiChu nvarchar(4000) NULL;

Go
ALTER TABLE YeuCauKhamBenhDonThuocChiTiet
ADD 
	ThoiGianDungSang int NULL,
	ThoiGianDungTrua int NULL,
	ThoiGianDungChieu int NULL,
	ThoiGianDungToi int NULL;
	
Go
ALTER TABLE BenhNhanDiUngThuoc
DROP COLUMN MaHoatChat,HoatChat;

Go
Delete from BenhNhanDiUngThuoc
Go
ALTER TABLE BenhNhanDiUngThuoc
ADD 
	LoaiDiUng int NOT NULL,
	TenDiUng nvarchar(1000) NOT NULL;
	
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HoSoYeuCauTiepNhan](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ma] [nvarchar](50) NULL,
	[Ten] [nvarchar](256) NOT NULL,
	[TenGuid] [nvarchar](256) NOT NULL,
	[KichThuoc] [bigint] NOT NULL,
	[DuongDan] [nvarchar](500) NOT NULL,
	[LoaiTapTin] [int] NOT NULL,
	[YeuCauTiepNhanId] [bigint] NOT NULL,
	[LoaiHoSoYeuCauTiepNhanId] [bigint] NOT NULL,
	[MoTa] [nvarchar](2000) NULL,
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
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LoaiHoSoYeuCauTiepNhan](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[IsDefault] [bit] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[HoSoYeuCauTiepNhan]  WITH CHECK ADD  CONSTRAINT [FK_HoSoYeuCauTiepNhan_LoaiHoSoYeuCauTiepNhan] FOREIGN KEY([LoaiHoSoYeuCauTiepNhanId])
REFERENCES [dbo].[LoaiHoSoYeuCauTiepNhan] ([Id])
GO
ALTER TABLE [dbo].[HoSoYeuCauTiepNhan] CHECK CONSTRAINT [FK_HoSoYeuCauTiepNhan_LoaiHoSoYeuCauTiepNhan]
GO
ALTER TABLE [dbo].[HoSoYeuCauTiepNhan]  WITH CHECK ADD  CONSTRAINT [FK_HoSoYeuCauTiepNhan_YeuCauTiepNhan] FOREIGN KEY([YeuCauTiepNhanId])
REFERENCES [dbo].[YeuCauTiepNhan] ([Id])
GO
ALTER TABLE [dbo].[HoSoYeuCauTiepNhan] CHECK CONSTRAINT [FK_HoSoYeuCauTiepNhan_YeuCauTiepNhan]
GO
Update CauHinh
Set [Value] = '0.0.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'