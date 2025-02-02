CREATE TABLE [dbo].[MauMayXetNghiem] (
    [Id]                   BIGINT          IDENTITY (1, 1) NOT NULL,
    [Ma]                   NVARCHAR (25)   NOT NULL,
    [Ten]                  NVARCHAR (100)  NOT NULL,
    [TenTiengAnh]          VARCHAR (100)   NULL,
    [NhaSanXuat]           NVARCHAR (200)  NULL,
    [NhomDichVuBenhVienId] BIGINT          NULL,
    [MoTa]                 NVARCHAR (4000) NULL,
    [CreatedById]          BIGINT          NOT NULL,
    [LastUserId]           BIGINT          NOT NULL,
    [LastTime]             DATETIME        NOT NULL,
    [CreatedOn]            DATETIME        NOT NULL,
    [LastModified]         ROWVERSION      NOT NULL,
    CONSTRAINT [PK_MauMayXetNghiem] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MauMayXetNghiem_NhomDichVuBenhVien] FOREIGN KEY ([NhomDichVuBenhVienId]) REFERENCES [dbo].[NhomDichVuBenhVien] ([Id])
);
GO

CREATE TABLE [dbo].[MauMayXetNghiemChiSo] (
    [Id]                BIGINT         IDENTITY (1, 1) NOT NULL,
    [MauMayXetNghiemID] BIGINT         NOT NULL,
    [MaChiSo]           NVARCHAR (25)  NOT NULL,
    [TenChiSo]          NVARCHAR (100) NULL,
    [DonVi]             NVARCHAR (30)  NULL,
    [CreatedById]       BIGINT         NOT NULL,
    [LastUserId]        BIGINT         NOT NULL,
    [LastTime]          DATETIME       NOT NULL,
    [CreatedOn]         DATETIME       NOT NULL,
    [LastModified]      ROWVERSION     NOT NULL,
    CONSTRAINT [PK_MauMayXetNghiemChiSo] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MauMayXetNghiemChiSo_MauMayXetNghiem] FOREIGN KEY ([MauMayXetNghiemID]) REFERENCES [dbo].[MauMayXetNghiem] ([Id])
);
GO

CREATE TABLE [dbo].[MayXetNghiem] (
    [Id]                 BIGINT         IDENTITY (1, 1) NOT NULL,
    [Ma]                 NVARCHAR (25)  NOT NULL,
    [Ten]                NVARCHAR (100) NOT NULL,
    [MauMayXetNghiemID]  BIGINT         NOT NULL,
    [NhaCungCap]         NVARCHAR (100) NULL,
	[HieuLuc]          	 BIT            NOT NULL,
    [HostName]           VARCHAR (20)   NULL,
    [PortName]           VARCHAR (5)    NULL,
    [BaudRate]           INT            NULL,
    [DataBits]           TINYINT        NULL,
    [StopBits]           VARCHAR (10)   NULL,
    [Parity]             VARCHAR (10)   NULL,
    [Handshake]          VARCHAR (20)   NULL,
    [Encoding]           VARCHAR (20)   NULL,
    [ReadBufferSize]     INT            NULL,
    [RtsEnable]          BIT            NOT NULL,
    [DtrEnable]          BIT            NOT NULL,
    [DiscardNull]        BIT            NOT NULL,
    [ConnectionMode]     VARCHAR (20)   NULL,
    [ConnectionProtocol] VARCHAR (10)   NULL,
    [AutoOpenPort]       BIT            NOT NULL,
    [AutoOpenForm]       BIT            NOT NULL,
    [ConnectionStatus]   INT            NULL,
    [OpenById]           BIGINT         NULL,
    [OpenDateTime]       DATETIME       NULL,
    [CloseDateTime]      DATETIME       NULL,
    [LogDataEnabled]     BIT            NOT NULL,
    [CreatedById]        BIGINT         NOT NULL,
    [LastUserId]         BIGINT         NOT NULL,
    [LastTime]           DATETIME       NOT NULL,
    [CreatedOn]          DATETIME       NOT NULL,
    [LastModified]       ROWVERSION     NOT NULL,
    CONSTRAINT [PK_MayXetNghiem] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MayXetNghiem_MauMayXetNghiem] FOREIGN KEY ([MauMayXetNghiemID]) REFERENCES [dbo].[MauMayXetNghiem] ([Id])
);
GO

