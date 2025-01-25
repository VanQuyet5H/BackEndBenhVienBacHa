CREATE FULLTEXT CATALOG Catalog_ICD;

  CREATE UNIQUE INDEX icd ON ICD([ID]); 

  CREATE FULLTEXT INDEX ON ICD
(  
    [Ma]  
        Language 1066,
	[TenTiengViet]  
        Language 1066
)  
KEY INDEX icd ON Catalog_ICD --Unique index  
GO 

CREATE FULLTEXT CATALOG Catalog_BenhVien;

  CREATE UNIQUE INDEX benh_vien ON BenhVien([ID]); 

  CREATE FULLTEXT INDEX ON BenhVien
(  
	[Ten]  
        Language 1066
)  
KEY INDEX benh_vien ON Catalog_BenhVien --Unique index  
GO

Update CauHinh
Set [Value] = '0.4.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'

