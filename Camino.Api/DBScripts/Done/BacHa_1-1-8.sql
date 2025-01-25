ALTER TABLE DuTruMuaDuocPhamChiTiet
   ADD GhiChu NVARCHAR(2000) NULL

ALTER TABLE dbo.DuTruMuaVatTuChiTiet
   ADD GhiChu NVARCHAR(2000) NULL

  Update dbo.CauHinh
Set [Value] = '1.1.8' where [Name] = 'CauHinhHeThong.DatabaseVesion'