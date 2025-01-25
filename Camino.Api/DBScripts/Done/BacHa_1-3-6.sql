CREATE TABLE [dbo].[NoiTruHoSoKhacFileDinhKem](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[NoiTruHoSoKhacId] [bigint] NOT NULL,
	[Ma] [nvarchar](50) NULL,
	[Ten] [nvarchar](256) NOT NULL,
	[TenGuid] [nvarchar](256) NOT NULL,
	[KichThuoc] [bigint] NOT NULL,
	[DuongDan] [nvarchar](500) NOT NULL,
	[LoaiTapTin] [int] NOT NULL,
	[MoTa] [nvarchar](2000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
	
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[NoiTruHoSoKhacFileDinhKem]  WITH CHECK ADD  CONSTRAINT [FK_NoiTruHoSoKhacFileDinhKem_NoiTruHoSoKhac] FOREIGN KEY([NoiTruHoSoKhacId])
REFERENCES [dbo].[NoiTruHoSoKhac] ([Id])
GO
ALTER TABLE [dbo].[NoiTruHoSoKhacFileDinhKem] CHECK CONSTRAINT [FK_NoiTruHoSoKhacFileDinhKem_NoiTruHoSoKhac]

GO
CREATE TABLE [dbo].[NoiTruPhieuDieuTriChiTietDienBien] (
    [Id]                   BIGINT          IDENTITY (1, 1) NOT NULL,
    [NoiTruPhieuDieuTriId] BIGINT          NOT NULL,
    [MoTaDienBien]         NVARCHAR (4000) NOT NULL,
    [GioDienBien]          INT             NOT NULL,
    [ThoiDiemCapNhat]      DATETIME        NULL,
    [NhanVienCapNhatId]    BIGINT          NULL,
    [CreatedById]          BIGINT          NOT NULL,
    [LastUserId]           BIGINT          NOT NULL,
    [LastTime]             DATETIME        NOT NULL,
    [CreatedOn]            DATETIME        NOT NULL,
    [LastModified]         ROWVERSION      NOT NULL,
    CONSTRAINT [PK__NoiTruPhieuDieuTriChiTietDienBien] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
ALTER TABLE [dbo].[NoiTruPhieuDieuTriChiTietDienBien] WITH CHECK
    ADD CONSTRAINT [FK_NoiTruPhieuDieuTriChiTietDienBien_NoiTruPhieuDieuTri] FOREIGN KEY ([NoiTruPhieuDieuTriId]) REFERENCES [dbo].[NoiTruPhieuDieuTri] ([Id]);
GO
ALTER TABLE [dbo].[NoiTruPhieuDieuTriChiTietDienBien] WITH CHECK CHECK CONSTRAINT [FK_NoiTruPhieuDieuTriChiTietDienBien_NoiTruPhieuDieuTri];

GO
ALTER TABLE [dbo].[NoiTruPhieuDieuTriChiTietDienBien] WITH CHECK
    ADD CONSTRAINT [FK_NoiTruPhieuDieuTriChiTietDienBien_NhanVien] FOREIGN KEY ([NhanVienCapNhatId]) REFERENCES [dbo].[NhanVien] ([Id]);
GO
ALTER TABLE [dbo].[NoiTruPhieuDieuTriChiTietDienBien] WITH CHECK CHECK CONSTRAINT [FK_NoiTruPhieuDieuTriChiTietDienBien_NhanVien];

GO
ALTER TABLE dbo.NoiTruPhieuDieuTriChiTietYLenh
ADD   
	[YeuCauTruyenMauId]         BIGINT          NULL,
	[GioYLenh]                  INT             NOT NULL,
    [NhanVienChiDinhId]         BIGINT          NOT NULL,
    [NoiChiDinhId]              BIGINT          NOT NULL,
	[ThoiDiemCapNhat]           DATETIME        NULL,
    [NhanVienCapNhatId]         BIGINT          NULL
GO
ALTER TABLE dbo.NoiTruPhieuDieuTriChiTietYLenh
ADD CONSTRAINT [FK_NoiTruPhieuDieuTriChiTietYLenh_YeuCauTruyenMau] FOREIGN KEY ([YeuCauTruyenMauId]) REFERENCES [dbo].[YeuCauTruyenMau] ([Id]);
GO
ALTER TABLE dbo.NoiTruPhieuDieuTriChiTietYLenh
ADD CONSTRAINT [FK_NoiTruPhieuDieuTriChiTietYLenh_NhanVien1] FOREIGN KEY ([NhanVienChiDinhId]) REFERENCES [dbo].[NhanVien] ([Id]);
GO
ALTER TABLE dbo.NoiTruPhieuDieuTriChiTietYLenh
ADD CONSTRAINT [FK_NoiTruPhieuDieuTriChiTietYLenh_NhanVien2] FOREIGN KEY ([NhanVienCapNhatId]) REFERENCES [dbo].[NhanVien] ([Id]);
GO
ALTER TABLE dbo.NoiTruPhieuDieuTriChiTietYLenh
ADD CONSTRAINT [FK_NoiTruPhieuDieuTriChiTietYLenh_PhongBenhVien] FOREIGN KEY ([NoiChiDinhId]) REFERENCES [dbo].[PhongBenhVien] ([Id]);
GO
ALTER TABLE dbo.NoiTruPhieuDieuTriChiTietYLenh
DROP COLUMN ThoiDiemChiDinhThucHien;
GO
Update dbo.CauHinh
Set [Value] = '1.3.6' where [Name] = 'CauHinhHeThong.DatabaseVesion'

