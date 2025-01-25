ALTER TABLE [YeuCauDichVuKyThuatTuongTrinhPTTT]
DROP COLUMN LuocDoPhauThuat;
GO

CREATE TABLE [dbo].[YeuCauDichVuKyThuatLuocDoPhauThuat](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[YeuCauDichVuKyThuatTuongTrinhPTTTId] [bigint] NOT NULL,	
	[LuocDo] [text] NOT NULL,
	[Mota] nvarchar(1000) NULL,	
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[YeuCauDichVuKyThuatLuocDoPhauThuat]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauDichVuKyThuatLuocDoPhauThuat_YeuCauDichVuKyThuatTuongTrinhPTTT] FOREIGN KEY([YeuCauDichVuKyThuatTuongTrinhPTTTId])
REFERENCES [dbo].[YeuCauDichVuKyThuatTuongTrinhPTTT] ([Id])
GO

ALTER TABLE [dbo].[YeuCauDichVuKyThuatLuocDoPhauThuat] CHECK CONSTRAINT [FK_YeuCauDichVuKyThuatLuocDoPhauThuat_YeuCauDichVuKyThuatTuongTrinhPTTT]

GO
Update CauHinh
Set [Value] = '0.3.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'