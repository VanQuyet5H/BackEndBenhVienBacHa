
CREATE TABLE [dbo].[CheDoAn](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](500) NOT NULL,
	[KyHieu] [nvarchar](50) NOT NULL,
	[MoTa] [nvarchar](2000) NULL,
	[IsDisabled] [bit] NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__CheDoAn__3214EC07F83AEC89] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
alter table NoiTruPhieuDieuTri add CheDoAnId bigint null;
GO
Update CauHinh
Set [Value] = '1.7.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'