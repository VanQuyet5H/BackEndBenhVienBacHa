
CREATE TABLE [dbo].[NoiTruPhieuDieuTriTuVanThuoc](
	[Id] [BIGINT] IDENTITY(1,1) NOT NULL,
	[YeuCauTiepNhanId] [BIGINT] NOT NULL,
	[NoiTruPhieuDieuTriId] [BIGINT] NOT NULL,
	[DuocPhamId] [BIGINT] NOT NULL,
	[LaDuocPhamBenhVien] [BIT] NOT NULL,
	[Ten] [NVARCHAR](250) NOT NULL,
	[TenTiengAnh] [NVARCHAR](250) NULL,
	[SoDangKy] [NVARCHAR](100) NOT NULL,
	[STTHoatChat] [INT] NULL,
	[MaHoatChat] [NVARCHAR](20) NOT NULL,
	[HoatChat] [NVARCHAR](500) NOT NULL,
	[LoaiThuocHoacHoatChat] [INT] NOT NULL,
	[NhaSanXuat] [NVARCHAR](250) NULL,
	[NuocSanXuat] [NVARCHAR](250) NULL,
	[DuongDungId] [BIGINT] NOT NULL,
	[HamLuong] [NVARCHAR](500) NULL,
	[QuyCach] [NVARCHAR](250) NULL,
	[TieuChuan] [NVARCHAR](50) NULL,
	[DangBaoChe] [NVARCHAR](250) NULL,
	[DonViTinhId] [BIGINT] NOT NULL,
	[HuongDan] [NVARCHAR](4000) NULL,
	[MoTa] [NVARCHAR](4000) NULL,
	[ChiDinh] [NVARCHAR](4000) NULL,
	[ChongChiDinh] [NVARCHAR](4000) NULL,
	[LieuLuongCachDung] [NVARCHAR](4000) NULL,
	[TacDungPhu] [NVARCHAR](4000) NULL,
	[ChuYDePhong] [NVARCHAR](4000) NULL,
	[SoLuong] [FLOAT] NOT NULL,
	[SoNgayDung] [INT] NULL,
	[DungSang] [FLOAT] NULL,
	[DungTrua] [FLOAT] NULL,
	[DungChieu] [FLOAT] NULL,
	[DungToi] [FLOAT] NULL,
	[ThoiGianDungSang] [INT] NULL,
	[ThoiGianDungTrua] [INT] NULL,
	[ThoiGianDungChieu] [INT] NULL,
	[ThoiGianDungToi] [INT] NULL,
	[GhiChu] [NVARCHAR](1000) NULL,
	[CreatedById] [BIGINT] NOT NULL,
	[LastUserId] [BIGINT] NOT NULL,
	[LastTime] [DATETIME] NOT NULL,
	[CreatedOn] [DATETIME] NOT NULL,
	[LastModified] [TIMESTAMP] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[NoiTruPhieuDieuTriTuVanThuoc]  WITH CHECK ADD  CONSTRAINT [FK_NoiTruPhieuDieuTriTuVanThuoc_YeuCauTiepNhan] FOREIGN KEY([YeuCauTiepNhanId])
REFERENCES [dbo].[YeuCauTiepNhan] ([Id])
GO

ALTER TABLE [dbo].[NoiTruPhieuDieuTriTuVanThuoc] CHECK CONSTRAINT [FK_NoiTruPhieuDieuTriTuVanThuoc_YeuCauTiepNhan]
GO

ALTER TABLE [dbo].[NoiTruPhieuDieuTriTuVanThuoc]  WITH CHECK ADD  CONSTRAINT [FK_NoiTruPhieuDieuTriTuVanThuoc_NoiTruPhieuDieuTri] FOREIGN KEY([NoiTruPhieuDieuTriId])
REFERENCES [dbo].[NoiTruPhieuDieuTri] ([Id])
GO

ALTER TABLE [dbo].[NoiTruPhieuDieuTriTuVanThuoc] CHECK CONSTRAINT [FK_NoiTruPhieuDieuTriTuVanThuoc_NoiTruPhieuDieuTri]
GO

UPDATE CauHinh
Set [Value] = '2.0.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'