CREATE TABLE [dbo].[DichVuXetNghiem] (
    [Id]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [NhomDichVuBenhVienId] BIGINT         NOT NULL,
    [Ma]                   NVARCHAR (50)  NOT NULL,
    [Ten]                  NVARCHAR (250) NOT NULL,
    [CapDichVu]            INT            NOT NULL,
    [DichVuXetNghiemChaId] BIGINT         NULL,
    [DonVi]                NVARCHAR (30)  NULL,
    [SoThuTu]              INT            NULL,
    [HieuLuc]              BIT            NOT NULL,
    [NamMin]              NVARCHAR (20)  NULL,
    [NamMax]              NVARCHAR (20)  NULL,
    [NuMin]               NVARCHAR (20)  NULL,
    [NuMax]               NVARCHAR (20)  NULL,
    [TreEmMin]            NVARCHAR (20)  NULL,
    [TreEmMax]            NVARCHAR (20)  NULL,
    [NguyHiemMax]         NVARCHAR (20)  NULL,
    [NguyHiemMin]         NVARCHAR (20)  NULL,
    [KieuDuLieu]           NVARCHAR (4)   NULL,
    [TreEm6Min]           NVARCHAR (20)  NULL,
    [TreEm6Max]           NVARCHAR (20)  NULL,
    [TreEm612Min]         NVARCHAR (20)  NULL,
    [TreEm612Max]         NVARCHAR (20)  NULL,
    [TreEm1218Min]        NVARCHAR (20)  NULL,
    [TreEm1218Max]        NVARCHAR (20)  NULL,
    [CreatedById]          BIGINT         NOT NULL,
    [LastUserId]           BIGINT         NOT NULL,
    [LastTime]             DATETIME       NOT NULL,
    [CreatedOn]            DATETIME       NOT NULL,
    [LastModified]         ROWVERSION     NOT NULL,
    CONSTRAINT [PK_DichVuXetNghiem] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO
CREATE TABLE [dbo].[DichVuXetNghiemKetNoiChiSo] (
    [Id]                BIGINT          IDENTITY (1, 1) NOT NULL,
    [MaKetNoi]          NVARCHAR (100)  NOT NULL,
    [TenKetNoi]         NVARCHAR (100)  NOT NULL,
    [DichVuXetNghiemId] BIGINT          NOT NULL,
    [MaChiSo]           NVARCHAR (25)   NOT NULL,
    [MauMayXetNghiemId] BIGINT          NOT NULL,
    [HieuLuc]           BIT             NOT NULL,
    [TiLe]              FLOAT (53)      NOT NULL,
    [MoTa]              NVARCHAR (4000) NULL,
    [CreatedById]       BIGINT          NOT NULL,
    [LastUserId]        BIGINT          NOT NULL,
    [LastTime]          DATETIME        NOT NULL,
    [CreatedOn]         DATETIME        NOT NULL,
    [LastModified]      ROWVERSION      NOT NULL,
    CONSTRAINT [PK_DichVuXetNghiemKetNoiChiSo] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_DichVuXetNghiemKetNoiChiSo_DichVuXetNghiem] FOREIGN KEY ([DichVuXetNghiemId]) REFERENCES [dbo].[DichVuXetNghiem] ([Id]),
    CONSTRAINT [FK_DichVuXetNghiemKetNoiChiSo_MauMayXetNghiem] FOREIGN KEY ([MauMayXetNghiemId]) REFERENCES [dbo].[MauMayXetNghiem] ([Id])
);
GO

