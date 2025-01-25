CREATE TABLE [dbo].[TinhTrangRaVienHoSoKhac](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TenTinhTrangRaVien] [nvarchar](max) NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_TinhTrangRaVienHoSoKhac] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO 


Update dbo.CauHinh
Set [Value] = '3.4.8' where [Name] = 'CauHinhHeThong.DatabaseVesion'
