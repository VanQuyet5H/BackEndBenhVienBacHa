CREATE FULLTEXT CATALOG Catalog_HopDongKhamSucKhoe;

  CREATE UNIQUE INDEX hop_dong_kham_suc_khoe ON HopDongKhamSucKhoe([ID]); 

  CREATE FULLTEXT INDEX ON HopDongKhamSucKhoe
(  
	[SoHopDong]  
        Language 1066
)  
KEY INDEX hop_dong_kham_suc_khoe ON Catalog_HopDongKhamSucKhoe --Unique index  
GO

Update CauHinh
Set [Value] = '1.6.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'