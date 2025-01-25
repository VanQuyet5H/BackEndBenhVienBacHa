CREATE FULLTEXT CATALOG Catalog_CongTyKhamSucKhoe;

  CREATE UNIQUE INDEX cong_ty_kham_suc_khoe ON CongTyKhamSucKhoe([ID]); 

  CREATE FULLTEXT INDEX ON CongTyKhamSucKhoe
(  
	[Ma]  
        Language 1066,
	[Ten]  
        Language 1066
)  
KEY INDEX cong_ty_kham_suc_khoe ON Catalog_CongTyKhamSucKhoe --Unique index  
GO

Update CauHinh
Set [Value] = '1.6.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'