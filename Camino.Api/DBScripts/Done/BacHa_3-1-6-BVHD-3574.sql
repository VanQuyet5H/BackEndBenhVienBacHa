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
           (N'CauHinhKhamBenh.KhoaNoi'
           ,2
           ,N'Khoa nội Id'
           ,0
           ,1
           ,1
           ,GETDATE()
           ,GETDATE())
GO
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
           (N'CauHinhKhamBenh.KhoaNgoai'
           ,2
           ,N'Khoa ngoại Id'
           ,0
           ,1
           ,1
           ,GETDATE()
           ,GETDATE())
GO


DECLARE @khoaNoiId BIGINT = NULL, @khoaNgoaiId BIGINT = NULL
SELECT TOP 1 @khoaNoiId = id FROM KhoaPhong WHERE Ma = N'KN'
SELECT TOP 1 @khoaNgoaiId = id FROM KhoaPhong WHERE Ma = N'KNG'
Update CauHinh SET Value = @khoaNoiId WHERE Name = N'CauHinhKhamBenh.KhoaNoi'
Update CauHinh SET Value = @khoaNgoaiId WHERE Name = N'CauHinhKhamBenh.KhoaNgoai'
Go

ALTER TABLE YeuCauKhamBenh
ADD NoiDungKhamBenh NVARCHAR(4000) NULL
GO

CREATE TABLE [dbo].[NoiDungMauKhamBenh](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ma] [nvarchar](20) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[NoiDung] [nvarchar](4000) NOT NULL,
	[BacSiId] [bigint] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL
 CONSTRAINT [PK_NoiDungMauKhamBenh] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[NoiDungMauKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_NoiDungMauKhamBenh_NhanVien] FOREIGN KEY([BacSiId])
REFERENCES [dbo].[NhanVien] ([Id])
Go

Update dbo.CauHinh
Set [Value] = '2.9.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'