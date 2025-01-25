ALTER TABLE dbo.[ChuongTrinhGoiDichVu] DROP COLUMN [TiLeChietKhau];
GO
ALTER TABLE dbo.[ChuongTrinhGoiDichVu] ADD [GoiSoSinh]         BIT             NULL;

GO
ALTER TABLE dbo.[ChuongTrinhGoiDichVuDichVuGiuong] 
	ADD 
		[DonGiaTruocChietKhau]          DECIMAL (15, 2) NOT NULL,
		[DonGiaSauChietKhau]            DECIMAL (15, 2) NOT NULL;
		
GO
ALTER TABLE dbo.[ChuongTrinhGoiDichVuDichVuKhamBenh] 
	ADD 
		[DonGiaTruocChietKhau]          DECIMAL (15, 2) NOT NULL,
		[DonGiaSauChietKhau]            DECIMAL (15, 2) NOT NULL;
		
GO
ALTER TABLE dbo.[ChuongTrinhGoiDichVuDichVuKyThuat] 
	ADD 
		[DonGiaTruocChietKhau]          DECIMAL (15, 2) NOT NULL,
		[DonGiaSauChietKhau]            DECIMAL (15, 2) NOT NULL;
		
GO
ALTER TABLE dbo.[MienGiamChiPhi] 
	ADD 
		[YeuCauGoiDichVuKhuyenMaiId]                 BIGINT          NULL,
		CONSTRAINT [FK_MienGiamChiPhi_YeuCauGoiDichVu1] FOREIGN KEY ([YeuCauGoiDichVuKhuyenMaiId]) REFERENCES [dbo].[YeuCauGoiDichVu] ([Id]);
		
GO
ALTER TABLE dbo.[TaiKhoanBenhNhanThu] ADD [ThuTienGoiDichVu]                                  BIT             NULL;

GO
ALTER TABLE dbo.[YeuCauDichVuGiuongBenhVien] 
	ADD 
		[DonGiaTruocChietKhau]          DECIMAL (15, 2) NULL,
		[DonGiaSauChietKhau]            DECIMAL (15, 2) NULL;

GO
ALTER TABLE dbo.[YeuCauDichVuGiuongBenhVienChiPhiBenhVien]
	ADD 
		[DonGiaTruocChietKhau]          DECIMAL (15, 2) NULL,
		[DonGiaSauChietKhau]            DECIMAL (15, 2) NULL,
		[YeuCauGoiDichVuId]    			BIGINT          NULL,
		CONSTRAINT [FK_YeuCauDichVuGiuongBenhVienChiPhiBenhVien_YeuCauGoiDichVu] FOREIGN KEY ([YeuCauGoiDichVuId]) REFERENCES [dbo].[YeuCauGoiDichVu] ([Id]);

GO
ALTER TABLE dbo.[YeuCauDichVuKyThuat]
	ADD 
		[DonGiaTruocChietKhau]          DECIMAL (15, 2) NULL,
		[DonGiaSauChietKhau]            DECIMAL (15, 2) NULL;
		
GO
ALTER TABLE dbo.[YeuCauGoiDichVu] DROP COLUMN [TiLeChietKhau];
GO
ALTER TABLE dbo.[YeuCauGoiDichVu]
	ADD 
		[NhanVienTuVanId]           BIGINT          NULL,
		[BenhNhanSoSinhId]          BIGINT          NULL,
		[GoiSoSinh]                 BIT             NULL,
		[DaQuyetToan]               BIT             NULL,
		[ThoiDiemQuyetToan]         DATETIME        NULL,
		[NhanVienQuyetToanId]       BIGINT          NULL,
		[NoiQuyetToanId]			BIGINT          NULL,
		[SoTienTraLai]              DECIMAL (15, 2) NULL,
		[LyDoHuyQuyetToan]          NVARCHAR (1000) NULL,
		[ThoiDiemHuyQuyetToan]      DATETIME        NULL,
		[NhanVienHuyQuyetToanId]    BIGINT          NULL,
		CONSTRAINT [FK_YeuCauGoiDichVu_BenhNhan1] FOREIGN KEY ([BenhNhanSoSinhId]) REFERENCES [dbo].[BenhNhan] ([Id]),
		CONSTRAINT [FK_YeuCauGoiDichVu_NhanVien1] FOREIGN KEY ([NhanVienTuVanId]) REFERENCES [dbo].[NhanVien] ([Id]),
		CONSTRAINT [FK_YeuCauGoiDichVu_NhanVien2] FOREIGN KEY ([NhanVienQuyetToanId]) REFERENCES [dbo].[NhanVien] ([Id]),
		CONSTRAINT [FK_YeuCauGoiDichVu_NhanVien3] FOREIGN KEY ([NhanVienHuyQuyetToanId]) REFERENCES [dbo].[NhanVien] ([Id]),
		CONSTRAINT [FK_YeuCauGoiDichVu_PhongBenhVien1] FOREIGN KEY ([NoiQuyetToanId]) REFERENCES [dbo].[PhongBenhVien] ([Id]);

GO
ALTER TABLE dbo.[YeuCauKhamBenh]
	ADD 
		[DonGiaTruocChietKhau]          DECIMAL (15, 2) NULL,
		[DonGiaSauChietKhau]            DECIMAL (15, 2) NULL;

