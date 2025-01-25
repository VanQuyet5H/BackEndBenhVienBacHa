ALTER TABLE KetQuaXetNghiemChiTiet
   ALTER Column DichVuXetNghiemChaId bigint NULL
GO
Update dbo.CauHinh
Set [Value] = '1.2.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'