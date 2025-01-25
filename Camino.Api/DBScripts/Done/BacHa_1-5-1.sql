ALTER TABLE YeuCauTiepNhan
ADD HoTenBo NVARCHAR(100) NULL
GO

ALTER TABLE YeuCauTiepNhan
ADD TrinhDoVanHoaCuaBo NVARCHAR(150) NULL
GO

ALTER TABLE YeuCauTiepNhan
ADD NgheNghiepCuaBoId BIGINT NULL
GO

ALTER TABLE YeuCauTiepNhan
ADD HoTenMe NVARCHAR(100) NULL
GO

ALTER TABLE YeuCauTiepNhan
ADD TrinhDoVanHoaCuaMe NVARCHAR(150) NULL
GO

ALTER TABLE YeuCauTiepNhan
ADD NgheNghiepCuaMeId BIGINT NULL
GO

Update dbo.CauHinh
Set [Value] = '1.5.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'