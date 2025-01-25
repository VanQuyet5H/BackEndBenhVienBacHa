CREATE FULLTEXT CATALOG Catalog_MauVaChePham;

  CREATE UNIQUE INDEX mau_va_che_pham ON MauVaChePham([ID]); 

  CREATE FULLTEXT INDEX ON MauVaChePham
(  
	[Ma]  
        Language 1066,
	[Ten]  
        Language 1066
)  
KEY INDEX mau_va_che_pham ON Catalog_MauVaChePham --Unique index  
GO

Update CauHinh
Set [Value] = '1.9.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'