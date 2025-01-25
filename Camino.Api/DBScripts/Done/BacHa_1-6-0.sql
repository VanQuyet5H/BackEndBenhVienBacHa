ALTER TABLE [dbo].[DichVuKhamBenhBenhVien]
    ADD [ChuyenKhoaKhamSucKhoe] INT NULL;

GO
ALTER TABLE [dbo].[PhongBenhVien]
    ADD [HopDongKhamSucKhoeId] BIGINT NULL;

GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat]
    ADD [GoiKhamSucKhoeId]   BIGINT          NULL,
        [SoThuTuKhamSucKhoe] INT             NULL,
        [DonGiaUuDai]        DECIMAL (15, 2) NULL,
        [DonGiaChuaUuDai]    DECIMAL (15, 2) NULL;

GO
ALTER TABLE [dbo].[YeuCauKhamBenh]
    ADD [GoiKhamSucKhoeId]      BIGINT          NULL,
        [ChuyenKhoaKhamSucKhoe] INT             NULL,
        [SoThuTuKhamSucKhoe]    INT             NULL,
        [DonGiaUuDai]           DECIMAL (15, 2) NULL,
        [DonGiaChuaUuDai]       DECIMAL (15, 2) NULL;

GO
ALTER TABLE [dbo].[YeuCauTiepNhan]
    ADD [HopDongKhamSucKhoeNhanVienId]   BIGINT          NULL,
        [KSKPhanLoaiTheLuc]              NVARCHAR (50)   NULL,
        [KSKKetQuaCanLamSang]            NVARCHAR (1000) NULL,
        [KSKDanhGiaCanLamSang]           NVARCHAR (1000) NULL,
        [KSKNhanVienDanhGiaCanLamSangId] BIGINT          NULL,
        [KSKKetLuanPhanLoaiSucKhoe]      NVARCHAR (50)   NULL,
        [KSKKetLuanGhiChu]               NVARCHAR (1000) NULL,
        [KSKKetLuanCacBenhTat]           NVARCHAR (1000) NULL,
        [KSKNhanVienKetLuanId]           BIGINT          NULL,
        [KSKThoiDiemKetLuan]             DATETIME        NULL;

