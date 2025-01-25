INSERT INTO [dbo].[CauHinh]
           ([Name]
           ,[DataType]
           ,[Description]
           ,[Value]
           ,[CreatedById]
           ,[LastUserId]
           ,[LastTime]
           ,[CreatedOn])
     VALUES
           (N'CauHinhKhamSucKhoe.IcdKhamSucKhoe'
           ,2
           ,N'ICD mặc định cho người bệnh khám sức khỏe'
           ,0
           ,1
           ,1
           ,GETDATE()
           ,GETDATE())
GO

DECLARE @id BIGINT = NULL
SELECT TOP 1 @id = id FROM ICD WHERE Ma = N'Z00.0'
Update CauHinh SET Value = @id WHERE Name = N'CauHinhKhamSucKhoe.IcdKhamSucKhoe'

Update dbo.CauHinh
Set [Value] = '3.0.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'