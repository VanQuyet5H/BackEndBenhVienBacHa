CREATE TABLE [dbo].[NoiTruBenhAn] (
    [Id]                       BIGINT          NOT NULL,
    [BenhNhanId]               BIGINT          NOT NULL,
    [SoBenhAn]                 NVARCHAR (20)   NOT NULL,
    [SoLuuTru]                 NVARCHAR (20)   NOT NULL,
    [ThuTuSapXepLuuTru]        NVARCHAR (50)   NULL,
    [NhanVienLuuTruId]         BIGINT          NULL,
    [NgayLuuTru]               DATETIME        NULL,
    [KhoaPhongNhapVienId]      BIGINT          NOT NULL,
    [ThoiDiemNhapVien]         DATETIME        NOT NULL,
    [ThoiDiemRaVien]           DATETIME        NULL,
    [LaCapCuu]                 BIT             NOT NULL,
    [LoaiBenhAn]               INT             NOT NULL,
    [ThoiDiemTaoBenhAn]        DATETIME        NOT NULL,
    [NhanVienTaoBenhAnId]      BIGINT          NOT NULL,
    [SoLanVaoVienDoBenhNay]    INT             NULL,
    [KetQuaDieuTri]            INT             NULL,
    [TinhTrangRaVien]          INT             NULL,
    [HinhThucRaVien]           INT             NULL,
    [CoThuThuat]               BIT             NULL,
    [CoPhauThuat]              BIT             NULL,
    [NgayTaiKham]              DATETIME        NULL,
    [GhiChuTaiKham]            NVARCHAR (1000) NULL,
    [CoChuyenVien]             BIT             NULL,
    [LoaiChuyenTuyen]          INT             NULL,
    [ChuyenDenBenhVienId]      BIGINT          NULL,
    [ThoiDiemChuyenVien]       DATETIME        NULL,
    [ThongTinTaiNanThuongTich] NVARCHAR (MAX)  NULL,
    [ThongTinBenhAn]           NVARCHAR (MAX)  NULL,
    [ThongTinTongKetBenhAn]    NVARCHAR (MAX)  NULL,
    [ThongTinRaVien]           NVARCHAR (MAX)  NULL,
    [CreatedById]              BIGINT          NOT NULL,
    [LastUserId]               BIGINT          NOT NULL,
    [LastTime]                 DATETIME        NOT NULL,
    [CreatedOn]                DATETIME        NOT NULL,
    [LastModified]             ROWVERSION      NOT NULL,
    CONSTRAINT [PK__NoiTruBenhAn] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[NoiTruEkipDieuTri] (
    [Id]                 BIGINT     IDENTITY (1, 1) NOT NULL,
    [NoiTruBenhAnId]     BIGINT     NOT NULL,
    [BacSiId]            BIGINT     NOT NULL,
    [DieuDuongId]        BIGINT     NOT NULL,
    [NhanVienLapId]      BIGINT     NOT NULL,
    [KhoaPhongDieuTriId] BIGINT     NOT NULL,
    [TuNgay]             DATETIME   NOT NULL,
    [DenNgay]            DATETIME   NULL,
    [CreatedById]        BIGINT     NOT NULL,
    [LastUserId]         BIGINT     NOT NULL,
    [LastTime]           DATETIME   NOT NULL,
    [CreatedOn]          DATETIME   NOT NULL,
    [LastModified]       ROWVERSION NOT NULL,
    CONSTRAINT [PK__NoiTruEkipDieuTri] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[NoiTruHoSoKhac] (
    [Id]                    BIGINT         IDENTITY (1, 1) NOT NULL,
    [YeuCauTiepNhanId]      BIGINT         NOT NULL,
    [LoaiHoSoDieuTriNoiTru] INT            NOT NULL,
    [ThongTinHoSo]          NVARCHAR (MAX) NOT NULL,
    [NhanVienThucHienId]    BIGINT         NOT NULL,
    [ThoiDiemThucHien]      DATETIME       NOT NULL,
    [NoiThucHienId]         BIGINT         NOT NULL,
    [CreatedById]           BIGINT         NOT NULL,
    [LastUserId]            BIGINT         NOT NULL,
    [LastTime]              DATETIME       NOT NULL,
    [CreatedOn]             DATETIME       NOT NULL,
    [LastModified]          ROWVERSION     NOT NULL,
    CONSTRAINT [PK__NoiTruHoSoKhac] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[NoiTruKhoaPhongDieuTri] (
    [Id]                    BIGINT          IDENTITY (1, 1) NOT NULL,
    [NoiTruBenhAnId]        BIGINT          NOT NULL,
    [KhoaPhongChuyenDiId]   BIGINT          NULL,
    [KhoaPhongChuyenDenId]  BIGINT          NOT NULL,
    [ThoiDiemVaoKhoa]       DATETIME        NOT NULL,
    [ThoiDiemRaKhoa]        DATETIME        NULL,
    [ChanDoanVaoKhoaICDId]  BIGINT          NULL,
    [ChanDoanVaoKhoaGhiChu] NVARCHAR (4000) NULL,
    [LyDoChuyenKhoa]        NVARCHAR (1000) NULL,
    [NhanVienChiDinhId]     BIGINT          NOT NULL,
    [CreatedById]           BIGINT          NOT NULL,
    [LastUserId]            BIGINT          NOT NULL,
    [LastTime]              DATETIME        NOT NULL,
    [CreatedOn]             DATETIME        NOT NULL,
    [LastModified]          ROWVERSION      NOT NULL,
    CONSTRAINT [PK__NoiTruKhoaPhongDieuTri] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[NoiTruPhieuDieuTri] (
    [Id]                  BIGINT          IDENTITY (1, 1) NOT NULL,
    [NoiTruBenhAnId]      BIGINT          NOT NULL,
    [NhanVienLapId]       BIGINT          NOT NULL,
    [KhoaPhongDieuTriId]  BIGINT          NOT NULL,
    [NgayDieuTri]         DATETIME        NOT NULL,
    [ThoiDiemThamKham]    DATETIME        NULL,
    [ChanDoanChinhICDId]  BIGINT          NULL,
    [ChanDoanChinhGhiChu] NVARCHAR (4000) NULL,
    [DienBien]            NVARCHAR (MAX)  NULL,
    [CheDoChamSoc]        INT             NULL,
    [GhiChuChamSoc]       NVARCHAR (1000) NULL,
    [CreatedById]         BIGINT          NOT NULL,
    [LastUserId]          BIGINT          NOT NULL,
    [LastTime]            DATETIME        NOT NULL,
    [CreatedOn]           DATETIME        NOT NULL,
    [LastModified]        ROWVERSION      NOT NULL,
    CONSTRAINT [PK__NoiTruPhieuDieuTri] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[NoiTruPhieuDieuTriChiTietYLenh] (
    [Id]                        BIGINT          IDENTITY (1, 1) NOT NULL,
    [NoiTruPhieuDieuTriId]      BIGINT          NOT NULL,
    [YeuCauDichVuKyThuatId]     BIGINT          NULL,
    [YeuCauDuocPhamBenhVienId]  BIGINT          NULL,
    [YeuCauVatTuBenhVienId]     BIGINT          NULL,
    [MoTaYLenh]                 NVARCHAR (1000) NOT NULL,
    [ThoiDiemChiDinhThucHien]   INT             NULL,
    [XacNhanThucHien]           BIT             NULL,
    [ThoiDiemXacNhanThucHien]   DATETIME        NULL,
    [NhanVienXacNhanThucHienId] BIGINT          NULL,
    [LyDoKhongThucHien]         NVARCHAR (1000) NULL,
    [CreatedById]               BIGINT          NOT NULL,
    [LastUserId]                BIGINT          NOT NULL,
    [LastTime]                  DATETIME        NOT NULL,
    [CreatedOn]                 DATETIME        NOT NULL,
    [LastModified]              ROWVERSION      NOT NULL,
    CONSTRAINT [PK__NoiTruPhieuDieuTriChiTietYLenh] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[NoiTruThamKhamChanDoanKemTheo] (
    [Id]                   BIGINT          IDENTITY (1, 1) NOT NULL,
    [NoiTruPhieuDieuTriId] BIGINT          NOT NULL,
    [ICDId]                BIGINT          NOT NULL,
    [GhiChu]               NVARCHAR (4000) NULL,
    [CreatedById]          BIGINT          NOT NULL,
    [LastUserId]           BIGINT          NOT NULL,
    [LastTime]             DATETIME        NOT NULL,
    [CreatedOn]            DATETIME        NOT NULL,
    [LastModified]         ROWVERSION      NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[YeuCauDichVuGiuongBenhVienChiPhi] (
    [Id]                            BIGINT          IDENTITY (1, 1) NOT NULL,
    [YeuCauTiepNhanId]              BIGINT          NOT NULL,
    [DichVuGiuongBenhVienId]        BIGINT          NOT NULL,
    [NhomGiaDichVuGiuongBenhVienId] BIGINT          NOT NULL,
    [Ten]                           NVARCHAR (250)  NOT NULL,
    [Ma]                            NVARCHAR (50)   NOT NULL,
    [MaTT37]                        NVARCHAR (50)   NULL,
    [LoaiGiuong]                    INT             NOT NULL,
    [MoTa]                          NVARCHAR (4000) NULL,
    [Gia]                           DECIMAL (15, 2) NOT NULL,
    [SoNgay]                        INT             NOT NULL,
    [DuocHuongBaoHiem]              BIT             NOT NULL,
    [BaoHiemChiTra]                 BIT             NULL,
    [ThoiDiemDuyetBaoHiem]          DATETIME        NULL,
    [NhanVienDuyetBaoHiemId]        BIGINT          NULL,
    [GiaBaoHiemThanhToan]           DECIMAL (15, 2) NULL,
    [SoTienBenhNhanDaChi]           DECIMAL (15, 2) NULL,
    [TrangThaiThanhToan]            INT             NOT NULL,
    [NhanVienHuyThanhToanId]        BIGINT          NULL,
    [LyDoHuyThanhToan]              NVARCHAR (1000) NULL,
	[DoiTuongSuDung]                INT             NULL,
    [GhiChu]                        NVARCHAR (1000) NULL,
    [CreatedById]                   BIGINT          NOT NULL,
    [LastUserId]                    BIGINT          NOT NULL,
    [LastTime]                      DATETIME        NOT NULL,
    [CreatedOn]                     DATETIME        NOT NULL,
    [LastModified]                  ROWVERSION      NOT NULL,
    [SoTienBaoHiemTuNhanChiTra]     DECIMAL (15, 2) NULL,
    [SoTienMienGiam]                DECIMAL (15, 2) NULL,
    [DonGiaBaoHiem]                 DECIMAL (15, 2) NULL,
    [MucHuongBaoHiem]               INT             NULL,
    [TiLeBaoHiemThanhToan]          INT             NULL,
    CONSTRAINT [PK__YeuCauDichVuGiuongBenhVienChiPhi] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[YeuCauNhapVien] (
    [Id]                     BIGINT          IDENTITY (1, 1) NOT NULL,
    [BenhNhanId]             BIGINT          NOT NULL,
    [BacSiChiDinhId]         BIGINT          NOT NULL,
    [NoiChiDinhId]           BIGINT          NOT NULL,
    [ThoiDiemChiDinh]        DATETIME        NOT NULL,
    [YeuCauKhamBenhId]       BIGINT          NULL,
    [KhoaPhongNhapVienId]    BIGINT          NOT NULL,
    [LyDoNhapVien]           NVARCHAR (1000) NULL,
    [LaCapCuu]               BIT             NOT NULL,
    [ChanDoanNhapVienICDId]  BIGINT          NULL,
    [ChanDoanNhapVienGhiChu] NVARCHAR (4000) NULL,
    [CreatedById]            BIGINT          NOT NULL,
    [LastUserId]             BIGINT          NOT NULL,
    [LastTime]               DATETIME        NOT NULL,
    [CreatedOn]              DATETIME        NOT NULL,
    [LastModified]           ROWVERSION      NOT NULL,
    CONSTRAINT [PK__YeuCauNhapVien] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[YeuCauNhapVienChanDoanKemTheo] (
    [Id]               BIGINT          IDENTITY (1, 1) NOT NULL,
    [YeuCauNhapVienId] BIGINT          NOT NULL,
    [ICDId]            BIGINT          NOT NULL,
    [GhiChu]           NVARCHAR (4000) NULL,
    [CreatedById]      BIGINT          NOT NULL,
    [LastUserId]       BIGINT          NOT NULL,
    [LastTime]         DATETIME        NOT NULL,
    [CreatedOn]        DATETIME        NOT NULL,
    [LastModified]     ROWVERSION      NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[YeuCauTraDuocPhamTuBenhNhan] (
    [Id]               BIGINT          IDENTITY (1, 1) NOT NULL,
    [KhoTraId]         BIGINT          NOT NULL,
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
    [SoPhieu]          AS              (concat('PT', RIGHT(datepart(year, [CreatedOn]), (2)), RIGHT('000000' + ltrim(str([Id])), (6)))),
    CONSTRAINT [PK__YeuCauTraDuocPhamTuBenhNhan] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[YeuCauTraDuocPhamTuBenhNhanChiTiet] (
    [Id]                            BIGINT     IDENTITY (1, 1) NOT NULL,
    [YeuCauTraDuocPhamTuBenhNhanId] BIGINT     NOT NULL,
    [YeuCauDuocPhamBenhVienId]      BIGINT     NOT NULL,
    [DuocPhamBenhVienId]            BIGINT     NOT NULL,
    [LaDuocPhamBHYT]                BIT        NOT NULL,
    [SoLuongTra]                    FLOAT (53) NOT NULL,
    [CreatedById]                   BIGINT     NOT NULL,
    [LastUserId]                    BIGINT     NOT NULL,
    [LastTime]                      DATETIME   NOT NULL,
    [CreatedOn]                     DATETIME   NOT NULL,
    [LastModified]                  ROWVERSION NOT NULL,
    CONSTRAINT [PK_YeuCauTraDuocPhamTuBenhNhanChiTiet] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[YeuCauTraVatTuTuBenhNhan] (
    [Id]               BIGINT          IDENTITY (1, 1) NOT NULL,
    [KhoTraId]         BIGINT          NOT NULL,
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
    [SoPhieu]          AS              (concat('PT', RIGHT(datepart(year, [CreatedOn]), (2)), RIGHT('000000' + ltrim(str([Id])), (6)))),
    CONSTRAINT [PK__YeuCauTraVatTuTuBenhNhan] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE TABLE [dbo].[YeuCauTraVatTuTuBenhNhanChiTiet] (
    [Id]                         BIGINT     IDENTITY (1, 1) NOT NULL,
    [YeuCauTraVatTuTuBenhNhanId] BIGINT     NOT NULL,
    [YeuCauVatTuBenhVienId]      BIGINT     NOT NULL,
    [VatTuBenhVienId]            BIGINT     NOT NULL,
    [LaVatTuBHYT]                BIT        NOT NULL,
    [SoLuongTra]                 FLOAT (53) NOT NULL,
    [CreatedById]                BIGINT     NOT NULL,
    [LastUserId]                 BIGINT     NOT NULL,
    [LastTime]                   DATETIME   NOT NULL,
    [CreatedOn]                  DATETIME   NOT NULL,
    [LastModified]               ROWVERSION NOT NULL,
    CONSTRAINT [PK_YeuCauTraVatTuTuBenhNhanChiTiet] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
ALTER TABLE [dbo].[BenhNhan] ADD [YeuToRh]                     INT             NULL;

GO
ALTER TABLE [dbo].[KetQuaSinhHieu] ADD [NoiTruPhieuDieuTriId] BIGINT     NULL;

GO
ALTER TABLE [dbo].[XuatKhoDuocPhamChiTietViTri] 
	ADD [YeuCauTraDuocPhamTuBenhNhanChiTietId] BIGINT          NULL,
		[GhiChu]                               NVARCHAR (1000) NULL;

GO
ALTER TABLE [dbo].[XuatKhoVatTuChiTietViTri] 
	ADD [YeuCauTraVatTuTuBenhNhanChiTietId] 	BIGINT          NULL,
		[GhiChu]                                NVARCHAR (1000) NULL;

GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat] 
	ADD [NoiTruPhieuDieuTriId]           BIGINT          NULL,
		[DoiTuongSuDung]                 INT             NULL;
		
GO
ALTER TABLE [dbo].[YeuCauDuocPhamBenhVien]
	ADD [NoiTruPhieuDieuTriId]      BIGINT          NULL,
		[SoLanDungTrongNgay]        INT             NULL,
		[DungSang]                  FLOAT (53)      NULL,
		[DungTrua]                  FLOAT (53)      NULL,
		[DungChieu]                 FLOAT (53)      NULL,
		[DungToi]                   FLOAT (53)      NULL,
		[ThoiGianDungSang]          INT             NULL,
		[ThoiGianDungTrua]          INT             NULL,
		[ThoiGianDungChieu]         INT             NULL,
		[ThoiGianDungToi]           INT             NULL,
		[LaDichTruyen]              BIT             NULL,
		[TocDoTruyen]               INT             NULL,
		[DonViTocDoTruyen]          INT             NULL,
		[ThoiGianBatDauTruyen]      INT             NULL,
		[CachGioTruyenDich]         FLOAT (53)      NULL;
		
GO
ALTER TABLE [dbo].[YeuCauTiepNhan] 
	ADD [YeuToRh]                      INT             NULL,
		[YeuCauNhapVienId]             BIGINT          NULL;
		
GO
ALTER TABLE [dbo].[YeuCauVatTuBenhVien] 
	ADD [NoiTruPhieuDieuTriId]      BIGINT          NULL;

GO
ALTER TABLE [dbo].[YeuCauDichVuGiuongBenhVien] 
	ADD [DoiTuongSuDung]                 INT             NULL;

GO
ALTER TABLE [dbo].[NoiTruBenhAn]
    ADD CONSTRAINT [FK_NoiTruBenhAn_YeuCauTiepNhan] FOREIGN KEY ([Id]) REFERENCES [dbo].[YeuCauTiepNhan] ([Id]);

GO
ALTER TABLE [dbo].[NoiTruBenhAn]
    ADD CONSTRAINT [FK_NoiTruBenhAn_BenhNhan] FOREIGN KEY ([BenhNhanId]) REFERENCES [dbo].[BenhNhan] ([Id]);

GO
ALTER TABLE [dbo].[NoiTruBenhAn]
    ADD CONSTRAINT [FK_NoiTruBenhAn_NhanVien] FOREIGN KEY ([NhanVienLuuTruId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[NoiTruBenhAn]
    ADD CONSTRAINT [FK_NoiTruBenhAn_KhoaPhong] FOREIGN KEY ([KhoaPhongNhapVienId]) REFERENCES [dbo].[KhoaPhong] ([Id]);

GO
ALTER TABLE [dbo].[NoiTruBenhAn]
    ADD CONSTRAINT [FK_NoiTruBenhAn_NhanVien1] FOREIGN KEY ([NhanVienTaoBenhAnId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[NoiTruBenhAn]
    ADD CONSTRAINT [FK_NoiTruBenhAn_BenhVien] FOREIGN KEY ([ChuyenDenBenhVienId]) REFERENCES [dbo].[BenhVien] ([Id]);

GO
ALTER TABLE [dbo].[NoiTruEkipDieuTri]
    ADD CONSTRAINT [FK_NoiTruEkipDieuTri_NoiTruBenhAn] FOREIGN KEY ([NoiTruBenhAnId]) REFERENCES [dbo].[NoiTruBenhAn] ([Id]);

GO
ALTER TABLE [dbo].[NoiTruEkipDieuTri]
    ADD CONSTRAINT [FK_NoiTruEkipDieuTri_NhanVien] FOREIGN KEY ([BacSiId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[NoiTruEkipDieuTri]
    ADD CONSTRAINT [FK_NoiTruEkipDieuTri_NhanVien1] FOREIGN KEY ([DieuDuongId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[NoiTruEkipDieuTri]
    ADD CONSTRAINT [FK_NoiTruEkipDieuTri_NhanVien2] FOREIGN KEY ([NhanVienLapId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[NoiTruEkipDieuTri]
    ADD CONSTRAINT [FK_NoiTruEkipDieuTri_KhoaPhong] FOREIGN KEY ([KhoaPhongDieuTriId]) REFERENCES [dbo].[KhoaPhong] ([Id]);

GO
ALTER TABLE [dbo].[NoiTruHoSoKhac]
    ADD CONSTRAINT [FK_NoiTruHoSoKhac_YeuCauTiepNhan] FOREIGN KEY ([YeuCauTiepNhanId]) REFERENCES [dbo].[YeuCauTiepNhan] ([Id]);

GO
ALTER TABLE [dbo].[NoiTruHoSoKhac]
    ADD CONSTRAINT [FK_NoiTruHoSoKhac_NhanVien] FOREIGN KEY ([NhanVienThucHienId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[NoiTruHoSoKhac]
    ADD CONSTRAINT [FK_NoiTruHoSoKhac_PhongBenhVien] FOREIGN KEY ([NoiThucHienId]) REFERENCES [dbo].[PhongBenhVien] ([Id]);

GO
ALTER TABLE [dbo].[NoiTruKhoaPhongDieuTri]
    ADD CONSTRAINT [FK_NoiTruKhoaPhongDieuTri_KhoaPhong] FOREIGN KEY ([KhoaPhongChuyenDiId]) REFERENCES [dbo].[KhoaPhong] ([Id]);

GO
ALTER TABLE [dbo].[NoiTruKhoaPhongDieuTri]
    ADD CONSTRAINT [FK_NoiTruKhoaPhongDieuTri_KhoaPhong1] FOREIGN KEY ([KhoaPhongChuyenDenId]) REFERENCES [dbo].[KhoaPhong] ([Id]);

GO
ALTER TABLE [dbo].[NoiTruKhoaPhongDieuTri]
    ADD CONSTRAINT [FK_NoiTruKhoaPhongDieuTri_ICD] FOREIGN KEY ([ChanDoanVaoKhoaICDId]) REFERENCES [dbo].[ICD] ([Id]);

GO
ALTER TABLE [dbo].[NoiTruKhoaPhongDieuTri]
    ADD CONSTRAINT [FK_NoiTruKhoaPhongDieuTri_NhanVien] FOREIGN KEY ([NhanVienChiDinhId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[NoiTruKhoaPhongDieuTri]
    ADD CONSTRAINT [FK_NoiTruKhoaPhongDieuTri_NoiTruBenhAn] FOREIGN KEY ([NoiTruBenhAnId]) REFERENCES [dbo].[NoiTruBenhAn] ([Id]);

GO
ALTER TABLE [dbo].[NoiTruPhieuDieuTri]
    ADD CONSTRAINT [FK_NoiTruPhieuDieuTri_NhanVien] FOREIGN KEY ([NhanVienLapId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[NoiTruPhieuDieuTri]
    ADD CONSTRAINT [FK_NoiTruPhieuDieuTri_KhoaPhong] FOREIGN KEY ([KhoaPhongDieuTriId]) REFERENCES [dbo].[KhoaPhong] ([Id]);

GO
ALTER TABLE [dbo].[NoiTruPhieuDieuTri]
    ADD CONSTRAINT [FK_NoiTruPhieuDieuTri_ICD] FOREIGN KEY ([ChanDoanChinhICDId]) REFERENCES [dbo].[ICD] ([Id]);

GO
ALTER TABLE [dbo].[NoiTruPhieuDieuTri]
    ADD CONSTRAINT [FK_NoiTruPhieuDieuTri_NoiTruBenhAn] FOREIGN KEY ([NoiTruBenhAnId]) REFERENCES [dbo].[NoiTruBenhAn] ([Id]);

GO
ALTER TABLE [dbo].[NoiTruPhieuDieuTriChiTietYLenh]
    ADD CONSTRAINT [FK_NoiTruPhieuDieuTriChiTietYLenh_YeuCauDichVuKyThuat] FOREIGN KEY ([YeuCauDichVuKyThuatId]) REFERENCES [dbo].[YeuCauDichVuKyThuat] ([Id]);

GO
ALTER TABLE [dbo].[NoiTruPhieuDieuTriChiTietYLenh]
    ADD CONSTRAINT [FK_NoiTruPhieuDieuTriChiTietYLenh_YeuCauDuocPhamBenhVien] FOREIGN KEY ([YeuCauDuocPhamBenhVienId]) REFERENCES [dbo].[YeuCauDuocPhamBenhVien] ([Id]);

GO
ALTER TABLE [dbo].[NoiTruPhieuDieuTriChiTietYLenh]
    ADD CONSTRAINT [FK_NoiTruPhieuDieuTriChiTietYLenh_YeuCauVatTuBenhVien] FOREIGN KEY ([YeuCauVatTuBenhVienId]) REFERENCES [dbo].[YeuCauVatTuBenhVien] ([Id]);

GO
ALTER TABLE [dbo].[NoiTruPhieuDieuTriChiTietYLenh]
    ADD CONSTRAINT [FK_NoiTruPhieuDieuTriChiTietYLenh_NoiTruPhieuDieuTri] FOREIGN KEY ([NoiTruPhieuDieuTriId]) REFERENCES [dbo].[NoiTruPhieuDieuTri] ([Id]);

GO
ALTER TABLE [dbo].[NoiTruPhieuDieuTriChiTietYLenh]
    ADD CONSTRAINT [FK_NoiTruPhieuDieuTriChiTietYLenh_NhanVien] FOREIGN KEY ([NhanVienXacNhanThucHienId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[NoiTruThamKhamChanDoanKemTheo]
    ADD CONSTRAINT [FK_NoiTruThamKhamChanDoanKemTheo_ICD] FOREIGN KEY ([ICDId]) REFERENCES [dbo].[ICD] ([Id]);

GO
ALTER TABLE [dbo].[NoiTruThamKhamChanDoanKemTheo]
    ADD CONSTRAINT [FK_NoiTruThamKhamChanDoanKemTheo_NoiTruPhieuDieuTri] FOREIGN KEY ([NoiTruPhieuDieuTriId]) REFERENCES [dbo].[NoiTruPhieuDieuTri] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauDichVuGiuongBenhVienChiPhi]
    ADD CONSTRAINT [FK_YeuCauDichVuGiuongBenhVienChiPhi_YeuCauTiepNhan] FOREIGN KEY ([YeuCauTiepNhanId]) REFERENCES [dbo].[YeuCauTiepNhan] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauDichVuGiuongBenhVienChiPhi]
    ADD CONSTRAINT [FK_YeuCauDichVuGiuongBenhVienChiPhi_DichVuGiuongBenhVien] FOREIGN KEY ([DichVuGiuongBenhVienId]) REFERENCES [dbo].[DichVuGiuongBenhVien] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauDichVuGiuongBenhVienChiPhi]
    ADD CONSTRAINT [FK_YeuCauDichVuGiuongBenhVienChiPhi_NhomGiaDichVuGiuongBenhVien] FOREIGN KEY ([NhomGiaDichVuGiuongBenhVienId]) REFERENCES [dbo].[NhomGiaDichVuGiuongBenhVien] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauDichVuGiuongBenhVienChiPhi]
    ADD CONSTRAINT [FK_YeuCauDichVuGiuongBenhVienChiPhi_NhanVien] FOREIGN KEY ([NhanVienDuyetBaoHiemId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauDichVuGiuongBenhVienChiPhi]
    ADD CONSTRAINT [FK_YeuCauDichVuGiuongBenhVienChiPhi_NhanVien1] FOREIGN KEY ([NhanVienHuyThanhToanId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauNhapVien]
    ADD CONSTRAINT [FK_YeuCauNhapVien_ICD] FOREIGN KEY ([ChanDoanNhapVienICDId]) REFERENCES [dbo].[ICD] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauNhapVien]
    ADD CONSTRAINT [FK_YeuCauNhapVien_BenhNhan] FOREIGN KEY ([BenhNhanId]) REFERENCES [dbo].[BenhNhan] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauNhapVien]
    ADD CONSTRAINT [FK_YeuCauNhapVien_NhanVien] FOREIGN KEY ([BacSiChiDinhId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauNhapVien]
    ADD CONSTRAINT [FK_YeuCauNhapVien_PhongBenhVien] FOREIGN KEY ([NoiChiDinhId]) REFERENCES [dbo].[PhongBenhVien] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauNhapVien]
    ADD CONSTRAINT [FK_YeuCauNhapVien_YeuCauKhamBenh] FOREIGN KEY ([YeuCauKhamBenhId]) REFERENCES [dbo].[YeuCauKhamBenh] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauNhapVien]
    ADD CONSTRAINT [FK_YeuCauNhapVien_KhoaPhong] FOREIGN KEY ([KhoaPhongNhapVienId]) REFERENCES [dbo].[KhoaPhong] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauNhapVienChanDoanKemTheo]
    ADD CONSTRAINT [FK_YeuCauNhapVienChanDoanKemTheo_YeuCauNhapVien] FOREIGN KEY ([YeuCauNhapVienId]) REFERENCES [dbo].[YeuCauNhapVien] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauNhapVienChanDoanKemTheo]
    ADD CONSTRAINT [FK_YeuCauNhapVienChanDoanKemTheo_ICD] FOREIGN KEY ([ICDId]) REFERENCES [dbo].[ICD] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauTraDuocPhamTuBenhNhan]
    ADD CONSTRAINT [FK_YeuCauTraDuocPhamTuBenhNhan_Kho] FOREIGN KEY ([KhoTraId]) REFERENCES [dbo].[Kho] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauTraDuocPhamTuBenhNhan]
    ADD CONSTRAINT [FK_YeuCauTraDuocPhamTuBenhNhan_NhanVien] FOREIGN KEY ([NhanVienYeuCauId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauTraDuocPhamTuBenhNhan]
    ADD CONSTRAINT [FK_YeuCauTraDuocPhamTuBenhNhan_NhanVien1] FOREIGN KEY ([NhanVienDuyetId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauTraDuocPhamTuBenhNhanChiTiet]
    ADD CONSTRAINT [FK_YeuCauTraDuocPhamTuBenhNhanChiTiet_YeuCauDuocPhamBenhVien] FOREIGN KEY ([YeuCauDuocPhamBenhVienId]) REFERENCES [dbo].[YeuCauDuocPhamBenhVien] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauTraDuocPhamTuBenhNhanChiTiet]
    ADD CONSTRAINT [FK_YeuCauTraDuocPhamTuBenhNhanChiTiet_YeuCauTraDuocPhamTuBenhNhan] FOREIGN KEY ([YeuCauTraDuocPhamTuBenhNhanId]) REFERENCES [dbo].[YeuCauTraDuocPhamTuBenhNhan] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauTraDuocPhamTuBenhNhanChiTiet]
    ADD CONSTRAINT [FK_YeuCauTraDuocPhamTuBenhNhanChiTiet_DuocPhamBenhVien] FOREIGN KEY ([DuocPhamBenhVienId]) REFERENCES [dbo].[DuocPhamBenhVien] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauTraVatTuTuBenhNhan]
    ADD CONSTRAINT [FK_YeuCauTraVatTuTuBenhNhan_Kho] FOREIGN KEY ([KhoTraId]) REFERENCES [dbo].[Kho] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauTraVatTuTuBenhNhan]
    ADD CONSTRAINT [FK_YeuCauTraVatTuTuBenhNhan_NhanVien] FOREIGN KEY ([NhanVienYeuCauId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauTraVatTuTuBenhNhan]
    ADD CONSTRAINT [FK_YeuCauTraVatTuTuBenhNhan_NhanVien1] FOREIGN KEY ([NhanVienDuyetId]) REFERENCES [dbo].[NhanVien] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauTraVatTuTuBenhNhanChiTiet]
    ADD CONSTRAINT [FK_YeuCauTraVatTuTuBenhNhanChiTiet_VatTuBenhVien] FOREIGN KEY ([VatTuBenhVienId]) REFERENCES [dbo].[VatTuBenhVien] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauTraVatTuTuBenhNhanChiTiet]
    ADD CONSTRAINT [FK_YeuCauTraVatTuTuBenhNhanChiTiet_YeuCauTraVatTuTuBenhNhan] FOREIGN KEY ([YeuCauTraVatTuTuBenhNhanId]) REFERENCES [dbo].[YeuCauTraVatTuTuBenhNhan] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauTraVatTuTuBenhNhanChiTiet]
    ADD CONSTRAINT [FK_YeuCauTraVatTuTuBenhNhanChiTiet_YeuCauVatTuBenhVien] FOREIGN KEY ([YeuCauVatTuBenhVienId]) REFERENCES [dbo].[YeuCauVatTuBenhVien] ([Id]);
	
GO
ALTER TABLE [dbo].[KetQuaSinhHieu]	
    ADD CONSTRAINT [FK_KetQuaSinhHieu_NoiTruPhieuDieuTri] FOREIGN KEY ([NoiTruPhieuDieuTriId]) REFERENCES [dbo].[NoiTruPhieuDieuTri] ([Id]);

GO
ALTER TABLE [dbo].[XuatKhoDuocPhamChiTietViTri]
    ADD CONSTRAINT [FK_XuatKhoDuocPhamChiTietViTri_YeuCauTraDuocPhamTuBenhNhanChiTiet] FOREIGN KEY ([YeuCauTraDuocPhamTuBenhNhanChiTietId]) REFERENCES [dbo].[YeuCauTraDuocPhamTuBenhNhanChiTiet] ([Id]);

GO
ALTER TABLE [dbo].[XuatKhoVatTuChiTietViTri]
    ADD CONSTRAINT [FK_XuatKhoVatTuChiTietViTri_YeuCauTraVatTuTuBenhNhanChiTiet] FOREIGN KEY ([YeuCauTraVatTuTuBenhNhanChiTietId]) REFERENCES [dbo].[YeuCauTraVatTuTuBenhNhanChiTiet] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat]
    ADD CONSTRAINT [FK_YeuCauDichVuKyThuat_NoiTruPhieuDieuTri] FOREIGN KEY ([NoiTruPhieuDieuTriId]) REFERENCES [dbo].[NoiTruPhieuDieuTri] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauDuocPhamBenhVien]
    ADD CONSTRAINT [FK_YeuCauDuocPhamBenhVien_NoiTruPhieuDieuTri] FOREIGN KEY ([NoiTruPhieuDieuTriId]) REFERENCES [dbo].[NoiTruPhieuDieuTri] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauDuocPhamBenhVien]
    ADD CONSTRAINT [FK_YeuCauDuocPhamBenhVien_YeuCauDichVuKyThuat] FOREIGN KEY ([YeuCauDichVuKyThuatId]) REFERENCES [dbo].[YeuCauDichVuKyThuat] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauTiepNhan]
    ADD CONSTRAINT [FK_YeuCauTiepNhan_YeuCauNhapVien] FOREIGN KEY ([YeuCauNhapVienId]) REFERENCES [dbo].[YeuCauNhapVien] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauVatTuBenhVien]
    ADD CONSTRAINT [FK_YeuCauVatTuBenhVien_NoiTruPhieuDieuTri] FOREIGN KEY ([NoiTruPhieuDieuTriId]) REFERENCES [dbo].[NoiTruPhieuDieuTri] ([Id]);
	
GO
Update dbo.CauHinh
Set [Value] = '1.3.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'


