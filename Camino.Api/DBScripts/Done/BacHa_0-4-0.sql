ALTER TABLE [YeuCauTiepNhan]
ADD DaGuiCongBHYT bit NULL;
GO

CREATE TABLE [dbo].[DuLieuGuiCongBHYT](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DuLieuTongHop] [nvarchar](max) NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[YeuCauTiepNhanDuLieuGuiCongBHYT](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[YeuCauTiepNhanId] [bigint] NOT NULL,
	[DuLieuGuiCongBHYTId] [bigint] NULL,
	[DuLieu] [nvarchar](max) NOT NULL,
	[Version] [int] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[YeuCauTiepNhanDuLieuGuiCongBHYT]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauTiepNhanDuLieuGuiCongBHYT_DuLieuGuiCongBHYT] FOREIGN KEY([DuLieuGuiCongBHYTId])
REFERENCES [dbo].[DuLieuGuiCongBHYT] ([Id])
GO

ALTER TABLE [dbo].[YeuCauTiepNhanDuLieuGuiCongBHYT] CHECK CONSTRAINT [FK_YeuCauTiepNhanDuLieuGuiCongBHYT_DuLieuGuiCongBHYT]
GO

ALTER TABLE [dbo].[YeuCauTiepNhanDuLieuGuiCongBHYT]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauTiepNhanDuLieuGuiCongBHYT_YeuCauTiepNhan] FOREIGN KEY([YeuCauTiepNhanId])
REFERENCES [dbo].[YeuCauTiepNhan] ([Id])
GO

ALTER TABLE [dbo].[YeuCauTiepNhanDuLieuGuiCongBHYT] CHECK CONSTRAINT [FK_YeuCauTiepNhanDuLieuGuiCongBHYT_YeuCauTiepNhan]
GO

Update CauHinh
Set [Value] = '0.4.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'