GO
CREATE TABLE [dbo].[ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuong] (
    [Id]                            BIGINT          IDENTITY (1, 1) NOT NULL,
    [ChuongTrinhGoiDichVuId]        BIGINT          NOT NULL,
    [DichVuGiuongBenhVienId]        BIGINT          NOT NULL,
    [NhomGiaDichVuGiuongBenhVienId] BIGINT          NOT NULL,
    [DonGia]                        DECIMAL (15, 2) NOT NULL,
    [DonGiaKhuyenMai]               DECIMAL (15, 2) NOT NULL,
    [SoLan]                         INT             NOT NULL,
    [SoNgaySuDung]                  INT             NOT NULL,
    [GhiChu]                        NVARCHAR (1000) NULL,
    [CreatedById]                   BIGINT          NOT NULL,
    [LastUserId]                    BIGINT          NOT NULL,
    [LastTime]                      DATETIME        NOT NULL,
    [CreatedOn]                     DATETIME        NOT NULL,
    [LastModified]                  ROWVERSION      NOT NULL,
    CONSTRAINT [PK__ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuong] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuong_ChuongTrinhGoiDichVu] FOREIGN KEY ([ChuongTrinhGoiDichVuId]) REFERENCES [dbo].[ChuongTrinhGoiDichVu] ([Id]),
	CONSTRAINT [FK_ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuong_DichVuGiuongBenhVien] FOREIGN KEY ([DichVuGiuongBenhVienId]) REFERENCES [dbo].[DichVuGiuongBenhVien] ([Id]),
	CONSTRAINT [FK_ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuong_NhomGiaDichVuGiuongBenhVien] FOREIGN KEY ([NhomGiaDichVuGiuongBenhVienId]) REFERENCES [dbo].[NhomGiaDichVuGiuongBenhVien] ([Id])
);
GO
CREATE TABLE [dbo].[ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh] (
    [Id]                              BIGINT          IDENTITY (1, 1) NOT NULL,
    [ChuongTrinhGoiDichVuId]          BIGINT          NOT NULL,
    [DichVuKhamBenhBenhVienId]        BIGINT          NOT NULL,
    [NhomGiaDichVuKhamBenhBenhVienId] BIGINT          NOT NULL,
    [DonGia]                          DECIMAL (15, 2) NOT NULL,
    [DonGiaKhuyenMai]                 DECIMAL (15, 2) NOT NULL,
    [SoLan]                           INT             NOT NULL,
    [SoNgaySuDung]                    INT             NOT NULL,
    [GhiChu]                          NVARCHAR (1000) NULL,
    [CreatedById]                     BIGINT          NOT NULL,
    [LastUserId]                      BIGINT          NOT NULL,
    [LastTime]                        DATETIME        NOT NULL,
    [CreatedOn]                       DATETIME        NOT NULL,
    [LastModified]                    ROWVERSION      NOT NULL,
    CONSTRAINT [PK__ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh_ChuongTrinhGoiDichVu] FOREIGN KEY ([ChuongTrinhGoiDichVuId]) REFERENCES [dbo].[ChuongTrinhGoiDichVu] ([Id]),
	CONSTRAINT [FK_ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh_DichVuKhamBenhBenhVien] FOREIGN KEY ([DichVuKhamBenhBenhVienId]) REFERENCES [dbo].[DichVuKhamBenhBenhVien] ([Id]),
	CONSTRAINT [FK_ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh_NhomGiaDichVuKhamBenhBenhVien] FOREIGN KEY ([NhomGiaDichVuKhamBenhBenhVienId]) REFERENCES [dbo].[NhomGiaDichVuKhamBenhBenhVien] ([Id])
);
GO
CREATE TABLE [dbo].[ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat] (
    [Id]                             BIGINT          IDENTITY (1, 1) NOT NULL,
    [ChuongTrinhGoiDichVuId]         BIGINT          NOT NULL,
    [DichVuKyThuatBenhVienId]        BIGINT          NOT NULL,
    [NhomGiaDichVuKyThuatBenhVienId] BIGINT          NOT NULL,
    [DonGia]                         DECIMAL (15, 2) NOT NULL,
    [DonGiaKhuyenMai]                DECIMAL (15, 2) NOT NULL,
    [SoLan]                          INT             NOT NULL,
    [SoNgaySuDung]                   INT             NOT NULL,
    [GhiChu]                         NVARCHAR (1000) NULL,
    [CreatedById]                    BIGINT          NOT NULL,
    [LastUserId]                     BIGINT          NOT NULL,
    [LastTime]                       DATETIME        NOT NULL,
    [CreatedOn]                      DATETIME        NOT NULL,
    [LastModified]                   ROWVERSION      NOT NULL,
    CONSTRAINT [PK__ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat_ChuongTrinhGoiDichVu] FOREIGN KEY ([ChuongTrinhGoiDichVuId]) REFERENCES [dbo].[ChuongTrinhGoiDichVu] ([Id]),
	CONSTRAINT [FK_ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat_DichVuKyThuatBenhVien] FOREIGN KEY ([DichVuKyThuatBenhVienId]) REFERENCES [dbo].[DichVuKyThuatBenhVien] ([Id]),
	CONSTRAINT [FK_ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat_NhomGiaDichVuKyThuatBenhVien] FOREIGN KEY ([NhomGiaDichVuKyThuatBenhVienId]) REFERENCES [dbo].[NhomGiaDichVuKyThuatBenhVien] ([Id])
);

GO
UPDATE CauHinh
Set [Value] = '2.0.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'