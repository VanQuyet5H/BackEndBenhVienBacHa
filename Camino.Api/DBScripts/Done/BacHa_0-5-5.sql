CREATE TABLE [dbo].[InputStringStored]
(
[Id] [bigint] NOT NULL IDENTITY(1, 1),
[Key] [int] NOT NULL,
[Value] [nvarchar] (4000) COLLATE SQL_Latin1_General_CP1_CI_AI NOT NULL,
[CreatedById] [bigint] NOT NULL,
[LastUserId] [bigint] NOT NULL,
[LastTime] [datetime] NOT NULL,
[CreatedOn] [datetime] NOT NULL,
[LastModified] [timestamp] NOT NULL
) ON [PRIMARY]
GO

Update dbo.CauHinh
Set [Value] = '0.5.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'