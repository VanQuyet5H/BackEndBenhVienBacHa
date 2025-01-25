CREATE TABLE [dbo].[YeuCauDichVuGiuongBenhVienChiPhiBHYT] (
    [Id]                     BIGINT          IDENTITY (1, 1) NOT NULL,
    [NgayPhatSinh]           DATETIME        NOT NULL,
    [YeuCauTiepNhanId]       BIGINT          NOT NULL,
    [DichVuGiuongBenhVienId] BIGINT          NOT NULL,
    [GiuongBenhId]           BIGINT          NOT NULL,
    [PhongBenhVienId]        BIGINT          NOT NULL,
    [KhoaPhongId]            BIGINT          NOT NULL,
    [Ten]                    NVARCHAR (250)  NOT NULL,
    [Ma]                     NVARCHAR (50)   NOT NULL,
    [MaTT37]                 NVARCHAR (50)   NULL,
    [LoaiGiuong]             INT             NOT NULL,
    [MoTa]                   NVARCHAR (4000) NULL,
    [SoLuong]                FLOAT (53)      NOT NULL,
    [SoLuongGhep]            INT             NOT NULL,
    [DuocHuongBaoHiem]       BIT             NOT NULL,
    [BaoHiemChiTra]          BIT             NULL,
    [ThoiDiemDuyetBaoHiem]   DATETIME        NULL,
    [NhanVienDuyetBaoHiemId] BIGINT          NULL,
    [TrangThaiThanhToan]     INT             NOT NULL,
    [GhiChu]                 NVARCHAR (1000) NULL,
    [CreatedById]            BIGINT          NOT NULL,
    [LastUserId]             BIGINT          NOT NULL,
    [LastTime]               DATETIME        NOT NULL,
    [CreatedOn]              DATETIME        NOT NULL,
    [LastModified]           ROWVERSION      NOT NULL,
    [DonGiaBaoHiem]          DECIMAL (15, 2) NULL,
    [MucHuongBaoHiem]        INT             NULL,
    [TiLeBaoHiemThanhToan]   INT             NULL,
    [HeThongTuPhatSinh]      BIT             NULL,
    CONSTRAINT [PK__YeuCauDichVuGiuongBenhVienChiPhiBHYT] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_YeuCauDichVuGiuongBenhVienChiPhiBHYT_DichVuGiuongBenhVien] FOREIGN KEY ([DichVuGiuongBenhVienId]) REFERENCES [dbo].[DichVuGiuongBenhVien] ([Id]),
    CONSTRAINT [FK_YeuCauDichVuGiuongBenhVienChiPhiBHYT_GiuongBenh] FOREIGN KEY ([GiuongBenhId]) REFERENCES [dbo].[GiuongBenh] ([Id]),
    CONSTRAINT [FK_YeuCauDichVuGiuongBenhVienChiPhiBHYT_KhoaPhong] FOREIGN KEY ([KhoaPhongId]) REFERENCES [dbo].[KhoaPhong] ([Id]),
    CONSTRAINT [FK_YeuCauDichVuGiuongBenhVienChiPhiBHYT_NhanVien] FOREIGN KEY ([NhanVienDuyetBaoHiemId]) REFERENCES [dbo].[NhanVien] ([Id]),
    CONSTRAINT [FK_YeuCauDichVuGiuongBenhVienChiPhiBHYT_PhongBenhVien] FOREIGN KEY ([PhongBenhVienId]) REFERENCES [dbo].[PhongBenhVien] ([Id]),
    CONSTRAINT [FK_YeuCauDichVuGiuongBenhVienChiPhiBHYT_YeuCauTiepNhan] FOREIGN KEY ([YeuCauTiepNhanId]) REFERENCES [dbo].[YeuCauTiepNhan] ([Id])
);
GO
CREATE TABLE [dbo].[YeuCauDichVuGiuongBenhVienChiPhiBenhVien] (
    [Id]                            BIGINT          IDENTITY (1, 1) NOT NULL,
    [NgayPhatSinh]                  DATETIME        NOT NULL,
    [YeuCauTiepNhanId]              BIGINT          NOT NULL,
    [DichVuGiuongBenhVienId]        BIGINT          NOT NULL,
    [NhomGiaDichVuGiuongBenhVienId] BIGINT          NOT NULL,
    [GiuongBenhId]                  BIGINT          NOT NULL,
    [PhongBenhVienId]               BIGINT          NOT NULL,
    [KhoaPhongId]                   BIGINT          NOT NULL,
    [Ten]                           NVARCHAR (250)  NOT NULL,
    [Ma]                            NVARCHAR (50)   NOT NULL,
    [MaTT37]                        NVARCHAR (50)   NULL,
    [LoaiGiuong]                    INT             NOT NULL,
    [MoTa]                          NVARCHAR (4000) NULL,
    [Gia]                           DECIMAL (15, 2) NOT NULL,
    [BaoPhong]                      BIT             NULL,
    [SoLuong]                       FLOAT (53)      NOT NULL,
    [SoLuongGhep]                   INT             NOT NULL,
    [SoTienBenhNhanDaChi]           DECIMAL (15, 2) NULL,
    [TrangThaiThanhToan]            INT             NOT NULL,
    [NhanVienHuyThanhToanId]        BIGINT          NULL,
    [LyDoHuyThanhToan]              NVARCHAR (1000) NULL,
    [GhiChu]                        NVARCHAR (1000) NULL,
    [CreatedById]                   BIGINT          NOT NULL,
    [LastUserId]                    BIGINT          NOT NULL,
    [LastTime]                      DATETIME        NOT NULL,
    [CreatedOn]                     DATETIME        NOT NULL,
    [LastModified]                  ROWVERSION      NOT NULL,
    [SoTienBaoHiemTuNhanChiTra]     DECIMAL (15, 2) NULL,
    [SoTienMienGiam]                DECIMAL (15, 2) NULL,
    [DoiTuongSuDung]                INT             NOT NULL,
    [HeThongTuPhatSinh]             BIT             NULL,
    CONSTRAINT [PK__YeuCauDichVuGiuongBenhVienChiPhiBenhVien] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_YeuCauDichVuGiuongBenhVienChiPhiBenhVien_DichVuGiuongBenhVien] FOREIGN KEY ([DichVuGiuongBenhVienId]) REFERENCES [dbo].[DichVuGiuongBenhVien] ([Id]),
    CONSTRAINT [FK_YeuCauDichVuGiuongBenhVienChiPhiBenhVien_GiuongBenh] FOREIGN KEY ([GiuongBenhId]) REFERENCES [dbo].[GiuongBenh] ([Id]),
    CONSTRAINT [FK_YeuCauDichVuGiuongBenhVienChiPhiBenhVien_KhoaPhong] FOREIGN KEY ([KhoaPhongId]) REFERENCES [dbo].[KhoaPhong] ([Id]),
    CONSTRAINT [FK_YeuCauDichVuGiuongBenhVienChiPhiBenhVien_NhanVien] FOREIGN KEY ([NhanVienHuyThanhToanId]) REFERENCES [dbo].[NhanVien] ([Id]),
    CONSTRAINT [FK_YeuCauDichVuGiuongBenhVienChiPhiBenhVien_NhomGiaDichVuGiuongBenhVien] FOREIGN KEY ([NhomGiaDichVuGiuongBenhVienId]) REFERENCES [dbo].[NhomGiaDichVuGiuongBenhVien] ([Id]),
    CONSTRAINT [FK_YeuCauDichVuGiuongBenhVienChiPhiBenhVien_PhongBenhVien] FOREIGN KEY ([PhongBenhVienId]) REFERENCES [dbo].[PhongBenhVien] ([Id]),
    CONSTRAINT [FK_YeuCauDichVuGiuongBenhVienChiPhiBenhVien_YeuCauTiepNhan] FOREIGN KEY ([YeuCauTiepNhanId]) REFERENCES [dbo].[YeuCauTiepNhan] ([Id])
);
GO
ALTER TABLE [dbo].[YeuCauTiepNhan]
	ADD	
	[BHYTDuocGiaHanThe]                    BIT             NULL,
    [BHYTThe2MaSoThe]                      NVARCHAR (20)   NULL,
    [BHYTThe2MucHuong]                     INT             NULL,
    [BHYTThe2MaDKBD]                       NVARCHAR (20)   NULL,
    [BHYTThe2NgayHieuLuc]                  DATETIME        NULL,
    [BHYTThe2NgayHetHan]                   DATETIME        NULL,
    [BHYTThe2DiaChi]                       NVARCHAR (200)  NULL,
    [BHYTThe2CoQuanBHXH]                   NVARCHAR (200)  NULL,
    [BHYTThe2NgayDu5Nam]                   DATETIME        NULL,
    [BHYTThe2NgayDuocMienCungChiTra]       DATETIME        NULL,
    [BHYTThe2MaKhuVuc]                     NVARCHAR (5)    NULL,
	[QuyetToanTheoNoiTru]                  BIT             NULL,
    [YeuCauTiepNhanNgoaiTruCanQuyetToanId] BIGINT          NULL
