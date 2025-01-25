INSERT INTO [dbo].[CauHinh] ([Name] ,[DataType] ,[Description] ,[Value] ,[CreatedById] ,[LastUserId] ,[LastTime] ,[CreatedOn])
     VALUES (N'CauHinhKhamBenh.DichVuKhamCapCuu',2 ,N'Dịch vụ khám cấp cứu Id' ,0 ,1 ,1 ,GETDATE() ,GETDATE())
GO
INSERT INTO [dbo].[CauHinh] ([Name] ,[DataType] ,[Description] ,[Value] ,[CreatedById] ,[LastUserId] ,[LastTime] ,[CreatedOn])
     VALUES (N'CauHinhKhamBenh.DichVuKhamThai',2 ,N'Dịch vụ khám thai Id' ,0 ,1 ,1 ,GETDATE() ,GETDATE())
GO


DECLARE @khamCapCuuId BIGINT = NULL, @khamThaiId BIGINT = NULL
SELECT TOP 1 @khamCapCuuId = id FROM DichVuKhamBenhBenhVien WHERE Ten = N'Khám Cấp cứu'
SELECT TOP 1 @khamThaiId = id FROM DichVuKhamBenhBenhVien WHERE Ten = N'Khám thai' OR Ma = N'KB001'
Update CauHinh SET Value = @khamCapCuuId WHERE Name = N'CauHinhKhamBenh.DichVuKhamCapCuu'
Update CauHinh SET Value = @khamThaiId WHERE Name = N'CauHinhKhamBenh.DichVuKhamThai'
Go

Update dbo.CauHinh
Set [Value] = '2.9.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'