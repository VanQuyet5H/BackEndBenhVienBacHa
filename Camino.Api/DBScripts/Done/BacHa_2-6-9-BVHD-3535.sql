ALTER TABLE [dbo].[DichVuKyThuatBenhVien]
	ADD [SoLanThucHienXetNghiem] [int] NULL
GO
ALTER TABLE [dbo].PhienXetNghiemChiTiet
	ADD [ThoiDiemCoKetQua] datetime NULL
GO
update PhienXetNghiemChiTiet set [ThoiDiemCoKetQua] = [ThoiDiemKetLuan]
GO
update A set A.[ThoiDiemCoKetQua] = MBG.ThoiDiemNhanKetQua	
FROM PhienXetNghiemChiTiet A
OUTER APPLY 
    (SELECT TOP 1 um.ThoiDiemNhanKetQua
     FROM KetQuaXetNghiemChiTiet um (NOLOCK)
         INNER JOIN PhienXetNghiemChiTiet m (NOLOCK)  ON m.Id= um.PhienXetNghiemChiTietId 
     WHERE um.PhienXetNghiemChiTietId=A.id and um.ThoiDiemNhanKetQua is not null
     ORDER BY um.ThoiDiemNhanKetQua
    ) AS MBG
where A.ThoiDiemCoKetQua is null
GO
CREATE TABLE [dbo].[LichSuKetQuaXetNghiemChiTiet](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[KetQuaXetNghiemChiTietId] [bigint] NOT NULL,	
	[BarCodeID] [varchar](20) NOT NULL,
	[BarCodeNumber] [int] NOT NULL,
	[PhienXetNghiemChiTietId] [bigint] NOT NULL,
	[YeuCauDichVuKyThuatId] [bigint] NOT NULL,
	[DichVuKyThuatBenhVienId] [bigint] NOT NULL,
	[NhomDichVuBenhVienId] [bigint] NOT NULL,
	[LanThucHien] [int] NOT NULL,
	[DichVuXetNghiemId] [bigint] NOT NULL,
	[DichVuXetNghiemChaId] [bigint] NULL,
	[DichVuXetNghiemMa] [nvarchar](50) NOT NULL,
	[DichVuXetNghiemTen] [nvarchar](250) NOT NULL,
	[CapDichVu] [int] NOT NULL,
	[DonVi] [nvarchar](30) NULL,
	[SoThuTu] [int] NULL,
	[DichVuXetNghiemKetNoiChiSoId] [bigint] NULL,
	[MaChiSo] [nvarchar](25) NULL,
	[TiLe] [float] NULL,
	[MauMayXetNghiemId] [bigint] NULL,
	[MayXetNghiemId] [bigint] NULL,
	[GiaTriTuMay] [nvarchar](100) NULL,
	[GiaTriNhapTay] [nvarchar](500) NULL,
	[GiaTriDuyet] [nvarchar](100) NULL,
	[GiaTriCu] [nvarchar](100) NULL,
	[NhanVienNhapTayId] [bigint] NULL,
	[GiaTriMin] [nvarchar](20) NULL,
	[GiaTriMax] [nvarchar](20) NULL,
	[GiaTriNguyHiemMin] [nvarchar](20) NULL,
	[GiaTriNguyHiemMax] [nvarchar](20) NULL,
	[GiaTriKhacThuong] [bit] NULL,
	[GiaTriNguyHiem] [bit] NULL,
	[ToDamGiaTri] [bit] NULL,
	[DaDuyet] [bit] NULL,
	[ThoiDiemGuiYeuCau] [datetime] NULL,
	[ThoiDiemNhanKetQua] [datetime] NULL,
	[ThoiDiemDuyetKetQua] [datetime] NULL,
	[NhanVienDuyetId] [bigint] NULL,
	[Rack] [varchar](50) NULL,
	[Comment] [nvarchar](100) NULL,
	[StripType] [nvarchar](50) NULL,
	[LotId] [varchar](30) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_LichSuKetQuaXetNghiemChiTiet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TRIGGER [dbo].[trg_KetQuaXetNghiemChiTiet_update] on [dbo].[KetQuaXetNghiemChiTiet] after update AS
BEGIN
   INSERT INTO [LichSuKetQuaXetNghiemChiTiet](
	[KetQuaXetNghiemChiTietId],	
	[BarCodeID],
	[BarCodeNumber],
	[PhienXetNghiemChiTietId],
	[YeuCauDichVuKyThuatId],
	[DichVuKyThuatBenhVienId],
	[NhomDichVuBenhVienId],
	[LanThucHien],
	[DichVuXetNghiemId],
	[DichVuXetNghiemChaId],
	[DichVuXetNghiemMa],
	[DichVuXetNghiemTen],
	[CapDichVu],
	[DonVi],
	[SoThuTu],
	[DichVuXetNghiemKetNoiChiSoId],
	[MaChiSo],
	[TiLe],
	[MauMayXetNghiemId],
	[MayXetNghiemId],
	[GiaTriTuMay],
	[GiaTriNhapTay],
	[GiaTriDuyet],
	[GiaTriCu],
	[NhanVienNhapTayId],
	[GiaTriMin],
	[GiaTriMax],
	[GiaTriNguyHiemMin],
	[GiaTriNguyHiemMax],
	[GiaTriKhacThuong],
	[GiaTriNguyHiem],
	[ToDamGiaTri],
	[DaDuyet],
	[ThoiDiemGuiYeuCau],
	[ThoiDiemNhanKetQua],
	[ThoiDiemDuyetKetQua],
	[NhanVienDuyetId],
	[Rack],
	[Comment],
	[StripType],
	[LotId],
	[CreatedById],
	[LastUserId],
	[LastTime],
	[CreatedOn])
	SELECT
		D.[Id]
      ,D.[BarCodeID]
      ,D.[BarCodeNumber]
      ,D.[PhienXetNghiemChiTietId]
      ,D.[YeuCauDichVuKyThuatId]
      ,D.[DichVuKyThuatBenhVienId]
      ,D.[NhomDichVuBenhVienId]
      ,D.[LanThucHien]
      ,D.[DichVuXetNghiemId]
      ,D.[DichVuXetNghiemChaId]
      ,D.[DichVuXetNghiemMa]
      ,D.[DichVuXetNghiemTen]
      ,D.[CapDichVu]
      ,D.[DonVi]
      ,D.[SoThuTu]
      ,D.[DichVuXetNghiemKetNoiChiSoId]
      ,D.[MaChiSo]
      ,D.[TiLe]
      ,D.[MauMayXetNghiemId]
      ,D.[MayXetNghiemId]
      ,D.[GiaTriTuMay]
      ,D.[GiaTriNhapTay]
      ,D.[GiaTriDuyet]
      ,D.[GiaTriCu]
      ,D.[NhanVienNhapTayId]
      ,D.[GiaTriMin]
      ,D.[GiaTriMax]
      ,D.[GiaTriNguyHiemMin]
      ,D.[GiaTriNguyHiemMax]
      ,D.[GiaTriKhacThuong]
      ,D.[GiaTriNguyHiem]
      ,D.[ToDamGiaTri]
      ,D.[DaDuyet]
      ,D.[ThoiDiemGuiYeuCau]
      ,D.[ThoiDiemNhanKetQua]
      ,D.[ThoiDiemDuyetKetQua]
      ,D.[NhanVienDuyetId]
      ,D.[Rack]
      ,D.[Comment]
      ,D.[StripType]
      ,D.[LotId]
      ,D.[CreatedById]
      ,D.[LastUserId]
      ,D.[LastTime]
      ,GETDATE()
	FROM [dbo].[KetQuaXetNghiemChiTiet] A
	INNER JOIN Inserted I ON A.Id = I.Id
    INNER JOIN Deleted D ON D.Id = I.Id
    WHERE	I.[GiaTriTuMay] != D.[GiaTriTuMay] or (I.[GiaTriTuMay] is null AND D.[GiaTriTuMay] is not null) or
			I.[GiaTriNhapTay] != D.[GiaTriNhapTay] or (I.[GiaTriNhapTay] is null AND D.[GiaTriNhapTay] is not null) or
			I.[GiaTriDuyet] != D.[GiaTriDuyet] or (I.[GiaTriDuyet] is null AND D.[GiaTriDuyet] is not null)
end
GO

ALTER TABLE [dbo].[KetQuaXetNghiemChiTiet] ENABLE TRIGGER [trg_KetQuaXetNghiemChiTiet_update]
GO
Update dbo.CauHinh
Set [Value] = '2.6.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'