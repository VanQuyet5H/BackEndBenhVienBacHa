ALTER TABLE [dbo].[NoiTruBenhAn]
ADD [SoNgayDieuTriBenhAnSoSinh] [decimal](7, 2) NULL
GO
CREATE TABLE [dbo].[NoiTruThoiGianDieuTriBenhAnSoSinh](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[NoiTruBenhAnId] [bigint] NOT NULL,
	[NoiTruPhieuDieuTriId] [bigint] NOT NULL,
	[NgayDieuTri] [datetime] NOT NULL,
	[GioBatDau] [int] NOT NULL,
	[GioKetThuc] [int] NULL,
	[GhiChuDieuTri] [nvarchar](1000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__NoiTruThoiGianDieuTriBenhAnSoSinh] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[NoiTruThoiGianDieuTriBenhAnSoSinh]  WITH CHECK ADD  CONSTRAINT [FK_NoiTruThoiGianDieuTriBenhAnSoSinh_NoiTruBenhAn] FOREIGN KEY([NoiTruBenhAnId])
REFERENCES [dbo].[NoiTruBenhAn] ([Id])
GO

ALTER TABLE [dbo].[NoiTruThoiGianDieuTriBenhAnSoSinh] CHECK CONSTRAINT [FK_NoiTruThoiGianDieuTriBenhAnSoSinh_NoiTruBenhAn]
GO

ALTER TABLE [dbo].[NoiTruThoiGianDieuTriBenhAnSoSinh]  WITH CHECK ADD  CONSTRAINT [FK_NoiTruThoiGianDieuTriBenhAnSoSinh_NoiTruPhieuDieuTri] FOREIGN KEY([NoiTruPhieuDieuTriId])
REFERENCES [dbo].[NoiTruPhieuDieuTri] ([Id])
GO

ALTER TABLE [dbo].[NoiTruThoiGianDieuTriBenhAnSoSinh] CHECK CONSTRAINT [FK_NoiTruThoiGianDieuTriBenhAnSoSinh_NoiTruPhieuDieuTri]
GO
INSERT [dbo].[CauHinh] ([Name], [DataType], [Description], [Value], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) 
VALUES (N'CauHinhBaoCao.SoGiuongKeHoach', 2, N'Số giường kế hoạch', N'35', 1, 1, CAST(N'2021-08-08T00:00:00.000' AS DateTime), CAST(N'2021-08-08T00:00:00.000' AS DateTime))
GO
Update dbo.CauHinh
Set [Value] = '2.7.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'
