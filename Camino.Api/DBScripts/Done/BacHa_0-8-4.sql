ALTER TABLE VatTuBenhVien
ADD MaVatTuBenhVien NVARCHAR(50) NULL

Update CauHinh
Set [Value] = '0.8.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'