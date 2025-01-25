 delete FROM XuatKhoDuocPhamChiTietViTri
 delete FROM NhapKhoDuocPhamChiTiet
 delete FROM DinhMucDuocPhamTonKho 
 delete FROM NhapKhoDuocPham 
 delete FROM XuatKhoDuocPhamChiTiet
 delete FROM XuatKhoDuocPham
 delete FROM [KhoDuocPhamViTri]
 delete FROM [KhoDuocPham]
GO
ALTER TABLE [dbo].[DinhMucDuocPhamTonKho] DROP CONSTRAINT [FK_DinhMucDuocPhamTonKho_KhoDuocPham]
GO
ALTER TABLE [dbo].[NhapKhoDuocPham] DROP CONSTRAINT [FK_NhapKhoDuocPham_KhoDuocPham]
GO
ALTER TABLE [dbo].[NhapKhoDuocPhamChiTiet] DROP CONSTRAINT [FK_NhapKhoDuocPhamChiTiet_KhoDuocPhamViTri]
GO
ALTER TABLE [dbo].[XuatKhoDuocPham] DROP CONSTRAINT [FK_XuatKhoDuocPham_KhoDuocPham1]
GO
ALTER TABLE [dbo].[XuatKhoDuocPham] DROP CONSTRAINT [FK_XuatKhoDuocPham_KhoDuocPham]
GO
DROP TABLE [dbo].[KhoDuocPhamViTri]
GO
DROP TABLE [dbo].[KhoDuocPham]


