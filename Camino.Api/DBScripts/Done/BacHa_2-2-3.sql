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
	       (N'CauHinhNoiTru.NhomVatTuYTeBenhVien',2,N'Nhóm vật tư y tế bệnh viện',86,1,1,GETDATE(),GETDATE())
          
GO

Update dbo.CauHinh
Set [Value] = '2.2.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'