CREATE FULLTEXT CATALOG Catalog_QT;

  CREATE UNIQUE INDEX qt ON QuaTang([ID]); 

  CREATE FULLTEXT INDEX ON QuaTang
(  
    [Ten]
        Language 1066,
	[DonViTinh]
        Language 1066
)  
KEY INDEX qt ON Catalog_QT --Unique index  
GO

Update dbo.CauHinh
Set [Value] = '1.1.7' where [Name] = 'CauHinhHeThong.DatabaseVesion'