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
           (N'CauHinhBaoCao.NhomThuThuat'
           ,2
           ,N'Nhóm thủ thuật id'
           ,0
           ,1
           ,1
           ,GETDATE()
           ,GETDATE())
GO

DECLARE @id BIGINT = NULL
SELECT TOP 1 @id = id FROM NhomDichVuBenhVien WHERE Ma = N'TT'
Update CauHinh SET Value = @id WHERE Name = N'CauHinhBaoCao.NhomThuThuat'

Update dbo.CauHinh
Set [Value] = '2.8.7' where [Name] = 'CauHinhHeThong.DatabaseVesion'