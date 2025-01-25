CREATE TABLE [dbo].[TaiKhoanBenhNhanChiThongTin](
	[Id] [bigint] NOT NULL,
	[LoaiNoiDung] [int] NOT NULL,
	[NoiDung] [nvarchar](max) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__TaiKhoanBenhNhanChiThongTin] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[TaiKhoanBenhNhanChiThongTin]  WITH CHECK ADD  CONSTRAINT [FK_TaiKhoanBenhNhanChiThongTin_TaiKhoanBenhNhanChi] FOREIGN KEY([Id])
REFERENCES [dbo].[TaiKhoanBenhNhanChi] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[TaiKhoanBenhNhanChiThongTin] CHECK CONSTRAINT [FK_TaiKhoanBenhNhanChiThongTin_TaiKhoanBenhNhanChi]
GO

Update dbo.CauHinh
Set [Value] = '3.0.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'