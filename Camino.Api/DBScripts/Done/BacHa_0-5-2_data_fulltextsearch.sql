  CREATE FULLTEXT CATALOG Catalog_User;

  CREATE UNIQUE INDEX nv_user ON [User]([ID]); 

  CREATE FULLTEXT INDEX ON dbo.[User]
(  
    [HoTen]
        Language 1066,
	[SoDienThoai]  
        Language 1066
)  
KEY INDEX nv_user ON Catalog_User --Unique index  
GO  

Update dbo.CauHinh
Set [Value] = '0.5.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'