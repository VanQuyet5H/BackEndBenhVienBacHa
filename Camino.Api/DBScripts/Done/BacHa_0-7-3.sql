CREATE TABLE [dbo].[TheoDoiSauPhauThuatThuThuat] (
    [Id]                         BIGINT          IDENTITY (1, 1) NOT NULL,
    [YeuCauTiepNhanId]           BIGINT          NOT NULL,
    [TrangThai]                  INT             NOT NULL,
    [ThoiDiemBatDauTheoDoi]      DATETIME        NULL,
    [ThoiDiemKetThucTheoDoi]     DATETIME        NULL,
    [BacSiPhuTrachTheoDoiId]     BIGINT          NULL,
    [DieuDuongPhuTrachTheoDoiId] BIGINT          NULL,
    [GhiChuTheoDoi]              NVARCHAR (4000) NULL,
	[TuVongTrongPTTT] int NULL,
    [KhoangThoiGianTuVong] int NULL,
    [ThoiDiemTuVong] datetime NULL
    [CreatedById]                BIGINT          NOT NULL,
    [LastUserId]                 BIGINT          NOT NULL,
    [LastTime]                   DATETIME        NOT NULL,
    [CreatedOn]                  DATETIME        NOT NULL,
    [LastModified]               ROWVERSION      NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PhauThuatThuThuat_NhanVien] FOREIGN KEY ([BacSiPhuTrachTheoDoiId]) REFERENCES [dbo].[NhanVien] ([Id]),
    CONSTRAINT [FK_PhauThuatThuThuat_NhanVien1] FOREIGN KEY ([DieuDuongPhuTrachTheoDoiId]) REFERENCES [dbo].[NhanVien] ([Id]),
    CONSTRAINT [FK_PhauThuatThuThuat_YeuCauTiepNhan] FOREIGN KEY ([YeuCauTiepNhanId]) REFERENCES [dbo].[YeuCauTiepNhan] ([Id])
);
GO
CREATE TABLE [dbo].[TemplateKhamTheoDoi] (
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
CREATE TABLE [dbo].[PhauThuatThuThuatEkipBacSi] (
    [Id]                                  BIGINT     IDENTITY (1, 1) NOT NULL,
    [YeuCauDichVuKyThuatTuongTrinhPTTTId] BIGINT     NOT NULL,
    [NhanVienId]                          BIGINT     NOT NULL,
    [VaiTroBacSi]                         INT        NOT NULL,
    [CreatedById]                         BIGINT     NOT NULL,
    [LastUserId]                          BIGINT     NOT NULL,
    [LastTime]                            DATETIME   NOT NULL,
    [CreatedOn]                           DATETIME   NOT NULL,
    [LastModified]                        ROWVERSION NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PhauThuatThuThuatEkipBacSi_YeuCauDichVuKyThuatTuongTrinhPTTT] FOREIGN KEY ([YeuCauDichVuKyThuatTuongTrinhPTTTId]) REFERENCES [dbo].[YeuCauDichVuKyThuatTuongTrinhPTTT] ([Id]),
    CONSTRAINT [FK_YeuCauDichVuKyThuatEkipBacSi_NhanVien] FOREIGN KEY ([NhanVienId]) REFERENCES [dbo].[NhanVien] ([Id])
);
GO
CREATE TABLE [dbo].[PhauThuatThuThuatEkipDieuDuong] (
    [Id]                                  BIGINT     IDENTITY (1, 1) NOT NULL,
    [YeuCauDichVuKyThuatTuongTrinhPTTTId] BIGINT     NOT NULL,
    [NhanVienId]                          BIGINT     NOT NULL,
    [VaiTroDieuDuong]                     INT        NOT NULL,
    [CreatedById]                         BIGINT     NOT NULL,
    [LastUserId]                          BIGINT     NOT NULL,
    [LastTime]                            DATETIME   NOT NULL,
    [CreatedOn]                           DATETIME   NOT NULL,
    [LastModified]                        ROWVERSION NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PhauThuatThuThuatEkipDieuDuong_YeuCauDichVuKyThuatTuongTrinhPTTT] FOREIGN KEY ([YeuCauDichVuKyThuatTuongTrinhPTTTId]) REFERENCES [dbo].[YeuCauDichVuKyThuatTuongTrinhPTTT] ([Id]),
    CONSTRAINT [FK_YeuCauDichVuKyThuatEkipDieuDuong_NhanVien] FOREIGN KEY ([NhanVienId]) REFERENCES [dbo].[NhanVien] ([Id])
);
GO
CREATE TABLE [dbo].[KhamTheoDoi] (
    [Id]                            BIGINT          IDENTITY (1, 1) NOT NULL,
    [TheoDoiSauPhauThuatThuThuatId] BIGINT          NOT NULL,
    [ThoiDiemBatDauKham]            DATETIME        NOT NULL,
    [ThoiDiemHoanThanhKham]         DATETIME        NULL,
    [NoiThucHienId]                 BIGINT          NULL,
    [NhanVienThucHienId]            BIGINT          NULL,
    [KhamToanThan]                  NVARCHAR (4000) NULL,
    [ThongTinKhamTheoDoiTemplate]   NVARCHAR (MAX)  NULL,
    [ThongTinKhamTheoDoiData]       NVARCHAR (MAX)  NULL,
    [CreatedById]                   BIGINT          NOT NULL,
    [LastUserId]                    BIGINT          NOT NULL,
    [LastTime]                      DATETIME        NOT NULL,
    [CreatedOn]                     DATETIME        NOT NULL,
    [LastModified]                  ROWVERSION      NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_KhamTheoDoi_NhanVien] FOREIGN KEY ([NhanVienThucHienId]) REFERENCES [dbo].[NhanVien] ([Id]),
    CONSTRAINT [FK_KhamTheoDoi_PhongBenhVien] FOREIGN KEY ([NoiThucHienId]) REFERENCES [dbo].[PhongBenhVien] ([Id]),
    CONSTRAINT [FK_PhauThuatThuThuatKhamTheoDoi_PhauThuatThuThuat] FOREIGN KEY ([TheoDoiSauPhauThuatThuThuatId]) REFERENCES [dbo].[TheoDoiSauPhauThuatThuThuat] ([Id])
);
GO
CREATE TABLE [dbo].[KhamTheoDoiBoPhanKhac] (
    [Id]            BIGINT          IDENTITY (1, 1) NOT NULL,
    [Ten]           NVARCHAR (250)  NOT NULL,
    [NoiDung]       NVARCHAR (4000) NOT NULL,
    [KhamTheoDoiId] BIGINT          NOT NULL,
    [CreatedById]   BIGINT          NOT NULL,
    [LastUserId]    BIGINT          NOT NULL,
    [LastTime]      DATETIME        NOT NULL,
    [CreatedOn]     DATETIME        NOT NULL,
    [LastModified]  ROWVERSION      NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PhauThuatThuThuatKhamTheoDoiBoPhanKhac_PhauThuatThuThuatKhamTheoDoi] FOREIGN KEY ([KhamTheoDoiId]) REFERENCES [dbo].[KhamTheoDoi] ([Id])
);
GO