GO
CREATE TABLE [dbo].[CauHinhThapGia] (
    [Id]              BIGINT          IDENTITY (1, 1) NOT NULL,
    [LoaiThapGia]     INT             NOT NULL,
    [GiaTu]           DECIMAL (15, 2) NULL,
    [GiaDen]          DECIMAL (15, 2) NULL,
    [TiLeTheoThapGia] INT             NOT NULL,
    [CreatedById]     BIGINT          NOT NULL,
    [LastUserId]      BIGINT          NOT NULL,
    [LastTime]        DATETIME        NOT NULL,
    [CreatedOn]       DATETIME        NOT NULL,
    [LastModified]    ROWVERSION      NOT NULL,
    CONSTRAINT [PK_CauHinhThapGia] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[DinhMucVatTuTonKho] (
    [Id]                   BIGINT          IDENTITY (1, 1) NOT NULL,
    [VatTuBenhVienId]      BIGINT          NOT NULL,
    [KhoId]                BIGINT          NOT NULL,
    [TonToiThieu]          INT             NULL,
    [TonToiDa]             INT             NULL,
    [SoNgayTruocKhiHetHan] INT             NULL,
    [MoTa]                 NVARCHAR (2000) NULL,
    [CreatedById]          BIGINT          NOT NULL,
    [LastUserId]           BIGINT          NULL,
    [LastTime]             DATETIME        NOT NULL,
    [CreatedOn]            DATETIME        NOT NULL,
    [LastModified]         ROWVERSION      NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[DonVTYTThanhToan] (
    [Id]                      BIGINT          IDENTITY (1, 1) NOT NULL,
    [YeuCauKhamBenhDonVTYTId] BIGINT          NULL,
    [YeuCauKhamBenhId]        BIGINT          NULL,
    [YeuCauTiepNhanId]        BIGINT          NULL,
    [BenhNhanId]              BIGINT          NULL,
    [TrangThai]               INT             NOT NULL,
    [TrangThaiThanhToan]      INT             NOT NULL,
    [NhanVienHuyThanhToanId]  BIGINT          NULL,
    [LyDoHuyThanhToan]        NVARCHAR (1000) NULL,
    [NoiCapVTYTId]            BIGINT          NULL,
    [NhanVienCapVTYTId]       BIGINT          NULL,
    [ThoiDiemCapVTYT]         DATETIME        NULL,
    [GhiChu]                  NVARCHAR (1000) NULL,
    [CreatedById]             BIGINT          NOT NULL,
    [LastUserId]              BIGINT          NOT NULL,
    [LastTime]                DATETIME        NOT NULL,
    [CreatedOn]               DATETIME        NOT NULL,
    [LastModified]            ROWVERSION      NOT NULL,
    CONSTRAINT [PK__DonVTYTThanhToan] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[DonVTYTThanhToanChiTiet] (
    [Id]                             BIGINT          IDENTITY (1, 1) NOT NULL,
    [DonVTYTThanhToanId]             BIGINT          NOT NULL,
    [YeuCauKhamBenhDonVTYTChiTietId] BIGINT          NULL,
    [VatTuBenhVienId]                BIGINT          NOT NULL,
    [XuatKhoVatTuChiTietViTriId]     BIGINT          NOT NULL,
    [Ten]                            NVARCHAR (250)  NOT NULL,
    [Ma]                             NVARCHAR (50)   NOT NULL,
    [NhomVatTuId]                    BIGINT          NOT NULL,
    [DonViTinh]                      NVARCHAR (50)   NULL,
    [NhaSanXuat]                     NVARCHAR (250)  NULL,
    [NuocSanXuat]                    NVARCHAR (250)  NULL,
    [QuyCach]                        NVARCHAR (250)  NULL,
    [TieuChuan]                      NVARCHAR (50)   NULL,
    [MoTa]                           NVARCHAR (4000) NULL,
    [DonGiaNhap]                     DECIMAL (15, 2) NOT NULL,
    [TiLeTheoThapGia]                INT             NOT NULL,
    [VAT]                            INT             NOT NULL,
    [SoLuong]                        FLOAT (53)      NOT NULL,
    [HopDongThauVatTuId]             BIGINT          NULL,
    [NhaThauId]                      BIGINT          NULL,
    [SoHopDongThau]                  NVARCHAR (50)   NULL,
    [SoQuyetDinhThau]                NVARCHAR (50)   NULL,
    [LoaiThau]                       INT             NULL,
    [NhomThau]                       NVARCHAR (50)   NULL,
    [GoiThau]                        NVARCHAR (2)    NULL,
    [NamThau]                        INT             NULL,
    [SoTienBenhNhanDaChi]            DECIMAL (15, 2) NULL,
    [GhiChu]                         NVARCHAR (1000) NULL,
    [CreatedById]                    BIGINT          NOT NULL,
    [LastUserId]                     BIGINT          NOT NULL,
    [LastTime]                       DATETIME        NOT NULL,
    [CreatedOn]                      DATETIME        NOT NULL,
    [LastModified]                   ROWVERSION      NOT NULL,
    [SoTienBaoHiemTuNhanChiTra]      DECIMAL (15, 2) NULL,
    [SoTienMienGiam]                 DECIMAL (15, 2) NULL,
    CONSTRAINT [PK__DonVTYTThanhToanChiTiet] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[DuocPhamBenhVienPhanNhom] (
    [Id]           BIGINT         IDENTITY (1, 1) NOT NULL,
    [Ten]          NVARCHAR (250) NULL,
    [NhomChaId]    BIGINT         NULL,
    [CapNhom]      INT            NOT NULL,
    [CreatedById]  BIGINT         NOT NULL,
    [LastUserId]   BIGINT         NOT NULL,
    [LastTime]     DATETIME       NOT NULL,
    [CreatedOn]    DATETIME       NOT NULL,
    [LastModified] ROWVERSION     NOT NULL,
    CONSTRAINT [PK_DuocPhamBenhVienPhanNhom] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[DuocPhamVaVatTuBenhVien] (
    [Id]                    BIGINT         IDENTITY (1, 1) NOT NULL,
    [Ma]                    NVARCHAR (50)  NULL,
    [Ten]                   NVARCHAR (250) NOT NULL,
    [HoatChat]              NVARCHAR (550) NULL,
    [LoaiDuocPhamHoacVatTu] INT            NOT NULL,
    [DuocPhamBenhVienId]    BIGINT         NULL,
    [VatTuBenhVienId]       BIGINT         NULL,
    [HieuLuc]               BIT            NOT NULL,
    [CreatedById]           BIGINT         NOT NULL,
    [LastUserId]            BIGINT         NOT NULL,
    [LastTime]              DATETIME       NOT NULL,
    [CreatedOn]             DATETIME       NOT NULL,
    [LastModified]          ROWVERSION     NOT NULL,
    CONSTRAINT [PK__DuocPhamVaVatTuBenhVien] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[HopDongThauVatTu] (
    [Id]           BIGINT        IDENTITY (1, 1) NOT NULL,
    [NhaThauId]    BIGINT        NOT NULL,
    [SoHopDong]    NVARCHAR (50) NOT NULL,
    [SoQuyetDinh]  NVARCHAR (50) NOT NULL,
    [CongBo]       DATETIME      NOT NULL,
    [NgayKy]       DATETIME      NOT NULL,
    [NgayHieuLuc]  DATETIME      NOT NULL,
    [NgayHetHan]   DATETIME      NOT NULL,
    [LoaiThau]     INT           NOT NULL,
    [NhomThau]     NVARCHAR (50) NOT NULL,
    [GoiThau]      NVARCHAR (2)  NOT NULL,
    [Nam]          INT           NOT NULL,
    [CreatedById]  BIGINT        NOT NULL,
    [LastUserId]   BIGINT        NOT NULL,
    [LastTime]     DATETIME      NOT NULL,
    [CreatedOn]    DATETIME      NOT NULL,
    [LastModified] ROWVERSION    NOT NULL,
    CONSTRAINT [PK__HopDongThauVatTu] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[HopDongThauVatTuChiTiet] (
    [Id]                 BIGINT          IDENTITY (1, 1) NOT NULL,
    [HopDongThauVatTuId] BIGINT          NOT NULL,
    [VatTuId]            BIGINT          NOT NULL,
    [Gia]                DECIMAL (15, 2) NOT NULL,
    [SoLuong]            FLOAT (53)      NOT NULL,
    [SoLuongDaCap]       FLOAT (53)      NOT NULL,
    [CreatedById]        BIGINT          NOT NULL,
    [LastUserId]         BIGINT          NOT NULL,
    [LastTime]           DATETIME        NOT NULL,
    [CreatedOn]          DATETIME        NOT NULL,
    [LastModified]       ROWVERSION      NOT NULL,
    CONSTRAINT [PK__HopDongThauVatTuChiTiet] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[Kho] (
    [Id]              BIGINT        IDENTITY (1, 1) NOT NULL,
    [Ten]             NVARCHAR (50) NOT NULL,
    [LoaiKho]         INT           NOT NULL,
    [KhoaPhongId]     BIGINT        NULL,
    [PhongBenhVienId] BIGINT        NULL,
    [IsDefault]       BIT           NOT NULL,
    [CreatedById]     BIGINT        NOT NULL,
    [LastUserId]      BIGINT        NOT NULL,
    [LastTime]        DATETIME      NOT NULL,
    [CreatedOn]       DATETIME      NOT NULL,
    [LastModified]    ROWVERSION    NOT NULL,
    CONSTRAINT [PK_Kho] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[KhoNhanVienQuanLy] (
    [Id]           BIGINT     IDENTITY (1, 1) NOT NULL,
    [KhoId]        BIGINT     NOT NULL,
    [NhanVienId]   BIGINT     NOT NULL,
    [CreatedById]  BIGINT     NOT NULL,
    [LastUserId]   BIGINT     NOT NULL,
    [LastTime]     DATETIME   NOT NULL,
    [CreatedOn]    DATETIME   NOT NULL,
    [LastModified] ROWVERSION NOT NULL,
    CONSTRAINT [PK_KhoNhanVienQuanLy] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[KhoViTri] (
    [Id]           BIGINT          IDENTITY (1, 1) NOT NULL,
    [KhoId]        BIGINT          NOT NULL,
    [Ten]          NVARCHAR (120)  NOT NULL,
    [MoTa]         NVARCHAR (2000) NULL,
    [IsDisabled]   BIT             NULL,
    [CreatedById]  BIGINT          NOT NULL,
    [LastUserId]   BIGINT          NULL,
    [LastTime]     DATETIME        NOT NULL,
    [CreatedOn]    DATETIME        NOT NULL,
    [LastModified] ROWVERSION      NOT NULL,
    CONSTRAINT [PK_KhoViTri] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[NhapKhoVatTu] (
    [Id]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [YeuCauNhapKhoVatTuId] BIGINT         NULL,
    [KhoId]                BIGINT         NOT NULL,
    [YeuCauLinhVatTuId]    BIGINT         NULL,
    [SoChungTu]            NVARCHAR (50)  NULL,
    [XuatKhoVatTuId]       BIGINT         NULL,
    [TenNguoiGiao]         NVARCHAR (100) NULL,
    [NguoiGiaoId]          BIGINT         NULL,
    [NguoiNhapId]          BIGINT         NOT NULL,
    [LoaiNguoiGiao]        INT            NOT NULL,
    [NgayNhap]             DATETIME       NOT NULL,
    [DaHet]                BIT            NULL,
    [CreatedById]          BIGINT         NOT NULL,
    [LastUserId]           BIGINT         NOT NULL,
    [LastTime]             DATETIME       NOT NULL,
    [CreatedOn]            DATETIME       NOT NULL,
    [LastModified]         ROWVERSION     NOT NULL,
    CONSTRAINT [PK__NhapKhoVatTu] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[NhapKhoVatTuChiTiet] (
    [Id]                  BIGINT          IDENTITY (1, 1) NOT NULL,
    [NhapKhoVatTuId]      BIGINT          NOT NULL,
    [VatTuBenhVienId]     BIGINT          NOT NULL,
    [HopDongThauVatTuId]  BIGINT          NOT NULL,
    [LaVatTuBHYT]         BIT             NOT NULL,
    [NgayNhapVaoBenhVien] DATETIME        NOT NULL,
    [Solo]                NVARCHAR (50)   NOT NULL,
    [HanSuDung]           DATETIME        NOT NULL,
    [SoLuongNhap]         FLOAT (53)      NOT NULL,
    [DonGiaNhap]          DECIMAL (15, 2) NOT NULL,
    [TiLeTheoThapGia]     INT             NOT NULL,
    [VAT]                 INT             NOT NULL,
    [MaVach]              NVARCHAR (100)  NULL,
    [KhoViTriId]          BIGINT          NULL,
    [SoLuongDaXuat]       FLOAT (53)      NOT NULL,
    [NgayNhap]            DATETIME        NOT NULL,
    [CreatedById]         BIGINT          NOT NULL,
    [LastUserId]          BIGINT          NOT NULL,
    [LastTime]            DATETIME        NOT NULL,
    [CreatedOn]           DATETIME        NOT NULL,
    [LastModified]        ROWVERSION      NOT NULL,
    CONSTRAINT [PK__NhapKhoVatTuChiTiet] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[VatTuBenhVienGiaBaoHiem] (
    [Id]                   BIGINT          IDENTITY (1, 1) NOT NULL,
    [VatTuBenhVienId]      BIGINT          NOT NULL,
    [Gia]                  DECIMAL (15, 2) NOT NULL,
    [TuNgay]               DATETIME        NOT NULL,
    [DenNgay]              DATETIME        NULL,
    [TiLeBaoHiemThanhToan] INT             NOT NULL,
    [CreatedById]          BIGINT          NOT NULL,
    [LastUserId]           BIGINT          NOT NULL,
    [LastTime]             DATETIME        NOT NULL,
    [CreatedOn]            DATETIME        NOT NULL,
    [LastModified]         ROWVERSION      NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[XuatKhoVatTu] (
    [Id]                BIGINT          IDENTITY (1, 1) NOT NULL,
    [KhoXuatId]         BIGINT          NOT NULL,
    [KhoNhapId]         BIGINT          NULL,
    [YeuCauLinhVatTuId] BIGINT          NULL,
    [LoaiXuatKho]       INT             NOT NULL,
    [LyDoXuatKho]       NVARCHAR (1000) NOT NULL,
    [TenNguoiNhan]      NVARCHAR (100)  NULL,
    [NguoiNhanId]       BIGINT          NULL,
    [NguoiXuatId]       BIGINT          NOT NULL,
    [CreatedById]       BIGINT          NOT NULL,
    [LastUserId]        BIGINT          NOT NULL,
    [LastTime]          DATETIME        NOT NULL,
    [CreatedOn]         DATETIME        NOT NULL,
    [LastModified]      ROWVERSION      NOT NULL,
    [LoaiNguoiNhan]     INT             NOT NULL,
    [NgayXuat]          DATETIME        NOT NULL,
    [SoPhieu]           AS              (concat(RIGHT(datepart(year, [CreatedOn]), (2)), RIGHT('000000' + ltrim(str([Id])), (6)))),
    CONSTRAINT [PK__XuatKhoVatTu] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[XuatKhoVatTuChiTiet] (
    [Id]              BIGINT     IDENTITY (1, 1) NOT NULL,
    [XuatKhoVatTuId]  BIGINT     NULL,
    [VatTuBenhVienId] BIGINT     NOT NULL,
    [NgayXuat]        DATETIME   NULL,
    [CreatedById]     BIGINT     NOT NULL,
    [LastUserId]      BIGINT     NOT NULL,
    [LastTime]        DATETIME   NOT NULL,
    [CreatedOn]       DATETIME   NOT NULL,
    [LastModified]    ROWVERSION NOT NULL,
    CONSTRAINT [PK__XuatKhoVatTuChiTiet] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[XuatKhoVatTuChiTietViTri] (
    [Id]                    BIGINT     IDENTITY (1, 1) NOT NULL,
    [XuatKhoVatTuChiTietId] BIGINT     NOT NULL,
    [NhapKhoVatTuChiTietId] BIGINT     NOT NULL,
    [SoLuongXuat]           FLOAT (53) NOT NULL,
    [NgayXuat]              DATETIME   NULL,
    [CreatedById]           BIGINT     NOT NULL,
    [LastUserId]            BIGINT     NOT NULL,
    [LastTime]              DATETIME   NOT NULL,
    [CreatedOn]             DATETIME   NOT NULL,
    [LastModified]          ROWVERSION NOT NULL,
    CONSTRAINT [PK__XuatKhoVatTuChiTietViTri] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[YeuCauKhamBenhDonVTYT] (
    [Id]               BIGINT          IDENTITY (1, 1) NOT NULL,
    [YeuCauKhamBenhId] BIGINT          NOT NULL,
    [TrangThai]        INT             NOT NULL,
    [BacSiKeDonId]     BIGINT          NOT NULL,
    [NoiKeDonId]       BIGINT          NOT NULL,
    [ThoiDiemKeDon]    DATETIME        NOT NULL,
    [GhiChu]           NVARCHAR (1000) NULL,
    [CreatedById]      BIGINT          NOT NULL,
    [LastUserId]       BIGINT          NOT NULL,
    [LastTime]         DATETIME        NOT NULL,
    [CreatedOn]        DATETIME        NOT NULL,
    [LastModified]     ROWVERSION      NOT NULL,
    CONSTRAINT [PK__YeuCauKhamBenhDonVTYT] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[YeuCauKhamBenhDonVTYTChiTiet] (
    [Id]                      BIGINT          IDENTITY (1, 1) NOT NULL,
    [YeuCauKhamBenhDonVTYTId] BIGINT          NOT NULL,
    [VatTuBenhVienId]         BIGINT          NOT NULL,
    [Ten]                     NVARCHAR (250)  NOT NULL,
    [Ma]                      NVARCHAR (50)   NOT NULL,
    [NhomVatTuId]             BIGINT          NOT NULL,
    [DonViTinh]               NVARCHAR (50)   NULL,
    [NhaSanXuat]              NVARCHAR (250)  NULL,
    [NuocSanXuat]             NVARCHAR (250)  NULL,
    [QuyCach]                 NVARCHAR (250)  NULL,
    [TieuChuan]               NVARCHAR (50)   NULL,
    [MoTa]                    NVARCHAR (4000) NULL,
    [SoLuong]                 FLOAT (53)      NOT NULL,
    [GhiChu]                  NVARCHAR (1000) NULL,
    [CreatedById]             BIGINT          NOT NULL,
    [LastUserId]              BIGINT          NOT NULL,
    [LastTime]                DATETIME        NOT NULL,
    [CreatedOn]               DATETIME        NOT NULL,
    [LastModified]            ROWVERSION      NOT NULL,
    CONSTRAINT [PK__YeuCauKhamBenhDonVTYTChiTiet] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[YeuCauLinhDuocPham] (
    [Id]               BIGINT          IDENTITY (1, 1) NOT NULL,
    [KhoXuatId]        BIGINT          NOT NULL,
    [KhoNhapId]        BIGINT          NOT NULL,
    [LoaiPhieuLinh]    INT             NOT NULL,
    [NhanVienYeuCauId] BIGINT          NOT NULL,
    [NgayYeuCau]       DATETIME        NOT NULL,
    [GhiChu]           NVARCHAR (4000) NULL,
    [DuocDuyet]        BIT             NULL,
    [NgayDuyet]        DATETIME        NULL,
    [NhanVienDuyetId]  BIGINT          NULL,
    [LyDoKhongDuyet]   NVARCHAR (4000) NULL,
    [CreatedById]      BIGINT          NOT NULL,
    [LastUserId]       BIGINT          NOT NULL,
    [LastTime]         DATETIME        NOT NULL,
    [CreatedOn]        DATETIME        NOT NULL,
    [LastModified]     ROWVERSION      NOT NULL,
    CONSTRAINT [PK__YeuCauLinhDuocPham] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[YeuCauLinhDuocPhamChiTiet] (
    [Id]                   BIGINT     IDENTITY (1, 1) NOT NULL,
    [YeuCauLinhDuocPhamId] BIGINT     NOT NULL,
    [DuocPhamBenhVienId]   BIGINT     NOT NULL,
    [LaDuocPhamBHYT]       BIT        NOT NULL,
    [SoLuong]              FLOAT (53) NOT NULL,
    [CreatedById]          BIGINT     NOT NULL,
    [LastUserId]           BIGINT     NOT NULL,
    [LastTime]             DATETIME   NOT NULL,
    [CreatedOn]            DATETIME   NOT NULL,
    [LastModified]         ROWVERSION NOT NULL,
    CONSTRAINT [PK_YeuCauLinhDuocPhamChiTiet] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[YeuCauLinhVatTu] (
    [Id]               BIGINT          IDENTITY (1, 1) NOT NULL,
    [KhoXuatId]        BIGINT          NOT NULL,
    [KhoNhapId]        BIGINT          NOT NULL,
    [LoaiPhieuLinh]    INT             NOT NULL,
    [NhanVienYeuCauId] BIGINT          NOT NULL,
    [NgayYeuCau]       DATETIME        NOT NULL,
    [GhiChu]           NVARCHAR (4000) NULL,
    [DuocDuyet]        BIT             NULL,
    [NgayDuyet]        DATETIME        NULL,
    [NhanVienDuyetId]  BIGINT          NULL,
    [LyDoKhongDuyet]   NVARCHAR (4000) NULL,
    [CreatedById]      BIGINT          NOT NULL,
    [LastUserId]       BIGINT          NOT NULL,
    [LastTime]         DATETIME        NOT NULL,
    [CreatedOn]        DATETIME        NOT NULL,
    [LastModified]     ROWVERSION      NOT NULL,
    CONSTRAINT [PK__YeuCauLinhVatTu] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[YeuCauLinhVatTuChiTiet] (
    [Id]                BIGINT     IDENTITY (1, 1) NOT NULL,
    [YeuCauLinhVatTuId] BIGINT     NOT NULL,
    [VatTuBenhVienId]   BIGINT     NOT NULL,
    [LaVatTuBHYT]       BIT        NOT NULL,
    [SoLuong]           FLOAT (53) NOT NULL,
    [CreatedById]       BIGINT     NOT NULL,
    [LastUserId]        BIGINT     NOT NULL,
    [LastTime]          DATETIME   NOT NULL,
    [CreatedOn]         DATETIME   NOT NULL,
    [LastModified]      ROWVERSION NOT NULL,
    CONSTRAINT [PK_YeuCauLinhVatTuChiTiet] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[YeuCauNhapKhoDuocPham] (
    [Id]              BIGINT          IDENTITY (1, 1) NOT NULL,
    [KhoId]           BIGINT          NOT NULL,
    [SoChungTu]       NVARCHAR (50)   NOT NULL,
    [LoaiNguoiGiao]   INT             NOT NULL,
    [TenNguoiGiao]    NVARCHAR (100)  NULL,
    [NguoiGiaoId]     BIGINT          NULL,
    [NgayNhap]        DATETIME        NOT NULL,
    [NguoiNhapId]     BIGINT          NOT NULL,
    [DuocKeToanDuyet] BIT             NULL,
    [NgayDuyet]       DATETIME        NULL,
    [NhanVienDuyetId] BIGINT          NULL,
    [LyDoKhongDuyet]  NVARCHAR (4000) NULL,
    [CreatedById]     BIGINT          NOT NULL,
    [LastUserId]      BIGINT          NOT NULL,
    [LastTime]        DATETIME        NOT NULL,
    [CreatedOn]       DATETIME        NOT NULL,
    [LastModified]    ROWVERSION      NOT NULL,
    CONSTRAINT [PK__YeuCauNhapKhoDuocPham] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[YeuCauNhapKhoDuocPhamChiTiet] (
    [Id]                         BIGINT          IDENTITY (1, 1) NOT NULL,
    [YeuCauNhapKhoDuocPhamId]    BIGINT          NOT NULL,
    [DuocPhamBenhVienId]         BIGINT          NOT NULL,
    [HopDongThauDuocPhamId]      BIGINT          NOT NULL,
    [LaDuocPhamBHYT]             BIT             NOT NULL,
    [DuocPhamBenhVienPhanNhomId] BIGINT          NOT NULL,
    [Solo]                       NVARCHAR (50)   NOT NULL,
    [HanSuDung]                  DATETIME        NOT NULL,
    [SoLuongNhap]                FLOAT (53)      NOT NULL,
    [DonGiaNhap]                 DECIMAL (15, 2) NOT NULL,
    [TiLeTheoThapGia]            INT             NOT NULL,
    [VAT]                        INT             NOT NULL,
    [MaVach]                     NVARCHAR (100)  NULL,
    [KhoViTriId]                 BIGINT          NULL,
    [CreatedById]                BIGINT          NOT NULL,
    [LastUserId]                 BIGINT          NOT NULL,
    [LastTime]                   DATETIME        NOT NULL,
    [CreatedOn]                  DATETIME        NOT NULL,
    [LastModified]               ROWVERSION      NOT NULL,
    [NgayNhap]                   DATETIME        NOT NULL,
    CONSTRAINT [PK_YeuCauNhapKhoDuocPhamChiTiet] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[YeuCauNhapKhoVatTu] (
    [Id]              BIGINT          IDENTITY (1, 1) NOT NULL,
    [KhoId]           BIGINT          NOT NULL,
    [SoChungTu]       NVARCHAR (50)   NOT NULL,
    [LoaiNguoiGiao]   INT             NOT NULL,
    [TenNguoiGiao]    NVARCHAR (100)  NULL,
    [NguoiGiaoId]     BIGINT          NULL,
    [NgayNhap]        DATETIME        NOT NULL,
    [NguoiNhapId]     BIGINT          NOT NULL,
    [DuocKeToanDuyet] BIT             NULL,
    [NgayDuyet]       DATETIME        NULL,
    [NhanVienDuyetId] BIGINT          NULL,
    [LyDoKhongDuyet]  NVARCHAR (4000) NULL,
    [CreatedById]     BIGINT          NOT NULL,
    [LastUserId]      BIGINT          NOT NULL,
    [LastTime]        DATETIME        NOT NULL,
    [CreatedOn]       DATETIME        NOT NULL,
    [LastModified]    ROWVERSION      NOT NULL,
    CONSTRAINT [PK__YeuCauNhapKhoVatTu] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[YeuCauNhapKhoVatTuChiTiet] (
    [Id]                   BIGINT          IDENTITY (1, 1) NOT NULL,
    [YeuCauNhapKhoVatTuId] BIGINT          NOT NULL,
    [VatTuBenhVienId]      BIGINT          NOT NULL,
    [HopDongThauVatTuId]   BIGINT          NOT NULL,
    [LaVatTuBHYT]          BIT             NOT NULL,
    [Solo]                 NVARCHAR (50)   NOT NULL,
    [HanSuDung]            DATETIME        NOT NULL,
    [SoLuongNhap]          FLOAT (53)      NOT NULL,
    [DonGiaNhap]           DECIMAL (15, 2) NOT NULL,
    [TiLeTheoThapGia]      INT             NOT NULL,
    [VAT]                  INT             NOT NULL,
    [MaVach]               NVARCHAR (100)  NULL,
    [KhoViTriId]           BIGINT          NULL,
    [NgayNhap]             DATETIME        NOT NULL,
    [CreatedById]          BIGINT          NOT NULL,
    [LastUserId]           BIGINT          NOT NULL,
    [LastTime]             DATETIME        NOT NULL,
    [CreatedOn]            DATETIME        NOT NULL,
    [LastModified]         ROWVERSION      NOT NULL,
    CONSTRAINT [PK_YeuCauNhapKhoVatTuChiTiet] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[YeuCauTraDuocPham] (
    [Id]               BIGINT          IDENTITY (1, 1) NOT NULL,
    [KhoXuatId]        BIGINT          NOT NULL,
    [KhoNhapId]        BIGINT          NOT NULL,
    [NhanVienYeuCauId] BIGINT          NOT NULL,
    [NgayYeuCau]       DATETIME        NOT NULL,
    [GhiChu]           NVARCHAR (4000) NULL,
    [DuocDuyet]        BIT             NULL,
    [NgayDuyet]        DATETIME        NULL,
    [NhanVienDuyetId]  BIGINT          NULL,
    [LyDoKhongDuyet]   NVARCHAR (4000) NULL,
    [CreatedById]      BIGINT          NOT NULL,
    [LastUserId]       BIGINT          NOT NULL,
    [LastTime]         DATETIME        NOT NULL,
    [CreatedOn]        DATETIME        NOT NULL,
    [LastModified]     ROWVERSION      NOT NULL,
    CONSTRAINT [PK__YeuCauTraDuocPham] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[YeuCauTraDuocPhamChiTiet] (
    [Id]                            BIGINT     IDENTITY (1, 1) NOT NULL,
    [YeuCauTraDuocPhamId]           BIGINT     NOT NULL,
    [XuatKhoDuocPhamChiTietViTriId] BIGINT     NOT NULL,
    [CreatedById]                   BIGINT     NOT NULL,
    [LastUserId]                    BIGINT     NOT NULL,
    [LastTime]                      DATETIME   NOT NULL,
    [CreatedOn]                     DATETIME   NOT NULL,
    [LastModified]                  ROWVERSION NOT NULL,
    CONSTRAINT [PK_YeuCauTraDuocPhamChiTiet] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[YeuCauTraVatTu] (
    [Id]               BIGINT          IDENTITY (1, 1) NOT NULL,
    [KhoXuatId]        BIGINT          NOT NULL,
    [KhoNhapId]        BIGINT          NOT NULL,
    [NhanVienYeuCauId] BIGINT          NOT NULL,
    [NgayYeuCau]       DATETIME        NOT NULL,
    [GhiChu]           NVARCHAR (4000) NULL,
    [DuocDuyet]        BIT             NULL,
    [NgayDuyet]        DATETIME        NULL,
    [NhanVienDuyetId]  BIGINT          NULL,
    [LyDoKhongDuyet]   NVARCHAR (4000) NULL,
    [CreatedById]      BIGINT          NOT NULL,
    [LastUserId]       BIGINT          NOT NULL,
    [LastTime]         DATETIME        NOT NULL,
    [CreatedOn]        DATETIME        NOT NULL,
    [LastModified]     ROWVERSION      NOT NULL,
    CONSTRAINT [PK__YeuCauTraVatTu] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[YeuCauTraVatTuChiTiet] (
    [Id]                         BIGINT     IDENTITY (1, 1) NOT NULL,
    [YeuCauTraVatTuId]           BIGINT     NOT NULL,
    [XuatKhoVatTuChiTietViTriId] BIGINT     NOT NULL,
    [CreatedById]                BIGINT     NOT NULL,
    [LastUserId]                 BIGINT     NOT NULL,
    [LastTime]                   DATETIME   NOT NULL,
    [CreatedOn]                  DATETIME   NOT NULL,
    [LastModified]               ROWVERSION NOT NULL,
    CONSTRAINT [PK_YeuCauTraVatTuChiTiet] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE FULLTEXT CATALOG [Catalog_DuocPhamVaVatTuBenhVien]
    WITH ACCENT_SENSITIVITY = OFF
    AUTHORIZATION [dbo];
GO
CREATE FULLTEXT INDEX ON [dbo].[DuocPhamVaVatTuBenhVien]
    ([Ma] LANGUAGE 1066, [Ten] LANGUAGE 1066, [HoatChat] LANGUAGE 1066)
    KEY INDEX [PK__DuocPhamVaVatTuBenhVien]
    ON [Catalog_DuocPhamVaVatTuBenhVien];
GO
---------------------------------------------
ALTER TABLE dbo.[CongTyBaoHiemTuNhanCongNo]
ADD [DonVTYTThanhToanChiTietId]    BIGINT          NULL;
GO
ALTER TABLE [dbo].[CongTyBaoHiemTuNhanCongNo]  WITH CHECK ADD CONSTRAINT [FK_CongTyBaoHiemTuNhanCongNo_DonVTYTThanhToanChiTiet] FOREIGN KEY ([DonVTYTThanhToanChiTietId]) REFERENCES [dbo].[DonVTYTThanhToanChiTiet] ([Id])
GO

ALTER TABLE dbo.[DinhMucDuocPhamTonKho] DROP COLUMN [KhoDuocPhamId];
GO
ALTER TABLE dbo.[DinhMucDuocPhamTonKho] ADD [KhoId]                BIGINT          NOT NULL;
GO
ALTER TABLE [dbo].[DinhMucDuocPhamTonKho]  WITH CHECK ADD CONSTRAINT [FK_DinhMucDuocPhamTonKho_Kho] FOREIGN KEY ([KhoId]) REFERENCES [dbo].[Kho] ([Id])
GO

ALTER TABLE [dbo].[DinhMucVatTuTonKho]
    ADD CONSTRAINT [FK_DinhMucVatTuTonKho_Kho] FOREIGN KEY ([KhoId]) REFERENCES [dbo].[Kho] ([Id]);
GO
ALTER TABLE [dbo].[DinhMucVatTuTonKho]
    ADD CONSTRAINT [FK_DinhMucVatTuTonKho_VatTuBenhVien] FOREIGN KEY ([VatTuBenhVienId]) REFERENCES [dbo].[VatTuBenhVien] ([Id]);
GO

ALTER TABLE dbo.[DonThuocThanhToanChiTiet] ADD [DonGiaNhap]                      DECIMAL (15, 2) NOT NULL;
GO
ALTER TABLE dbo.[DonThuocThanhToanChiTiet] ADD [TiLeTheoThapGia]                 INT             NOT NULL;
GO
ALTER TABLE dbo.[DonThuocThanhToanChiTiet] ADD [VAT]                             INT             NOT NULL;
GO

ALTER TABLE [dbo].[DonVTYTThanhToan]
    ADD CONSTRAINT [FK_DonVTYTThanhToan_BenhNhan] FOREIGN KEY ([BenhNhanId]) REFERENCES [dbo].[BenhNhan] ([Id]);
GO
ALTER TABLE [dbo].[DonVTYTThanhToan]
    ADD CONSTRAINT [FK_DonVTYTThanhToan_NhanVien] FOREIGN KEY ([NhanVienHuyThanhToanId]) REFERENCES [dbo].[NhanVien] ([Id]);
GO
ALTER TABLE [dbo].[DonVTYTThanhToan]
    ADD CONSTRAINT [FK_DonVTYTThanhToan_NhanVien1] FOREIGN KEY ([NhanVienCapVTYTId]) REFERENCES [dbo].[NhanVien] ([Id]);
GO
ALTER TABLE [dbo].[DonVTYTThanhToan]
    ADD CONSTRAINT [FK_DonVTYTThanhToan_PhongBenhVien] FOREIGN KEY ([NoiCapVTYTId]) REFERENCES [dbo].[PhongBenhVien] ([Id]);
GO
ALTER TABLE [dbo].[DonVTYTThanhToan]
    ADD CONSTRAINT [FK_DonVTYTThanhToan_YeuCauKhamBenh] FOREIGN KEY ([YeuCauKhamBenhId]) REFERENCES [dbo].[YeuCauKhamBenh] ([Id]);
GO
ALTER TABLE [dbo].[DonVTYTThanhToan]
    ADD CONSTRAINT [FK_DonVTYTThanhToan_YeuCauKhamBenhDonVTYT] FOREIGN KEY ([YeuCauKhamBenhDonVTYTId]) REFERENCES [dbo].[YeuCauKhamBenhDonVTYT] ([Id]);
GO
ALTER TABLE [dbo].[DonVTYTThanhToan]
    ADD CONSTRAINT [FK_DonVTYTThanhToan_YeuCauTiepNhan] FOREIGN KEY ([YeuCauTiepNhanId]) REFERENCES [dbo].[YeuCauTiepNhan] ([Id]);
GO

ALTER TABLE [dbo].[DonVTYTThanhToanChiTiet]
    ADD CONSTRAINT [FK_DonVTYTThanhToanChiTiet_DonVTYTThanhToan] FOREIGN KEY ([DonVTYTThanhToanId]) REFERENCES [dbo].[DonVTYTThanhToan] ([Id]);
GO
ALTER TABLE [dbo].[DonVTYTThanhToanChiTiet]
    ADD CONSTRAINT [FK_DonVTYTThanhToanChiTiet_VatTuBenhVien] FOREIGN KEY ([VatTuBenhVienId]) REFERENCES [dbo].[VatTuBenhVien] ([Id]);
GO
ALTER TABLE [dbo].[DonVTYTThanhToanChiTiet]
    ADD CONSTRAINT [FK_DonVTYTThanhToanChiTiet_XuatKhoVatTuChiTietViTri] FOREIGN KEY ([XuatKhoVatTuChiTietViTriId]) REFERENCES [dbo].[XuatKhoVatTuChiTietViTri] ([Id]);
GO
ALTER TABLE [dbo].[DonVTYTThanhToanChiTiet]
    ADD CONSTRAINT [FK_DonVTYTThanhToanChiTiet_YeuCauKhamBenhDonVTYTChiTiet] FOREIGN KEY ([YeuCauKhamBenhDonVTYTChiTietId]) REFERENCES [dbo].[YeuCauKhamBenhDonVTYTChiTiet] ([Id]);
GO

ALTER TABLE dbo.[DuocPhamBenhVien] ADD [DuocPhamBenhVienPhanNhomId] BIGINT          NULL;
GO
ALTER TABLE [dbo].[DuocPhamBenhVien]
    ADD CONSTRAINT [FK_DuocPhamBenhVien_DuocPhamBenhVienPhanNhom] FOREIGN KEY ([DuocPhamBenhVienPhanNhomId]) REFERENCES [dbo].[DuocPhamBenhVienPhanNhom] ([Id]);
GO

ALTER TABLE [dbo].[DuocPhamBenhVienPhanNhom]
    ADD CONSTRAINT [FK_DuocPhamBenhVienPhanNhom_DuocPhamBenhVienPhanNhom] FOREIGN KEY ([NhomChaId]) REFERENCES [dbo].[DuocPhamBenhVienPhanNhom] ([Id]);
GO
------------------------------------------------
ALTER TABLE [dbo].[DuocPhamVaVatTuBenhVien]
    ADD CONSTRAINT [FK_DuocPhamVaVatTuBenhVien_DuocPhamBenhVien] FOREIGN KEY ([DuocPhamBenhVienId]) REFERENCES [dbo].[DuocPhamBenhVien] ([Id]);
GO
ALTER TABLE [dbo].[DuocPhamVaVatTuBenhVien]
    ADD CONSTRAINT [FK_DuocPhamVaVatTuBenhVien_VatTuBenhVien] FOREIGN KEY ([VatTuBenhVienId]) REFERENCES [dbo].[VatTuBenhVien] ([Id]);
GO
ALTER TABLE [dbo].[HopDongThauVatTu]
    ADD CONSTRAINT [FK_HopDongThauVatTu_NhaThau] FOREIGN KEY ([NhaThauId]) REFERENCES [dbo].[NhaThau] ([Id]);
GO
ALTER TABLE [dbo].[HopDongThauVatTuChiTiet]
    ADD CONSTRAINT [FK_HopDongThauVatTuChiTiet_HopDongThauVatTu] FOREIGN KEY ([HopDongThauVatTuId]) REFERENCES [dbo].[HopDongThauVatTu] ([Id]);
GO
ALTER TABLE [dbo].[HopDongThauVatTuChiTiet]
    ADD CONSTRAINT [FK_HopDongThauVatTuChiTiet_VatTu] FOREIGN KEY ([VatTuId]) REFERENCES [dbo].[VatTu] ([Id]);
GO
ALTER TABLE [dbo].[Kho]
    ADD CONSTRAINT [FK_Kho_KhoaPhong] FOREIGN KEY ([KhoaPhongId]) REFERENCES [dbo].[KhoaPhong] ([Id]);
GO
ALTER TABLE [dbo].[Kho]
    ADD CONSTRAINT [FK_Kho_PhongBenhVien] FOREIGN KEY ([PhongBenhVienId]) REFERENCES [dbo].[PhongBenhVien] ([Id]);
GO
ALTER TABLE [dbo].[KhoNhanVienQuanLy]
    ADD CONSTRAINT [FK_KhoNhanVienQuanLy_Kho] FOREIGN KEY ([KhoId]) REFERENCES [dbo].[Kho] ([Id]);
GO
ALTER TABLE [dbo].[KhoNhanVienQuanLy]
    ADD CONSTRAINT [FK_KhoNhanVienQuanLy_NhanVien] FOREIGN KEY ([NhanVienId]) REFERENCES [dbo].[NhanVien] ([Id]);
GO
ALTER TABLE [dbo].[KhoViTri]
    ADD CONSTRAINT [FK_KhoViTri_Kho] FOREIGN KEY ([KhoId]) REFERENCES [dbo].[Kho] ([Id]);
GO

ALTER TABLE dbo.[MienGiamChiPhi] ADD [DonVTYTThanhToanChiTietId]    BIGINT          NULL;
GO
ALTER TABLE [dbo].[MienGiamChiPhi]
    ADD CONSTRAINT [FK_MienGiamChiPhi_DonVTYTThanhToanChiTiet] FOREIGN KEY ([DonVTYTThanhToanChiTietId]) REFERENCES [dbo].[DonVTYTThanhToanChiTiet] ([Id]);
GO

ALTER TABLE dbo.[NhapKhoDuocPham] ADD [YeuCauNhapKhoDuocPhamId] BIGINT         NULL;
GO
ALTER TABLE dbo.[NhapKhoDuocPham] ADD [KhoId]                   BIGINT         NOT NULL;
GO
ALTER TABLE dbo.[NhapKhoDuocPham] ADD [YeuCauLinhDuocPhamId]    BIGINT         NULL;
GO
ALTER TABLE dbo.[NhapKhoDuocPham] DROP COLUMN [KhoDuocPhamId];
GO
ALTER TABLE dbo.[NhapKhoDuocPham] DROP COLUMN [LoaiNhapKho];
GO
ALTER TABLE dbo.[NhapKhoDuocPham] ALTER COLUMN [SoChungTu]               NVARCHAR (50)  NULL;
GO
ALTER TABLE [dbo].[NhapKhoDuocPham]
    ADD CONSTRAINT [FK_NhapKhoDuocPham_Kho] FOREIGN KEY ([KhoId]) REFERENCES [dbo].[Kho] ([Id]);
GO
ALTER TABLE [dbo].[NhapKhoDuocPham]
    ADD CONSTRAINT [FK_NhapKhoDuocPham_YeuCauLinhDuocPham] FOREIGN KEY ([YeuCauLinhDuocPhamId]) REFERENCES [dbo].[YeuCauLinhDuocPham] ([Id]);
GO
ALTER TABLE [dbo].[NhapKhoDuocPham]
    ADD CONSTRAINT [FK_NhapKhoDuocPham_YeuCauNhapKhoDuocPham] FOREIGN KEY ([YeuCauNhapKhoDuocPhamId]) REFERENCES [dbo].[YeuCauNhapKhoDuocPham] ([Id]);
GO
-------------------------------------------------------------------------------------
ALTER TABLE dbo.[NhapKhoDuocPhamChiTiet] ADD [LaDuocPhamBHYT]             BIT             NOT NULL;
GO
ALTER TABLE dbo.[NhapKhoDuocPhamChiTiet] ADD [DuocPhamBenhVienPhanNhomId] BIGINT          NOT NULL;
GO
ALTER TABLE dbo.[NhapKhoDuocPhamChiTiet] ADD [NgayNhapVaoBenhVien]        DATETIME        NOT NULL;
GO
ALTER TABLE dbo.[NhapKhoDuocPhamChiTiet] ADD [TiLeTheoThapGia]            INT             NOT NULL;
GO
ALTER TABLE dbo.[NhapKhoDuocPhamChiTiet] ADD [KhoViTriId]                 BIGINT          NULL;
GO
ALTER TABLE dbo.[NhapKhoDuocPhamChiTiet] DROP COLUMN [DatChatLuong];
GO
ALTER TABLE dbo.[NhapKhoDuocPhamChiTiet] DROP COLUMN [DonGiaBan];
GO
ALTER TABLE dbo.[NhapKhoDuocPhamChiTiet] DROP COLUMN [ChietKhau];
GO
ALTER TABLE dbo.[NhapKhoDuocPhamChiTiet] DROP COLUMN [KhoDuocPhamViTriId];
GO
ALTER TABLE dbo.[NhapKhoDuocPhamChiTiet] ALTER COLUMN [VAT]                        INT             NOT NULL;
GO
ALTER TABLE [dbo].[NhapKhoDuocPhamChiTiet]
    ADD CONSTRAINT [FK_NhapKhoDuocPhamChiTiet_DuocPhamBenhVienPhanNhom] FOREIGN KEY ([DuocPhamBenhVienPhanNhomId]) REFERENCES [dbo].[DuocPhamBenhVienPhanNhom] ([Id]);
GO
ALTER TABLE [dbo].[NhapKhoDuocPhamChiTiet]
    ADD CONSTRAINT [FK_NhapKhoDuocPhamChiTiet_KhoViTri] FOREIGN KEY ([KhoViTriId]) REFERENCES [dbo].[KhoViTri] ([Id]);
GO

ALTER TABLE [dbo].[NhapKhoVatTu]
    ADD CONSTRAINT [FK_NhapKhoVatTu_Kho] FOREIGN KEY ([KhoId]) REFERENCES [dbo].[Kho] ([Id]);
GO
ALTER TABLE [dbo].[NhapKhoVatTu]
    ADD CONSTRAINT [FK_NhapKhoVatTu_NhanVien] FOREIGN KEY ([NguoiGiaoId]) REFERENCES [dbo].[NhanVien] ([Id]);
GO
ALTER TABLE [dbo].[NhapKhoVatTu]
    ADD CONSTRAINT [FK_NhapKhoVatTu_NhanVien1] FOREIGN KEY ([NguoiNhapId]) REFERENCES [dbo].[NhanVien] ([Id]);
GO
ALTER TABLE [dbo].[NhapKhoVatTu]
    ADD CONSTRAINT [FK_NhapKhoVatTu_YeuCauLinhVatTu] FOREIGN KEY ([YeuCauLinhVatTuId]) REFERENCES [dbo].[YeuCauLinhVatTu] ([Id]);
GO
ALTER TABLE [dbo].[NhapKhoVatTu]
    ADD CONSTRAINT [FK_NhapKhoVatTu_YeuCauNhapKhoVatTu] FOREIGN KEY ([YeuCauNhapKhoVatTuId]) REFERENCES [dbo].[YeuCauNhapKhoVatTu] ([Id]);
GO
ALTER TABLE [dbo].[NhapKhoVatTuChiTiet]
    ADD CONSTRAINT [FK_NhapKhoVatTuChiTiet_HopDongThauVatTu] FOREIGN KEY ([HopDongThauVatTuId]) REFERENCES [dbo].[HopDongThauVatTu] ([Id]);
GO
ALTER TABLE [dbo].[NhapKhoVatTuChiTiet]
    ADD CONSTRAINT [FK_NhapKhoVatTuChiTiet_KhoViTri] FOREIGN KEY ([KhoViTriId]) REFERENCES [dbo].[KhoViTri] ([Id]);
GO
ALTER TABLE [dbo].[NhapKhoVatTuChiTiet]
    ADD CONSTRAINT [FK_NhapKhoVatTuChiTiet_NhapKhoVatTu] FOREIGN KEY ([NhapKhoVatTuId]) REFERENCES [dbo].[NhapKhoVatTu] ([Id]);
GO
ALTER TABLE [dbo].[NhapKhoVatTuChiTiet]
    ADD CONSTRAINT [FK_NhapKhoVatTuChiTiet_VatTuBenhVien] FOREIGN KEY ([VatTuBenhVienId]) REFERENCES [dbo].[VatTuBenhVien] ([Id]);
GO
ALTER TABLE dbo.[PhuongPhapVoCam] ALTER COLUMN [MoTa]         NVARCHAR (2000) NULL;
GO
ALTER TABLE dbo.[TaiKhoanBenhNhanChi] ADD [DonVTYTThanhToanChiTietId]    BIGINT          NULL;
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanChi]
    ADD CONSTRAINT [FK_TaiKhoanBenhNhanChi_DonVTYTThanhToanChiTiet] FOREIGN KEY ([DonVTYTThanhToanChiTietId]) REFERENCES [dbo].[DonVTYTThanhToanChiTiet] ([Id]);
GO
ALTER TABLE dbo.[TaiKhoanBenhNhanThu] ADD [HoanTraDonVTYTThanhToanId]           BIGINT          NULL;
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanThu]
    ADD CONSTRAINT [FK_TaiKhoanBenhNhanThu_DonVTYTThanhToan] FOREIGN KEY ([HoanTraDonVTYTThanhToanId]) REFERENCES [dbo].[DonVTYTThanhToan] ([Id]);
GO
ALTER TABLE dbo.[VatTuBenhVien] ADD [LoaiSuDung]               INT             NULL;
GO
ALTER TABLE dbo.[VatTuBenhVien] ADD [DieuKienBaoHiemThanhToan] NVARCHAR (4000) NULL;
GO
ALTER TABLE dbo.[VatTuBenhVien] DROP COLUMN [BaoHiemChiTra];
GO
ALTER TABLE dbo.[VatTuBenhVien] DROP COLUMN [TiLeBaoHiemThanhToan];
GO
ALTER TABLE [dbo].[VatTuBenhVienGiaBaoHiem]
    ADD CONSTRAINT [FK_VatTuBenhVienGiaBaoHiem_VatTuBenhVien] FOREIGN KEY ([VatTuBenhVienId]) REFERENCES [dbo].[VatTuBenhVien] ([Id]);
GO
-----------------------------------------------------------------------------
ALTER TABLE dbo.[XuatKhoDuocPham] DROP COLUMN [KhoDuocPhamXuatId];
GO
ALTER TABLE dbo.[XuatKhoDuocPham] DROP COLUMN [KhoDuocPhamNhapId];
GO
ALTER TABLE dbo.[XuatKhoDuocPham] DROP COLUMN [SoPhieu];
GO
ALTER TABLE dbo.[XuatKhoDuocPham] ADD [KhoXuatId]            BIGINT          NOT NULL;
GO
ALTER TABLE dbo.[XuatKhoDuocPham] ADD [KhoNhapId]            BIGINT          NULL;
GO
ALTER TABLE dbo.[XuatKhoDuocPham] ADD [YeuCauLinhDuocPhamId] BIGINT          NULL;
GO
ALTER TABLE dbo.[XuatKhoDuocPham] ADD [SoPhieu]              AS              (concat(right(datepart(year,[CreatedOn]),(2)),right('000000'+ltrim(str([Id])),(6))));
GO
ALTER TABLE [dbo].[XuatKhoDuocPham]
    ADD CONSTRAINT [FK_XuatKhoDuocPham_Kho] FOREIGN KEY ([KhoXuatId]) REFERENCES [dbo].[Kho] ([Id]);
GO
ALTER TABLE [dbo].[XuatKhoDuocPham]
    ADD CONSTRAINT [FK_XuatKhoDuocPham_Kho1] FOREIGN KEY ([KhoNhapId]) REFERENCES [dbo].[Kho] ([Id]);
GO
ALTER TABLE [dbo].[XuatKhoDuocPham]
    ADD CONSTRAINT [FK_XuatKhoDuocPham_YeuCauLinhDuocPham] FOREIGN KEY ([YeuCauLinhDuocPhamId]) REFERENCES [dbo].[YeuCauLinhDuocPham] ([Id]);
GO
ALTER TABLE dbo.[XuatKhoDuocPhamChiTiet] DROP COLUMN [DatChatLuong];
GO
ALTER TABLE dbo.[XuatKhoDuocPhamChiTiet] ALTER COLUMN [XuatKhoDuocPhamId]  BIGINT     NULL;
GO
ALTER TABLE dbo.[XuatKhoDuocPhamChiTiet] ALTER COLUMN [NgayXuat]           DATETIME   NULL;
GO
ALTER TABLE dbo.[XuatKhoDuocPhamChiTietViTri] ALTER COLUMN [NgayXuat]           DATETIME   NULL;
GO
ALTER TABLE [dbo].[XuatKhoVatTu]
    ADD CONSTRAINT [FK_XuatKhoVatTu_Kho] FOREIGN KEY ([KhoXuatId]) REFERENCES [dbo].[Kho] ([Id]);
GO
ALTER TABLE [dbo].[XuatKhoVatTu]
    ADD CONSTRAINT [FK_XuatKhoVatTu_Kho1] FOREIGN KEY ([KhoNhapId]) REFERENCES [dbo].[Kho] ([Id]);
GO
ALTER TABLE [dbo].[XuatKhoVatTu]
    ADD CONSTRAINT [FK_XuatKhoVatTu_NhanVien] FOREIGN KEY ([NguoiNhanId]) REFERENCES [dbo].[NhanVien] ([Id]);
GO
ALTER TABLE [dbo].[XuatKhoVatTu]
    ADD CONSTRAINT [FK_XuatKhoVatTu_NhanVien1] FOREIGN KEY ([NguoiXuatId]) REFERENCES [dbo].[NhanVien] ([Id]);
GO
ALTER TABLE [dbo].[XuatKhoVatTu]
    ADD CONSTRAINT [FK_XuatKhoVatTu_YeuCauLinhVatTu] FOREIGN KEY ([YeuCauLinhVatTuId]) REFERENCES [dbo].[YeuCauLinhVatTu] ([Id]);
GO
ALTER TABLE [dbo].[XuatKhoVatTuChiTiet]
    ADD CONSTRAINT [FK_XuatKhoVatTuChiTiet_VatTuBenhVien] FOREIGN KEY ([VatTuBenhVienId]) REFERENCES [dbo].[VatTuBenhVien] ([Id]);
GO
ALTER TABLE [dbo].[XuatKhoVatTuChiTiet]
    ADD CONSTRAINT [FK_XuatKhoVatTuChiTiet_XuatKhoVatTu] FOREIGN KEY ([XuatKhoVatTuId]) REFERENCES [dbo].[XuatKhoVatTu] ([Id]);
GO
ALTER TABLE [dbo].[XuatKhoVatTuChiTietViTri]
    ADD CONSTRAINT [FK_XuatKhoVatTuChiTietViTri_NhapKhoVatTuChiTiet] FOREIGN KEY ([NhapKhoVatTuChiTietId]) REFERENCES [dbo].[NhapKhoVatTuChiTiet] ([Id]);
GO
ALTER TABLE [dbo].[XuatKhoVatTuChiTietViTri]
    ADD CONSTRAINT [FK_XuatKhoVatTuChiTietViTri_XuatKhoVatTuChiTiet] FOREIGN KEY ([XuatKhoVatTuChiTietId]) REFERENCES [dbo].[XuatKhoVatTuChiTiet] ([Id]);
GO
--------------------------------------------------------------------------------------
ALTER TABLE dbo.[YeuCauDuocPhamBenhVien] ADD [XuatKhoDuocPhamChiTietId]  BIGINT          NULL;
GO
ALTER TABLE dbo.[YeuCauDuocPhamBenhVien] ADD [LoaiPhieuLinh]             INT             NOT NULL;
GO
ALTER TABLE dbo.[YeuCauDuocPhamBenhVien] ADD [KhoLinhId]                 BIGINT          NULL;
GO
ALTER TABLE dbo.[YeuCauDuocPhamBenhVien] ADD [YeuCauLinhDuocPhamId]      BIGINT          NULL;
GO
ALTER TABLE dbo.[YeuCauDuocPhamBenhVien] ADD [LaDuocPhamBHYT]            BIT             NOT NULL;
GO
ALTER TABLE dbo.[YeuCauDuocPhamBenhVien] ADD [DonGiaNhap]                DECIMAL (15, 2) NOT NULL;
GO
ALTER TABLE dbo.[YeuCauDuocPhamBenhVien] ADD [TiLeTheoThapGia]           INT             NOT NULL;
GO
ALTER TABLE dbo.[YeuCauDuocPhamBenhVien] ADD [VAT]                       INT             NOT NULL;
GO
ALTER TABLE dbo.[YeuCauDuocPhamBenhVien] DROP CONSTRAINT [FK_YeuCauDuocPhamBenhVien_XuatKhoDuocPhamChiTietViTri];
GO
ALTER TABLE dbo.[YeuCauDuocPhamBenhVien] DROP COLUMN [XuatKhoDuocPhamChiTietViTriId];
GO
ALTER TABLE dbo.[YeuCauDuocPhamBenhVien] DROP COLUMN [Gia];
GO
ALTER TABLE [dbo].[YeuCauDuocPhamBenhVien]
    ADD CONSTRAINT [FK_YeuCauDuocPhamBenhVien_Kho] FOREIGN KEY ([KhoLinhId]) REFERENCES [dbo].[Kho] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauDuocPhamBenhVien]
    ADD CONSTRAINT [FK_YeuCauDuocPhamBenhVien_XuatKhoDuocPhamChiTiet] FOREIGN KEY ([XuatKhoDuocPhamChiTietId]) REFERENCES [dbo].[XuatKhoDuocPhamChiTiet] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauDuocPhamBenhVien]
    ADD CONSTRAINT [FK_YeuCauDuocPhamBenhVien_YeuCauLinhDuocPham] FOREIGN KEY ([YeuCauLinhDuocPhamId]) REFERENCES [dbo].[YeuCauLinhDuocPham] ([Id]);
GO
------------------------------------------------------------------------------------------------
ALTER TABLE [dbo].[YeuCauKhamBenhDonVTYT]
    ADD CONSTRAINT [FK_YeuCauKhamBenhDonVTYT_NhanVien] FOREIGN KEY ([BacSiKeDonId]) REFERENCES [dbo].[NhanVien] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauKhamBenhDonVTYT]
    ADD CONSTRAINT [FK_YeuCauKhamBenhDonVTYT_PhongBenhVien] FOREIGN KEY ([NoiKeDonId]) REFERENCES [dbo].[PhongBenhVien] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauKhamBenhDonVTYT]
    ADD CONSTRAINT [FK_YeuCauKhamBenhDonVTYT_YeuCauKhamBenh] FOREIGN KEY ([YeuCauKhamBenhId]) REFERENCES [dbo].[YeuCauKhamBenh] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauKhamBenhDonVTYTChiTiet]
    ADD CONSTRAINT [FK_YeuCauKhamBenhDonVTYTChiTiet_VatTuBenhVien] FOREIGN KEY ([VatTuBenhVienId]) REFERENCES [dbo].[VatTuBenhVien] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauKhamBenhDonVTYTChiTiet]
    ADD CONSTRAINT [FK_YeuCauKhamBenhDonVTYTChiTiet_YeuCauKhamBenhDonVTYT] FOREIGN KEY ([YeuCauKhamBenhDonVTYTId]) REFERENCES [dbo].[YeuCauKhamBenhDonVTYT] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauLinhDuocPham]
    ADD CONSTRAINT [FK_YeuCauLinhDuocPham_Kho] FOREIGN KEY ([KhoXuatId]) REFERENCES [dbo].[Kho] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauLinhDuocPham]
    ADD CONSTRAINT [FK_YeuCauLinhDuocPham_Kho1] FOREIGN KEY ([KhoNhapId]) REFERENCES [dbo].[Kho] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauLinhDuocPham]
    ADD CONSTRAINT [FK_YeuCauLinhDuocPham_NhanVien] FOREIGN KEY ([NhanVienDuyetId]) REFERENCES [dbo].[NhanVien] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauLinhDuocPham]
    ADD CONSTRAINT [FK_YeuCauLinhDuocPham_NhanVien1] FOREIGN KEY ([NhanVienYeuCauId]) REFERENCES [dbo].[NhanVien] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauLinhDuocPhamChiTiet]
    ADD CONSTRAINT [FK_YeuCauLinhDuocPhamChiTiet_DuocPhamBenhVien] FOREIGN KEY ([DuocPhamBenhVienId]) REFERENCES [dbo].[DuocPhamBenhVien] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauLinhDuocPhamChiTiet]
    ADD CONSTRAINT [FK_YeuCauLinhDuocPhamChiTiet_YeuCauLinhDuocPham] FOREIGN KEY ([YeuCauLinhDuocPhamId]) REFERENCES [dbo].[YeuCauLinhDuocPham] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauLinhVatTu]
    ADD CONSTRAINT [FK_YeuCauLinhVatTu_Kho] FOREIGN KEY ([KhoXuatId]) REFERENCES [dbo].[Kho] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauLinhVatTu]
    ADD CONSTRAINT [FK_YeuCauLinhVatTu_Kho1] FOREIGN KEY ([KhoNhapId]) REFERENCES [dbo].[Kho] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauLinhVatTu]
    ADD CONSTRAINT [FK_YeuCauLinhVatTu_NhanVien] FOREIGN KEY ([NhanVienDuyetId]) REFERENCES [dbo].[NhanVien] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauLinhVatTu]
    ADD CONSTRAINT [FK_YeuCauLinhVatTu_NhanVien1] FOREIGN KEY ([NhanVienYeuCauId]) REFERENCES [dbo].[NhanVien] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauLinhVatTuChiTiet]
    ADD CONSTRAINT [FK_YeuCauLinhVatTuChiTiet_VatTuBenhVien] FOREIGN KEY ([VatTuBenhVienId]) REFERENCES [dbo].[VatTuBenhVien] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauLinhVatTuChiTiet]
    ADD CONSTRAINT [FK_YeuCauLinhVatTuChiTiet_YeuCauLinhVatTu] FOREIGN KEY ([YeuCauLinhVatTuId]) REFERENCES [dbo].[YeuCauLinhVatTu] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauNhapKhoDuocPham]
    ADD CONSTRAINT [FK_YeuCauNhapKhoDuocPham_Kho] FOREIGN KEY ([KhoId]) REFERENCES [dbo].[Kho] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauNhapKhoDuocPham]
    ADD CONSTRAINT [FK_YeuCauNhapKhoDuocPham_NhanVien] FOREIGN KEY ([NguoiGiaoId]) REFERENCES [dbo].[NhanVien] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauNhapKhoDuocPham]
    ADD CONSTRAINT [FK_YeuCauNhapKhoDuocPham_NhanVien1] FOREIGN KEY ([NguoiNhapId]) REFERENCES [dbo].[NhanVien] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauNhapKhoDuocPham]
    ADD CONSTRAINT [FK_YeuCauNhapKhoDuocPham_NhanVien2] FOREIGN KEY ([NhanVienDuyetId]) REFERENCES [dbo].[NhanVien] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauNhapKhoDuocPhamChiTiet]
    ADD CONSTRAINT [FK_YeuCauNhapKhoDuocPhamChiTiet_DuocPhamBenhVien] FOREIGN KEY ([DuocPhamBenhVienId]) REFERENCES [dbo].[DuocPhamBenhVien] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauNhapKhoDuocPhamChiTiet]
    ADD CONSTRAINT [FK_YeuCauNhapKhoDuocPhamChiTiet_DuocPhamBenhVienPhanNhom] FOREIGN KEY ([DuocPhamBenhVienPhanNhomId]) REFERENCES [dbo].[DuocPhamBenhVienPhanNhom] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauNhapKhoDuocPhamChiTiet]
    ADD CONSTRAINT [FK_YeuCauNhapKhoDuocPhamChiTiet_HopDongThauDuocPham] FOREIGN KEY ([HopDongThauDuocPhamId]) REFERENCES [dbo].[HopDongThauDuocPham] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauNhapKhoDuocPhamChiTiet]
    ADD CONSTRAINT [FK_YeuCauNhapKhoDuocPhamChiTiet_KhoViTri] FOREIGN KEY ([KhoViTriId]) REFERENCES [dbo].[KhoViTri] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauNhapKhoDuocPhamChiTiet]
    ADD CONSTRAINT [FK_YeuCauNhapKhoDuocPhamChiTiet_YeuCauNhapKhoDuocPham] FOREIGN KEY ([YeuCauNhapKhoDuocPhamId]) REFERENCES [dbo].[YeuCauNhapKhoDuocPham] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauNhapKhoVatTu]
    ADD CONSTRAINT [FK_YeuCauNhapKhoVatTu_Kho] FOREIGN KEY ([KhoId]) REFERENCES [dbo].[Kho] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauNhapKhoVatTu]
    ADD CONSTRAINT [FK_YeuCauNhapKhoVatTu_NhanVien] FOREIGN KEY ([NguoiGiaoId]) REFERENCES [dbo].[NhanVien] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauNhapKhoVatTu]
    ADD CONSTRAINT [FK_YeuCauNhapKhoVatTu_NhanVien1] FOREIGN KEY ([NguoiNhapId]) REFERENCES [dbo].[NhanVien] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauNhapKhoVatTu]
    ADD CONSTRAINT [FK_YeuCauNhapKhoVatTu_NhanVien2] FOREIGN KEY ([NhanVienDuyetId]) REFERENCES [dbo].[NhanVien] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauNhapKhoVatTuChiTiet]
    ADD CONSTRAINT [FK_YeuCauNhapKhoVatTuChiTiet_HopDongThauVatTu] FOREIGN KEY ([HopDongThauVatTuId]) REFERENCES [dbo].[HopDongThauVatTu] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauNhapKhoVatTuChiTiet]
    ADD CONSTRAINT [FK_YeuCauNhapKhoVatTuChiTiet_KhoViTri] FOREIGN KEY ([KhoViTriId]) REFERENCES [dbo].[KhoViTri] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauNhapKhoVatTuChiTiet]
    ADD CONSTRAINT [FK_YeuCauNhapKhoVatTuChiTiet_VatTuBenhVien] FOREIGN KEY ([VatTuBenhVienId]) REFERENCES [dbo].[VatTuBenhVien] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauNhapKhoVatTuChiTiet]
    ADD CONSTRAINT [FK_YeuCauNhapKhoVatTuChiTiet_YeuCauNhapKhoVatTu] FOREIGN KEY ([YeuCauNhapKhoVatTuId]) REFERENCES [dbo].[YeuCauNhapKhoVatTu] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauTraDuocPham]
    ADD CONSTRAINT [FK_YeuCauTraDuocPham_Kho] FOREIGN KEY ([KhoXuatId]) REFERENCES [dbo].[Kho] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauTraDuocPham]
    ADD CONSTRAINT [FK_YeuCauTraDuocPham_Kho1] FOREIGN KEY ([KhoNhapId]) REFERENCES [dbo].[Kho] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauTraDuocPham]
    ADD CONSTRAINT [FK_YeuCauTraDuocPham_NhanVien] FOREIGN KEY ([NhanVienYeuCauId]) REFERENCES [dbo].[NhanVien] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauTraDuocPham]
    ADD CONSTRAINT [FK_YeuCauTraDuocPham_NhanVien1] FOREIGN KEY ([NhanVienDuyetId]) REFERENCES [dbo].[NhanVien] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauTraDuocPhamChiTiet]
    ADD CONSTRAINT [FK_YeuCauTraDuocPhamChiTiet_XuatKhoDuocPhamChiTietViTri] FOREIGN KEY ([XuatKhoDuocPhamChiTietViTriId]) REFERENCES [dbo].[XuatKhoDuocPhamChiTietViTri] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauTraDuocPhamChiTiet]
    ADD CONSTRAINT [FK_YeuCauTraDuocPhamChiTiet_YeuCauTraDuocPham] FOREIGN KEY ([YeuCauTraDuocPhamId]) REFERENCES [dbo].[YeuCauTraDuocPham] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauTraVatTu]
    ADD CONSTRAINT [FK_YeuCauTraVatTu_Kho] FOREIGN KEY ([KhoXuatId]) REFERENCES [dbo].[Kho] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauTraVatTu]
    ADD CONSTRAINT [FK_YeuCauTraVatTu_Kho1] FOREIGN KEY ([KhoNhapId]) REFERENCES [dbo].[Kho] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauTraVatTu]
    ADD CONSTRAINT [FK_YeuCauTraVatTu_NhanVien] FOREIGN KEY ([NhanVienYeuCauId]) REFERENCES [dbo].[NhanVien] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauTraVatTu]
    ADD CONSTRAINT [FK_YeuCauTraVatTu_NhanVien1] FOREIGN KEY ([NhanVienDuyetId]) REFERENCES [dbo].[NhanVien] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauTraVatTuChiTiet]
    ADD CONSTRAINT [FK_YeuCauTraVatTuChiTiet_XuatKhoVatTuChiTietViTri] FOREIGN KEY ([XuatKhoVatTuChiTietViTriId]) REFERENCES [dbo].[XuatKhoVatTuChiTietViTri] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauTraVatTuChiTiet]
    ADD CONSTRAINT [FK_YeuCauTraVatTuChiTiet_YeuCauTraVatTu] FOREIGN KEY ([YeuCauTraVatTuId]) REFERENCES [dbo].[YeuCauTraVatTu] ([Id]);
