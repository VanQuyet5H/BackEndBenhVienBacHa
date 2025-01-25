CREATE TABLE [dbo].[NhapKhoMau] (
    [Id]              BIGINT          IDENTITY (1, 1) NOT NULL,
    [SoPhieu]         AS              (concat('PN', RIGHT(datepart(year, [CreatedOn]), (2)), RIGHT('000000' + ltrim(str([Id])), (6)))),
    [SoHoaDon]        NVARCHAR (50)   NOT NULL,
    [KyHieuHoaDon]    NVARCHAR (50)   NULL,
    [NgayHoaDon]      DATETIME        NOT NULL,
    [LoaiNguoiGiao]   INT             NOT NULL,
    [NguoiGiaoId]     BIGINT          NULL,
    [TenNguoiGiao]    NVARCHAR (100)  NULL,
    [NhaThauId]       BIGINT          NULL,
    [NgayNhap]        DATETIME        NOT NULL,
    [NguoiNhapId]     BIGINT          NOT NULL,
    [DuocKeToanDuyet] BIT             NULL,
    [NgayDuyet]       DATETIME        NULL,
    [NhanVienDuyetId] BIGINT          NULL,
    [GhiChu]          NVARCHAR (1000) NULL,
    [CreatedById]     BIGINT          NOT NULL,
    [LastUserId]      BIGINT          NOT NULL,
    [LastTime]        DATETIME        NOT NULL,
    [CreatedOn]       DATETIME        NOT NULL,
    [LastModified]    ROWVERSION      NOT NULL,
    CONSTRAINT [PK__NhapKhoMau] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[NhapKhoMauChiTiet] (
    [Id]                      BIGINT          IDENTITY (1, 1) NOT NULL,
    [NhapKhoMauId]            BIGINT          NOT NULL,
    [YeuCauTruyenMauId]       BIGINT          NOT NULL,
    [MaTuiMau]                NVARCHAR (50)   NOT NULL,
    [MauVaChePhamId]          BIGINT          NOT NULL,
    [MaDichVu]                NVARCHAR (50)   NOT NULL,
    [TenDichVu]               NVARCHAR (500)  NOT NULL,
    [PhanLoaiMau]             INT             NOT NULL,
    [TheTich]                 BIGINT          NOT NULL,
    [NhomMau]                 INT             NULL,
    [YeuToRh]                 INT             NULL,
    [NgaySanXuat]             DATETIME        NOT NULL,
    [HanSuDung]               DATETIME        NOT NULL,
    [DonGiaNhap]              DECIMAL (15, 2) NULL,
    [DonGiaBan]               DECIMAL (15, 2) NULL,
    [DonGiaBaoHiem]           DECIMAL (15, 2) NULL,
    [NgayNhap]                DATETIME        NOT NULL,
    [KetQuaXetNghiemSotRet]   INT             NULL,
    [KetQuaXetNghiemGiangMai] INT             NULL,
    [KetQuaXetNghiemHCV]      INT             NULL,
    [KetQuaXetNghiemHBV]      INT             NULL,
    [KetQuaXetNghiemHIV]      INT             NULL,
    [NguoiThucHien1]          NVARCHAR (100)  NULL,
    [NguoiThucHien2]          NVARCHAR (100)  NULL,
    [KetQuaPhanUngCheoOngI]   NVARCHAR (100)  NULL,
    [KetQuaPhanUngCheoOngII]  NVARCHAR (100)  NULL,
    [CreatedById]             BIGINT          NOT NULL,
    [LastUserId]              BIGINT          NOT NULL,
    [LastTime]                DATETIME        NOT NULL,
    [CreatedOn]               DATETIME        NOT NULL,
    [LastModified]            ROWVERSION      NOT NULL,
    CONSTRAINT [PK_NhapKhoMauChiTiet] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[XuatKhoMau] (
    [Id]           BIGINT          IDENTITY (1, 1) NOT NULL,
    [SoPhieu]      AS              (concat('PX', RIGHT(datepart(year, [CreatedOn]), (2)), RIGHT('000000' + ltrim(str([Id])), (6)))),
    [NguoiXuatId]  BIGINT          NOT NULL,
    [NguoiNhanId]  BIGINT          NOT NULL,
    [NgayXuat]     DATETIME        NOT NULL,
    [GhiChu]       NVARCHAR (1000) NULL,
    [CreatedById]  BIGINT          NOT NULL,
    [LastUserId]   BIGINT          NOT NULL,
    [LastTime]     DATETIME        NOT NULL,
    [CreatedOn]    DATETIME        NOT NULL,
    [LastModified] ROWVERSION      NOT NULL,
    CONSTRAINT [PK__XuatKhoMau] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[XuatKhoMauChiTiet] (
    [Id]                  BIGINT     IDENTITY (1, 1) NOT NULL,
    [XuatKhoMauId]        BIGINT     NOT NULL,
    [NhapKhoMauChiTietId] BIGINT     NOT NULL,
    [CreatedById]         BIGINT     NOT NULL,
    [LastUserId]          BIGINT     NOT NULL,
    [LastTime]            DATETIME   NOT NULL,
    [CreatedOn]           DATETIME   NOT NULL,
    [LastModified]        ROWVERSION NOT NULL,
    CONSTRAINT [PK__XuatKhoMauChiTiet] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[YeuCauTruyenMau] (
    [Id]                        BIGINT          IDENTITY (1, 1) NOT NULL,
    [YeuCauTiepNhanId]          BIGINT          NOT NULL,
    [NoiTruPhieuDieuTriId]      BIGINT          NOT NULL,
    [MauVaChePhamId]            BIGINT          NOT NULL,
    [MaDichVu]                  NVARCHAR (50)   NOT NULL,
    [TenDichVu]                 NVARCHAR (500)  NOT NULL,
    [PhanLoaiMau]               INT             NOT NULL,
    [TheTich]                   BIGINT          NOT NULL,
    [NhomMau]                   INT             NULL,
    [YeuToRh]                   INT             NULL,
    [XuatKhoMauChiTietId]       BIGINT          NULL,
    [DonGiaNhap]                DECIMAL (15, 2) NULL,
    [DonGiaBan]                 DECIMAL (15, 2) NULL,
    [DuocHuongBaoHiem]          BIT             NOT NULL,
    [BaoHiemChiTra]             BIT             NULL,
    [ThoiDiemDuyetBaoHiem]      DATETIME        NULL,
    [NhanVienDuyetBaoHiemId]    BIGINT          NULL,
    [MoTa]                      NVARCHAR (4000) NULL,
    [TrangThai]                 INT             NOT NULL,
    [SoTienBenhNhanDaChi]       DECIMAL (15, 2) NULL,
    [TrangThaiThanhToan]        INT             NOT NULL,
    [NhanVienHuyThanhToanId]    BIGINT          NULL,
    [LyDoHuyThanhToan]          NVARCHAR (1000) NULL,
    [NhanVienChiDinhId]         BIGINT          NOT NULL,
    [NoiChiDinhId]              BIGINT          NOT NULL,
    [ThoiDiemChiDinh]           DATETIME        NOT NULL,
    [ThoiGianBatDauTruyen]      INT             NULL,
    [NoiThucHienId]             BIGINT          NULL,
    [MaGiuong]                  NVARCHAR (20)   NULL,
    [ThoiDiemThucHien]          DATETIME        NULL,
    [ThoiDiemHoanThanh]         DATETIME        NULL,
    [NhanVienThucHienId]        BIGINT          NULL,
    [CreatedById]               BIGINT          NOT NULL,
    [LastUserId]                BIGINT          NOT NULL,
    [LastTime]                  DATETIME        NOT NULL,
    [CreatedOn]                 DATETIME        NOT NULL,
    [LastModified]              ROWVERSION      NOT NULL,
    [SoTienBaoHiemTuNhanChiTra] DECIMAL (15, 2) NULL,
    [SoTienMienGiam]            DECIMAL (15, 2) NULL,
    [DonGiaBaoHiem]             DECIMAL (15, 2) NULL,
    [MucHuongBaoHiem]           INT             NULL,
    [TiLeBaoHiemThanhToan]      INT             NULL,
    [GhiChuMienGiamThem]        NVARCHAR (1000) NULL,
    CONSTRAINT [PK__YeuCauTruyenMau] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
ALTER TABLE [dbo].[DuyetBaoHiemChiTiet]
   ADD [YeuCauTruyenMauId] bigint NULL

GO
ALTER TABLE [CongTyBaoHiemTuNhanCongNo]
   ADD [YeuCauTruyenMauId] bigint NULL

GO
ALTER TABLE [MienGiamChiPhi]
   ADD [YeuCauTruyenMauId] bigint NULL

GO
ALTER TABLE [dbo].[DuyetBaoHiemChiTiet] WITH NOCHECK
    ADD CONSTRAINT [FK_DuyetBaoHiemChiTiet_YeuCauTruyenMau] FOREIGN KEY ([YeuCauTruyenMauId]) REFERENCES [dbo].[YeuCauTruyenMau] ([Id]);

GO
ALTER TABLE [dbo].[NhapKhoMau] WITH NOCHECK
    ADD CONSTRAINT [FK_NhapKhoMau_NhaThau] FOREIGN KEY ([NhaThauId]) REFERENCES [dbo].[NhaThau] ([Id]);

GO
ALTER TABLE [dbo].[NhapKhoMau] WITH NOCHECK
    ADD CONSTRAINT [FK_NhapKhoMau_NhanVien1] FOREIGN KEY ([NguoiNhapId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[NhapKhoMau] WITH NOCHECK
    ADD CONSTRAINT [FK_NhapKhoMau_NhanVien2] FOREIGN KEY ([NhanVienDuyetId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[NhapKhoMau] WITH NOCHECK
    ADD CONSTRAINT [FK_NhapKhoMau_NhanVien3] FOREIGN KEY ([NguoiGiaoId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[NhapKhoMauChiTiet] WITH NOCHECK
    ADD CONSTRAINT [FK_NhapKhoMauChiTiet_MauVaChePham] FOREIGN KEY ([MauVaChePhamId]) REFERENCES [dbo].[MauVaChePham] ([Id]);

GO
ALTER TABLE [dbo].[NhapKhoMauChiTiet] WITH NOCHECK
    ADD CONSTRAINT [FK_NhapKhoMauChiTiet_NhapKhoMau] FOREIGN KEY ([NhapKhoMauId]) REFERENCES [dbo].[NhapKhoMau] ([Id]);

GO
ALTER TABLE [dbo].[NhapKhoMauChiTiet] WITH NOCHECK
    ADD CONSTRAINT [FK_NhapKhoMauChiTiet_YeuCauTruyenMau] FOREIGN KEY ([YeuCauTruyenMauId]) REFERENCES [dbo].[YeuCauTruyenMau] ([Id]);

GO
ALTER TABLE [dbo].[XuatKhoMau] WITH NOCHECK
    ADD CONSTRAINT [FK_XuatKhoMau_NhanVien] FOREIGN KEY ([NguoiXuatId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[XuatKhoMau] WITH NOCHECK
    ADD CONSTRAINT [FK_XuatKhoMau_NhanVien1] FOREIGN KEY ([NguoiNhanId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[XuatKhoMauChiTiet] WITH NOCHECK
    ADD CONSTRAINT [FK_XuatKhoMauChiTiet_XuatKhoMau] FOREIGN KEY ([XuatKhoMauId]) REFERENCES [dbo].[XuatKhoMau] ([Id]);

GO
ALTER TABLE [dbo].[XuatKhoMauChiTiet] WITH NOCHECK
    ADD CONSTRAINT [FK_XuatKhoMauChiTiet_NhapKhoMauChiTiet] FOREIGN KEY ([NhapKhoMauChiTietId]) REFERENCES [dbo].[NhapKhoMauChiTiet] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauTruyenMau] WITH NOCHECK
    ADD CONSTRAINT [FK_YeuCauTruyenMau_YeuCauTiepNhan] FOREIGN KEY ([YeuCauTiepNhanId]) REFERENCES [dbo].[YeuCauTiepNhan] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauTruyenMau] WITH NOCHECK
    ADD CONSTRAINT [FK_YeuCauTruyenMau_NoiTruPhieuDieuTri] FOREIGN KEY ([NoiTruPhieuDieuTriId]) REFERENCES [dbo].[NoiTruPhieuDieuTri] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauTruyenMau] WITH NOCHECK
    ADD CONSTRAINT [FK_YeuCauTruyenMau_MauVaChePham] FOREIGN KEY ([MauVaChePhamId]) REFERENCES [dbo].[MauVaChePham] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauTruyenMau] WITH NOCHECK
    ADD CONSTRAINT [FK_YeuCauTruyenMau_XuatKhoMauChiTiet] FOREIGN KEY ([XuatKhoMauChiTietId]) REFERENCES [dbo].[XuatKhoMauChiTiet] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauTruyenMau] WITH NOCHECK
    ADD CONSTRAINT [FK_YeuCauTruyenMau_NhanVien] FOREIGN KEY ([NhanVienDuyetBaoHiemId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauTruyenMau] WITH NOCHECK
    ADD CONSTRAINT [FK_YeuCauTruyenMau_NhanVien1] FOREIGN KEY ([NhanVienHuyThanhToanId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauTruyenMau] WITH NOCHECK
    ADD CONSTRAINT [FK_YeuCauTruyenMau_NhanVien2] FOREIGN KEY ([NhanVienChiDinhId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauTruyenMau] WITH NOCHECK
    ADD CONSTRAINT [FK_YeuCauTruyenMau_PhongBenhVien] FOREIGN KEY ([NoiChiDinhId]) REFERENCES [dbo].[PhongBenhVien] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauTruyenMau] WITH NOCHECK
    ADD CONSTRAINT [FK_YeuCauTruyenMau_PhongBenhVien1] FOREIGN KEY ([NoiThucHienId]) REFERENCES [dbo].[PhongBenhVien] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauTruyenMau] WITH NOCHECK
    ADD CONSTRAINT [FK_YeuCauTruyenMau_NhanVien3] FOREIGN KEY ([NhanVienThucHienId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[CongTyBaoHiemTuNhanCongNo] WITH NOCHECK
    ADD CONSTRAINT [FK_CongTyBaoHiemTuNhanCongNo_YeuCauTruyenMau] FOREIGN KEY ([YeuCauTruyenMauId]) REFERENCES [dbo].[YeuCauTruyenMau] ([Id]);

GO
ALTER TABLE [dbo].[MienGiamChiPhi] WITH NOCHECK
    ADD CONSTRAINT [FK_MienGiamChiPhi_YeuCauTruyenMau] FOREIGN KEY ([YeuCauTruyenMauId]) REFERENCES [dbo].[YeuCauTruyenMau] ([Id]);

GO
Update dbo.CauHinh
Set [Value] = '1.3.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'