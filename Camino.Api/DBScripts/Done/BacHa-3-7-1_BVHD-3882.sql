DECLARE @nhomGiaId BIGINT = NULL
SELECT TOP 1 @nhomGiaId = Id FROM NhomGiaDichVuGiuongBenhVien
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
           (N'CauHinhDichVuGiuong.NhomGiaThuong'
           ,2
           ,N'Nhóm giá thường'
           ,CAST(ISNULL(@nhomGiaId, 0) as NVARCHAR)
           ,1
           ,1
           ,GETDATE()
           ,GETDATE())

Go
Update dbo.CauHinh
Set [Value] = '3.4.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'