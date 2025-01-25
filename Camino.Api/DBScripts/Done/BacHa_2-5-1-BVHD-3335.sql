ALTER TABLE dbo.YeuCauXuatKhoDuocPham
  ADD NhaThauId BIGINT NULL;

ALTER TABLE dbo.YeuCauXuatKhoDuocPham
  ADD SoChungTu NVARCHAR(50) NULL;

ALTER TABLE dbo.YeuCauXuatKhoVatTu
  ADD NhaThauId BIGINT NULL;

ALTER TABLE dbo.YeuCauXuatKhoVatTu
  ADD SoChungTu NVARCHAR(50) NULL;


ALTER TABLE dbo.XuatKhoDuocPham
  ADD NhaThauId BIGINT NULL;

ALTER TABLE dbo.XuatKhoDuocPham
  ADD SoChungTu NVARCHAR(50) NULL;

ALTER TABLE dbo.XuatKhoVatTu
  ADD NhaThauId BIGINT NULL;

ALTER TABLE dbo.XuatKhoVatTu
  ADD SoChungTu NVARCHAR(50) NULL;


Update dbo.CauHinh
Set [Value] = '2.5.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'
