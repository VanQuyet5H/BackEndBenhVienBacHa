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
           (N'CauHinhNoiTru.KhoaNhi'
           ,2
           ,N'Khoa nhi'
           ,0
           ,1
           ,1
           ,GETDATE()
           ,GETDATE())
GO

DECLARE @khoaId BIGINT = NULL
SELECT TOP 1 @khoaId = id FROM KhoaPhong WHERE Ma = N'KNH'
Update CauHinh SET Value = @khoaId WHERE Name = N'CauHinhNoiTru.KhoaNhi'

Update dbo.CauHinh
Set [Value] = '2.3.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'
