
CREATE TABLE [dbo].[KetQuaVaKetLuanMau](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ma] [nvarchar](20) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[NoiDung] [nvarchar](2000) NOT NULL,
	[LoaiKetQuaVaKetLuanMau] [int] NOT NULL,

	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_KetQuaVaKetLuanMau] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


Update dbo.CauHinh
Set [Value] = '1.4.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'