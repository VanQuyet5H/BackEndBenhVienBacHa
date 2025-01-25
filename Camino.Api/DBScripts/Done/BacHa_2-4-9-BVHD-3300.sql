DECLARE @nhomGiaId BIGINT = NULL
SELECT TOP 1 @nhomGiaId = Id FROM NhomGiaDichVuKyThuatBenhVien
WHERE TEN LIKE N'%thường%'

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
           (N'CauHinhDichVuKyThuat.NhomGiaThuong'
           ,2
           ,N'Nhóm giá thường'
           ,CAST(ISNULL(@nhomGiaId, 0) as NVARCHAR)
           ,1
           ,1
           ,GETDATE()
           ,GETDATE())
GO

Update dbo.CauHinh
Set [Value] = '2.4.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'
