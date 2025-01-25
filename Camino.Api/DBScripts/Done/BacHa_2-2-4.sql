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
           (N'CauHinhNoiTru.KhoaPhuSan'
           ,2
           ,N'Khoa phụ sản'
           ,0
           ,1
           ,1
           ,GETDATE()
           ,GETDATE())
GO

DECLARE @khoaId BIGINT = NULL
SELECT TOP 1 @khoaId = id FROM KhoaPhong WHERE Ma = N'KPS'
Update CauHinh SET Value = @khoaId WHERE Name = N'CauHinhNoiTru.KhoaPhuSan'

Update dbo.CauHinh
Set [Value] = '2.2.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'
