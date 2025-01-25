select name, is_fulltext_enabled
from sys.databases

CREATE FULLTEXT CATALOG Catalog_TiepNhanBenhNhan;
CREATE FULLTEXT CATALOG Catalog_DichVuKyThuatBenhVien;
CREATE FULLTEXT CATALOG Catalog_DichVuGiuongBenhVien;
CREATE FULLTEXT CATALOG Catalog_DichVuKhamBenhBenhVien;
CREATE FULLTEXT CATALOG Catalog_GoiDichVu;
CREATE FULLTEXT CATALOG Catalog_DichVuBenhVienTongHop;
CREATE UNIQUE INDEX dich_vu_ky_thuat ON DichVuKyThuatBenhVien([ID]); 
CREATE UNIQUE INDEX dich_vu_kham_benh ON DichVuKhamBenhBenhVien([ID]); 
CREATE UNIQUE INDEX dich_vu_giuong ON DichVuGiuongBenhVien([ID]); 
CREATE UNIQUE INDEX goi_dich_vu ON GoiDichVu([ID]);
CREATE UNIQUE INDEX dich_vu_tong_hop ON DichVuBenhVienTongHop([ID]); 
CREATE FULLTEXT INDEX ON DichVuKyThuatBenhVien
(  
    [Ten]  
        Language 1066,
    [Ma]  
        Language 1066
)  
KEY INDEX dich_vu_ky_thuat ON Catalog_DichVuKyThuatBenhVien --Unique index  
GO  

CREATE FULLTEXT INDEX ON DichVuKhamBenhBenhVien
(  
    [Ten]  
        Language 1066,
    [Ma]  
        Language 1066
)  
KEY INDEX dich_vu_kham_benh ON Catalog_DichVuKhamBenhBenhVien --Unique index  
GO

CREATE FULLTEXT INDEX ON DichVuGiuongBenhVien
(  
    [Ten]  
        Language 1066,
    [Ma]  
        Language 1066
)  
KEY INDEX dich_vu_giuong ON Catalog_DichVuGiuongBenhVien --Unique index  
GO

CREATE FULLTEXT INDEX ON GoiDichVu
(  
    [Ten]  
        Language 1066
)  
KEY INDEX goi_dich_vu ON Catalog_GoiDichVu --Unique index  
GO

 CREATE FULLTEXT CATALOG Catalog_DuocPham;

  CREATE UNIQUE INDEX duoc_pham ON DuocPham([ID]); 

  CREATE FULLTEXT INDEX ON DuocPham
(  
    [Ten]  
        Language 1066,
	[HoatChat]  
        Language 1066
)  
KEY INDEX duoc_pham ON Catalog_DuocPham --Unique index  
GO  

  CREATE FULLTEXT INDEX ON DichVuBenhVienTongHop
(  
    [Ten]  
        Language 1066,
	[Ma]  
        Language 1066
)  
KEY INDEX dich_vu_tong_hop ON Catalog_DichVuBenhVienTongHop --Unique index  
GO  