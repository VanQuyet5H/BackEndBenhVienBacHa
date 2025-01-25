UPDATE [dbo].[DichVuKyThuatBenhVien]
   SET [LoaiMauXetNghiem] = 1
 WHERE [NhomDichVuBenhVienId] in (112,114,115,116,117)

GO
UPDATE [dbo].[DichVuKyThuatBenhVien]
   SET [LoaiMauXetNghiem] = 2
 WHERE [NhomDichVuBenhVienId] = 113

UPDATE dbo.CauHinh
Set [Value] = '1.2.7' where [Name] = 'CauHinhHeThong.DatabaseVesion'