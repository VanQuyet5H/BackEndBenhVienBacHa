CREATE TABLE [dbo].[GiuongBenh] (
    [Id]              BIGINT          IDENTITY (1, 1) NOT NULL,
    [PhongBenhVienId] BIGINT          NOT NULL,
    [Ten]             NVARCHAR (250)  NOT NULL,
    [MoTa]            NVARCHAR (4000) NULL,
    [IsDisabled]      BIT             NULL,
    [CreatedById]     BIGINT          NOT NULL,
    [LastUserId]      BIGINT          NOT NULL,
    [LastTime]        DATETIME        NOT NULL,
    [CreatedOn]       DATETIME        NOT NULL,
    [LastModified]    ROWVERSION      NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_GiuongBenh_PhongBenhVien] FOREIGN KEY ([PhongBenhVienId]) REFERENCES [dbo].[PhongBenhVien] ([Id])
);
GO
CREATE TABLE [dbo].[HoatDongGiuongBenh] (
    [Id]                           BIGINT     IDENTITY (1, 1) NOT NULL,
    [GiuongBenhId]                 BIGINT     NOT NULL,
    [YeuCauTiepNhanId]             BIGINT     NOT NULL,
    [YeuCauDichVuGiuongBenhVienId] BIGINT     NOT NULL,
    [YeuCauKhamBenhId]             BIGINT     NULL,
    [YeuCauDichVuKyThuatId]        BIGINT     NULL,
    [ThoiDiemBatDau]               DATETIME   NOT NULL,
    [ThoiDiemKetThuc]              DATETIME   NULL,
    [NamGhep]                      BIT        NULL,
    [CreatedById]                  BIGINT     NOT NULL,
    [LastUserId]                   BIGINT     NOT NULL,
    [LastTime]                     DATETIME   NOT NULL,
    [CreatedOn]                    DATETIME   NOT NULL,
    [LastModified]                 ROWVERSION NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_HoatDongGiuongBenh_GiuongBenh] FOREIGN KEY ([GiuongBenhId]) REFERENCES [dbo].[GiuongBenh] ([Id]),
    CONSTRAINT [FK_HoatDongGiuongBenh_YeuCauDichVuGiuongBenhVien] FOREIGN KEY ([YeuCauDichVuGiuongBenhVienId]) REFERENCES [dbo].[YeuCauDichVuGiuongBenhVien] ([Id]),
    CONSTRAINT [FK_HoatDongGiuongBenh_YeuCauDichVuKyThuat] FOREIGN KEY ([YeuCauDichVuKyThuatId]) REFERENCES [dbo].[YeuCauDichVuKyThuat] ([Id]),
    CONSTRAINT [FK_HoatDongGiuongBenh_YeuCauKhamBenh] FOREIGN KEY ([YeuCauKhamBenhId]) REFERENCES [dbo].[YeuCauKhamBenh] ([Id]),
    CONSTRAINT [FK_HoatDongGiuongBenh_YeuCauTiepNhan] FOREIGN KEY ([YeuCauTiepNhanId]) REFERENCES [dbo].[YeuCauTiepNhan] ([Id])
);
GO

ALTER TABLE [YeuCauDichVuGiuongBenhVien]
ADD 	
	[GiuongBenhId] bigint NULL;
GO
ALTER TABLE [dbo].[YeuCauDichVuGiuongBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDichVuGiuongBenhVien_GiuongBenh] FOREIGN KEY([GiuongBenhId])
REFERENCES [dbo].[GiuongBenh] ([Id])
GO

ALTER TABLE [dbo].[YeuCauDichVuGiuongBenhVien] CHECK CONSTRAINT [FK_YeuCauDichVuGiuongBenhVien_GiuongBenh]
GO

Update CauHinh
Set [Value] = '0.3.7' where [Name] = 'CauHinhHeThong.DatabaseVesion'