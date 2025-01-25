
ALTER TABLE GoiKhamSucKhoeChung
ADD GoiDichVuPhatSinh BIT NULL
GO

ALTER TABLE GoiKhamSucKhoe
ADD GoiDichVuPhatSinh BIT NULL
GO

ALTER TABLE YeuCauTiepNhan
ADD YeuCauTiepNhanKhamSucKhoeId BIGINT NULL
Go

ALTER TABLE YeuCauKhamBenh
ADD GoiKhamSucKhoeDichVuPhatSinhId BIGINT NULL
GO

ALTER TABLE YeuCauDichVuKyThuat
ADD GoiKhamSucKhoeDichVuPhatSinhId BIGINT NULL
GO

ALTER TABLE YeuCauKhamBenh
ADD GiaBenhVienTaiThoiDiemChiDinh DECIMAL(15,2) NULL
GO

ALTER TABLE GoiKhamSucKhoeDichVuKhamBenh
  ALTER COLUMN ChuyenKhoaKhamSucKhoe INT NULL;
Go

INSERT INTO [dbo].[CauHinh] ([Name] ,[DataType] ,[Description] ,[Value] ,[CreatedById] ,[LastUserId] ,[LastTime] ,[CreatedOn])
     VALUES (N'CauHinhKhamSucKhoe.LyDoTiepNhanLaKhamBenh',2 ,N'Lý do tiếp nhận là khám bệnh Id' ,0 ,1 ,1 ,GETDATE() ,GETDATE())
GO
DECLARE @lyDoTiepNhanKhamBenhId BIGINT = NULL
select Top 1 @lyDoTiepNhanKhamBenhId = Id from LyDoTiepNhan where LyDoTiepNhanChaId is not null and Ten = N'Khám bệnh'
Update CauHinh SET Value = @lyDoTiepNhanKhamBenhId WHERE Name = N'CauHinhKhamSucKhoe.LyDoTiepNhanLaKhamBenh'
Go

Update dbo.CauHinh
Set [Value] = '3.2.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'