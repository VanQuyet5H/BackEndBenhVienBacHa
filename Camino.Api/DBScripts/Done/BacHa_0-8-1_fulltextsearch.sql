CREATE FULLTEXT CATALOG Catalog_VatTu;

  CREATE UNIQUE INDEX vt ON VatTu([ID]); 

  CREATE FULLTEXT INDEX ON VatTu
(  
    [Ma]  
        Language 1066,
	[Ten]  
        Language 1066
)  
KEY INDEX vt ON Catalog_VatTu --Unique index  
GO 
Update CauHinh
Set [Value] = '0.8.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'







