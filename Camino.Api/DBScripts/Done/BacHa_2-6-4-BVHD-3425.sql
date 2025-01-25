
ALTER TABLE dbo.YeuCauNhapKhoDuocPhamChiTiet
  ADD [ThueVatLamTron] [DECIMAL] (15, 2) NULL;
  
ALTER TABLE dbo.YeuCauNhapKhoVatTuChiTiet
  ADD [ThueVatLamTron] [decimal] (15, 2) NULL;


Update dbo.CauHinh
Set [Value] = '2.6.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'