CREATE TABLE [dbo].[PhienXetNghiem] (
    [Id]                 BIGINT          IDENTITY (1, 1) NOT NULL,
    [BenhNhanId]         BIGINT          NOT NULL,
    [YeuCauTiepNhanId]   BIGINT          NOT NULL,
    [MaSo]               INT             NOT NULL,
    [ThoiDiemBatDau]     DATETIME        NOT NULL,
    [PhongThucHienId]    BIGINT          NULL,
    [NhanVienThucHienId] BIGINT          NULL,
    [KetLuan]            NVARCHAR (200)  NULL,
    [NhanVienKetLuanId]  BIGINT          NULL,
    [ThoiDiemKetLuan]    DATETIME        NULL,
    [GhiChu]             NVARCHAR (1000) NULL,
    [DaTraKetQua]        BIT             NULL,
    [ThoiDiemTraKetQua]  DATETIME        NULL,
    [CreatedById]        BIGINT          NOT NULL,
    [LastUserId]         BIGINT          NOT NULL,
    [LastTime]           DATETIME        NOT NULL,
    [CreatedOn]          DATETIME        NOT NULL,
    [LastModified]       ROWVERSION      NOT NULL,
	[BarCodeNumber]  AS ([MaSo]),
	[BarCodeId]  AS (concat(right(datepart(year,[ThoiDiemBatDau]),(2)),right(datepart(month,[ThoiDiemBatDau]),(2)),right(datepart(day,[ThoiDiemBatDau]),(2)),right('0000'+ltrim(str([MaSo])),(4)))),
    CONSTRAINT [PK_PhienXetNghiem] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PhienXetNghiem_BenhNhan] FOREIGN KEY ([BenhNhanId]) REFERENCES [dbo].[BenhNhan] ([Id]),
    CONSTRAINT [FK_PhienXetNghiem_NhanVien] FOREIGN KEY ([NhanVienThucHienId]) REFERENCES [dbo].[NhanVien] ([Id]),
    CONSTRAINT [FK_PhienXetNghiem_NhanVien1] FOREIGN KEY ([NhanVienKetLuanId]) REFERENCES [dbo].[NhanVien] ([Id]),
    CONSTRAINT [FK_PhienXetNghiem_PhongBenhVien] FOREIGN KEY ([PhongThucHienId]) REFERENCES [dbo].[PhongBenhVien] ([Id]),
    CONSTRAINT [FK_PhienXetNghiem_YeuCauTiepNhan] FOREIGN KEY ([YeuCauTiepNhanId]) REFERENCES [dbo].[YeuCauTiepNhan] ([Id])
);
GO
CREATE TABLE [dbo].[PhieuGoiMauXetNghiem] (
    [Id]                BIGINT          IDENTITY (1, 1) NOT NULL,
    [NhanVienGoiMauId]  BIGINT          NOT NULL,
    [ThoiDiemGoiMau]    DATETIME        NOT NULL,
    [PhongNhanMauId]    BIGINT          NOT NULL,
    [NhanVienNhanMauId] BIGINT          NULL,
    [ThoiDiemNhanMau]   DATETIME        NULL,
    [DaNhanMau]         BIT             NULL,
    [GhiChu]            NVARCHAR (1000) NULL,
    [CreatedById]       BIGINT          NOT NULL,
    [LastUserId]        BIGINT          NOT NULL,
    [LastTime]          DATETIME        NOT NULL,
    [CreatedOn]         DATETIME        NOT NULL,
    [LastModified]      ROWVERSION      NOT NULL,
    CONSTRAINT [PK_PhieuGoiMauXetNghiem] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PhieuGoiMauXetNghiem_NhanVien] FOREIGN KEY ([NhanVienGoiMauId]) REFERENCES [dbo].[NhanVien] ([Id]),
    CONSTRAINT [FK_PhieuGoiMauXetNghiem_NhanVien1] FOREIGN KEY ([NhanVienNhanMauId]) REFERENCES [dbo].[NhanVien] ([Id]),
    CONSTRAINT [FK_PhieuGoiMauXetNghiem_PhongBenhVien] FOREIGN KEY ([PhongNhanMauId]) REFERENCES [dbo].[PhongBenhVien] ([Id])
);
GO
CREATE TABLE [dbo].[MauXetNghiem] (
    [Id]                     BIGINT          IDENTITY (1, 1) NOT NULL,
    [PhienXetNghiemId]       BIGINT          NOT NULL,
    [NhomDichVuBenhVienId]   BIGINT          NOT NULL,
    [LoaiMauXetNghiem]       INT             NOT NULL,
    [SoLuongMau]             INT             NOT NULL,
    [BarCodeId]              VARCHAR (20)    NULL,
    [BarCodeNumber]          INT             NULL,
    [ThoiDiemLayMau]         DATETIME        NULL,
    [PhongLayMauId]          BIGINT          NULL,
    [NhanVienLayMauId]       BIGINT          NULL,
    [PhieuGoiMauXetNghiemId] BIGINT          NULL,
    [DatChatLuong]           BIT             NULL,
    [NhanVienXetKhongDatId]  BIGINT          NULL,
    [LyDoKhongDat]           NVARCHAR (200)  NULL,
    [GhiChu]                 NVARCHAR (1000) NULL,
    [CreatedById]            BIGINT          NOT NULL,
    [LastUserId]             BIGINT          NOT NULL,
    [LastTime]               DATETIME        NOT NULL,
    [CreatedOn]              DATETIME        NOT NULL,
    [LastModified]           ROWVERSION      NOT NULL,
    CONSTRAINT [PK_MauXetNghiem] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MauXetNghiem_NhanVien] FOREIGN KEY ([NhanVienLayMauId]) REFERENCES [dbo].[NhanVien] ([Id]),
    CONSTRAINT [FK_MauXetNghiem_NhanVien1] FOREIGN KEY ([NhanVienXetKhongDatId]) REFERENCES [dbo].[NhanVien] ([Id]),
    CONSTRAINT [FK_MauXetNghiem_NhomDichVuBenhVien] FOREIGN KEY ([NhomDichVuBenhVienId]) REFERENCES [dbo].[NhomDichVuBenhVien] ([Id]),
    CONSTRAINT [FK_MauXetNghiem_PhienXetNghiem] FOREIGN KEY ([PhienXetNghiemId]) REFERENCES [dbo].[PhienXetNghiem] ([Id]),
    CONSTRAINT [FK_MauXetNghiem_PhieuGoiMauXetNghiem] FOREIGN KEY ([PhieuGoiMauXetNghiemId]) REFERENCES [dbo].[PhieuGoiMauXetNghiem] ([Id]),
    CONSTRAINT [FK_MauXetNghiem_PhongBenhVien] FOREIGN KEY ([PhongLayMauId]) REFERENCES [dbo].[PhongBenhVien] ([Id])
);
GO
CREATE TABLE [dbo].[YeuCauChayLaiXetNghiem] (
    [Id]                   BIGINT          IDENTITY (1, 1) NOT NULL,
    [PhienXetNghiemId]     BIGINT          NOT NULL,
    [NhomDichVuBenhVienId] BIGINT          NOT NULL,
    [NhanVienYeuCauId]     BIGINT          NOT NULL,
    [NgayYeuCau]           DATETIME        NOT NULL,
    [LyDoYeuCau]           NVARCHAR (1000) NULL,
    [DuocDuyet]            BIT             NULL,
    [NgayDuyet]            DATETIME        NULL,
    [NhanVienDuyetId]      BIGINT          NULL,
    [LyDoKhongDuyet]       NVARCHAR (1000) NULL,
    [CreatedById]          BIGINT          NOT NULL,
    [LastUserId]           BIGINT          NOT NULL,
    [LastTime]             DATETIME        NOT NULL,
    [CreatedOn]            DATETIME        NOT NULL,
    [LastModified]         ROWVERSION      NOT NULL,
    CONSTRAINT [PK__YeuCauChayLaiXetNghiem] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_YeuCauChayLaiXetNghiem_NhanVien] FOREIGN KEY ([NhanVienYeuCauId]) REFERENCES [dbo].[NhanVien] ([Id]),
    CONSTRAINT [FK_YeuCauChayLaiXetNghiem_NhanVien1] FOREIGN KEY ([NhanVienDuyetId]) REFERENCES [dbo].[NhanVien] ([Id]),
    CONSTRAINT [FK_YeuCauChayLaiXetNghiem_NhomDichVuBenhVien] FOREIGN KEY ([NhomDichVuBenhVienId]) REFERENCES [dbo].[NhomDichVuBenhVien] ([Id]),
    CONSTRAINT [FK_YeuCauChayLaiXetNghiem_PhienXetNghiem] FOREIGN KEY ([PhienXetNghiemId]) REFERENCES [dbo].[PhienXetNghiem] ([Id])
);
GO
CREATE TABLE [dbo].[PhienXetNghiemChiTiet] (
    [Id]                       BIGINT          IDENTITY (1, 1) NOT NULL,
    [PhienXetNghiemId]         BIGINT          NOT NULL,
    [NhomDichVuBenhVienId]     BIGINT          NOT NULL,
    [YeuCauDichVuKyThuatId]    BIGINT          NOT NULL,
    [DichVuKyThuatBenhVienId]  BIGINT          NOT NULL,
    [LanThucHien]              INT             NOT NULL,
    [DaGoiDuyet]               BIT             NULL,
    [KetLuan]                  NVARCHAR (200)  NULL,
    [NhanVienKetLuanId]        BIGINT          NULL,
    [ThoiDiemKetLuan]          DATETIME        NULL,
    [GhiChu]                   NVARCHAR (1000) NULL,
    [ChayLaiKetQua]            BIT             NULL,
    [YeuCauChayLaiXetNghiemId] BIGINT          NULL,
    [CreatedById]              BIGINT          NOT NULL,
    [LastUserId]               BIGINT          NOT NULL,
    [LastTime]                 DATETIME        NOT NULL,
    [CreatedOn]                DATETIME        NOT NULL,
    [LastModified]             ROWVERSION      NOT NULL,
    CONSTRAINT [PK_PhienXetNghiemChiTiet] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PhienXetNghiemChiTiet_NhanVien] FOREIGN KEY ([NhanVienKetLuanId]) REFERENCES [dbo].[NhanVien] ([Id]),
    CONSTRAINT [FK_PhienXetNghiemChiTiet_NhomDichVuBenhVien] FOREIGN KEY ([NhomDichVuBenhVienId]) REFERENCES [dbo].[NhomDichVuBenhVien] ([Id]),
    CONSTRAINT [FK_PhienXetNghiemChiTiet_PhienXetNghiem] FOREIGN KEY ([PhienXetNghiemId]) REFERENCES [dbo].[PhienXetNghiem] ([Id]),
    CONSTRAINT [FK_PhienXetNghiemChiTiet_YeuCauChayLaiXetNghiem] FOREIGN KEY ([YeuCauChayLaiXetNghiemId]) REFERENCES [dbo].[YeuCauChayLaiXetNghiem] ([Id]),
    CONSTRAINT [FK_PhienXetNghiemChiTiet_YeuCauDichVuKyThuat] FOREIGN KEY ([YeuCauDichVuKyThuatId]) REFERENCES [dbo].[YeuCauDichVuKyThuat] ([Id])
);
GO
CREATE TABLE [dbo].[KetQuaXetNghiemChiTiet] (
    [Id]                           BIGINT         IDENTITY (1, 1) NOT NULL,
    [BarCodeID]                    VARCHAR (20)   NOT NULL,
    [BarCodeNumber]                INT            NOT NULL,
    [PhienXetNghiemChiTietId]      BIGINT         NOT NULL,
    [YeuCauDichVuKyThuatId]        BIGINT         NOT NULL,
    [DichVuKyThuatBenhVienId]      BIGINT         NOT NULL,
    [NhomDichVuBenhVienId]         BIGINT         NOT NULL,
    [LanThucHien]                  INT            NOT NULL,
    [DichVuXetNghiemId]            BIGINT         NOT NULL,
    [DichVuXetNghiemChaId]         BIGINT         NOT NULL,
    [DichVuXetNghiemMa]            NVARCHAR (50)  NOT NULL,
    [DichVuXetNghiemTen]           NVARCHAR (250) NOT NULL,
    [CapDichVu]                    INT            NOT NULL,
    [DonVi]                        NVARCHAR (30)  NULL,
    [SoThuTu]                      INT            NULL,
    [DichVuXetNghiemKetNoiChiSoId] BIGINT         NULL,
    [MaChiSo]                      NVARCHAR (25)  NULL,
    [TiLe]                         FLOAT (53)     NULL,
    [MauMayXetNghiemId]            BIGINT         NULL,
    [MayXetNghiemId]               BIGINT         NULL,
    [GiaTriTuMay]                  NVARCHAR (20)  NULL,
    [GiaTriNhapTay]                NVARCHAR (20)  NULL,
    [GiaTriDuyet]                  NVARCHAR (20)  NULL,
    [GiaTriCu]                     NVARCHAR (20)  NULL,
    [NhanVienNhapTayId]            BIGINT         NULL,
    [GiaTriMin]                    NVARCHAR (20)  NULL,
    [GiaTriMax]                    NVARCHAR (20)  NULL,
    [GiaTriNguyHiemMin]            NVARCHAR (20)  NULL,
    [GiaTriNguyHiemMax]            NVARCHAR (20)  NULL,
    [GiaTriKhacThuong]             BIT            NULL,
    [GiaTriNguyHiem]               BIT            NULL,
    [ToDamGiaTri]                  BIT            NULL,
    [DaDuyet]                      BIT            NULL,
    [ThoiDiemGuiYeuCau]            DATETIME       NULL,
    [ThoiDiemNhanKetQua]           DATETIME       NULL,
    [ThoiDiemDuyetKetQua]          DATETIME       NULL,
    [NhanVienDuyetId]              BIGINT         NULL,
    [Rack]                         VARCHAR (50)   NULL,
    [Comment]                      NVARCHAR (100) NULL,
    [StripType]                    NVARCHAR (50)  NULL,
    [LotId]                        VARCHAR (30)   NULL,
    [CreatedById]                  BIGINT         NOT NULL,
    [LastUserId]                   BIGINT         NOT NULL,
    [LastTime]                     DATETIME       NOT NULL,
    [CreatedOn]                    DATETIME       NOT NULL,
    [LastModified]                 ROWVERSION     NOT NULL,
    CONSTRAINT [PK_KetQuaXetNghiemChiTiet] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_KetQuaXetNghiemChiTiet_DichVuKyThuatBenhVien] FOREIGN KEY ([DichVuKyThuatBenhVienId]) REFERENCES [dbo].[DichVuKyThuatBenhVien] ([Id]),
    CONSTRAINT [FK_KetQuaXetNghiemChiTiet_DichVuXetNghiem] FOREIGN KEY ([DichVuXetNghiemId]) REFERENCES [dbo].[DichVuXetNghiem] ([Id]),
    CONSTRAINT [FK_KetQuaXetNghiemChiTiet_DichVuXetNghiem1] FOREIGN KEY ([DichVuXetNghiemChaId]) REFERENCES [dbo].[DichVuXetNghiem] ([Id]),
    CONSTRAINT [FK_KetQuaXetNghiemChiTiet_DichVuXetNghiemKetNoiChiSo] FOREIGN KEY ([DichVuXetNghiemKetNoiChiSoId]) REFERENCES [dbo].[DichVuXetNghiemKetNoiChiSo] ([Id]),
    CONSTRAINT [FK_KetQuaXetNghiemChiTiet_NhanVien] FOREIGN KEY ([NhanVienDuyetId]) REFERENCES [dbo].[NhanVien] ([Id]),
    CONSTRAINT [FK_KetQuaXetNghiemChiTiet_NhanVien1] FOREIGN KEY ([NhanVienNhapTayId]) REFERENCES [dbo].[NhanVien] ([Id]),
    CONSTRAINT [FK_KetQuaXetNghiemChiTiet_NhomDichVuBenhVien] FOREIGN KEY ([NhomDichVuBenhVienId]) REFERENCES [dbo].[NhomDichVuBenhVien] ([Id]),
    CONSTRAINT [FK_KetQuaXetNghiemChiTiet_PhienXetNghiemChiTiet] FOREIGN KEY ([PhienXetNghiemChiTietId]) REFERENCES [dbo].[PhienXetNghiemChiTiet] ([Id]),
    CONSTRAINT [FK_KetQuaXetNghiemChiTiet_YeuCauDichVuKyThuat] FOREIGN KEY ([YeuCauDichVuKyThuatId]) REFERENCES [dbo].[YeuCauDichVuKyThuat] ([Id])
);
GO

ALTER TABLE dbo.DichVuKyThuatBenhVien
ADD   [DichVuXetNghiemId]        BIGINT          NULL
GO
ALTER TABLE dbo.DichVuKyThuatBenhVien
ADD   [LoaiMauXetNghiem]        INT          NULL
GO
ALTER TABLE [dbo].[DichVuKyThuatBenhVien] ADD CONSTRAINT [FK_DichVuKyThuatBenhVien_DichVuXetNghiem] FOREIGN KEY ([DichVuXetNghiemId]) REFERENCES [dbo].[DichVuXetNghiem] ([Id]);
GO
ALTER TABLE dbo.[YeuCauDichVuKyThuat]
ADD   [BenhPhamXetNghiem]              NVARCHAR (200)  NULL
GO
Update dbo.CauHinh
Set [Value] = '1.2.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'


