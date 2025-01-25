CREATE TABLE [dbo].[TemplateKhamSangLocTiemChung] (
    [Id]                BIGINT         IDENTITY (1, 1) NOT NULL,
    [Ten]               NVARCHAR (100) NOT NULL,
    [TieuDe]            NVARCHAR (500) NULL,
    [ComponentDynamics] NVARCHAR (MAX) NOT NULL,
    [CreatedById]       BIGINT         NOT NULL,
    [LastUserId]        BIGINT         NOT NULL,
    [LastTime]          DATETIME       NOT NULL,
    [CreatedOn]         DATETIME       NOT NULL,
    [LastModified]      ROWVERSION     NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO
CREATE TABLE [dbo].[YeuCauDichVuKyThuatKhamSangLocTiemChung] (
    [Id]                                   BIGINT          NOT NULL,
    [ThongTinKhamSangLocTiemChungTemplate] NVARCHAR (MAX)  NULL,
    [ThongTinKhamSangLocTiemChungData]     NVARCHAR (MAX)  NULL,
    [BenhNhanDeNghi]                       BIT             NOT NULL,
    [LyDoDeNghi]                           NVARCHAR (1000) NULL,
    [KetLuan]                              INT             NULL,
    [GhiChuKetLuan]                        NVARCHAR (1000) NULL,
    [SoNgayHenTiemMuiTiepTheo]             INT             NULL,
    [GhiChuHenTiemMuiTiepTheo]             NVARCHAR (1000) NULL,
    [NhanVienHoanThanhKhamSangLocId]       BIGINT          NULL,
    [ThoiDiemHoanThanhKhamSangLoc]         DATETIME        NULL,
    [NhanVienTheoDoiSauTiemId]             BIGINT          NULL,
    [ThoiDiemTheoDoiSauTiem]               DATETIME        NULL,
    [GhiChuTheoDoiSauTiem]                 NVARCHAR (1000) NULL,
    [LoaiPhanUngSauTiem]                   INT             NULL,
    [ThongTinPhanUngSauTiem]               NVARCHAR (MAX)  NULL,
    [CreatedById]                          BIGINT          NOT NULL,
    [LastUserId]                           BIGINT          NOT NULL,
    [LastTime]                             DATETIME        NOT NULL,
    [CreatedOn]                            DATETIME        NOT NULL,
    [LastModified]                         ROWVERSION      NOT NULL,
    CONSTRAINT [PK__YeuCauDi__3214EC07DB60A9D5] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_YeuCauDichVuKyThuatKhamSangLocTiemChung_NhanVien] FOREIGN KEY ([NhanVienTheoDoiSauTiemId]) REFERENCES [dbo].[NhanVien] ([Id]),
    CONSTRAINT [FK_YeuCauDichVuKyThuatKhamSangLocTiemChung_NhanVien1] FOREIGN KEY ([NhanVienHoanThanhKhamSangLocId]) REFERENCES [dbo].[NhanVien] ([Id]),
    CONSTRAINT [FK_YeuCauDichVuKyThuatKhamSangLocTiemChung_YeuCauDichVuKyThuat] FOREIGN KEY ([Id]) REFERENCES [dbo].[YeuCauDichVuKyThuat] ([Id])
);
GO
CREATE TABLE [dbo].[YeuCauDichVuKyThuatTiemChung] (
    [Id]                       BIGINT          NOT NULL,
    [DuocPhamBenhVienId]       BIGINT          NOT NULL,
    [TenDuocPham]              NVARCHAR (250)  NOT NULL,
    [TenDuocPhamTiengAnh]      NVARCHAR (250)  NULL,
    [SoDangKy]                 NVARCHAR (100)  NULL,
    [STTHoatChat]              INT             NULL,
    [MaHoatChat]               NVARCHAR (100)  NULL,
    [HoatChat]                 NVARCHAR (550)  NULL,
    [LoaiThuocHoacHoatChat]    INT             NOT NULL,
    [NhaSanXuat]               NVARCHAR (250)  NULL,
    [NuocSanXuat]              NVARCHAR (250)  NULL,
    [DuongDungId]              BIGINT          NOT NULL,
    [HamLuong]                 NVARCHAR (500)  NULL,
    [QuyCach]                  NVARCHAR (250)  NULL,
    [TieuChuan]                NVARCHAR (50)   NULL,
    [DangBaoChe]               NVARCHAR (250)  NULL,
    [DonViTinhId]              BIGINT          NOT NULL,
    [HuongDan]                 NVARCHAR (4000) NULL,
    [MoTa]                     NVARCHAR (4000) NULL,
    [ChiDinh]                  NVARCHAR (4000) NULL,
    [ChongChiDinh]             NVARCHAR (4000) NULL,
    [LieuLuongCachDung]        NVARCHAR (4000) NULL,
    [TacDungPhu]               NVARCHAR (4000) NULL,
    [ChuYDePhong]              NVARCHAR (MAX)  NULL,
    [HopDongThauDuocPhamId]    BIGINT          NULL,
    [NhaThauId]                BIGINT          NULL,
    [SoHopDongThau]            NVARCHAR (50)   NULL,
    [SoQuyetDinhThau]          NVARCHAR (50)   NULL,
    [LoaiThau]                 INT             NULL,
    [LoaiThuocThau]            INT             NULL,
    [NhomThau]                 NVARCHAR (50)   NULL,
    [GoiThau]                  NVARCHAR (2)    NULL,
    [NamThau]                  INT             NULL,
    [SoLuong]                  FLOAT (53)      NOT NULL,
    [XuatKhoDuocPhamChiTietId] BIGINT          NULL,
    [ViTriTiem]                INT             NOT NULL,
    [MuiSo]                    INT             NOT NULL,
    [TrangThaiTiemChung]       INT             NOT NULL,
    [NhanVienTiemId]           BIGINT          NULL,
    [ThoiDiemTiem]             DATETIME        NULL,
    [CreatedById]              BIGINT          NOT NULL,
    [LastUserId]               BIGINT          NOT NULL,
    [LastTime]                 DATETIME        NOT NULL,
    [CreatedOn]                DATETIME        NOT NULL,
    [LastModified]             ROWVERSION      NOT NULL,
    CONSTRAINT [PK__YeuCauDi__3214EC07047B5564] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_YeuCauDichVuKyThuatTiemChung_DonViTinh] FOREIGN KEY ([DonViTinhId]) REFERENCES [dbo].[DonViTinh] ([Id]),
    CONSTRAINT [FK_YeuCauDichVuKyThuatTiemChung_DuocPhamBenhVien] FOREIGN KEY ([DuocPhamBenhVienId]) REFERENCES [dbo].[DuocPhamBenhVien] ([Id]),
    CONSTRAINT [FK_YeuCauDichVuKyThuatTiemChung_DuongDung] FOREIGN KEY ([DuongDungId]) REFERENCES [dbo].[DuongDung] ([Id]),
    CONSTRAINT [FK_YeuCauDichVuKyThuatTiemChung_HopDongThauDuocPham] FOREIGN KEY ([HopDongThauDuocPhamId]) REFERENCES [dbo].[HopDongThauDuocPham] ([Id]),
    CONSTRAINT [FK_YeuCauDichVuKyThuatTiemChung_NhanVien] FOREIGN KEY ([NhanVienTiemId]) REFERENCES [dbo].[NhanVien] ([Id]),
    CONSTRAINT [FK_YeuCauDichVuKyThuatTiemChung_NhaThau] FOREIGN KEY ([NhaThauId]) REFERENCES [dbo].[NhaThau] ([Id]),
    CONSTRAINT [FK_YeuCauDichVuKyThuatTiemChung_XuatKhoDuocPhamChiTiet] FOREIGN KEY ([XuatKhoDuocPhamChiTietId]) REFERENCES [dbo].[XuatKhoDuocPhamChiTiet] ([Id]),
    CONSTRAINT [FK_YeuCauDichVuKyThuatTiemChung_YeuCauDichVuKyThuat] FOREIGN KEY ([Id]) REFERENCES [dbo].[YeuCauDichVuKyThuat] ([Id])
);
GO

