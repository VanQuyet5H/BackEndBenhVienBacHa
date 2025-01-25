CREATE FULLTEXT CATALOG Catalog_DVKT;

  CREATE UNIQUE INDEX dvkt ON DichVuKyThuat([ID]); 

  CREATE FULLTEXT INDEX ON DichVuKyThuat
(  
    [MaChung]  
        Language 1066,
	[TenChung]  
        Language 1066
)  
KEY INDEX dvkt ON Catalog_DVKT --Unique index  
GO 

CREATE FULLTEXT CATALOG Catalog_PPVC;

  CREATE UNIQUE INDEX ppvc ON PhuongPhapVoCam([ID]); 

  CREATE FULLTEXT INDEX ON PhuongPhapVoCam
(  
    [Ma]  
        Language 1066,
	[Ten]  
        Language 1066
)  
KEY INDEX ppvc ON Catalog_PPVC --Unique index  
GO 

CREATE FULLTEXT CATALOG Catalog_NhaThau;

  CREATE UNIQUE INDEX nt ON NhaThau([ID]); 

  CREATE FULLTEXT INDEX ON NhaThau
(  
    [Ten]  
        Language 1066,
	[DiaChi]  
        Language 1066
)  
KEY INDEX nt ON Catalog_NhaThau --Unique index  
GO 

Update dbo.CauHinh
Set [Value] = '0.4.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'