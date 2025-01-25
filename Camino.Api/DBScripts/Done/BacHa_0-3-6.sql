ALTER TABLE YeuCauKhamBenh
ADD ChanDoanCuaNoiGioiThieu NVARCHAR(4000) NULL
GO

CREATE TABLE [dbo].[YeuCauKhamBenhBoPhanTonThuong](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,

	YeuCauKhamBenhId BIGINT NOT NULL,
	MoTa NVARCHAR(4000) NOT NULL,
	HinhAnh text NOT NULL,

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

ALTER TABLE YeuCauKhamBenhBoPhanTonThuong 
WITH CHECK ADD  CONSTRAINT FK_YeuCauKhamBenh_YeuCauKhamBenhBoPhanTonThuong FOREIGN KEY (YeuCauKhamBenhId) REFERENCES YeuCauKhamBenh(Id)
GO

ALTER TABLE YeuCauKhamBenhBoPhanTonThuong CHECK CONSTRAINT FK_YeuCauKhamBenh_YeuCauKhamBenhBoPhanTonThuong
GO

Update CauHinh
Set [Value] = '0.3.6' where [Name] = 'CauHinhHeThong.DatabaseVesion'
