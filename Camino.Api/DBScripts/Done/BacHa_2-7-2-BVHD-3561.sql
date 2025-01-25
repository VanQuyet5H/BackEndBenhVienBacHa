ALTER TABLE [dbo].[NoiTruChiDinhDuocPham]
	ADD [NoiTruChiDinhPhaThuocTiemId] BIGINT NULL
GO

ALTER TABLE [dbo].[NoiTruChiDinhDuocPham]
	ADD [NoiTruChiDinhPhaThuocTruyenId] BIGINT NULL
GO

ALTER TABLE [dbo].[NoiTruChiDinhDuocPham]
	ADD [SoThuTu] INT NULL
GO

ALTER TABLE [dbo].[NoiTruChiDinhDuocPham]
	ADD [SoLanTrenVien] [int] NULL
GO

ALTER TABLE [dbo].[NoiTruChiDinhDuocPham]
 ADD [CachGioDungThuoc] [float] NULL
GO

ALTER TABLE [dbo].[NoiTruChiDinhDuocPham]
	ADD [LieuDungTrenNgay] [float] NULL
GO


CREATE TABLE [dbo].[NoiTruChiDinhPhaThuocTruyen]
(
[Id] [bigint] NOT NULL IDENTITY(1, 1),
[YeuCauTiepNhanId] [bigint] NOT NULL,
[NoiTruBenhAnId] [bigint] NOT NULL,
[NoiTruPhieuDieuTriId] [bigint] NULL,
[NhanVienChiDinhId] [bigint] NOT NULL,
[NoiChiDinhId] [bigint] NOT NULL,
[ThoiDiemChiDinh] [datetime] NOT NULL,
[TocDoTruyen] [int] NULL,
[DonViTocDoTruyen] [int] NULL,
[ThoiGianBatDauTruyen] [int] NULL,
[SoLanTrenNgay] [int] NULL,
[CachGioTruyen] [float] NULL,
[CreatedById] [bigint] NOT NULL,
[LastUserId] [bigint] NOT NULL,
[LastTime] [datetime] NOT NULL,
[CreatedOn] [datetime] NOT NULL,
[LastModified] [timestamp] NOT NULL
) ON [PRIMARY]
GO



ALTER TABLE [dbo].[NoiTruChiDinhPhaThuocTruyen] ADD CONSTRAINT [PK_NoiTruChiDinhPhaThuocTruyen] PRIMARY KEY CLUSTERED ([Id]) ON [PRIMARY]
GO
ALTER TABLE [dbo].[NoiTruChiDinhPhaThuocTruyen] ADD CONSTRAINT [FK_NoiTruChiDinhPhaThuocTruyen_NoiTruBenhAn] FOREIGN KEY ([NoiTruBenhAnId]) REFERENCES [dbo].[NoiTruBenhAn] ([Id])
GO
ALTER TABLE [dbo].[NoiTruChiDinhPhaThuocTruyen] ADD CONSTRAINT [FK_NoiTruChiDinhPhaThuocTruyen_NhanVien] FOREIGN KEY ([NhanVienChiDinhId]) REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[NoiTruChiDinhPhaThuocTruyen] ADD CONSTRAINT [FK_NoiTruChiDinhPhaThuocTruyen_PhongBenhVien] FOREIGN KEY ([NoiChiDinhId]) REFERENCES [dbo].[PhongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[NoiTruChiDinhPhaThuocTruyen] ADD CONSTRAINT [FK_NoiTruChiDinhPhaThuocTruyen_YeuCauTiepNhan] FOREIGN KEY ([YeuCauTiepNhanId]) REFERENCES [dbo].[YeuCauTiepNhan] ([Id])
GO


CREATE TABLE [dbo].[NoiTruChiDinhPhaThuocTiem]
(
[Id] [bigint] NOT NULL IDENTITY(1, 1),
[YeuCauTiepNhanId] [bigint] NOT NULL,
[NoiTruBenhAnId] [bigint] NOT NULL,
[NoiTruPhieuDieuTriId] [bigint] NULL,
[NhanVienChiDinhId] [bigint] NOT NULL,
[NoiChiDinhId] [bigint] NOT NULL,
[ThoiDiemChiDinh] [datetime] NOT NULL,
[ThoiGianBatDauTiem] [int] NULL,
[SoLanTrenMui] [int] NULL,
[SoLanTrenNgay] [int] NULL,
[CachGioTiem] [float] NULL,
[CreatedById] [bigint] NOT NULL,
[LastUserId] [bigint] NOT NULL,
[LastTime] [datetime] NOT NULL,
[CreatedOn] [datetime] NOT NULL,
[LastModified] [timestamp] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[NoiTruChiDinhPhaThuocTiem] ADD CONSTRAINT [PK_NoiTruChiDinhPhaThuocTiem] PRIMARY KEY CLUSTERED ([Id]) ON [PRIMARY]
GO
ALTER TABLE [dbo].[NoiTruChiDinhPhaThuocTiem] ADD CONSTRAINT [FK_NoiTruChiDinhPhaThuocTiem_NoiTruBenhAn] FOREIGN KEY ([NoiTruBenhAnId]) REFERENCES [dbo].[NoiTruBenhAn] ([Id])
GO
ALTER TABLE [dbo].[NoiTruChiDinhPhaThuocTiem] ADD CONSTRAINT [FK_NoiTruChiDinhPhaThuocTiem_NhanVien] FOREIGN KEY ([NhanVienChiDinhId]) REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[NoiTruChiDinhPhaThuocTiem] ADD CONSTRAINT [FK_NoiTruChiDinhPhaThuocTiem_PhongBenhVien] FOREIGN KEY ([NoiChiDinhId]) REFERENCES [dbo].[PhongBenhVien] ([Id])
GO
ALTER TABLE [dbo].[NoiTruChiDinhPhaThuocTiem] ADD CONSTRAINT [FK_NoiTruChiDinhPhaThuocTiem_YeuCauTiepNhan] FOREIGN KEY ([YeuCauTiepNhanId]) REFERENCES [dbo].[YeuCauTiepNhan] ([Id])
GO

Update dbo.CauHinh
Set [Value] = '2.7.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'