GO
ALTER TABLE dbo.[YeuCauVatTuBenhVien] ADD [XuatKhoVatTuChiTietId]     BIGINT          NULL;
GO
ALTER TABLE dbo.[YeuCauVatTuBenhVien] ADD [LoaiPhieuLinh]             INT             NOT NULL;
GO
ALTER TABLE dbo.[YeuCauVatTuBenhVien] ADD [KhoLinhId]                 BIGINT          NULL;
GO
ALTER TABLE dbo.[YeuCauVatTuBenhVien] ADD [YeuCauLinhVatTuId]         BIGINT          NULL;
GO
ALTER TABLE dbo.[YeuCauVatTuBenhVien] ADD [LaVatTuBHYT]               BIT             NOT NULL;
GO
ALTER TABLE dbo.[YeuCauVatTuBenhVien] ADD [DonGiaNhap]                DECIMAL (15, 2) NOT NULL;
GO
ALTER TABLE dbo.[YeuCauVatTuBenhVien] ADD [TiLeTheoThapGia]           INT             NOT NULL;
GO
ALTER TABLE dbo.[YeuCauVatTuBenhVien] ADD [VAT]                       INT             NOT NULL;
GO
ALTER TABLE dbo.[YeuCauVatTuBenhVien] ADD [HopDongThauVatTuId]        BIGINT          NULL;
GO
ALTER TABLE dbo.[YeuCauVatTuBenhVien] ADD [NhaThauId]                 BIGINT          NULL;
GO
ALTER TABLE dbo.[YeuCauVatTuBenhVien] ADD [SoHopDongThau]             NVARCHAR (50)   NULL;
GO
ALTER TABLE dbo.[YeuCauVatTuBenhVien] ADD [SoQuyetDinhThau]           NVARCHAR (50)   NULL;
GO
ALTER TABLE dbo.[YeuCauVatTuBenhVien] ADD [LoaiThau]                  INT             NULL;
GO
ALTER TABLE dbo.[YeuCauVatTuBenhVien] ADD [NhomThau]                  NVARCHAR (50)   NULL;
GO
ALTER TABLE dbo.[YeuCauVatTuBenhVien] ADD [GoiThau]                   NVARCHAR (2)    NULL;
GO
ALTER TABLE dbo.[YeuCauVatTuBenhVien] ADD [NamThau]                   INT             NULL;
GO
ALTER TABLE dbo.[YeuCauVatTuBenhVien] DROP COLUMN [Gia];
GO
ALTER TABLE [dbo].[YeuCauVatTuBenhVien]
    ADD CONSTRAINT [FK_YeuCauVatTuBenhVien_HopDongThauVatTu] FOREIGN KEY ([HopDongThauVatTuId]) REFERENCES [dbo].[HopDongThauVatTu] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauVatTuBenhVien]
    ADD CONSTRAINT [FK_YeuCauVatTuBenhVien_Kho] FOREIGN KEY ([KhoLinhId]) REFERENCES [dbo].[Kho] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauVatTuBenhVien]
    ADD CONSTRAINT [FK_YeuCauVatTuBenhVien_NhaThau] FOREIGN KEY ([NhaThauId]) REFERENCES [dbo].[NhaThau] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauVatTuBenhVien]
    ADD CONSTRAINT [FK_YeuCauVatTuBenhVien_XuatKhoVatTuChiTiet] FOREIGN KEY ([XuatKhoVatTuChiTietId]) REFERENCES [dbo].[XuatKhoVatTuChiTiet] ([Id]);
