CREATE FULLTEXT CATALOG Catalog_ThuocHoacHoatChat;

  CREATE UNIQUE INDEX thuoc_hoac_hoat_chat ON ThuocHoacHoatChat([ID]); 

  CREATE FULLTEXT INDEX ON ThuocHoacHoatChat
(  
    [Ma]  
        Language 1066,
	[Ten]  
        Language 1066
)  
KEY INDEX thuoc_hoac_hoat_chat ON Catalog_ThuocHoacHoatChat
GO 