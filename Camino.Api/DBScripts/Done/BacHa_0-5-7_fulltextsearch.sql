IF EXISTS (SELECT * FROM sys.fulltext_catalogs WHERE Name = 'InputStringStored')
    DROP FULLTEXT CATALOG InputStringStored
GO

DROP FULLTEXT INDEX ON [dbo].InputStringStored
GO

CREATE FULLTEXT CATALOG Catalog_InputStringStored
GO

CREATE UNIQUE INDEX cal_inputStringStoredIndex ON InputStringStored([ID]); 

  CREATE FULLTEXT INDEX ON dbo.InputStringStored
(  
	[Value]  
        Language 1066
)  
KEY INDEX cal_inputStringStoredIndex ON Catalog_InputStringStored --Unique index  
GO 


Update dbo.CauHinh
Set [Value] = '0.5.7' where [Name] = 'CauHinhHeThong.DatabaseVesion'