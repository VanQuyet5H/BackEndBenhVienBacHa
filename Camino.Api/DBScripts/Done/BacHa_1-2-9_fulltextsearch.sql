CREATE FULLTEXT CATALOG Catalog_NhomDichVuBenhVien;

CREATE UNIQUE INDEX ndvbv ON NhomDichVuBenhVien([ID]); 

CREATE FULLTEXT INDEX ON NhomDichVuBenhVien
(  
    [Ma]
        Language 1066,
	[Ten]
        Language 1066
)  
KEY INDEX ndvbv ON Catalog_NhomDichVuBenhVien --Unique index  
GO

Update dbo.CauHinh
Set [Value] = '1.2.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'