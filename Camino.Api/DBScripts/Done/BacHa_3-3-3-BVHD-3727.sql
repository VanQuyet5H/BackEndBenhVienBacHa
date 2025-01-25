CREATE TABLE [dbo].[GayBenhAn](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ma] [nvarchar](50) NOT NULL,
	[ViTriGay] [int] NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[IsDisabled] [bit] NULL,
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

CREATE TABLE [dbo].[GayBenhAnPhieuHoSo](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[GayBenhAnId] [bigint] NOT NULL,
	[LoaiPhieuHoSoBenhAnDienTu] [int] NOT NULL,
	[Value] [bigint] NULL,
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

ALTER TABLE [dbo].[GayBenhAnPhieuHoSo]  WITH CHECK ADD  CONSTRAINT [FK_GayBenhAnPhieuHoSo_GayBenhAn] FOREIGN KEY([GayBenhAnId])
REFERENCES [dbo].[GayBenhAn] ([Id])
GO
ALTER TABLE [dbo].[GayBenhAnPhieuHoSo] CHECK CONSTRAINT [FK_GayBenhAnPhieuHoSo_GayBenhAn]
GO

Update dbo.CauHinh
Set [Value] = '3.3.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'