ALTER TABLE dbo.[YeuCauDichVuKyThuat]
ADD [LanThucHien]                    INT             NULL;
GO
ALTER TABLE dbo.[YeuCauDichVuKyThuat]
ADD [TheoDoiSauPhauThuatThuThuatId]  BIGINT          NULL;
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDichVuKyThuat_TheoDoiSauPhauThuatThuThuat] FOREIGN KEY ([TheoDoiSauPhauThuatThuThuatId]) REFERENCES [dbo].[TheoDoiSauPhauThuatThuThuat] ([Id])
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat] CHECK CONSTRAINT [FK_YeuCauDichVuKyThuat_TheoDoiSauPhauThuatThuThuat]
GO

ALTER TABLE dbo.[YeuCauDichVuKyThuatTuongTrinhPTTT]
ADD [ThoiDiemPhauThuat]         DATETIME        NULL;
GO
ALTER TABLE dbo.[YeuCauDichVuKyThuatTuongTrinhPTTT]
ADD [KhoangThoiGianTuVong]      INT             NULL;
GO
ALTER TABLE dbo.[YeuCauDichVuKyThuatTuongTrinhPTTT]
ADD [NhanVienTuongTrinhId]      BIGINT          NULL;
GO
ALTER TABLE dbo.[YeuCauDichVuKyThuatTuongTrinhPTTT]
ADD [ThoiDiemKetThucTuongTrinh] DATETIME        NULL;
GO
ALTER TABLE dbo.[YeuCauDichVuKyThuatTuongTrinhPTTT]
ADD [KhongThucHien]             BIT             NULL;
GO
ALTER TABLE dbo.[YeuCauDichVuKyThuatTuongTrinhPTTT]
ADD [LyDoKhongThucHien]         NVARCHAR (4000) NULL;
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuatTuongTrinhPTTT]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDichVuKyThuatTuongTrinhPTTT_NhanVien] FOREIGN KEY ([NhanVienTuongTrinhId]) REFERENCES [dbo].[NhanVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuatTuongTrinhPTTT] CHECK CONSTRAINT [FK_YeuCauDichVuKyThuatTuongTrinhPTTT_NhanVien]
GO
ALTER TABLE dbo.[YeuCauTiepNhan]
ADD [TuVongTrongPTTT]              INT             NULL;
GO
ALTER TABLE dbo.[YeuCauTiepNhan]
ADD [KhoangThoiGianTuVong]         INT             NULL;
GO
ALTER TABLE dbo.[YeuCauTiepNhan]
ADD [ThoiDiemTuVong]               DATETIME        NULL;
GO
ALTER TABLE dbo.BenhNhan Add SoDienThoaiDisplay AS ([dbo].[LayFormatSoDienThoai]([SoDienThoai]))
GO
Update CauHinh
Set [Value] = '0.7.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'