GO
ALTER TABLE [dbo].[YeuCauVatTuBenhVien]
    ADD CONSTRAINT [FK_YeuCauVatTuBenhVien_YeuCauLinhVatTu] FOREIGN KEY ([YeuCauLinhVatTuId]) REFERENCES [dbo].[YeuCauLinhVatTu] ([Id]);
GO

INSERT INTO [DuocPhamVaVatTuBenhVien] ([LoaiDuocPhamHoacVatTu], [DuocPhamBenhVienId], [Ma],[Ten],[HoatChat],[HieuLuc],[CreatedById],[LastUserId],[LastTime],[CreatedOn])
SELECT 1, A.[Id], B.[MaHoatChat],B.[Ten],B.[HoatChat],[HieuLuc],A.[CreatedById],A.[LastUserId],A.[LastTime],A.[CreatedOn]
FROM [DuocPhamBenhVien] A
inner join [DuocPham] B on B.Id = A.Id
GO
INSERT INTO [DuocPhamVaVatTuBenhVien] ([LoaiDuocPhamHoacVatTu], [VatTuBenhVienId], [Ma],[Ten],[HoatChat],[HieuLuc],[CreatedById],[LastUserId],[LastTime],[CreatedOn])
SELECT 2, A.[Id], B.[Ma],B.[Ten],NULL,[HieuLuc],A.[CreatedById],A.[LastUserId],A.[LastTime],A.[CreatedOn]
FROM [VatTuBenhVien] A
inner join [VatTu] B on B.Id = A.Id

