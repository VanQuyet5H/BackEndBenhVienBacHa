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
           (N'CauHinhXetNghiem.TaoBarcodeTuDong'
           ,1
           ,N'Hệ thống tự động tạo Barcode'
           ,N'False'
           ,1
           ,1
           ,GETDATE()
           ,GETDATE())
GO
Update dbo.CauHinh
Set [Value] = '1.2.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'