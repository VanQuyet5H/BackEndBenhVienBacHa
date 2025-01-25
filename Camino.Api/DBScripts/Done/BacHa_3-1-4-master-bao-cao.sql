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
           (N'CauHinhBaoCao.HinhThucDenGioiThieu'
           ,2
           ,N'Hình thức đến khám của người bệnh là giới thiệu'
           ,0
           ,1
           ,1
           ,GETDATE()
           ,GETDATE())
GO


DECLARE @hinhThucDenId BIGINT = NULL
SELECT TOP 1 @hinhThucDenId = id FROM HinhThucDen WHERE Ten = N'Giới thiệu'
Update CauHinh SET Value = @hinhThucDenId WHERE Name = N'CauHinhBaoCao.HinhThucDenGioiThieu'
Go

Update dbo.CauHinh
Set [Value] = '3.1.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'