CREATE TABLE [dbo].[DichVuKyThuatBenhVienTiemChung](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DichVuKyThuatBenhVienId] [bigint] NOT NULL,
	[DuocPhamBenhVienId] [bigint] NOT NULL,
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

ALTER TABLE [dbo].[DichVuKyThuatBenhVienTiemChung]  WITH CHECK ADD  CONSTRAINT [FK_DichVuKyThuatBenhVienTiemChung_DichVuKyThuatBenhVien] FOREIGN KEY([DichVuKyThuatBenhVienId])
REFERENCES [dbo].[DichVuKyThuatBenhVien] ([Id])
GO

ALTER TABLE [dbo].[DichVuKyThuatBenhVienTiemChung] CHECK CONSTRAINT [FK_DichVuKyThuatBenhVienTiemChung_DichVuKyThuatBenhVien]
GO

ALTER TABLE [dbo].[DichVuKyThuatBenhVienTiemChung]  WITH CHECK ADD  CONSTRAINT [FK_DichVuKyThuatBenhVienTiemChung_DuocPhamBenhVien] FOREIGN KEY([DuocPhamBenhVienId])
REFERENCES [dbo].[DuocPhamBenhVien] ([Id])
GO

ALTER TABLE [dbo].[DichVuKyThuatBenhVienTiemChung] CHECK CONSTRAINT [FK_DichVuKyThuatBenhVienTiemChung_DuocPhamBenhVien]
GO

ALTER TABLE [dbo].[YeuCauDichVuKyThuat] 
ADD
	[YeuCauDichVuKyThuatKhamSangLocTiemChungId] BIGINT          NULL,
	CONSTRAINT [FK_YeuCauDichVuKyThuat_YeuCauDichVuKyThuatKhamSangLocTiemChung] FOREIGN KEY ([YeuCauDichVuKyThuatKhamSangLocTiemChungId]) REFERENCES [dbo].[YeuCauDichVuKyThuatKhamSangLocTiemChung] ([Id]);
GO
	
ALTER TABLE [dbo].[KetQuaSinhHieu] 
ADD
	[YeuCauDichVuKyThuatKhamSangLocTiemChungId] BIGINT          NULL,
	CONSTRAINT [FK_KetQuaSinhHieu_YeuCauDichVuKyThuatKhamSangLocTiemChung] FOREIGN KEY ([YeuCauDichVuKyThuatKhamSangLocTiemChungId]) REFERENCES [dbo].[YeuCauDichVuKyThuatKhamSangLocTiemChung] ([Id]);
	
Update dbo.CauHinh
Set [Value] = '2.7.8' where [Name] = 'CauHinhHeThong.DatabaseVesion'