GO
ALTER TABLE [dbo].[YeuCauTiepNhan]
	ADD	CONSTRAINT [FK_YeuCauTiepNhan_YeuCauTiepNhan] FOREIGN KEY ([YeuCauTiepNhanNgoaiTruCanQuyetToanId]) REFERENCES [dbo].[YeuCauTiepNhan] ([Id])
GO

ALTER TABLE [dbo].[CongTyBaoHiemTuNhanCongNo]	
	ADD	
	[YeuCauDichVuGiuongBenhVienChiPhiBenhVienId] BIGINT          NULL,
	CONSTRAINT [FK_CongTyBaoHiemTuNhanCongNo_YeuCauDichVuGiuongBenhVienChiPhiBenhVien] FOREIGN KEY ([YeuCauDichVuGiuongBenhVienChiPhiBenhVienId]) REFERENCES [dbo].[YeuCauDichVuGiuongBenhVienChiPhiBenhVien] ([Id])
GO
ALTER TABLE [dbo].[DuyetBaoHiemChiTiet]
	ADD	
	[YeuCauDichVuGiuongBenhVienChiPhiBHYTId] BIGINT          NULL,
	CONSTRAINT [FK_DuyetBaoHiemChiTiet_YeuCauDichVuGiuongBenhVienChiPhiBHYT] FOREIGN KEY ([YeuCauDichVuGiuongBenhVienChiPhiBHYTId]) REFERENCES [dbo].[YeuCauDichVuGiuongBenhVienChiPhiBHYT] ([Id])
