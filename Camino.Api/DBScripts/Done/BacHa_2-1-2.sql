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
           (N'CauHinhXetNghiem.KhoaXetNghiem'
           ,2
           ,N'Khoa xét nghiệm'
           ,0
           ,1
           ,1
           ,GETDATE()
           ,GETDATE())
GO

DECLARE @khoaId BIGINT = NULL
SELECT TOP 1 @khoaId = id FROM KhoaPhong WHERE Ma = N'XN'
Update CauHinh SET Value = @khoaId WHERE Name = N'CauHinhXetNghiem.KhoaXetNghiem'

Update dbo.CauHinh
Set [Value] = '2.1.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'