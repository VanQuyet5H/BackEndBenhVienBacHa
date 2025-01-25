ALTER TABLE dbo.DonThuocThanhToanChiTiet
   ADD GhiChuMienGiamThem [nvarchar] (1000) NULL;
ALTER TABLE dbo.DonVTYTThanhToanChiTiet
   ADD GhiChuMienGiamThem [nvarchar] (1000) NULL;
ALTER TABLE dbo.YeuCauDichVuGiuongBenhVien
   ADD GhiChuMienGiamThem [nvarchar] (1000) NULL;
ALTER TABLE dbo.YeuCauDichVuKyThuat
   ADD GhiChuMienGiamThem [nvarchar] (1000) NULL;
ALTER TABLE dbo.YeuCauDuocPhamBenhVien
   ADD GhiChuMienGiamThem [nvarchar] (1000) NULL;
ALTER TABLE dbo.YeuCauGoiDichVu
   ADD GhiChuMienGiamThem [nvarchar] (1000) NULL;
ALTER TABLE dbo.YeuCauKhamBenh
   ADD GhiChuMienGiamThem [nvarchar] (1000) NULL;
ALTER TABLE dbo.YeuCauVatTuBenhVien
   ADD GhiChuMienGiamThem [nvarchar] (1000) NULL;

Update CauHinh
Set [Value] = '1.0.8' where [Name] = 'CauHinhHeThong.DatabaseVesion'