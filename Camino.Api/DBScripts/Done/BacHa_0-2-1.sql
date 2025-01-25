ALTER TABLE BenhNhanDiUngThuoc
ADD MucDo INT NULL;
Go

UPDATE BenhNhanDiUngThuoc SET MucDo = 1
GO

ALTER TABLE BenhNhanDiUngThuoc
ALTER COLUMN MucDo INT NOT NULL;
Go

ALTER TABLE YEUCAUKHAMBENH
ALTER COLUMN KetQuaDieuTri INT NULL;
Go

ALTER TABLE YEUCAUKHAMBENH
ALTER COLUMN TinhTrangRaVien INT NULL;
GO

ALTER TABLE YEUCAUKHAMBENH
ADD 
	KhamToanThan NVARCHAR(4000) NULL,
	TuanHoan NVARCHAR(4000) NULL,
	HoHap NVARCHAR(4000) NULL,
	TieuHoa NVARCHAR(4000) NULL,
	ThanTietNieuSinhDuc NVARCHAR(4000) NULL,
	ThanKinh NVARCHAR(4000) NULL,
	CoXuongKhop NVARCHAR(4000) NULL,
	TaiMuiHong NVARCHAR(4000) NULL,
	RangHamMat NVARCHAR(4000) NULL,
	NoiTietDinhDuong NVARCHAR(4000) NULL,
	SanPhuKhoa NVARCHAR(4000) NULL,
	DaLieu NVARCHAR(4000) NULL,
	ChuanDoanSoBoICD BIGINT NULL,
	ChuanDoanSoBoGhiChu NVARCHAR(4000) NULL
GO

CREATE TABLE [dbo].[YeuCauKhamBenhKhamBoPhanKhac](
	[Id] [bigint] NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[NoiDung] [nvarchar](4000) NOT NULL,
	[YeuCauKhamBenhId] [bigint] NOT NULL,

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

ALTER TABLE [dbo].[YeuCauKhamBenhKhamBoPhanKhac]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhKhamBoPhanKhac_YeuCauKhamBenh] FOREIGN KEY([YeuCauKhamBenhId])
REFERENCES [dbo].[YeuCauKhamBenh] ([Id])
GO

CREATE TABLE [dbo].[YeuCauKhamBenhChanDoanPhanBiet](
	[Id] [bigint] NOT NULL,
	[ICDId] [bigint] NOT NULL,
	[GhiChu] [nvarchar](4000) NULL,
	[YeuCauKhamBenhId] [bigint] NOT NULL,

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

ALTER TABLE [dbo].[YeuCauKhamBenhChanDoanPhanBiet]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhChanDoanPhanBiet_ICD] FOREIGN KEY([ICDId])
REFERENCES [dbo].[ICD] ([Id])
GO
ALTER TABLE [dbo].[YeuCauKhamBenhChanDoanPhanBiet]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhChanDoanPhanBiet_YeuCauKhamBenh] FOREIGN KEY([YeuCauKhamBenhId])
REFERENCES [dbo].[YeuCauKhamBenh] ([Id])
GO

Update CauHinh
Set [Value] = '0.2.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'