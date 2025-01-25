DROP TABLE [YeuCauKhamBenhKhamBoPhanKhac]
GO
DROP TABLE [YeuCauKhamBenhChanDoanPhanBiet]
GO

CREATE TABLE [dbo].[YeuCauKhamBenhKhamBoPhanKhac](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[NoiDung] [nvarchar](4000) NOT NULL,
	[YeuCauKhamBenhId] [bigint] NOT NULL,

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

ALTER TABLE [dbo].[YeuCauKhamBenhKhamBoPhanKhac]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhKhamBoPhanKhac_YeuCauKhamBenh] FOREIGN KEY([YeuCauKhamBenhId])
REFERENCES [dbo].[YeuCauKhamBenh] ([Id])
GO

CREATE TABLE [dbo].[YeuCauKhamBenhChanDoanPhanBiet](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ICDId] [bigint] NOT NULL,
	[GhiChu] [nvarchar](4000) NULL,
	[YeuCauKhamBenhId] [bigint] NOT NULL,

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

ALTER TABLE [dbo].[YeuCauKhamBenhChanDoanPhanBiet]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhChanDoanPhanBiet_ICD] FOREIGN KEY([ICDId])
REFERENCES [dbo].[ICD] ([Id])
GO
ALTER TABLE [dbo].[YeuCauKhamBenhChanDoanPhanBiet]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenhChanDoanPhanBiet_YeuCauKhamBenh] FOREIGN KEY([YeuCauKhamBenhId])
REFERENCES [dbo].[YeuCauKhamBenh] ([Id])
GO

Update CauHinh
Set [Value] = '0.2.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'