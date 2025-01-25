SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DichVukyThuatBenhVienMauKetQua](
	[Id] [bigint] NOT NULL,
	[TenKetQuaMau] [nvarchar](500) NULL,
	[MaSo] [nvarchar](100) NULL,
	[KetQua] [nvarchar](max) NULL,
	[KetLuan] [nvarchar](max) NULL,
	[NhanVienThucHienId] [bigint] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_DichVukyThuatBenhVienMauKetQua] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[DichVukyThuatBenhVienMauKetQua]  WITH CHECK ADD  CONSTRAINT [FK_DichVukyThuatBenhVienMauKetQua_DichVuKyThuatBenhVien] FOREIGN KEY([Id])
REFERENCES [dbo].[DichVuKyThuatBenhVien] ([Id])
GO
ALTER TABLE [dbo].[DichVukyThuatBenhVienMauKetQua] CHECK CONSTRAINT [FK_DichVukyThuatBenhVienMauKetQua_DichVuKyThuatBenhVien]
GO

Update dbo.CauHinh
Set [Value] = '2.5.8' where [Name] = 'CauHinhHeThong.DatabaseVesion'
