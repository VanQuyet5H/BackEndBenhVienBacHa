

CREATE TABLE [dbo].[NoiDungMauLoiDanBacSi](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ma] [nvarchar](20) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
	[HuongXuLyLoiDanBacSi] [nvarchar](max) NOT NULL,
	[LoaiBenhAn] [int] NULL,
 CONSTRAINT [PK_NoiDungMauLoiDanBacSi] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

Update dbo.CauHinh
Set [Value] = '2.8.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'
