DROP TABLE dbo.InputStringStored
GO
CREATE TABLE [dbo].[InputStringStored]
(
[Id] [bigint] NOT NULL IDENTITY(1, 1),
[Set] [int] NOT NULL,
[Value] [nvarchar] (4000) COLLATE SQL_Latin1_General_CP1_CI_AI NOT NULL,
[CreatedById] [bigint] NOT NULL,
[LastUserId] [bigint] NOT NULL,
[LastTime] [datetime] NOT NULL,
[CreatedOn] [datetime] NOT NULL,
[LastModified] [timestamp] NOT NULL
) ON [PRIMARY]
GO

CREATE FULLTEXT CATALOG Catalog_InputStringStoreds
GO

CREATE UNIQUE INDEX cal_inputStringStoredIndexs ON InputStringStored([ID]); 

  CREATE FULLTEXT INDEX ON dbo.InputStringStored
(  
	[Value]  
        Language 1066
)  
KEY INDEX cal_inputStringStoredIndexs ON Catalog_InputStringStoreds --Unique index  
GO 

Update dbo.CauHinh
Set [Value] = '0.5.8' where [Name] = 'CauHinhHeThong.DatabaseVesion'

