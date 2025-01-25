INSERT INTO [dbo].[CauHinh] ([Name] ,[DataType] ,[Description] ,[Value] ,[CreatedById] ,[LastUserId] ,[LastTime] ,[CreatedOn])
     VALUES (N'CauHinhKhamBenh.DichVuKhamNoi',2 ,N'Dịch vụ khám nội Id' ,0 ,1 ,1 ,GETDATE() ,GETDATE())
GO
INSERT INTO [dbo].[CauHinh] ([Name] ,[DataType] ,[Description] ,[Value] ,[CreatedById] ,[LastUserId] ,[LastTime] ,[CreatedOn])
     VALUES (N'CauHinhKhamBenh.DichVuKhamNgoai',2 ,N'Dịch vụ khám ngoại Id' ,0 ,1 ,1 ,GETDATE() ,GETDATE())
GO
INSERT INTO [dbo].[CauHinh] ([Name] ,[DataType] ,[Description] ,[Value] ,[CreatedById] ,[LastUserId] ,[LastTime] ,[CreatedOn])
     VALUES (N'CauHinhKhamBenh.DichVuKhamPhuSan',2 ,N'Dịch vụ khám phụ sản Id' ,0 ,1 ,1 ,GETDATE() ,GETDATE())
GO
INSERT INTO [dbo].[CauHinh] ([Name] ,[DataType] ,[Description] ,[Value] ,[CreatedById] ,[LastUserId] ,[LastTime] ,[CreatedOn])
     VALUES (N'CauHinhKhamBenh.DichVuKhamNhi',2 ,N'Dịch vụ khám nhi Id' ,0 ,1 ,1 ,GETDATE() ,GETDATE())
GO


DECLARE @khamNoiId BIGINT = NULL, @khamNgoaiId BIGINT = NULL, @khamPhuSanId BIGINT = NULL, @khamNhiId BIGINT = NULL
SELECT TOP 1 @khamNoiId = id FROM DichVuKhamBenhBenhVien WHERE Ma = N'02.1897' AND Ten = N'Khám Nội'
SELECT TOP 1 @khamNgoaiId = id FROM DichVuKhamBenhBenhVien WHERE Ma = N'10.1897' AND Ten = N'Khám Ngoại'
SELECT TOP 1 @khamPhuSanId = id FROM DichVuKhamBenhBenhVien WHERE Ma = N'13.1897' AND Ten = N'Khám Phụ Sản'
SELECT TOP 1 @khamNhiId = id FROM DichVuKhamBenhBenhVien WHERE Ma = N'03.1897' AND Ten = N'Khám Nhi'
Update CauHinh SET Value = @khamNoiId WHERE Name = N'CauHinhKhamBenh.DichVuKhamNoi'
Update CauHinh SET Value = @khamNgoaiId WHERE Name = N'CauHinhKhamBenh.DichVuKhamNgoai'
Update CauHinh SET Value = @khamPhuSanId WHERE Name = N'CauHinhKhamBenh.DichVuKhamPhuSan'
Update CauHinh SET Value = @khamNhiId WHERE Name = N'CauHinhKhamBenh.DichVuKhamNhi'
Go

Update dbo.CauHinh
Set [Value] = '2.9.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'