GO
Create trigger trg_DuocPhamBenhVien_Into_DuocPhamVaVatTuBenhVien_Add on [dbo].[DuocPhamBenhVien] after insert as
begin
	insert into [dbo].[DuocPhamVaVatTuBenhVien](LoaiDuocPhamHoacVatTu, DuocPhamBenhVienId, Ma, Ten, HoatChat, HieuLuc ,CreatedById,LastUserId, CreatedOn, LastTime)
	select 1, i.[Id], d.MaHoatChat, d.Ten, d.HoatChat, i.HieuLuc, i.CreatedById, i.LastUserId, i.CreatedOn, i.LastTime
	from inserted i
	inner join [dbo].[DuocPham] d on i.Id = d.Id
end
GO

Create trigger trg_DuocPhamBenhVien_Into_DuocPhamVaVatTuBenhVien_Delete on [dbo].[DuocPhamBenhVien] after delete as
begin
	delete from [dbo].[DuocPhamVaVatTuBenhVien] where [dbo].[DuocPhamVaVatTuBenhVien].DuocPhamBenhVienId = (select d.[Id] from deleted d)
end
GO

Create trigger trg_DuocPhamBenhVien_Into_DuocPhamVaVatTuBenhVien_Update on [dbo].[DuocPhamBenhVien] after update as
begin
	update [dbo].[DuocPhamVaVatTuBenhVien]
	set HieuLuc = T1.HieuLuc
	
	from [dbo].[DuocPhamVaVatTuBenhVien] T inner join 
	(select i.Id, i.HieuLuc from inserted i) T1 on T.DuocPhamBenhVienId = T1.Id

