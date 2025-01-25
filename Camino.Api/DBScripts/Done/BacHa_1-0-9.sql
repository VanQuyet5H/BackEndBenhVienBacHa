CREATE TABLE [dbo].[GachNo](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[SoChungTu] [nvarchar](50) NOT NULL,
	[NgayChungTu] [datetime] NOT NULL,
	[LoaiChungTu] [int] NOT NULL,
	[KyKeToan] [nvarchar](50) NOT NULL,
	[TrangThai] [int] NOT NULL,
	[LoaiTienTe] [int] NOT NULL,
	[TyGia] [decimal](15,2) NOT NULL,
	[NgayThucThu] [datetime] NOT NULL,

	[LoaiDoiTuong] [int] NOT NULL,
	[CongTyBaoHiemTuNhanId] [bigint] NULL,
	[BenhNhanId] [bigint] NULL,

	[TaiKhoan] [nvarchar](50) NULL,
	[TaiKhoanLoaiTien] [nvarchar](150) NULL,
	[NguoiNop] [nvarchar](150) NULL,
	[ChungTuGoc] [nvarchar](250) NULL,
	[DienGiaiChung] [nvarchar](250) NULL,
	[SoTaiKhoanNganHang] [nvarchar](50) NULL,
	[NguyenTe] [nvarchar](150) NULL,
	[ThueNguyenTe] [nvarchar](150) NULL,
	[TongNguyenTe] [nvarchar](150) NULL,
	[HachToan] [nvarchar](150) NULL,
	[ThueHachToan] [nvarchar](150) NULL,
	[TongHachToan] [nvarchar](150) NULL,

	[LoaiThuChi] [nvarchar](50) NOT NULL,
	[VAT] [int] NULL,
	[TienHachToan] [decimal](15,2) NOT NULL,
	[KhoanMucPhi] [nvarchar](250) NULL,
	[SoHopDong] [nvarchar](150) NULL,
	[NgayHopDong] [datetime] NOT NULL,

	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_GachNo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[GachNo]  WITH CHECK ADD  CONSTRAINT [FK_GachNo_BenhNhan] FOREIGN KEY([BenhNhanId])
REFERENCES [dbo].[BenhNhan] ([Id])
GO
ALTER TABLE [dbo].[GachNo]  WITH CHECK ADD  CONSTRAINT [FK_GachNo_CongTyBaoHiemTuNhan] FOREIGN KEY([CongTyBaoHiemTuNhanId])
REFERENCES [dbo].[CongTyBaoHiemTuNhan] ([Id])

Update CauHinh
Set [Value] = '1.0.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'