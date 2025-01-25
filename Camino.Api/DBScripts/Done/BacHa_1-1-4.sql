CREATE TABLE [dbo].[DuTruMuaDuocPham] (
    [Id]                         BIGINT          IDENTITY (1, 1) NOT NULL,
    [SoPhieu]                    NVARCHAR (20)   NOT NULL,
    [NhomDuocPhamDuTru]          INT             NOT NULL,
    [KhoId]                      BIGINT          NOT NULL,
    [NhanVienYeuCauId]           BIGINT          NOT NULL,
    [NgayYeuCau]                 DATETIME        NOT NULL,
    [KyDuTruMuaDuocPhamVatTuId]  BIGINT          NOT NULL,
    [TuNgay]                     DATETIME        NOT NULL,
    [DenNgay]                    DATETIME        NOT NULL,
    [GhiChu]                     NVARCHAR (1000) NULL,
    [TruongKhoaDuyet]            BIT             NULL,
    [TruongKhoaId]               BIGINT          NULL,
    [NgayTruongKhoaDuyet]        DATETIME        NULL,
    [DuTruMuaDuocPhamTheoKhoaId] BIGINT          NULL,
    [LyDoTruongKhoaTuChoi]       NVARCHAR (1000) NULL,
    [DuTruMuaDuocPhamKhoDuocId]  BIGINT          NULL,
    [CreatedById]                BIGINT          NOT NULL,
    [LastUserId]                 BIGINT          NOT NULL,
    [LastTime]                   DATETIME        NOT NULL,
    [CreatedOn]                  DATETIME        NOT NULL,
    [LastModified]               ROWVERSION      NOT NULL,
    CONSTRAINT [PK__DuTruMuaDuocPham] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[DuTruMuaDuocPhamChiTiet] (
    [Id]                                BIGINT     IDENTITY (1, 1) NOT NULL,
    [DuTruMuaDuocPhamId]                BIGINT     NOT NULL,
    [DuocPhamId]                        BIGINT     NOT NULL,
    [LaDuocPhamBHYT]                    BIT        NOT NULL,
    [SoLuongDuTru]                      INT        NOT NULL,
    [SoLuongDuKienSuDung]               INT        NOT NULL,
    [SoLuongDuTruTruongKhoaDuyet]       INT        NULL,
    [NhomDieuTriDuPhong]                INT        NULL,
    [DuTruMuaDuocPhamTheoKhoaChiTietId] BIGINT     NULL,
    [DuTruMuaDuocPhamKhoDuocChiTietId]  BIGINT     NULL,
    [CreatedById]                       BIGINT     NOT NULL,
    [LastUserId]                        BIGINT     NOT NULL,
    [LastTime]                          DATETIME   NOT NULL,
    [CreatedOn]                         DATETIME   NOT NULL,
    [LastModified]                      ROWVERSION NOT NULL,
    CONSTRAINT [PK__DuTruMuaDuocPhamChiTiet] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[DuTruMuaDuocPhamKhoDuoc] (
    [Id]                        BIGINT          IDENTITY (1, 1) NOT NULL,
    [SoPhieu]                   NVARCHAR (20)   NOT NULL,
    [NhanVienYeuCauId]          BIGINT          NOT NULL,
    [NgayYeuCau]                DATETIME        NOT NULL,
    [KyDuTruMuaDuocPhamVatTuId] BIGINT          NOT NULL,
    [TuNgay]                    DATETIME        NOT NULL,
    [DenNgay]                   DATETIME        NOT NULL,
    [GhiChu]                    NVARCHAR (1000) NULL,
    [GiamDocDuyet]              BIT             NULL,
    [GiamDocId]                 BIGINT          NULL,
    [NgayGiamDocDuyet]          DATETIME        NULL,
    [LyDoGiamDocTuChoi]         NVARCHAR (1000) NULL,
    [CreatedById]               BIGINT          NOT NULL,
    [LastUserId]                BIGINT          NOT NULL,
    [LastTime]                  DATETIME        NOT NULL,
    [CreatedOn]                 DATETIME        NOT NULL,
    [LastModified]              ROWVERSION      NOT NULL,
    CONSTRAINT [PK__DuTruMuaDuocPhamKhoDuoc] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[DuTruMuaDuocPhamKhoDuocChiTiet] (
    [Id]                          BIGINT     IDENTITY (1, 1) NOT NULL,
    [DuTruMuaDuocPhamKhoDuocId]   BIGINT     NOT NULL,
    [DuocPhamId]                  BIGINT     NOT NULL,
    [LaDuocPhamBHYT]              BIT        NOT NULL,
    [SoLuongDuTru]                INT        NOT NULL,
    [SoLuongDuKienSuDung]         INT        NOT NULL,
    [SoLuongDuTruTruongKhoaDuyet] INT        NOT NULL,
    [SoLuongDuTruKhoDuocDuyet]    INT        NOT NULL,
    [SoLuongDuTruGiamDocDuyet]    INT        NULL,
    [CreatedById]                 BIGINT     NOT NULL,
    [LastUserId]                  BIGINT     NOT NULL,
    [LastTime]                    DATETIME   NOT NULL,
    [CreatedOn]                   DATETIME   NOT NULL,
    [LastModified]                ROWVERSION NOT NULL,
    CONSTRAINT [PK__DuTruMuaDuocPhamKhoDuocChiTiet] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[DuTruMuaDuocPhamTheoKhoa] (
    [Id]                        BIGINT          IDENTITY (1, 1) NOT NULL,
    [SoPhieu]                   NVARCHAR (20)   NOT NULL,
    [KhoaPhongId]               BIGINT          NOT NULL,
    [NhanVienYeuCauId]          BIGINT          NOT NULL,
    [NgayYeuCau]                DATETIME        NOT NULL,
    [KyDuTruMuaDuocPhamVatTuId] BIGINT          NOT NULL,
    [TuNgay]                    DATETIME        NOT NULL,
    [DenNgay]                   DATETIME        NOT NULL,
    [GhiChu]                    NVARCHAR (1000) NULL,
    [KhoDuocDuyet]              BIT             NULL,
    [NhanVienKhoDuocId]         BIGINT          NULL,
    [NgayKhoDuocDuyet]          DATETIME        NULL,
    [DuTruMuaDuocPhamKhoDuocId] BIGINT          NULL,
    [LyDoKhoDuocTuChoi]         NVARCHAR (1000) NULL,
    [CreatedById]               BIGINT          NOT NULL,
    [LastUserId]                BIGINT          NOT NULL,
    [LastTime]                  DATETIME        NOT NULL,
    [CreatedOn]                 DATETIME        NOT NULL,
    [LastModified]              ROWVERSION      NOT NULL,
    CONSTRAINT [PK__DuTruMuaDuocPhamTheoKhoa] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[DuTruMuaDuocPhamTheoKhoaChiTiet] (
    [Id]                               BIGINT     IDENTITY (1, 1) NOT NULL,
    [DuTruMuaDuocPhamTheoKhoaId]       BIGINT     NOT NULL,
    [DuocPhamId]                       BIGINT     NOT NULL,
    [LaDuocPhamBHYT]                   BIT        NOT NULL,
    [SoLuongDuTru]                     INT        NOT NULL,
    [SoLuongDuKienSuDung]              INT        NOT NULL,
    [SoLuongDuTruTruongKhoaDuyet]      INT        NOT NULL,
    [SoLuongDuTruKhoDuocDuyet]         INT        NULL,
    [DuTruMuaDuocPhamKhoDuocChiTietId] BIGINT     NULL,
    [CreatedById]                      BIGINT     NOT NULL,
    [LastUserId]                       BIGINT     NOT NULL,
    [LastTime]                         DATETIME   NOT NULL,
    [CreatedOn]                        DATETIME   NOT NULL,
    [LastModified]                     ROWVERSION NOT NULL,
    CONSTRAINT [PK__DuTruMuaDuocPhamTheoKhoaChiTiet] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[DuTruMuaVatTu] (
    [Id]                        BIGINT          IDENTITY (1, 1) NOT NULL,
    [SoPhieu]                   NVARCHAR (20)   NOT NULL,
    [KhoId]                     BIGINT          NOT NULL,
    [NhanVienYeuCauId]          BIGINT          NOT NULL,
    [NgayYeuCau]                DATETIME        NOT NULL,
    [KyDuTruMuaDuocPhamVatTuId] BIGINT          NOT NULL,
    [TuNgay]                    DATETIME        NOT NULL,
    [DenNgay]                   DATETIME        NOT NULL,
    [GhiChu]                    NVARCHAR (1000) NULL,
    [TruongKhoaDuyet]           BIT             NULL,
    [TruongKhoaId]              BIGINT          NULL,
    [NgayTruongKhoaDuyet]       DATETIME        NULL,
    [DuTruMuaVatTuTheoKhoaId]   BIGINT          NULL,
    [LyDoTruongKhoaTuChoi]      NVARCHAR (1000) NULL,
    [DuTruMuaVatTuKhoDuocId]    BIGINT          NULL,
    [CreatedById]               BIGINT          NOT NULL,
    [LastUserId]                BIGINT          NOT NULL,
    [LastTime]                  DATETIME        NOT NULL,
    [CreatedOn]                 DATETIME        NOT NULL,
    [LastModified]              ROWVERSION      NOT NULL,
    CONSTRAINT [PK__DuTruMuaVatTu] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[DuTruMuaVatTuChiTiet] (
    [Id]                             BIGINT     IDENTITY (1, 1) NOT NULL,
    [DuTruMuaVatTuId]                BIGINT     NOT NULL,
    [VatTuId]                        BIGINT     NOT NULL,
    [LaVatTuBHYT]                    BIT        NOT NULL,
    [SoLuongDuTru]                   INT        NOT NULL,
    [SoLuongDuKienSuDung]            INT        NOT NULL,
    [SoLuongDuTruTruongKhoaDuyet]    INT        NULL,
    [DuTruMuaVatTuTheoKhoaChiTietId] BIGINT     NULL,
    [DuTruMuaVatTuKhoDuocChiTietId]  BIGINT     NULL,
    [CreatedById]                    BIGINT     NOT NULL,
    [LastUserId]                     BIGINT     NOT NULL,
    [LastTime]                       DATETIME   NOT NULL,
    [CreatedOn]                      DATETIME   NOT NULL,
    [LastModified]                   ROWVERSION NOT NULL,
    CONSTRAINT [PK__DuTruMuaVatTuChiTiet] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[DuTruMuaVatTuKhoDuoc] (
    [Id]                        BIGINT          IDENTITY (1, 1) NOT NULL,
    [SoPhieu]                   NVARCHAR (20)   NOT NULL,
    [NhanVienYeuCauId]          BIGINT          NOT NULL,
    [NgayYeuCau]                DATETIME        NOT NULL,
    [KyDuTruMuaDuocPhamVatTuId] BIGINT          NOT NULL,
    [TuNgay]                    DATETIME        NOT NULL,
    [DenNgay]                   DATETIME        NOT NULL,
    [GhiChu]                    NVARCHAR (1000) NULL,
    [GiamDocDuyet]              BIT             NULL,
    [GiamDocId]                 BIGINT          NULL,
    [NgayGiamDocDuyet]          DATETIME        NULL,
    [LyDoGiamDocTuChoi]         NVARCHAR (1000) NULL,
    [CreatedById]               BIGINT          NOT NULL,
    [LastUserId]                BIGINT          NOT NULL,
    [LastTime]                  DATETIME        NOT NULL,
    [CreatedOn]                 DATETIME        NOT NULL,
    [LastModified]              ROWVERSION      NOT NULL,
    CONSTRAINT [PK__DuTruMuaVatTuKhoDuoc] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[DuTruMuaVatTuKhoDuocChiTiet] (
    [Id]                          BIGINT     IDENTITY (1, 1) NOT NULL,
    [DuTruMuaVatTuKhoDuocId]      BIGINT     NOT NULL,
    [VatTuId]                     BIGINT     NOT NULL,
    [LaVatTuBHYT]                 BIT        NOT NULL,
    [SoLuongDuTru]                INT        NOT NULL,
    [SoLuongDuKienSuDung]         INT        NOT NULL,
    [SoLuongDuTruTruongKhoaDuyet] INT        NOT NULL,
    [SoLuongDuTruKhoDuocDuyet]    INT        NOT NULL,
    [SoLuongDuTruGiamDocDuyet]    INT        NULL,
    [CreatedById]                 BIGINT     NOT NULL,
    [LastUserId]                  BIGINT     NOT NULL,
    [LastTime]                    DATETIME   NOT NULL,
    [CreatedOn]                   DATETIME   NOT NULL,
    [LastModified]                ROWVERSION NOT NULL,
    CONSTRAINT [PK__DuTruMuaVatTuKhoDuocChiTiet] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[DuTruMuaVatTuTheoKhoa] (
    [Id]                        BIGINT          IDENTITY (1, 1) NOT NULL,
    [SoPhieu]                   NVARCHAR (20)   NOT NULL,
    [KhoaPhongId]               BIGINT          NOT NULL,
    [NhanVienYeuCauId]          BIGINT          NOT NULL,
    [NgayYeuCau]                DATETIME        NOT NULL,
    [KyDuTruMuaDuocPhamVatTuId] BIGINT          NOT NULL,
    [TuNgay]                    DATETIME        NOT NULL,
    [DenNgay]                   DATETIME        NOT NULL,
    [GhiChu]                    NVARCHAR (1000) NULL,
    [KhoDuocDuyet]              BIT             NULL,
    [NhanVienKhoDuocId]         BIGINT          NULL,
    [NgayKhoDuocDuyet]          DATETIME        NULL,
    [DuTruMuaVatTuKhoDuocId]    BIGINT          NULL,
    [LyDoKhoDuocTuChoi]         NVARCHAR (1000) NULL,
    [CreatedById]               BIGINT          NOT NULL,
    [LastUserId]                BIGINT          NOT NULL,
    [LastTime]                  DATETIME        NOT NULL,
    [CreatedOn]                 DATETIME        NOT NULL,
    [LastModified]              ROWVERSION      NOT NULL,
    CONSTRAINT [PK__DuTruMuaVatTuTheoKhoa] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[DuTruMuaVatTuTheoKhoaChiTiet] (
    [Id]                            BIGINT     IDENTITY (1, 1) NOT NULL,
    [DuTruMuaVatTuTheoKhoaId]       BIGINT     NOT NULL,
    [VatTuId]                       BIGINT     NOT NULL,
    [LaVatTuBHYT]                   BIT        NOT NULL,
    [SoLuongDuTru]                  INT        NOT NULL,
    [SoLuongDuKienSuDung]           INT        NOT NULL,
    [SoLuongDuTruTruongKhoaDuyet]   INT        NOT NULL,
    [SoLuongDuTruKhoDuocDuyet]      INT        NULL,
    [DuTruMuaVatTuKhoDuocChiTietId] BIGINT     NULL,
    [CreatedById]                   BIGINT     NOT NULL,
    [LastUserId]                    BIGINT     NOT NULL,
    [LastTime]                      DATETIME   NOT NULL,
    [CreatedOn]                     DATETIME   NOT NULL,
    [LastModified]                  ROWVERSION NOT NULL,
    CONSTRAINT [PK__DuTruMuaVatTuTheoKhoaChiTiet] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[KyDuTruMuaDuocPhamVatTu] (
    [Id]            BIGINT          IDENTITY (1, 1) NOT NULL,
    [TuNgay]        DATETIME        NOT NULL,
    [DenNgay]       DATETIME        NOT NULL,
    [NhanVienTaoId] BIGINT          NOT NULL,
    [MuaDuocPham]   BIT             NULL,
    [MuaVatTu]      BIT             NULL,
    [MoTa]          NVARCHAR (1000) NULL,
    [HieuLuc]       BIT             NOT NULL,
    [CreatedById]   BIGINT          NOT NULL,
    [LastUserId]    BIGINT          NOT NULL,
    [LastTime]      DATETIME        NOT NULL,
    [CreatedOn]     DATETIME        NOT NULL,
    [LastModified]  ROWVERSION      NOT NULL,
    CONSTRAINT [PK__KyDuTruMuaDuocPhamVatTu] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
ALTER TABLE [dbo].[DuTruMuaDuocPham] ADD CONSTRAINT [FK_DuTruMuaDuocPham_KyDuTruMuaDuocPhamVatTu] FOREIGN KEY ([KyDuTruMuaDuocPhamVatTuId]) REFERENCES [dbo].[KyDuTruMuaDuocPhamVatTu] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaDuocPham] ADD CONSTRAINT [FK_DuTruMuaDuocPham_DuTruMuaDuocPhamTheoKhoa] FOREIGN KEY ([DuTruMuaDuocPhamTheoKhoaId]) REFERENCES [dbo].[DuTruMuaDuocPhamTheoKhoa] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaDuocPham] ADD CONSTRAINT [FK_DuTruMuaDuocPham_DuTruMuaDuocPhamKhoDuoc] FOREIGN KEY ([DuTruMuaDuocPhamKhoDuocId]) REFERENCES [dbo].[DuTruMuaDuocPhamKhoDuoc] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaDuocPham] ADD CONSTRAINT [FK_DuTruMuaDuocPham_Kho] FOREIGN KEY ([KhoId]) REFERENCES [dbo].[Kho] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaDuocPham] ADD CONSTRAINT [FK_DuTruMuaDuocPham_NhanVien] FOREIGN KEY ([NhanVienYeuCauId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaDuocPham] ADD CONSTRAINT [FK_DuTruMuaDuocPham_NhanVien1] FOREIGN KEY ([TruongKhoaId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaDuocPhamChiTiet] ADD CONSTRAINT [FK_DuTruMuaDuocPhamChiTiet_DuTruMuaDuocPhamKhoDuocChiTiet] FOREIGN KEY ([DuTruMuaDuocPhamKhoDuocChiTietId]) REFERENCES [dbo].[DuTruMuaDuocPhamKhoDuocChiTiet] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaDuocPhamChiTiet] ADD CONSTRAINT [FK_DuTruMuaDuocPhamChiTiet_DuTruMuaDuocPhamTheoKhoaChiTiet] FOREIGN KEY ([DuTruMuaDuocPhamTheoKhoaChiTietId]) REFERENCES [dbo].[DuTruMuaDuocPhamTheoKhoaChiTiet] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaDuocPhamChiTiet] ADD CONSTRAINT [FK_DuTruMuaDuocPhamChiTiet_DuTruMuaDuocPham] FOREIGN KEY ([DuTruMuaDuocPhamId]) REFERENCES [dbo].[DuTruMuaDuocPham] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaDuocPhamChiTiet] ADD CONSTRAINT [FK_DuTruMuaDuocPhamChiTiet_DuocPham] FOREIGN KEY ([DuocPhamId]) REFERENCES [dbo].[DuocPham] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaDuocPhamKhoDuoc] ADD CONSTRAINT [FK_DuTruMuaDuocPhamKhoDuoc_KyDuTruMuaDuocPhamVatTu] FOREIGN KEY ([KyDuTruMuaDuocPhamVatTuId]) REFERENCES [dbo].[KyDuTruMuaDuocPhamVatTu] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaDuocPhamKhoDuoc] ADD CONSTRAINT [FK_DuTruMuaDuocPhamKhoDuoc_NhanVien] FOREIGN KEY ([NhanVienYeuCauId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaDuocPhamKhoDuoc] ADD CONSTRAINT [FK_DuTruMuaDuocPhamKhoDuoc_NhanVien1] FOREIGN KEY ([GiamDocId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaDuocPhamKhoDuocChiTiet] ADD CONSTRAINT [FK_DuTruMuaDuocPhamKhoDuocChiTiet_DuTruMuaDuocPhamKhoDuoc] FOREIGN KEY ([DuTruMuaDuocPhamKhoDuocId]) REFERENCES [dbo].[DuTruMuaDuocPhamKhoDuoc] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaDuocPhamKhoDuocChiTiet] ADD CONSTRAINT [FK_DuTruMuaDuocPhamKhoDuocChiTiet_DuocPham] FOREIGN KEY ([DuocPhamId]) REFERENCES [dbo].[DuocPham] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaDuocPhamTheoKhoa] ADD CONSTRAINT [FK_DuTruMuaDuocPhamTheoKhoa_KyDuTruMuaDuocPhamVatTu] FOREIGN KEY ([KyDuTruMuaDuocPhamVatTuId]) REFERENCES [dbo].[KyDuTruMuaDuocPhamVatTu] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaDuocPhamTheoKhoa] ADD CONSTRAINT [FK_DuTruMuaDuocPhamTheoKhoa_DuTruMuaDuocPhamKhoDuoc] FOREIGN KEY ([DuTruMuaDuocPhamKhoDuocId]) REFERENCES [dbo].[DuTruMuaDuocPhamKhoDuoc] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaDuocPhamTheoKhoa] ADD CONSTRAINT [FK_DuTruMuaDuocPhamTheoKhoa_NhanVien] FOREIGN KEY ([NhanVienYeuCauId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaDuocPhamTheoKhoa] ADD CONSTRAINT [FK_DuTruMuaDuocPhamTheoKhoa_NhanVien1] FOREIGN KEY ([NhanVienKhoDuocId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaDuocPhamTheoKhoa] ADD CONSTRAINT [FK_DuTruMuaDuocPhamTheoKhoa_KhoaPhong] FOREIGN KEY ([KhoaPhongId]) REFERENCES [dbo].[KhoaPhong] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaDuocPhamTheoKhoaChiTiet] ADD CONSTRAINT [FK_DuTruMuaDuocPhamTheoKhoaChiTiet_DuTruMuaDuocPhamKhoDuocChiTiet] FOREIGN KEY ([DuTruMuaDuocPhamKhoDuocChiTietId]) REFERENCES [dbo].[DuTruMuaDuocPhamKhoDuocChiTiet] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaDuocPhamTheoKhoaChiTiet] ADD CONSTRAINT [FK_DuTruMuaDuocPhamTheoKhoaChiTiet_DuTruMuaDuocPhamTheoKhoa] FOREIGN KEY ([DuTruMuaDuocPhamTheoKhoaId]) REFERENCES [dbo].[DuTruMuaDuocPhamTheoKhoa] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaDuocPhamTheoKhoaChiTiet] ADD CONSTRAINT [FK_DuTruMuaDuocPhamTheoKhoaChiTiet_DuocPham] FOREIGN KEY ([DuocPhamId]) REFERENCES [dbo].[DuocPham] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaVatTu] ADD CONSTRAINT [FK_DuTruMuaVatTu_Kho] FOREIGN KEY ([KhoId]) REFERENCES [dbo].[Kho] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaVatTu] ADD CONSTRAINT [FK_DuTruMuaVatTu_NhanVien] FOREIGN KEY ([NhanVienYeuCauId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaVatTu] ADD CONSTRAINT [FK_DuTruMuaVatTu_KyDuTruMuaDuocPhamVatTu] FOREIGN KEY ([KyDuTruMuaDuocPhamVatTuId]) REFERENCES [dbo].[KyDuTruMuaDuocPhamVatTu] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaVatTu] ADD CONSTRAINT [FK_DuTruMuaVatTu_NhanVien1] FOREIGN KEY ([TruongKhoaId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaVatTu] ADD CONSTRAINT [FK_DuTruMuaVatTu_DuTruMuaVatTuTheoKhoa] FOREIGN KEY ([DuTruMuaVatTuTheoKhoaId]) REFERENCES [dbo].[DuTruMuaVatTuTheoKhoa] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaVatTu] ADD CONSTRAINT [FK_DuTruMuaVatTu_DuTruMuaVatTuKhoDuoc] FOREIGN KEY ([DuTruMuaVatTuKhoDuocId]) REFERENCES [dbo].[DuTruMuaVatTuKhoDuoc] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaVatTuChiTiet] ADD CONSTRAINT [FK_DuTruMuaVatTuChiTiet_DuTruMuaVatTu] FOREIGN KEY ([DuTruMuaVatTuId]) REFERENCES [dbo].[DuTruMuaVatTu] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaVatTuChiTiet] ADD CONSTRAINT [FK_DuTruMuaVatTuChiTiet_VatTu] FOREIGN KEY ([VatTuId]) REFERENCES [dbo].[VatTu] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaVatTuChiTiet] ADD CONSTRAINT [FK_DuTruMuaVatTuChiTiet_DuTruMuaVatTuTheoKhoaChiTiet] FOREIGN KEY ([DuTruMuaVatTuTheoKhoaChiTietId]) REFERENCES [dbo].[DuTruMuaVatTuTheoKhoaChiTiet] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaVatTuChiTiet] ADD CONSTRAINT [FK_DuTruMuaVatTuChiTiet_DuTruMuaVatTuKhoDuocChiTiet] FOREIGN KEY ([DuTruMuaVatTuKhoDuocChiTietId]) REFERENCES [dbo].[DuTruMuaVatTuKhoDuocChiTiet] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaVatTuKhoDuoc] ADD CONSTRAINT [FK_DuTruMuaVatTuKhoDuoc_NhanVien] FOREIGN KEY ([NhanVienYeuCauId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaVatTuKhoDuoc] ADD CONSTRAINT [FK_DuTruMuaVatTuKhoDuoc_KyDuTruMuaDuocPhamVatTu] FOREIGN KEY ([KyDuTruMuaDuocPhamVatTuId]) REFERENCES [dbo].[KyDuTruMuaDuocPhamVatTu] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaVatTuKhoDuoc] ADD CONSTRAINT [FK_DuTruMuaVatTuKhoDuoc_NhanVien1] FOREIGN KEY ([GiamDocId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaVatTuKhoDuocChiTiet] ADD CONSTRAINT [FK_DuTruMuaVatTuKhoDuocChiTiet_DuTruMuaVatTuKhoDuoc] FOREIGN KEY ([DuTruMuaVatTuKhoDuocId]) REFERENCES [dbo].[DuTruMuaVatTuKhoDuoc] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaVatTuKhoDuocChiTiet] ADD CONSTRAINT [FK_DuTruMuaVatTuKhoDuocChiTiet_VatTu] FOREIGN KEY ([VatTuId]) REFERENCES [dbo].[VatTu] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaVatTuTheoKhoa] ADD CONSTRAINT [FK_DuTruMuaVatTuTheoKhoa_KhoaPhong] FOREIGN KEY ([KhoaPhongId]) REFERENCES [dbo].[KhoaPhong] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaVatTuTheoKhoa] ADD CONSTRAINT [FK_DuTruMuaVatTuTheoKhoa_NhanVien] FOREIGN KEY ([NhanVienYeuCauId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaVatTuTheoKhoa] ADD CONSTRAINT [FK_DuTruMuaVatTuTheoKhoa_KyDuTruMuaDuocPhamVatTu] FOREIGN KEY ([KyDuTruMuaDuocPhamVatTuId]) REFERENCES [dbo].[KyDuTruMuaDuocPhamVatTu] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaVatTuTheoKhoa] ADD CONSTRAINT [FK_DuTruMuaVatTuTheoKhoa_NhanVien1] FOREIGN KEY ([NhanVienKhoDuocId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaVatTuTheoKhoa] ADD CONSTRAINT [FK_DuTruMuaVatTuTheoKhoa_DuTruMuaVatTuKhoDuoc] FOREIGN KEY ([DuTruMuaVatTuKhoDuocId]) REFERENCES [dbo].[DuTruMuaVatTuKhoDuoc] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaVatTuTheoKhoaChiTiet] ADD CONSTRAINT [FK_DuTruMuaVatTuTheoKhoaChiTiet_DuTruMuaVatTuTheoKhoa] FOREIGN KEY ([DuTruMuaVatTuTheoKhoaId]) REFERENCES [dbo].[DuTruMuaVatTuTheoKhoa] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaVatTuTheoKhoaChiTiet] ADD CONSTRAINT [FK_DuTruMuaVatTuTheoKhoaChiTiet_VatTu] FOREIGN KEY ([VatTuId]) REFERENCES [dbo].[VatTu] ([Id]);

GO
ALTER TABLE [dbo].[DuTruMuaVatTuTheoKhoaChiTiet] ADD CONSTRAINT [FK_DuTruMuaVatTuTheoKhoaChiTiet_DuTruMuaVatTuKhoDuocChiTiet] FOREIGN KEY ([DuTruMuaVatTuKhoDuocChiTietId]) REFERENCES [dbo].[DuTruMuaVatTuKhoDuocChiTiet] ([Id]);

GO
ALTER TABLE [dbo].[KyDuTruMuaDuocPhamVatTu] ADD CONSTRAINT [FK_KyDuTruMuaDuocPhamVatTu_NhanVien] FOREIGN KEY ([NhanVienTaoId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Thuốc - vacxin=1,Hóa chất - Hóa chất XN=2', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'DuTruMuaDuocPham', @level2type = N'COLUMN', @level2name = N'NhomDuocPhamDuTru';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Điều trị=1,Dự phòng=2', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'DuTruMuaDuocPhamChiTiet', @level2type = N'COLUMN', @level2name = N'NhomDieuTriDuPhong';

Update CauHinh
Set [Value] = '1.1.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'