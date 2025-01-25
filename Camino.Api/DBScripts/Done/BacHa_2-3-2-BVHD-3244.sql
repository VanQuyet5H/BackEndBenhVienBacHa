CREATE TABLE [dbo].[NoiTruDonThuoc]
(
[Id] [bigint] NOT NULL IDENTITY(1, 1),
[YeuCauTiepNhanId] [bigint] NOT NULL,
[ToaThuocMauId] [bigint] NULL,
[LoaiDonThuoc] [int] NOT NULL,
[TrangThai] [int] NOT NULL,
[BacSiKeDonId] [bigint] NOT NULL,
[NoiKeDonId] [bigint] NOT NULL,
[ThoiDiemKeDon] [datetime] NOT NULL,
[GhiChu] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
[CreatedById] [bigint] NOT NULL,
[LastUserId] [bigint] NOT NULL,
[LastTime] [datetime] NOT NULL,
[CreatedOn] [datetime] NOT NULL,
[LastModified] [timestamp] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[NoiTruDonThuocChiTiet]
(
[Id] [bigint] NOT NULL IDENTITY(1, 1),
[NoiTruDonThuocId] [bigint] NOT NULL,
[DuocPhamId] [bigint] NOT NULL,
[LaDuocPhamBenhVien] [bit] NOT NULL,
[Ten] [nvarchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AI NOT NULL,
[TenTiengAnh] [nvarchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
[SoDangKy] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
[STTHoatChat] [int] NULL,
[MaHoatChat] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
[HoatChat] [nvarchar] (550) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
[LoaiThuocHoacHoatChat] [int] NOT NULL,
[NhaSanXuat] [nvarchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
[NuocSanXuat] [nvarchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
[DuongDungId] [bigint] NOT NULL,
[HamLuong] [nvarchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
[QuyCach] [nvarchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
[TieuChuan] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
[DangBaoChe] [nvarchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
[DonViTinhId] [bigint] NOT NULL,
[HuongDan] [nvarchar] (4000) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
[MoTa] [nvarchar] (4000) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
[ChiDinh] [nvarchar] (4000) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
[ChongChiDinh] [nvarchar] (4000) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
[LieuLuongCachDung] [nvarchar] (4000) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
[TacDungPhu] [nvarchar] (4000) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
[ChuYDePhong] [nvarchar] (4000) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
[SoLuong] [float] NOT NULL,
[SoNgayDung] [int] NULL,
[DungSang] [float] NULL,
[DungTrua] [float] NULL,
[DungChieu] [float] NULL,
[DungToi] [float] NULL,
[DuocHuongBaoHiem] [bit] NOT NULL,
[BenhNhanMuaNgoai] [bit] NOT NULL,
[GhiChu] [nvarchar] (1000) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
[CreatedById] [bigint] NOT NULL,
[LastUserId] [bigint] NOT NULL,
[LastTime] [datetime] NOT NULL,
[CreatedOn] [datetime] NOT NULL,
[LastModified] [timestamp] NOT NULL,
[ThoiGianDungSang] [int] NULL,
[ThoiGianDungTrua] [int] NULL,
[ThoiGianDungChieu] [int] NULL,
[ThoiGianDungToi] [int] NULL
) ON [PRIMARY]
GO


ALTER TABLE dbo.DonThuocThanhToan
  ADD NoiTruDonThuocId BIGINT NULL;
GO

ALTER TABLE dbo.DonThuocThanhToanChiTiet
  ADD NoiTruDonThuocChiTietId BIGINT NULL;
GO
Update dbo.CauHinh
Set [Value] = '2.3.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'