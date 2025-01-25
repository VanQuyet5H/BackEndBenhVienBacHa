CREATE TABLE [dbo].[NhomDichVuBenhVien](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ma] [nvarchar](50) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[MoTa] [nvarchar](2000) NULL,
	[IsDefault] [bit] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
	[NhomDichVuBenhVienChaId] [bigint] NULL,
 CONSTRAINT [PK__NhomDich__3214EC075CCC5C83] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[NhomDichVuBenhVien] ON

Update CauHinh
Set [Value] = '0.3.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'