GO
ALTER TABLE [dbo].[MienGiamChiPhi]	
	ADD	
	[YeuCauDichVuGiuongBenhVienChiPhiBenhVienId] BIGINT          NULL,
	CONSTRAINT [FK_MienGiamChiPhi_YeuCauDichVuGiuongBenhVienChiPhiBenhVien] FOREIGN KEY ([YeuCauDichVuGiuongBenhVienChiPhiBenhVienId]) REFERENCES [dbo].[YeuCauDichVuGiuongBenhVienChiPhiBenhVien] ([Id])
GO
ALTER TABLE [dbo].[NoiTruBenhAn]
	ADD	
	[ChanDoanChinhRaVienICDId]  BIGINT          NULL,
    [ChanDoanChinhRaVienGhiChu] NVARCHAR (4000) NULL,
    [DaQuyetToan]               BIT             NULL,
	CONSTRAINT [FK_NoiTruBenhAn_ICD] FOREIGN KEY ([ChanDoanChinhRaVienICDId]) REFERENCES [dbo].[ICD] ([Id])
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanChi]	
	ADD	
	[YeuCauTruyenMauId] BIGINT          NULL,
	[YeuCauDichVuGiuongBenhVienChiPhiBenhVienId] BIGINT          NULL,
	CONSTRAINT [FK_TaiKhoanBenhNhanChi_YeuCauTruyenMau] FOREIGN KEY ([YeuCauTruyenMauId]) REFERENCES [dbo].[YeuCauTruyenMau] ([Id]),
	CONSTRAINT [FK_TaiKhoanBenhNhanChi_YeuCauDichVuGiuongBenhVienChiPhiBenhVien] FOREIGN KEY ([YeuCauDichVuGiuongBenhVienChiPhiBenhVienId]) REFERENCES [dbo].[YeuCauDichVuGiuongBenhVienChiPhiBenhVien] ([Id])
GO
ALTER TABLE [dbo].[TaiKhoanBenhNhanThu]	
	ADD	
	[HoanTraYeuCauDichVuGiuongBenhVienChiPhiBenhVienId] BIGINT          NULL,
	[HoanTraYeuCauTruyenMauId] BIGINT          NULL,
	CONSTRAINT [FK_TaiKhoanBenhNhanThu_YeuCauTruyenMau] FOREIGN KEY ([HoanTraYeuCauTruyenMauId]) REFERENCES [dbo].[YeuCauTruyenMau] ([Id]),
	CONSTRAINT [FK_TaiKhoanBenhNhanThu_YeuCauDichVuGiuongBenhVienChiPhiBenhVien] FOREIGN KEY ([HoanTraYeuCauDichVuGiuongBenhVienChiPhiBenhVienId]) REFERENCES [dbo].[YeuCauDichVuGiuongBenhVienChiPhiBenhVien] ([Id])

Update dbo.CauHinh
Set [Value] = '1.4.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'
	