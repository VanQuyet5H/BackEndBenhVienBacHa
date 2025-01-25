CREATE TABLE [dbo].[AuditGachNo](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](100) NULL,
	[TableName] [nvarchar](100) NOT NULL,
	[Action] [int] NULL,
	[KeyValues] [bigint] NOT NULL,
	[OldValues] [nvarchar](max) NULL,
	[NewValues] [nvarchar](max) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_AuditHopDongThueKho] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT INTO [dbo].[AuditTable]
           ([TableName]
           ,[IsRoot]
           ,[CreatedById]
           ,[LastUserId]
           ,[LastTime]
           ,[CreatedOn])
     VALUES
           (N'GachNo'
           ,1
           ,1
           ,1
           ,GETDATE()
           ,GETDATE())
GO


GO
Update CauHinh
Set [Value] = '1.1.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'