GO
CREATE TABLE [dbo].[CongTyKhamSucKhoe] (
    [Id]                 BIGINT         IDENTITY (1, 1) NOT NULL,
    [Ma]                 NVARCHAR (50)  NOT NULL,
    [Ten]                NVARCHAR (250) NOT NULL,
    [LoaiCongTy]         INT            NOT NULL,
    [DiaChi]             NVARCHAR (250) NOT NULL,
    [SoDienThoai]        NVARCHAR (100) NOT NULL,
    [Email]              NVARCHAR (200) NULL,
    [MaSoThue]           NVARCHAR (20)  NULL,
    [SoTaiKhoanNganHang] NVARCHAR (20)  NULL,
    [NguoiDaiDien]       NVARCHAR (100) NOT NULL,
    [NguoiLienHe]        NVARCHAR (100) NOT NULL,
    [CoHoatDong]         BIT            NOT NULL,
    [CreatedById]        BIGINT         NOT NULL,
    [LastUserId]         BIGINT         NOT NULL,
    [LastTime]           DATETIME       NOT NULL,
    [CreatedOn]          DATETIME       NOT NULL,
    [LastModified]       ROWVERSION     NOT NULL,
    [SoDienThoaiDisplay] AS             ([dbo].[LayFormatSoDienThoai]([SoDienThoai])),
    CONSTRAINT [PK__CongTyKh__3214EC072B6F88D8] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[GoiKhamSucKhoe] (
    [Id]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [HopDongKhamSucKhoeId] BIGINT         NOT NULL,
    [Ma]                   NVARCHAR (50)  NOT NULL,
    [Ten]                  NVARCHAR (250) NOT NULL,
    [CreatedById]          BIGINT         NOT NULL,
    [LastUserId]           BIGINT         NOT NULL,
    [LastTime]             DATETIME       NOT NULL,
    [CreatedOn]            DATETIME       NOT NULL,
    [LastModified]         ROWVERSION     NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[GoiKhamSucKhoeDichVuDichVuKyThuat] (
    [Id]                             BIGINT          IDENTITY (1, 1) NOT NULL,
    [GoiKhamSucKhoeId]               BIGINT          NOT NULL,
    [DichVuKyThuatBenhVienId]        BIGINT          NOT NULL,
    [NhomGiaDichVuKyThuatBenhVienId] BIGINT          NOT NULL,
    [DonGiaBenhVien]                 DECIMAL (15, 2) NOT NULL,
    [DonGiaUuDai]                    DECIMAL (15, 2) NOT NULL,
    [DonGiaChuaUuDai]                DECIMAL (15, 2) NOT NULL,
    [GioiTinhNam]                    BIT             NOT NULL,
    [GioiTinhNu]                     BIT             NOT NULL,
    [CoMangThai]                     BIT             NOT NULL,
    [KhongMangThai]                  BIT             NOT NULL,
    [ChuaLapGiaDinh]                 BIT             NOT NULL,
    [DaLapGiaDinh]                   BIT             NOT NULL,
    [SoTuoiTu]                       INT             NULL,
    [SoTuoiDen]                      INT             NULL,
    [CreatedById]                    BIGINT          NOT NULL,
    [LastUserId]                     BIGINT          NOT NULL,
    [LastTime]                       DATETIME        NOT NULL,
    [CreatedOn]                      DATETIME        NOT NULL,
    [LastModified]                   ROWVERSION      NOT NULL,
    CONSTRAINT [PK__GoiKhamS__3214EC078D35B9F2] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[GoiKhamSucKhoeDichVuKhamBenh] (
    [Id]                              BIGINT          IDENTITY (1, 1) NOT NULL,
    [GoiKhamSucKhoeId]                BIGINT          NOT NULL,
    [DichVuKhamBenhBenhVienId]        BIGINT          NOT NULL,
    [ChuyenKhoaKhamSucKhoe]           INT             NOT NULL,
    [NhomGiaDichVuKhamBenhBenhVienId] BIGINT          NOT NULL,
    [DonGiaBenhVien]                  DECIMAL (15, 2) NOT NULL,
    [DonGiaUuDai]                     DECIMAL (15, 2) NOT NULL,
    [DonGiaChuaUuDai]                 DECIMAL (15, 2) NOT NULL,
    [GioiTinhNam]                     BIT             NOT NULL,
    [GioiTinhNu]                      BIT             NOT NULL,
    [CoMangThai]                      BIT             NOT NULL,
    [KhongMangThai]                   BIT             NOT NULL,
    [ChuaLapGiaDinh]                  BIT             NOT NULL,
    [DaLapGiaDinh]                    BIT             NOT NULL,
    [SoTuoiTu]                        INT             NULL,
    [SoTuoiDen]                       INT             NULL,
    [CreatedById]                     BIGINT          NOT NULL,
    [LastUserId]                      BIGINT          NOT NULL,
    [LastTime]                        DATETIME        NOT NULL,
    [CreatedOn]                       DATETIME        NOT NULL,
    [LastModified]                    ROWVERSION      NOT NULL,
    CONSTRAINT [PK__GoiKhamS__3214EC0715DBD0A5] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[GoiKhamSucKhoeNoiThucHien] (
    [Id]                                  BIGINT     IDENTITY (1, 1) NOT NULL,
    [GoiKhamSucKhoeDichVuKhamBenhId]      BIGINT     NULL,
    [GoiKhamSucKhoeDichVuDichVuKyThuatId] BIGINT     NULL,
    [PhongBenhVienId]                     BIGINT     NOT NULL,
    [CreatedById]                         BIGINT     NOT NULL,
    [LastUserId]                          BIGINT     NOT NULL,
    [LastTime]                            DATETIME   NOT NULL,
    [CreatedOn]                           DATETIME   NOT NULL,
    [LastModified]                        ROWVERSION NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[HopDongKhamSucKhoe] (
    [Id]                   BIGINT          IDENTITY (1, 1) NOT NULL,
    [CongTyKhamSucKhoeId]  BIGINT          NOT NULL,
    [SoHopDong]            NVARCHAR (50)   NOT NULL,
    [NgayHopDong]          DATETIME        NOT NULL,
    [LoaiHopDong]          INT             NOT NULL,
    [SoNguoiKham]          INT             NOT NULL,
    [GiaTriHopDong]        DECIMAL (15, 2) NOT NULL,
    [ThanhToanPhatSinh]    DECIMAL (15, 2) NULL,
    [TiLeChietKhau]        DECIMAL (15, 2) NULL,
    [SoTienTamUng]         DECIMAL (15, 2) NULL,
    [SoTienChietKhau]      DECIMAL (15, 2) NULL,
    [SoTienPhatSinhThucTe] DECIMAL (15, 2) NULL,
    [SoTienChiChoNhanVien] DECIMAL (15, 2) NULL,
    [NgayHieuLuc]          DATETIME        NOT NULL,
    [NgayKetThuc]          DATETIME        NOT NULL,
    [DaKetThuc]            BIT             NOT NULL,
    [NguoiKyHopDong]       NVARCHAR (100)  NOT NULL,
    [ChucDanhNguoiKy]      NVARCHAR (100)  NOT NULL,
    [NguoiGioiThieu]       NVARCHAR (100)  NULL,
    [DienGiai]             NVARCHAR (4000) NULL,
    [CreatedById]          BIGINT          NOT NULL,
    [LastUserId]           BIGINT          NOT NULL,
    [LastTime]             DATETIME        NOT NULL,
    [CreatedOn]            DATETIME        NOT NULL,
    [LastModified]         ROWVERSION      NOT NULL,
    CONSTRAINT [PK__HopDongK__3214EC07D49C062B] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[HopDongKhamSucKhoeDiaDiem] (
    [Id]                   BIGINT          IDENTITY (1, 1) NOT NULL,
    [HopDongKhamSucKhoeId] BIGINT          NOT NULL,
    [DiaDiem]              NVARCHAR (250)  NOT NULL,
    [CongViec]             NVARCHAR (250)  NOT NULL,
    [Ngay]                 DATETIME        NULL,
    [GhiChu]               NVARCHAR (1000) NULL,
    [CreatedById]          BIGINT          NOT NULL,
    [LastUserId]           BIGINT          NOT NULL,
    [LastTime]             DATETIME        NOT NULL,
    [CreatedOn]            DATETIME        NOT NULL,
    [LastModified]         ROWVERSION      NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[HopDongKhamSucKhoeNhanVien] (
    [Id]                      BIGINT          IDENTITY (1, 1) NOT NULL,
    [HopDongKhamSucKhoeId]    BIGINT          NOT NULL,
    [BenhNhanId]              BIGINT          NULL,
    [MaNhanVien]              NVARCHAR (20)   NULL,
    [HoTen]                   NVARCHAR (100)  NOT NULL,
    [HoTenKhac]               NVARCHAR (100)  NULL,
    [NgaySinh]                INT             NULL,
    [ThangSinh]               INT             NULL,
    [NamSinh]                 INT             NULL,
    [SoDienThoai]             NVARCHAR (12)   NULL,
    [SoChungMinhThu]          NVARCHAR (12)   NULL,
    [GioiTinh]                INT             NULL,
    [NgheNghiepId]            BIGINT          NULL,
    [QuocTichId]              BIGINT          NULL,
    [DanTocId]                BIGINT          NULL,
    [DiaChi]                  NVARCHAR (200)  NULL,
    [PhuongXaId]              BIGINT          NULL,
    [QuanHuyenId]             BIGINT          NULL,
    [TinhThanhId]             BIGINT          NULL,
    [NhomMau]                 INT             NULL,
    [YeuToRh]                 INT             NULL,
    [Email]                   NVARCHAR (200)  NULL,
    [TenDonViHoacBoPhan]      NVARCHAR (300)  NULL,
    [NhomDoiTuongKhamSucKhoe] NVARCHAR (100)  NULL,
    [GoiKhamSucKhoeId]        BIGINT          NOT NULL,
    [DaLapGiaDinh]            BIT             NOT NULL,
    [CoMangThai]              BIT             NOT NULL,
    [GhiChuTienSuBenh]        NVARCHAR (4000) NULL,
    [GhiChuDiUngThuoc]        NVARCHAR (4000) NULL,
    [CreatedById]             BIGINT          NOT NULL,
    [LastUserId]              BIGINT          NOT NULL,
    [LastTime]                DATETIME        NOT NULL,
    [CreatedOn]               DATETIME        NOT NULL,
    [LastModified]            ROWVERSION      NOT NULL,
    [DiaChiDayDu]             AS              ([dbo].[LayDiaChiDayDu]([DiaChi], [PhuongXaId], [QuanHuyenId], [TinhThanhId])),
    [SoDienThoaiDisplay]      AS              ([dbo].[LayFormatSoDienThoai]([SoDienThoai])),
    [HuyKham]                 BIT             NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[NhanSuKhamSucKhoeTaiLieuDinhKem] (
    [Id]           BIGINT          IDENTITY (1, 1) NOT NULL,
    [Ma]           NVARCHAR (50)   NULL,
    [Ten]          NVARCHAR (256)  NOT NULL,
    [TenGuid]      NVARCHAR (256)  NOT NULL,
    [KichThuoc]    BIGINT          NOT NULL,
    [DuongDan]     NVARCHAR (500)  NOT NULL,
    [LoaiTapTin]   INT             NOT NULL,
    [MoTa]         NVARCHAR (2000) NULL,
    [CreatedById]  BIGINT          NOT NULL,
    [LastUserId]   BIGINT          NOT NULL,
    [LastTime]     DATETIME        NOT NULL,
    [CreatedOn]    DATETIME        NOT NULL,
    [LastModified] ROWVERSION      NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[TuVanThuocKhamSucKhoe] (
    [Id]                    BIGINT          IDENTITY (1, 1) NOT NULL,
    [YeuCauTiepNhanId]      BIGINT          NOT NULL,
    [DuocPhamId]            BIGINT          NOT NULL,
    [LaDuocPhamBenhVien]    BIT             NOT NULL,
    [Ten]                   NVARCHAR (250)  NOT NULL,
    [TenTiengAnh]           NVARCHAR (250)  NULL,
    [SoDangKy]              NVARCHAR (100)  NOT NULL,
    [STTHoatChat]           INT             NULL,
    [MaHoatChat]            NVARCHAR (20)   NOT NULL,
    [HoatChat]              NVARCHAR (500)  NOT NULL,
    [LoaiThuocHoacHoatChat] INT             NOT NULL,
    [NhaSanXuat]            NVARCHAR (250)  NULL,
    [NuocSanXuat]           NVARCHAR (250)  NULL,
    [DuongDungId]           BIGINT          NOT NULL,
    [HamLuong]              NVARCHAR (500)  NULL,
    [QuyCach]               NVARCHAR (250)  NULL,
    [TieuChuan]             NVARCHAR (50)   NULL,
    [DangBaoChe]            NVARCHAR (250)  NULL,
    [DonViTinhId]           BIGINT          NOT NULL,
    [HuongDan]              NVARCHAR (4000) NULL,
    [MoTa]                  NVARCHAR (4000) NULL,
    [ChiDinh]               NVARCHAR (4000) NULL,
    [ChongChiDinh]          NVARCHAR (4000) NULL,
    [LieuLuongCachDung]     NVARCHAR (4000) NULL,
    [TacDungPhu]            NVARCHAR (4000) NULL,
    [ChuYDePhong]           NVARCHAR (4000) NULL,
    [SoLuong]               FLOAT (53)      NOT NULL,
    [SoNgayDung]            INT             NULL,
    [DungSang]              FLOAT (53)      NULL,
    [DungTrua]              FLOAT (53)      NULL,
    [DungChieu]             FLOAT (53)      NULL,
    [DungToi]               FLOAT (53)      NULL,
    [ThoiGianDungSang]      INT             NULL,
    [ThoiGianDungTrua]      INT             NULL,
    [ThoiGianDungChieu]     INT             NULL,
    [ThoiGianDungToi]       INT             NULL,
    [GhiChu]                NVARCHAR (1000) NULL,
    [CreatedById]           BIGINT          NOT NULL,
    [LastUserId]            BIGINT          NOT NULL,
    [LastTime]              DATETIME        NOT NULL,
    [CreatedOn]             DATETIME        NOT NULL,
    [LastModified]          ROWVERSION      NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[YeuCauNhanSuKhamSucKhoe] (
    [Id]                    BIGINT          IDENTITY (1, 1) NOT NULL,
    [HopDongKhamSucKhoeId]  BIGINT          NOT NULL,
    [NhanVienGuiYeuCauId]   BIGINT          NOT NULL,
    [NgayGuiYeuCau]         DATETIME        NOT NULL,
    [DuocKHTHDuyet]         BIT             NULL,
    [NgayKHTHDuyet]         DATETIME        NULL,
    [NhanVienKHTHDuyetId]   BIGINT          NULL,
    [LyDoKHTHKhongDuyet]    NVARCHAR (4000) NULL,
    [DuocNhanSuDuyet]       BIT             NULL,
    [NgayNhanSuDuyet]       DATETIME        NULL,
    [NhanVienNhanSuDuyetId] BIGINT          NULL,
    [LyDoNhanSuKhongDuyet]  NVARCHAR (4000) NULL,
    [DuocGiamDocDuyet]      BIT             NULL,
    [NgayGiamDocDuyet]      DATETIME        NULL,
    [GiamDocId]             BIGINT          NULL,
    [LyDoGiamDocKhongDuyet] NVARCHAR (4000) NULL,
    [CreatedById]           BIGINT          NOT NULL,
    [LastUserId]            BIGINT          NOT NULL,
    [LastTime]              DATETIME        NOT NULL,
    [CreatedOn]             DATETIME        NOT NULL,
    [LastModified]          ROWVERSION      NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[YeuCauNhanSuKhamSucKhoeChiTiet] (
    [Id]                                BIGINT          IDENTITY (1, 1) NOT NULL,
    [YeuCauNhanSuKhamSucKhoeId]         BIGINT          NOT NULL,
    [NguonNhanSu]                       INT             NOT NULL,
    [HoTen]                             NVARCHAR (100)  NULL,
    [NhanVienId]                        BIGINT          NULL,
    [DonVi]                             NVARCHAR (100)  NULL,
    [ViTriLamViec]                      NVARCHAR (100)  NOT NULL,
    [SoDienThoai]                       VARCHAR (20)    NULL,
    [DoiTuongLamViec]                   INT             NOT NULL,
    [NguoiGioiThieuId]                  BIGINT          NULL,
    [NhanSuKhamSucKhoeTaiLieuDinhKemId] BIGINT          NULL,
    [GhiChu]                            NVARCHAR (1000) NULL,
    [CreatedById]                       BIGINT          NOT NULL,
    [LastUserId]                        BIGINT          NOT NULL,
    [LastTime]                          DATETIME        NOT NULL,
    [CreatedOn]                         DATETIME        NOT NULL,
    [LastModified]                      ROWVERSION      NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
ALTER TABLE [dbo].[GoiKhamSucKhoe] WITH NOCHECK
    ADD CONSTRAINT [FK_GoiKhamSucKhoe_HopDongKhamSucKhoe] FOREIGN KEY ([HopDongKhamSucKhoeId]) REFERENCES [dbo].[HopDongKhamSucKhoe] ([Id]);

GO
ALTER TABLE [dbo].[GoiKhamSucKhoeDichVuDichVuKyThuat] WITH NOCHECK
    ADD CONSTRAINT [FK_GoiKhamSucKhoeDichVuDichVuKyThuat_NhomGiaDichVuKyThuatBenhVien] FOREIGN KEY ([NhomGiaDichVuKyThuatBenhVienId]) REFERENCES [dbo].[NhomGiaDichVuKyThuatBenhVien] ([Id]);

GO
ALTER TABLE [dbo].[GoiKhamSucKhoeDichVuDichVuKyThuat] WITH NOCHECK
    ADD CONSTRAINT [FK_GoiKhamSucKhoeDichVuDichVuKyThuat_GoiKhamSucKhoe] FOREIGN KEY ([GoiKhamSucKhoeId]) REFERENCES [dbo].[GoiKhamSucKhoe] ([Id]);

GO
ALTER TABLE [dbo].[GoiKhamSucKhoeDichVuDichVuKyThuat] WITH NOCHECK
    ADD CONSTRAINT [FK_GoiKhamSucKhoeDichVuDichVuKyThuat_DichVuKyThuatBenhVien] FOREIGN KEY ([DichVuKyThuatBenhVienId]) REFERENCES [dbo].[DichVuKyThuatBenhVien] ([Id]);

GO
ALTER TABLE [dbo].[GoiKhamSucKhoeDichVuKhamBenh] WITH NOCHECK
    ADD CONSTRAINT [FK_GoiKhamSucKhoeDichVuKhamBenh_GoiKhamSucKhoe] FOREIGN KEY ([GoiKhamSucKhoeId]) REFERENCES [dbo].[GoiKhamSucKhoe] ([Id]);

GO
ALTER TABLE [dbo].[GoiKhamSucKhoeDichVuKhamBenh] WITH NOCHECK
    ADD CONSTRAINT [FK_GoiKhamSucKhoeDichVuKhamBenh_DichVuKhamBenhBenhVien] FOREIGN KEY ([DichVuKhamBenhBenhVienId]) REFERENCES [dbo].[DichVuKhamBenhBenhVien] ([Id]);

GO
ALTER TABLE [dbo].[GoiKhamSucKhoeDichVuKhamBenh] WITH NOCHECK
    ADD CONSTRAINT [FK_GoiKhamSucKhoeDichVuKhamBenh_NhomGiaDichVuKhamBenhBenhVien] FOREIGN KEY ([NhomGiaDichVuKhamBenhBenhVienId]) REFERENCES [dbo].[NhomGiaDichVuKhamBenhBenhVien] ([Id]);

GO
ALTER TABLE [dbo].[GoiKhamSucKhoeNoiThucHien] WITH NOCHECK
    ADD CONSTRAINT [FK_GoiKhamSucKhoeNoiThucHien_GoiKhamSucKhoeDichVuKhamBenh] FOREIGN KEY ([GoiKhamSucKhoeDichVuKhamBenhId]) REFERENCES [dbo].[GoiKhamSucKhoeDichVuKhamBenh] ([Id]);

GO
ALTER TABLE [dbo].[GoiKhamSucKhoeNoiThucHien] WITH NOCHECK
    ADD CONSTRAINT [FK_GoiKhamSucKhoeNoiThucHien_GoiKhamSucKhoeDichVuDichVuKyThuat] FOREIGN KEY ([GoiKhamSucKhoeDichVuDichVuKyThuatId]) REFERENCES [dbo].[GoiKhamSucKhoeDichVuDichVuKyThuat] ([Id]);

GO
ALTER TABLE [dbo].[HopDongKhamSucKhoe] WITH NOCHECK
    ADD CONSTRAINT [FK_HopDongKhamSucKhoe_CongTyKhamSucKhoe] FOREIGN KEY ([CongTyKhamSucKhoeId]) REFERENCES [dbo].[CongTyKhamSucKhoe] ([Id]);

GO
ALTER TABLE [dbo].[HopDongKhamSucKhoeDiaDiem] WITH NOCHECK
    ADD CONSTRAINT [FK_HopDongKhamSucKhoeDiaDiem_HopDongKhamSucKhoe] FOREIGN KEY ([HopDongKhamSucKhoeId]) REFERENCES [dbo].[HopDongKhamSucKhoe] ([Id]);

GO
ALTER TABLE [dbo].[HopDongKhamSucKhoeNhanVien] WITH NOCHECK
    ADD CONSTRAINT [FK_HopDongKhamSucKhoeNhanVien_BenhNhan] FOREIGN KEY ([BenhNhanId]) REFERENCES [dbo].[BenhNhan] ([Id]);

GO
ALTER TABLE [dbo].[HopDongKhamSucKhoeNhanVien] WITH NOCHECK
    ADD CONSTRAINT [FK_HopDongKhamSucKhoeNhanVien_QuocGia] FOREIGN KEY ([QuocTichId]) REFERENCES [dbo].[QuocGia] ([Id]);

GO
ALTER TABLE [dbo].[HopDongKhamSucKhoeNhanVien] WITH NOCHECK
    ADD CONSTRAINT [FK_HopDongKhamSucKhoeNhanVien_HopDongKhamSucKhoe] FOREIGN KEY ([HopDongKhamSucKhoeId]) REFERENCES [dbo].[HopDongKhamSucKhoe] ([Id]);

GO
ALTER TABLE [dbo].[HopDongKhamSucKhoeNhanVien] WITH NOCHECK
    ADD CONSTRAINT [FK_HopDongKhamSucKhoeNhanVien_DanToc] FOREIGN KEY ([DanTocId]) REFERENCES [dbo].[DanToc] ([Id]);

GO
ALTER TABLE [dbo].[HopDongKhamSucKhoeNhanVien] WITH NOCHECK
    ADD CONSTRAINT [FK_HopDongKhamSucKhoeNhanVien_NgheNghiep] FOREIGN KEY ([NgheNghiepId]) REFERENCES [dbo].[NgheNghiep] ([Id]);

GO
ALTER TABLE [dbo].[HopDongKhamSucKhoeNhanVien] WITH NOCHECK
    ADD CONSTRAINT [FK_HopDongKhamSucKhoeNhanVien_DonViHanhChinh] FOREIGN KEY ([PhuongXaId]) REFERENCES [dbo].[DonViHanhChinh] ([Id]);

GO
ALTER TABLE [dbo].[HopDongKhamSucKhoeNhanVien] WITH NOCHECK
    ADD CONSTRAINT [FK_HopDongKhamSucKhoeNhanVien_DonViHanhChinh1] FOREIGN KEY ([QuanHuyenId]) REFERENCES [dbo].[DonViHanhChinh] ([Id]);

GO
ALTER TABLE [dbo].[HopDongKhamSucKhoeNhanVien] WITH NOCHECK
    ADD CONSTRAINT [FK_HopDongKhamSucKhoeNhanVien_DonViHanhChinh2] FOREIGN KEY ([TinhThanhId]) REFERENCES [dbo].[DonViHanhChinh] ([Id]);

GO
ALTER TABLE [dbo].[HopDongKhamSucKhoeNhanVien] WITH NOCHECK
    ADD CONSTRAINT [FK_HopDongKhamSucKhoeNhanVien_GoiKhamSucKhoe] FOREIGN KEY ([GoiKhamSucKhoeId]) REFERENCES [dbo].[GoiKhamSucKhoe] ([Id]);

GO
ALTER TABLE [dbo].[TuVanThuocKhamSucKhoe] WITH NOCHECK
    ADD CONSTRAINT [FK_TuVanThuocKhamSucKhoe_YeuCauTiepNhan] FOREIGN KEY ([YeuCauTiepNhanId]) REFERENCES [dbo].[YeuCauTiepNhan] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauNhanSuKhamSucKhoe] WITH NOCHECK
    ADD CONSTRAINT [FK_YeuCauNhanSuKhamSucKhoe_HopDongKhamSucKhoe] FOREIGN KEY ([HopDongKhamSucKhoeId]) REFERENCES [dbo].[HopDongKhamSucKhoe] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauNhanSuKhamSucKhoe] WITH NOCHECK
    ADD CONSTRAINT [FK_YeuCauNhanSuKhamSucKhoe_NhanVien] FOREIGN KEY ([NhanVienGuiYeuCauId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauNhanSuKhamSucKhoe] WITH NOCHECK
    ADD CONSTRAINT [FK_YeuCauNhanSuKhamSucKhoe_NhanVien1] FOREIGN KEY ([NhanVienKHTHDuyetId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauNhanSuKhamSucKhoe] WITH NOCHECK
    ADD CONSTRAINT [FK_YeuCauNhanSuKhamSucKhoe_NhanVien2] FOREIGN KEY ([NhanVienNhanSuDuyetId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauNhanSuKhamSucKhoe] WITH NOCHECK
    ADD CONSTRAINT [FK_YeuCauNhanSuKhamSucKhoe_NhanVien3] FOREIGN KEY ([GiamDocId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauNhanSuKhamSucKhoeChiTiet] WITH NOCHECK
    ADD CONSTRAINT [FK_YeuCauNhanSuKhamSucKhoeChiTiet_NhanSuKhamSucKhoeTaiLieuDinhKem] FOREIGN KEY ([NhanSuKhamSucKhoeTaiLieuDinhKemId]) REFERENCES [dbo].[NhanSuKhamSucKhoeTaiLieuDinhKem] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauNhanSuKhamSucKhoeChiTiet] WITH NOCHECK
    ADD CONSTRAINT [FK_YeuCauNhanSuKhamSucKhoeChiTiet_YeuCauNhanSuKhamSucKhoe] FOREIGN KEY ([YeuCauNhanSuKhamSucKhoeId]) REFERENCES [dbo].[YeuCauNhanSuKhamSucKhoe] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauNhanSuKhamSucKhoeChiTiet] WITH NOCHECK
    ADD CONSTRAINT [FK_YeuCauNhanSuKhamSucKhoeChiTiet_NhanVien] FOREIGN KEY ([NhanVienId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauNhanSuKhamSucKhoeChiTiet] WITH NOCHECK
    ADD CONSTRAINT [FK_YeuCauNhanSuKhamSucKhoeChiTiet_NhanVien1] FOREIGN KEY ([NguoiGioiThieuId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[PhongBenhVien] WITH NOCHECK
    ADD CONSTRAINT [FK_PhongBenhVien_HopDongKhamSucKhoe] FOREIGN KEY ([HopDongKhamSucKhoeId]) REFERENCES [dbo].[HopDongKhamSucKhoe] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauTiepNhan] WITH NOCHECK
    ADD CONSTRAINT [FK_YeuCauTiepNhan_HopDongKhamSucKhoeNhanVien] FOREIGN KEY ([HopDongKhamSucKhoeNhanVienId]) REFERENCES [dbo].[HopDongKhamSucKhoeNhanVien] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauTiepNhan] WITH NOCHECK
    ADD CONSTRAINT [FK_YeuCauTiepNhan_NhanVien] FOREIGN KEY ([KSKNhanVienDanhGiaCanLamSangId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauTiepNhan] WITH NOCHECK
    ADD CONSTRAINT [FK_YeuCauTiepNhan_NhanVien1] FOREIGN KEY ([KSKNhanVienKetLuanId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[GoiKhamSucKhoe] WITH CHECK CHECK CONSTRAINT [FK_GoiKhamSucKhoe_HopDongKhamSucKhoe];

ALTER TABLE [dbo].[GoiKhamSucKhoeDichVuDichVuKyThuat] WITH CHECK CHECK CONSTRAINT [FK_GoiKhamSucKhoeDichVuDichVuKyThuat_NhomGiaDichVuKyThuatBenhVien];

ALTER TABLE [dbo].[GoiKhamSucKhoeDichVuDichVuKyThuat] WITH CHECK CHECK CONSTRAINT [FK_GoiKhamSucKhoeDichVuDichVuKyThuat_GoiKhamSucKhoe];

ALTER TABLE [dbo].[GoiKhamSucKhoeDichVuDichVuKyThuat] WITH CHECK CHECK CONSTRAINT [FK_GoiKhamSucKhoeDichVuDichVuKyThuat_DichVuKyThuatBenhVien];

ALTER TABLE [dbo].[GoiKhamSucKhoeDichVuKhamBenh] WITH CHECK CHECK CONSTRAINT [FK_GoiKhamSucKhoeDichVuKhamBenh_GoiKhamSucKhoe];

ALTER TABLE [dbo].[GoiKhamSucKhoeDichVuKhamBenh] WITH CHECK CHECK CONSTRAINT [FK_GoiKhamSucKhoeDichVuKhamBenh_DichVuKhamBenhBenhVien];

ALTER TABLE [dbo].[GoiKhamSucKhoeDichVuKhamBenh] WITH CHECK CHECK CONSTRAINT [FK_GoiKhamSucKhoeDichVuKhamBenh_NhomGiaDichVuKhamBenhBenhVien];

ALTER TABLE [dbo].[GoiKhamSucKhoeNoiThucHien] WITH CHECK CHECK CONSTRAINT [FK_GoiKhamSucKhoeNoiThucHien_GoiKhamSucKhoeDichVuKhamBenh];

ALTER TABLE [dbo].[GoiKhamSucKhoeNoiThucHien] WITH CHECK CHECK CONSTRAINT [FK_GoiKhamSucKhoeNoiThucHien_GoiKhamSucKhoeDichVuDichVuKyThuat];

ALTER TABLE [dbo].[HopDongKhamSucKhoe] WITH CHECK CHECK CONSTRAINT [FK_HopDongKhamSucKhoe_CongTyKhamSucKhoe];

ALTER TABLE [dbo].[HopDongKhamSucKhoeDiaDiem] WITH CHECK CHECK CONSTRAINT [FK_HopDongKhamSucKhoeDiaDiem_HopDongKhamSucKhoe];

ALTER TABLE [dbo].[HopDongKhamSucKhoeNhanVien] WITH CHECK CHECK CONSTRAINT [FK_HopDongKhamSucKhoeNhanVien_BenhNhan];

ALTER TABLE [dbo].[HopDongKhamSucKhoeNhanVien] WITH CHECK CHECK CONSTRAINT [FK_HopDongKhamSucKhoeNhanVien_QuocGia];

ALTER TABLE [dbo].[HopDongKhamSucKhoeNhanVien] WITH CHECK CHECK CONSTRAINT [FK_HopDongKhamSucKhoeNhanVien_HopDongKhamSucKhoe];

ALTER TABLE [dbo].[HopDongKhamSucKhoeNhanVien] WITH CHECK CHECK CONSTRAINT [FK_HopDongKhamSucKhoeNhanVien_DanToc];

ALTER TABLE [dbo].[HopDongKhamSucKhoeNhanVien] WITH CHECK CHECK CONSTRAINT [FK_HopDongKhamSucKhoeNhanVien_NgheNghiep];

ALTER TABLE [dbo].[HopDongKhamSucKhoeNhanVien] WITH CHECK CHECK CONSTRAINT [FK_HopDongKhamSucKhoeNhanVien_DonViHanhChinh];

ALTER TABLE [dbo].[HopDongKhamSucKhoeNhanVien] WITH CHECK CHECK CONSTRAINT [FK_HopDongKhamSucKhoeNhanVien_DonViHanhChinh1];

ALTER TABLE [dbo].[HopDongKhamSucKhoeNhanVien] WITH CHECK CHECK CONSTRAINT [FK_HopDongKhamSucKhoeNhanVien_DonViHanhChinh2];

ALTER TABLE [dbo].[HopDongKhamSucKhoeNhanVien] WITH CHECK CHECK CONSTRAINT [FK_HopDongKhamSucKhoeNhanVien_GoiKhamSucKhoe];

ALTER TABLE [dbo].[TuVanThuocKhamSucKhoe] WITH CHECK CHECK CONSTRAINT [FK_TuVanThuocKhamSucKhoe_YeuCauTiepNhan];

ALTER TABLE [dbo].[YeuCauNhanSuKhamSucKhoe] WITH CHECK CHECK CONSTRAINT [FK_YeuCauNhanSuKhamSucKhoe_HopDongKhamSucKhoe];

ALTER TABLE [dbo].[YeuCauNhanSuKhamSucKhoe] WITH CHECK CHECK CONSTRAINT [FK_YeuCauNhanSuKhamSucKhoe_NhanVien];

ALTER TABLE [dbo].[YeuCauNhanSuKhamSucKhoe] WITH CHECK CHECK CONSTRAINT [FK_YeuCauNhanSuKhamSucKhoe_NhanVien1];

ALTER TABLE [dbo].[YeuCauNhanSuKhamSucKhoe] WITH CHECK CHECK CONSTRAINT [FK_YeuCauNhanSuKhamSucKhoe_NhanVien2];

ALTER TABLE [dbo].[YeuCauNhanSuKhamSucKhoe] WITH CHECK CHECK CONSTRAINT [FK_YeuCauNhanSuKhamSucKhoe_NhanVien3];

ALTER TABLE [dbo].[YeuCauNhanSuKhamSucKhoeChiTiet] WITH CHECK CHECK CONSTRAINT [FK_YeuCauNhanSuKhamSucKhoeChiTiet_NhanSuKhamSucKhoeTaiLieuDinhKem];

ALTER TABLE [dbo].[YeuCauNhanSuKhamSucKhoeChiTiet] WITH CHECK CHECK CONSTRAINT [FK_YeuCauNhanSuKhamSucKhoeChiTiet_YeuCauNhanSuKhamSucKhoe];

ALTER TABLE [dbo].[YeuCauNhanSuKhamSucKhoeChiTiet] WITH CHECK CHECK CONSTRAINT [FK_YeuCauNhanSuKhamSucKhoeChiTiet_NhanVien];

ALTER TABLE [dbo].[YeuCauNhanSuKhamSucKhoeChiTiet] WITH CHECK CHECK CONSTRAINT [FK_YeuCauNhanSuKhamSucKhoeChiTiet_NhanVien1];

ALTER TABLE [dbo].[PhongBenhVien] WITH CHECK CHECK CONSTRAINT [FK_PhongBenhVien_HopDongKhamSucKhoe];

ALTER TABLE [dbo].[YeuCauTiepNhan] WITH CHECK CHECK CONSTRAINT [FK_YeuCauTiepNhan_HopDongKhamSucKhoeNhanVien];

ALTER TABLE [dbo].[YeuCauTiepNhan] WITH CHECK CHECK CONSTRAINT [FK_YeuCauTiepNhan_NhanVien];

ALTER TABLE [dbo].[YeuCauTiepNhan] WITH CHECK CHECK CONSTRAINT [FK_YeuCauTiepNhan_NhanVien1];

GO
Update dbo.CauHinh
Set [Value] = '1.6.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'
