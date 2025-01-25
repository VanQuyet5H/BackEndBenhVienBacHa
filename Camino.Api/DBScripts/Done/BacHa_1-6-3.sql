ALTER TABLE dbo.GoiKhamSucKhoeDichVuDichVuKyThuat
  ADD SoLan INT NOT NULL;

  Update CauHinh
Set [Value] = '1.6.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'