end
GO

Create trigger trg_VatTuBenhVien_Into_DuocPhamVaVatTuBenhVien_Add on [dbo].[VatTuBenhVien] after insert as
begin
	insert into [dbo].[DuocPhamVaVatTuBenhVien](LoaiDuocPhamHoacVatTu, VatTuBenhVienId, Ma, Ten, HieuLuc ,CreatedById,LastUserId, CreatedOn, LastTime)
	select 2, i.[Id], d.Ma, d.Ten, i.HieuLuc, i.CreatedById, i.LastUserId, i.CreatedOn, i.LastTime
	from inserted i
	inner join [dbo].[VatTu] d on i.Id = d.Id
end
GO

Create trigger trg_VatTuBenhVien_Into_DuocPhamVaVatTuBenhVien_Delete on [dbo].[VatTuBenhVien] after delete as
begin
	delete from [dbo].[DuocPhamVaVatTuBenhVien] where [dbo].[DuocPhamVaVatTuBenhVien].VatTuBenhVienId = (select d.[Id] from deleted d)
end
GO

Create trigger trg_VatTuBenhVien_Into_DuocPhamVaVatTuBenhVien_Update on [dbo].[VatTuBenhVien] after update as
begin
	update [dbo].[DuocPhamVaVatTuBenhVien]
	set HieuLuc = T1.HieuLuc
	
	from [dbo].[DuocPhamVaVatTuBenhVien] T inner join 
	(select i.Id, i.HieuLuc from inserted i) T1 on T.VatTuBenhVienId = T1.Id
