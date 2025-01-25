CREATE FULLTEXT CATALOG Catalog_LoaiICD;

  CREATE UNIQUE INDEX loai_icd ON dbo.LoaiICD([ID]); 

  CREATE FULLTEXT INDEX ON dbo.LoaiICD
(  
	[Ma]  
        Language 1066,
	[TenTiengViet]  
        Language 1066
)  
KEY INDEX loai_icd ON Catalog_LoaiICD --Unique index  
GO

Update CauHinh
Set [Value] = '1.0.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'