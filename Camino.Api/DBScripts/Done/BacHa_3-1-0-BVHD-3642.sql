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
           (N'CauHinhXetNghiem.MaxBarcodeTuDong'
           ,2
           ,N'Số lớn nhất barcode tự động tạo'
           ,5000
           ,1
           ,1
           ,GETDATE()
           ,GETDATE())

Go
Update dbo.CauHinh
Set [Value] = '3.1.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'