end

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Tháp giá thuốc không bảo hiểm=1, Tháp giá vật tư tiêu hao=2, Tháp giá vật tư thay thế=3', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'CauHinhThapGia', @level2type = N'COLUMN', @level2name = N'LoaiThapGia';
GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Thầu tập trung=1,
Thầu riêng=2,
Tự bào chế=3', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'HopDongThauVatTu', @level2type = N'COLUMN', @level2name = N'LoaiThau';
GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Kho tổng dược phẩm cấp I=1,Kho tổng dược phẩm cấp II=2,Kho tổng VTYT cấp I=3,Kho tổng VTYT cấp II=4
Kho lẻ=5,Nhà thuốc=6,Kho dược phẩm chờ xử lý=7,Kho VTYT chờ xử lý=8', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Kho', @level2type = N'COLUMN', @level2name = N'LoaiKho';
GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'1=Vật tư tiêu hao,2=Vật tư thay thế', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'VatTuBenhVien', @level2type = N'COLUMN', @level2name = N'LoaiSuDung';
GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Xuất qua kho khác=1,
Xuất trả nhà cung cấp=2,
Xuất cho bệnh nhân=3,
Xuất hủy=4', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'XuatKhoVatTu', @level2type = N'COLUMN', @level2name = N'LoaiXuatKho';
GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Lĩnh dự trù=1,
Lĩnh bù=2,
Lĩnh cho bệnh nhân=3,', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'YeuCauLinhDuocPham', @level2type = N'COLUMN', @level2name = N'LoaiPhieuLinh';
GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Lĩnh dự trù=1,
Lĩnh bù=2,
Lĩnh cho bệnh nhân=3,', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'YeuCauLinhVatTu', @level2type = N'COLUMN', @level2name = N'LoaiPhieuLinh';
GO
Update dbo.CauHinh
Set [Value] = '0.7.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'
