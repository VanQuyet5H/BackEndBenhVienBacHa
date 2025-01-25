CREATE TABLE [dbo].[YeuCauTiepNhanLichSuChuyenDoiTuong] (
    [Id]                     BIGINT         IDENTITY (1, 1) NOT NULL,
    [YeuCauTiepNhanId]       BIGINT         NOT NULL,
    [DoiTuongTiepNhan]       INT            NOT NULL,
    [MaSoThe]                NVARCHAR (20)  NULL,
    [MucHuong]               INT            NULL,
    [MaDKBD]                 NVARCHAR (20)  NULL,
    [NgayHieuLuc]            DATETIME       NULL,
    [NgayHetHan]             DATETIME       NULL,
    [DiaChi]                 NVARCHAR (200) NULL,
    [CoQuanBHXH]             NVARCHAR (200) NULL,
    [NgayDu5Nam]             DATETIME       NULL,
    [NgayDuocMienCungChiTra] DATETIME       NULL,
    [MaKhuVuc]               NVARCHAR (5)   NULL,
    [DuocMienCungChiTra]     BIT            NULL,
    [GiayMienCungChiTraId]   BIGINT         NULL,
    [TinhTrangThe]           INT            NULL,
    [IsCheckedBHYT]          BIT            NULL,
    [DuocGiaHanThe]          BIT            NULL,
    [DaHuy]                  BIT            NULL,
    [CreatedById]            BIGINT         NOT NULL,
    [LastUserId]             BIGINT         NOT NULL,
    [LastTime]               DATETIME       NOT NULL,
    [CreatedOn]              DATETIME       NOT NULL,
    [LastModified]           ROWVERSION     NULL,
    CONSTRAINT [PK__YeuCauTiepNhanLichSuChuyenDoiTuong] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_YeuCauTiepNhanLichSuChuyenDoiTuong_GiayMienCungChiTra] FOREIGN KEY ([GiayMienCungChiTraId]) REFERENCES [dbo].[GiayMienCungChiTra] ([Id]),
    CONSTRAINT [FK_YeuCauTiepNhanLichSuChuyenDoiTuong_YeuCauTiepNhan] FOREIGN KEY ([YeuCauTiepNhanId]) REFERENCES [dbo].[YeuCauTiepNhan] ([Id])
);
GO
CREATE TABLE [dbo].[YeuCauTiepNhanTheBHYT] (
    [Id]                     BIGINT         IDENTITY (1, 1) NOT NULL,
    [YeuCauTiepNhanId]       BIGINT         NOT NULL,
    [BenhNhanId]             BIGINT         NOT NULL,
    [MaSoThe]                NVARCHAR (20)  NOT NULL,
    [MucHuong]               INT            NOT NULL,
    [MaDKBD]                 NVARCHAR (20)  NOT NULL,
    [NgayHieuLuc]            DATETIME       NOT NULL,
    [NgayHetHan]             DATETIME       NULL,
    [DiaChi]                 NVARCHAR (200) NOT NULL,
    [CoQuanBHXH]             NVARCHAR (200) NULL,
    [NgayDu5Nam]             DATETIME       NULL,
    [NgayDuocMienCungChiTra] DATETIME       NULL,
    [MaKhuVuc]               NVARCHAR (5)   NULL,
    [DuocMienCungChiTra]     BIT            NULL,
    [GiayMienCungChiTraId]   BIGINT         NULL,
    [TinhTrangThe]           INT            NULL,
    [IsCheckedBHYT]          BIT            NULL,
    [DuocGiaHanThe]          BIT            NULL,
    [CreatedById]            BIGINT         NOT NULL,
    [LastUserId]             BIGINT         NOT NULL,
    [LastTime]               DATETIME       NOT NULL,
    [CreatedOn]              DATETIME       NOT NULL,
    [LastModified]           ROWVERSION     NULL,
    CONSTRAINT [PK__YeuCauTiepNhanTheBHYT] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_YeuCauTiepNhanTheBHYT_BenhNhan] FOREIGN KEY ([BenhNhanId]) REFERENCES [dbo].[BenhNhan] ([Id]),
    CONSTRAINT [FK_YeuCauTiepNhanTheBHYT_GiayMienCungChiTra] FOREIGN KEY ([GiayMienCungChiTraId]) REFERENCES [dbo].[GiayMienCungChiTra] ([Id]),
    CONSTRAINT [FK_YeuCauTiepNhanTheBHYT_YeuCauTiepNhan] FOREIGN KEY ([YeuCauTiepNhanId]) REFERENCES [dbo].[YeuCauTiepNhan] ([Id])
);
GO
ALTER TABLE [dbo].[YeuCauTiepNhan]
	ADD 
		[ChoTamUngThem]                                      BIT  NULL;
GO
ALTER TABLE [dbo].[CongTyBaoHiemTuNhanCongNo]
	ADD 
		[DaHuy]                                      BIT  NULL;
GO
ALTER TABLE [dbo].[MienGiamChiPhi]
	ADD 
		[DaHuy]                                      BIT  NULL;
GO
Update dbo.CauHinh
Set [Value] = '1.5.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'