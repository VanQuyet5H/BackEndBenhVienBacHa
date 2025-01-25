CREATE TABLE [dbo].[FileKetQuaCanLamSang](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ma] [nvarchar](50) NULL,
	[Ten] [nvarchar](256) NOT NULL,
	[TenGuid] [nvarchar](256) NOT NULL,
	[KichThuoc] [bigint] NOT NULL,
	[DuongDan] [nvarchar](500) NOT NULL,
	[LoaiTapTin] [int] NOT NULL,
	[MoTa] [nvarchar](2000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [YeuCauDichVuKyThuat]
ADD 	
	[FileKetQuaCanLamSangId] bigint NULL;
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDichVuKyThuat_FileKetQuaCanLamSang] FOREIGN KEY([FileKetQuaCanLamSangId])
REFERENCES [dbo].[FileKetQuaCanLamSang] ([Id])
GO

ALTER TABLE [dbo].[YeuCauDichVuKyThuat] CHECK CONSTRAINT [FK_YeuCauDichVuKyThuat_FileKetQuaCanLamSang]
GO

Update CauHinh
Set [Value] = '0.2.8' where [Name] = 'CauHinhHeThong.DatabaseVesion'


