
CREATE TABLE [dbo].[TemplateDichVuKhamSucKhoe](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ChuyenKhoaKhamSucKhoe] [int] NOT NULL,
	[Ten] [nvarchar](100) NOT NULL,
	[TieuDe] [nvarchar](500) NULL,
	[ComponentDynamics] [nvarchar](max) NOT NULL,
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
Update CauHinh
Set [Value] = '1.7.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'
