
INSERT INTO [dbo].[NgheNghiep]
           ([Ten]
           ,[TenVietTat]
           ,[IsDisabled]
           ,[CreatedById]
           ,[LastUserId]
           ,[LastTime]
           ,[CreatedOn])
     VALUES
           (N'Trẻ em < 6 tuổi',N'<6Tuoi' ,0,1,1,GETDATE(),GETDATE())
GO

Update dbo.CauHinh
Set [Value] = '1.4.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'