ALTER TABLE [dbo].[GoiDichVu] DROP CONSTRAINT [FK_GoiDichVu_NhanVien]
ALTER TABLE [dbo].[GoiDichVu] DROP COLUMN [CoChietKhau]
ALTER TABLE [dbo].[GoiDichVu] DROP COLUMN [ChiPhiGoiDichVu]
ALTER TABLE [dbo].[GoiDichVu] DROP COLUMN [NhanVienTaoGoiId]
ALTER TABLE [dbo].[GoiDichVu] DROP COLUMN [NgayBatDau]
ALTER TABLE [dbo].[GoiDichVu] DROP COLUMN [NgayKetThuc]
GO
ALTER TABLE [dbo].[GoiDichVuChiTietDichVuGiuong] ADD [SoLan]                         INT             NOT NULL
ALTER TABLE [dbo].[GoiDichVuChiTietDichVuGiuong] ADD [GhiChu]                        NVARCHAR (1000) NULL
GO
ALTER TABLE [dbo].[GoiDichVuChiTietDichVuKhamBenh] ADD [SoLan]                         INT             NOT NULL
ALTER TABLE [dbo].[GoiDichVuChiTietDichVuKhamBenh] ADD [GhiChu]                        NVARCHAR (1000) NULL
GO
ALTER TABLE [dbo].[GoiDichVuChiTietDichVuKyThuat] ADD [GhiChu]                        NVARCHAR (1000) NULL
GO
ALTER TABLE [dbo].[MienGiamChiPhi] ADD [LoaiChietKhau]                INT             NULL
ALTER TABLE [dbo].[MienGiamChiPhi] ADD [TiLe]                         INT             NULL
GO
UPDATE [dbo].[MienGiamChiPhi] SET [LoaiChietKhau] = 2
GO
ALTER TABLE [dbo].[MienGiamChiPhi] ALTER COLUMN [LoaiChietKhau]                INT            NOT NULL
GO
ALTER TABLE [dbo].[TheVoucher] DROP COLUMN [TriGia]
ALTER TABLE [dbo].[TheVoucher] ALTER COLUMN [Ma]                 NVARCHAR (25)   NOT NULL
ALTER TABLE [dbo].[TheVoucher] ALTER COLUMN [DenNgay]            DATETIME        NULL
GO
ALTER TABLE [dbo].[Voucher] DROP COLUMN [LoaiVoucher]
ALTER TABLE [dbo].[Voucher] DROP COLUMN [TriGia]
ALTER TABLE [dbo].[Voucher] DROP COLUMN [IsDisabled]
ALTER TABLE [dbo].[Voucher] ADD [Ma]                   NVARCHAR (20)   NOT NULL
ALTER TABLE [dbo].[Voucher] ADD [SoLuongPhatHanh]      INT             NOT NULL
ALTER TABLE [dbo].[Voucher] ADD [TuNgay]               DATETIME        NOT NULL
ALTER TABLE [dbo].[Voucher] ADD [DenNgay]              DATETIME        NULL
ALTER TABLE [dbo].[Voucher] ADD [ChietKhauTatCaDichVu] BIT             NULL
ALTER TABLE [dbo].[Voucher] ADD [LoaiChietKhau]        INT             NULL
ALTER TABLE [dbo].[Voucher] ADD [TiLeChietKhau]        INT             NULL
ALTER TABLE [dbo].[Voucher] ADD [SoTienChietKhau]      DECIMAL (15, 2) NULL
GO
ALTER TABLE [dbo].[VoucherChiTietMienGiam] DROP CONSTRAINT [FK_VoucherChiTietMienGiam_DichVuGiuongBenhVien]
ALTER TABLE [dbo].[VoucherChiTietMienGiam] DROP CONSTRAINT [FK_VoucherChiTietMienGiam_DuocPhamBenhVien]
ALTER TABLE [dbo].[VoucherChiTietMienGiam] DROP CONSTRAINT [FK_VoucherChiTietMienGiam_GoiDichVu]
ALTER TABLE [dbo].[VoucherChiTietMienGiam] DROP CONSTRAINT [FK_VoucherChiTietMienGiam_VatTuBenhVien]
ALTER TABLE [dbo].[VoucherChiTietMienGiam] DROP COLUMN [TiLeMienGiam]
ALTER TABLE [dbo].[VoucherChiTietMienGiam] DROP COLUMN [DuocPhamBenhVienId]
ALTER TABLE [dbo].[VoucherChiTietMienGiam] DROP COLUMN [VatTuBenhVienId]
ALTER TABLE [dbo].[VoucherChiTietMienGiam] DROP COLUMN [DichVuGiuongBenhVienId]
ALTER TABLE [dbo].[VoucherChiTietMienGiam] DROP COLUMN [GoiDichVuId]
ALTER TABLE [dbo].[VoucherChiTietMienGiam] ADD [NhomGiaDichVuKhamBenhBenhVienId] BIGINT          NULL
ALTER TABLE [dbo].[VoucherChiTietMienGiam] ADD [NhomGiaDichVuKyThuatBenhVienId]  BIGINT          NULL
ALTER TABLE [dbo].[VoucherChiTietMienGiam] ADD [NhomDichVuBenhVienId]            BIGINT          NULL
ALTER TABLE [dbo].[VoucherChiTietMienGiam] ADD [NhomDichVuKhamBenh]              BIT             NULL
ALTER TABLE [dbo].[VoucherChiTietMienGiam] ADD [LoaiChietKhau]                   INT             NOT NULL
ALTER TABLE [dbo].[VoucherChiTietMienGiam] ADD [TiLeChietKhau]                   INT             NULL
ALTER TABLE [dbo].[VoucherChiTietMienGiam] ADD [SoTienChietKhau]                 DECIMAL (15, 2) NULL
ALTER TABLE [dbo].[VoucherChiTietMienGiam] ADD [GhiChu]                          NVARCHAR (1000) NULL
GO
ALTER TABLE [dbo].[YeuCauGoiDichVu] DROP CONSTRAINT [FK_YeuCauGoiDichVu_GoiDichVu]
ALTER TABLE [dbo].[YeuCauGoiDichVu] DROP CONSTRAINT [FK_YeuCauGoiDichVu_YeuCauKhamBenh]
ALTER TABLE [dbo].[YeuCauGoiDichVu] DROP CONSTRAINT [FK_YeuCauGoiDichVu_YeuCauTiepNhan]
ALTER TABLE [dbo].[YeuCauGoiDichVu] DROP COLUMN [YeuCauTiepNhanId]
ALTER TABLE [dbo].[YeuCauGoiDichVu] DROP COLUMN [YeuCauKhamBenhId]
ALTER TABLE [dbo].[YeuCauGoiDichVu] DROP COLUMN [GoiDichVuId]
ALTER TABLE [dbo].[YeuCauGoiDichVu] DROP COLUMN [LoaiGoiDichVu]
ALTER TABLE [dbo].[YeuCauGoiDichVu] DROP COLUMN [Ten]
ALTER TABLE [dbo].[YeuCauGoiDichVu] DROP COLUMN [CoChietKhau]
ALTER TABLE [dbo].[YeuCauGoiDichVu] DROP COLUMN [ChiPhiGoiDichVu]
ALTER TABLE [dbo].[YeuCauGoiDichVu] DROP COLUMN [MoTa]
ALTER TABLE [dbo].[YeuCauGoiDichVu] ALTER COLUMN [TiLeChietKhau]             FLOAT      NOT NULL
ALTER TABLE [dbo].[YeuCauGoiDichVu] ADD [BenhNhanId]                BIGINT          NOT NULL
ALTER TABLE [dbo].[YeuCauGoiDichVu] ADD [ChuongTrinhGoiDichVuId]    BIGINT          NOT NULL
ALTER TABLE [dbo].[YeuCauGoiDichVu] ADD [MaChuongTrinh]             NVARCHAR (20)   NOT NULL
ALTER TABLE [dbo].[YeuCauGoiDichVu] ADD [TenChuongTrinh]            NVARCHAR (250)  NOT NULL
ALTER TABLE [dbo].[YeuCauGoiDichVu] ADD [GiaTruocChietKhau]         DECIMAL (15, 2) NOT NULL
ALTER TABLE [dbo].[YeuCauGoiDichVu] ADD [GiaSauChietKhau]           DECIMAL (15, 2) NOT NULL
ALTER TABLE [dbo].[YeuCauGoiDichVu] ADD [TenGoiDichVu]              NVARCHAR (200)  NOT NULL
ALTER TABLE [dbo].[YeuCauGoiDichVu] ADD [MoTaGoiDichVu]             NVARCHAR (4000) NULL
ALTER TABLE [dbo].[YeuCauGoiDichVu] ADD [BoPhanMarketingDangKy]     BIT             NULL
ALTER TABLE [dbo].[YeuCauGoiDichVu] ADD [BoPhanMarketingDaNhanTien] BIT             NULL
ALTER TABLE [dbo].[YeuCauGoiDichVu] ADD [DaTangQua]                 BIT             NULL
GO
CREATE TABLE [dbo].[ChuongTrinhGoiDichVu] (
    [Id]                BIGINT          IDENTITY (1, 1) NOT NULL,
    [Ma]                NVARCHAR (20)   NOT NULL,
    [Ten]               NVARCHAR (250)  NOT NULL,
    [GoiDichVuId]       BIGINT          NOT NULL,
    [TenGoiDichVu]      NVARCHAR (200)  NOT NULL,
    [MoTaGoiDichVu]     NVARCHAR (4000) NULL,
    [GiaTruocChietKhau] DECIMAL (15, 2) NOT NULL,
    [TiLeChietKhau]     FLOAT (53)      NOT NULL,
    [GiaSauChietKhau]   DECIMAL (15, 2) NOT NULL,
    [TuNgay]            DATETIME        NOT NULL,
    [DenNgay]           DATETIME        NULL,
    [TamNgung]          BIT             NULL,
    [CreatedById]       BIGINT          NOT NULL,
    [LastUserId]        BIGINT          NOT NULL,
    [LastTime]          DATETIME        NOT NULL,
    [CreatedOn]         DATETIME        NOT NULL,
    [LastModified]      ROWVERSION      NOT NULL,
    CONSTRAINT [PK__ChuongTrinhGoiDichVu] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ChuongTrinhGoiDichVu_GoiDichVu] FOREIGN KEY ([GoiDichVuId]) REFERENCES [dbo].[GoiDichVu] ([Id])
);
GO
CREATE TABLE [dbo].[ChuongTrinhGoiDichVuDichVuGiuong] (
    [Id]                            BIGINT          IDENTITY (1, 1) NOT NULL,
    [ChuongTrinhGoiDichVuId]        BIGINT          NOT NULL,
    [DichVuGiuongBenhVienId]        BIGINT          NOT NULL,
    [NhomGiaDichVuGiuongBenhVienId] BIGINT          NOT NULL,
    [DonGia]                        DECIMAL (15, 2) NOT NULL,
    [SoLan]                         INT             NOT NULL,
    [CreatedById]                   BIGINT          NOT NULL,
    [LastUserId]                    BIGINT          NOT NULL,
    [LastTime]                      DATETIME        NOT NULL,
    [CreatedOn]                     DATETIME        NOT NULL,
    [LastModified]                  ROWVERSION      NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ChuongTrinhGoiDichVuDichVuGiuong_ChuongTrinhGoiDichVu] FOREIGN KEY ([ChuongTrinhGoiDichVuId]) REFERENCES [dbo].[ChuongTrinhGoiDichVu] ([Id])
);
GO
CREATE TABLE [dbo].[ChuongTrinhGoiDichVuDichVuKhamBenh] (
    [Id]                              BIGINT          IDENTITY (1, 1) NOT NULL,
    [ChuongTrinhGoiDichVuId]          BIGINT          NOT NULL,
    [DichVuKhamBenhBenhVienId]        BIGINT          NOT NULL,
    [NhomGiaDichVuKhamBenhBenhVienId] BIGINT          NOT NULL,
    [DonGia]                          DECIMAL (15, 2) NOT NULL,
    [SoLan]                           INT             NOT NULL,
    [CreatedById]                     BIGINT          NOT NULL,
    [LastUserId]                      BIGINT          NOT NULL,
    [LastTime]                        DATETIME        NOT NULL,
    [CreatedOn]                       DATETIME        NOT NULL,
    [LastModified]                    ROWVERSION      NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ChuongTrinhGoiDichVuDichVuKhamBenh_ChuongTrinhGoiDichVu] FOREIGN KEY ([ChuongTrinhGoiDichVuId]) REFERENCES [dbo].[ChuongTrinhGoiDichVu] ([Id])
);
GO
CREATE TABLE [dbo].[ChuongTrinhGoiDichVuDichVuKyThuat] (
    [Id]                             BIGINT          IDENTITY (1, 1) NOT NULL,
    [ChuongTrinhGoiDichVuId]         BIGINT          NOT NULL,
    [DichVuKyThuatBenhVienId]        BIGINT          NOT NULL,
    [NhomGiaDichVuKyThuatBenhVienId] BIGINT          NOT NULL,
    [DonGia]                         DECIMAL (15, 2) NOT NULL,
    [SoLan]                          INT             NOT NULL,
    [CreatedById]                    BIGINT          NOT NULL,
    [LastUserId]                     BIGINT          NOT NULL,
    [LastTime]                       DATETIME        NOT NULL,
    [CreatedOn]                      DATETIME        NOT NULL,
    [LastModified]                   ROWVERSION      NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ChuongTrinhGoiDichVuDichVuKyThuat_ChuongTrinhGoiDichVu] FOREIGN KEY ([ChuongTrinhGoiDichVuId]) REFERENCES [dbo].[ChuongTrinhGoiDichVu] ([Id])
);
GO
CREATE TABLE [dbo].[QuaTang] (
    [Id]           BIGINT          IDENTITY (1, 1) NOT NULL,
    [Ten]          NVARCHAR (250)  NOT NULL,
    [DonViTinh]    NVARCHAR (50)   NOT NULL,
    [MoTa]         NVARCHAR (4000) NULL,
    [HieuLuc]      BIT             NOT NULL,
    [CreatedById]  BIGINT          NOT NULL,
    [LastUserId]   BIGINT          NOT NULL,
    [LastTime]     DATETIME        NOT NULL,
    [CreatedOn]    DATETIME        NOT NULL,
    [LastModified] ROWVERSION      NOT NULL,
    CONSTRAINT [PK__QuaTang] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO
CREATE TABLE [dbo].[ChuongTrinhGoiDichVuQuaTang] (
    [Id]                     BIGINT          IDENTITY (1, 1) NOT NULL,
    [ChuongTrinhGoiDichVuId] BIGINT          NOT NULL,
    [QuaTangId]              BIGINT          NOT NULL,
    [SoLuong]                INT             NOT NULL,
    [GhiChu]                 NVARCHAR (1000) NULL,
    [CreatedById]            BIGINT          NOT NULL,
    [LastUserId]             BIGINT          NOT NULL,
    [LastTime]               DATETIME        NOT NULL,
    [CreatedOn]              DATETIME        NOT NULL,
    [LastModified]           ROWVERSION      NOT NULL,
    CONSTRAINT [PK__ChuongTrinhGoiDichVuQuaTang] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ChuongTrinhGoiDichVuQuaTang_ChuongTrinhGoiDichVu] FOREIGN KEY ([ChuongTrinhGoiDichVuId]) REFERENCES [dbo].[ChuongTrinhGoiDichVu] ([Id]),
    CONSTRAINT [FK_ChuongTrinhGoiDichVuQuaTang_QuaTang] FOREIGN KEY ([QuaTangId]) REFERENCES [dbo].[QuaTang] ([Id])
);
GO
CREATE TABLE [dbo].[NhapKhoQuaTang] (
    [Id]            BIGINT         IDENTITY (1, 1) NOT NULL,
    [SoPhieu]       AS             (concat('PN',right(datepart(year,[CreatedOn]),(2)),right('000000'+ltrim(str([Id])),(6)))),
    [SoChungTu]     NVARCHAR (50)  NULL,
    [LoaiNguoiGiao] INT            NOT NULL,
    [TenNguoiGiao]  NVARCHAR (100) NULL,
    [NguoiGiaoId]   BIGINT         NULL,
    [NguoiNhapId]   BIGINT         NOT NULL,
    [NgayNhap]      DATETIME       NOT NULL,
    [DaHet]         BIT            NULL,
    [CreatedById]   BIGINT         NOT NULL,
    [LastUserId]    BIGINT         NOT NULL,
    [LastTime]      DATETIME       NOT NULL,
    [CreatedOn]     DATETIME       NOT NULL,
    [LastModified]  ROWVERSION     NOT NULL,
    CONSTRAINT [PK__NhapKhoQuaTang] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_NhapKhoQuaTang_NhanVien] FOREIGN KEY ([NguoiGiaoId]) REFERENCES [dbo].[NhanVien] ([Id]),
    CONSTRAINT [FK_NhapKhoQuaTang_NhanVien1] FOREIGN KEY ([NguoiNhapId]) REFERENCES [dbo].[NhanVien] ([Id])
);
GO
CREATE TABLE [dbo].[NhapKhoQuaTangChiTiet] (
    [Id]               BIGINT          IDENTITY (1, 1) NOT NULL,
    [NhapKhoQuaTangId] BIGINT          NOT NULL,
    [QuaTangId]        BIGINT          NOT NULL,
    [NhaCungCap]       NVARCHAR (250)  NOT NULL,
    [SoLuongNhap]      INT             NOT NULL,
    [DonGiaNhap]       DECIMAL (15, 2) NOT NULL,
    [SoLuongDaXuat]    INT             NOT NULL,
    [CreatedById]      BIGINT          NOT NULL,
    [LastUserId]       BIGINT          NOT NULL,
    [LastTime]         DATETIME        NOT NULL,
    [CreatedOn]        DATETIME        NOT NULL,
    [LastModified]     ROWVERSION      NOT NULL,
    CONSTRAINT [PK__NhapKhoQuaTangChiTiet] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_NhapKhoQuaTangChiTiet_NhapKhoQuaTang] FOREIGN KEY ([NhapKhoQuaTangId]) REFERENCES [dbo].[NhapKhoQuaTang] ([Id]),
    CONSTRAINT [FK_NhapKhoQuaTangChiTiet_QuaTang] FOREIGN KEY ([QuaTangId]) REFERENCES [dbo].[QuaTang] ([Id])
);
GO
CREATE TABLE [dbo].[XuatKhoQuaTang] (
    [Id]                BIGINT          IDENTITY (1, 1) NOT NULL,
    [SoPhieu]           AS              (concat('PX',right(datepart(year,[CreatedOn]),(2)),right('000000'+ltrim(str([Id])),(6)))),
    [NguoiXuatId]       BIGINT          NOT NULL,
    [BenhNhanId]        BIGINT          NOT NULL,
    [YeuCauGoiDichVuId] BIGINT          NULL,
    [NgayXuat]          DATETIME        NOT NULL,
    [GhiChu]            NVARCHAR (1000) NULL,
    [CreatedById]       BIGINT          NOT NULL,
    [LastUserId]        BIGINT          NOT NULL,
    [LastTime]          DATETIME        NOT NULL,
    [CreatedOn]         DATETIME        NOT NULL,
    [LastModified]      ROWVERSION      NOT NULL,
    CONSTRAINT [PK__XuatKhoQuaTang] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_XuatKhoQuaTang_BenhNhan] FOREIGN KEY ([BenhNhanId]) REFERENCES [dbo].[BenhNhan] ([Id]),
    CONSTRAINT [FK_XuatKhoQuaTang_NhanVien] FOREIGN KEY ([NguoiXuatId]) REFERENCES [dbo].[NhanVien] ([Id]),
    CONSTRAINT [FK_XuatKhoQuaTang_YeuCauGoiDichVu] FOREIGN KEY ([YeuCauGoiDichVuId]) REFERENCES [dbo].[YeuCauGoiDichVu] ([Id])
);
GO
CREATE TABLE [dbo].[XuatKhoQuaTangChiTiet] (
    [Id]                      BIGINT     IDENTITY (1, 1) NOT NULL,
    [XuatKhoQuaTangId]        BIGINT     NOT NULL,
    [QuaTangId]               BIGINT     NOT NULL,
    [NhapKhoQuaTangChiTietId] BIGINT     NOT NULL,
    [SoLuongXuat]             INT        NOT NULL,
    [CreatedById]             BIGINT     NOT NULL,
    [LastUserId]              BIGINT     NOT NULL,
    [LastTime]                DATETIME   NOT NULL,
    [CreatedOn]               DATETIME   NOT NULL,
    [LastModified]            ROWVERSION NOT NULL,
    CONSTRAINT [PK__XuatKhoQuaTangChiTiet] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_XuatKhoQuaTangChiTiet_NhapKhoQuaTangChiTiet] FOREIGN KEY ([NhapKhoQuaTangChiTietId]) REFERENCES [dbo].[NhapKhoQuaTangChiTiet] ([Id]),
    CONSTRAINT [FK_XuatKhoQuaTangChiTiet_QuaTang] FOREIGN KEY ([QuaTangId]) REFERENCES [dbo].[QuaTang] ([Id]),
    CONSTRAINT [FK_XuatKhoQuaTangChiTiet_XuatKhoQuaTang] FOREIGN KEY ([XuatKhoQuaTangId]) REFERENCES [dbo].[XuatKhoQuaTang] ([Id])
);
GO
ALTER TABLE [dbo].[VoucherChiTietMienGiam] ADD CONSTRAINT [FK_VoucherChiTietMienGiam_NhomDichVuBenhVien] FOREIGN KEY ([NhomDichVuBenhVienId]) REFERENCES [dbo].[NhomDichVuBenhVien] ([Id])
ALTER TABLE [dbo].[VoucherChiTietMienGiam] ADD CONSTRAINT [FK_VoucherChiTietMienGiam_NhomGiaDichVuKhamBenhBenhVien] FOREIGN KEY ([NhomGiaDichVuKhamBenhBenhVienId]) REFERENCES [dbo].[NhomGiaDichVuKhamBenhBenhVien] ([Id])
ALTER TABLE [dbo].[VoucherChiTietMienGiam] ADD CONSTRAINT [FK_VoucherChiTietMienGiam_NhomGiaDichVuKyThuatBenhVien] FOREIGN KEY ([NhomGiaDichVuKyThuatBenhVienId]) REFERENCES [dbo].[NhomGiaDichVuKyThuatBenhVien] ([Id])
GO
ALTER TABLE [dbo].[YeuCauGoiDichVu] ADD CONSTRAINT [FK_YeuCauGoiDichVu_BenhNhan] FOREIGN KEY ([BenhNhanId]) REFERENCES [dbo].[BenhNhan] ([Id])
ALTER TABLE [dbo].[YeuCauGoiDichVu] ADD CONSTRAINT [FK_YeuCauGoiDichVu_ChuongTrinhGoiDichVu] FOREIGN KEY ([ChuongTrinhGoiDichVuId]) REFERENCES [dbo].[ChuongTrinhGoiDichVu] ([Id])
ALTER TABLE [dbo].[YeuCauGoiDichVu] ADD CONSTRAINT [FK_YeuCauGoiDichVu_NhanVien] FOREIGN KEY ([NhanVienChiDinhId]) REFERENCES [dbo].[NhanVien] ([Id])
GO
Update CauHinh
Set [Value] = '0.9.8' where [Name] = 'CauHinhHeThong.DatabaseVesion'