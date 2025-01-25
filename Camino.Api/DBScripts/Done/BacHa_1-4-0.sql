ALTER TABLE dbo.YeuCauDuocPhamBenhVien
   DROP COLUMN SoLanDungTrongNgay;

   ALTER TABLE dbo.YeuCauDuocPhamBenhVien
   DROP COLUMN DungSang;

   ALTER TABLE dbo.YeuCauDuocPhamBenhVien
   DROP COLUMN DungTrua;

   ALTER TABLE dbo.YeuCauDuocPhamBenhVien
   DROP COLUMN DungChieu;

   ALTER TABLE dbo.YeuCauDuocPhamBenhVien
   DROP COLUMN DungToi;

   ALTER TABLE dbo.YeuCauDuocPhamBenhVien
   DROP COLUMN ThoiGianDungSang;

   ALTER TABLE dbo.YeuCauDuocPhamBenhVien
   DROP COLUMN ThoiGianDungTrua;

   ALTER TABLE dbo.YeuCauDuocPhamBenhVien
   DROP COLUMN ThoiGianDungChieu;

    ALTER TABLE dbo.YeuCauDuocPhamBenhVien
   DROP COLUMN ThoiGianDungToi;

   ALTER TABLE dbo.YeuCauDuocPhamBenhVien
   DROP COLUMN TocDoTruyen;

    ALTER TABLE dbo.YeuCauDuocPhamBenhVien
   DROP COLUMN DonViTocDoTruyen;

   ALTER TABLE dbo.YeuCauDuocPhamBenhVien
   DROP COLUMN ThoiGianBatDauTruyen;

   
   ALTER TABLE dbo.YeuCauDuocPhamBenhVien
   DROP COLUMN CachGioTruyenDich;

   Update dbo.CauHinh
Set [Value] = '1.4.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'