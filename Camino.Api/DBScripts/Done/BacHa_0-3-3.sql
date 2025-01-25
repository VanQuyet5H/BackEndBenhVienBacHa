ALTER TABLE [dbo].[YeuCauDichVuKyThuat]
    ADD [DieuTriNgoaiTru]       BIT      NULL,
        [ThoiDiemBatDauDieuTri] DATETIME NULL;

GO
ALTER TABLE [dbo].[YeuCauKhamBenh]
    ADD [LaThamVan]         BIT NULL,
        [CoDieuTriNgoaiTru] BIT NULL;

GO
CREATE TABLE [dbo].[YeuCauDichVuKyThuatTuongTrinhPTTT] (
    [Id]                      BIGINT          NOT NULL,
    [ICDTruocPhauThuat]       BIGINT          NULL,
    [ICDSauPhauThuat]         BIGINT          NULL,
    [GhiChuICDTruocPhauThuat] NVARCHAR (4000) NULL,
    [GhiChuICDSauPhauThuat]   NVARCHAR (4000) NULL,
    [MaPhuongPhapPTTT]        NVARCHAR (50)   NULL,
    [TenPhuongPhapPTTT]       NVARCHAR (250)  NULL,
    [LoaiPhauThuatThuThuat]   INT             NULL,
    [PhuongPhapVoCamId]       BIGINT          NULL,
    [TinhHinhPTTT]            INT             NULL,
    [TaiBienPTTT]             INT             NULL,
    [TuVongTrongPTTT]         INT             NULL,
    [TrinhTuPhauThuat]        NVARCHAR (4000) NULL,
    [LuocDoPhauThuat]         TEXT            NULL,
    [CreatedById]             BIGINT          NOT NULL,
    [LastUserId]              BIGINT          NOT NULL,
    [LastTime]                DATETIME        NOT NULL,
    [CreatedOn]               DATETIME        NOT NULL,
    [LastModified]            ROWVERSION      NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuatTuongTrinhPTTT] WITH NOCHECK
    ADD CONSTRAINT [FK_YeuCauDichVuKyThuatTuongTrinhPTTT_YeuCauDichVuKyThuat] FOREIGN KEY ([Id]) REFERENCES [dbo].[YeuCauDichVuKyThuat] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuatTuongTrinhPTTT] WITH NOCHECK
    ADD CONSTRAINT [FK_YeuCauDichVuKyThuatTuongTrinhPTTT_ICD] FOREIGN KEY ([ICDTruocPhauThuat]) REFERENCES [dbo].[ICD] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuatTuongTrinhPTTT] WITH NOCHECK
    ADD CONSTRAINT [FK_YeuCauDichVuKyThuatTuongTrinhPTTT_ICD1] FOREIGN KEY ([ICDSauPhauThuat]) REFERENCES [dbo].[ICD] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuatTuongTrinhPTTT] WITH NOCHECK
    ADD CONSTRAINT [FK_YeuCauDichVuKyThuatTuongTrinhPTTT_PhuongPhapVoCam] FOREIGN KEY ([PhuongPhapVoCamId]) REFERENCES [dbo].[PhuongPhapVoCam] ([Id]);

GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuatTuongTrinhPTTT] WITH CHECK CHECK CONSTRAINT [FK_YeuCauDichVuKyThuatTuongTrinhPTTT_YeuCauDichVuKyThuat];

ALTER TABLE [dbo].[YeuCauDichVuKyThuatTuongTrinhPTTT] WITH CHECK CHECK CONSTRAINT [FK_YeuCauDichVuKyThuatTuongTrinhPTTT_ICD];

ALTER TABLE [dbo].[YeuCauDichVuKyThuatTuongTrinhPTTT] WITH CHECK CHECK CONSTRAINT [FK_YeuCauDichVuKyThuatTuongTrinhPTTT_ICD1];

ALTER TABLE [dbo].[YeuCauDichVuKyThuatTuongTrinhPTTT] WITH CHECK CHECK CONSTRAINT [FK_YeuCauDichVuKyThuatTuongTrinhPTTT_PhuongPhapVoCam];

GO
Update CauHinh
Set [Value] = '0.3.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'

