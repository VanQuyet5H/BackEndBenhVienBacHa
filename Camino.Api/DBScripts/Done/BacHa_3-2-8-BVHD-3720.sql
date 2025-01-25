CREATE TABLE [dbo].[DonViMau]
(
[Id] [bigint] NOT NULL IDENTITY(1, 1),
[Ten] [nvarchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AI NOT NULL,
[Ma] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AI NOT NULL,
[IsDefault] [bit] NOT NULL,
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


CREATE TABLE [dbo].[LoaiGoiDichVu]
(
[Id] [bigint] NOT NULL IDENTITY(1, 1),
[Ten] [nvarchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AI NOT NULL,
[Ma] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AI NOT NULL,
[IsDefault] [bit] NOT NULL,
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


ALTER TABLE dbo.NoiGioiThieu
ADD DonViMauId BIGINT NULL
GO

ALTER TABLE dbo.ChuongTrinhGoiDichVu
ADD LoaiGoiDichVuId BIGINT NULL
GO

ALTER TABLE [dbo].[NoiGioiThieu]  WITH CHECK ADD  CONSTRAINT [FK_DonViMau_NoiGioiThieu] FOREIGN KEY([DonViMauId])
REFERENCES [dbo].[DonViMau] ([Id])
GO

ALTER TABLE [dbo].[ChuongTrinhGoiDichVu]  WITH CHECK ADD  CONSTRAINT [FK_ChuongTrinhGoiDichVu_LoaiGoiDichVu] FOREIGN KEY([LoaiGoiDichVuId])
REFERENCES [dbo].[LoaiGoiDichVu] ([Id])
GO



INSERT dbo.LoaiGoiDichVu
(
    Ten,
    Ma,
    IsDefault,
    CreatedById,
    LastUserId,
    LastTime,
    CreatedOn
)
VALUES
(   N'Gói dành cho trẻ sơ sinh',       -- Ten - nvarchar(250)
    N'01',       -- Ma - nvarchar(50)
    1,      -- IsDefault - bit
    1,         -- CreatedById - bigint
    1,         -- LastUserId - bigint
    GETDATE(), -- LastTime - datetime
    GETDATE()  -- CreatedOn - datetime
    )

INSERT dbo.LoaiGoiDichVu
(
    Ten,
    Ma,
    IsDefault,
    CreatedById,
    LastUserId,
    LastTime,
    CreatedOn
)
VALUES
(   N'Gói dành cho sản phụ',       -- Ten - nvarchar(250)
    N'02',       -- Ma - nvarchar(50)
    1,      -- IsDefault - bit
    1,         -- CreatedById - bigint
    1,         -- LastUserId - bigint
    GETDATE(), -- LastTime - datetime
    GETDATE()  -- CreatedOn - datetime
    )


	UPDATE dbo.ChuongTrinhGoiDichVu
	SET	LoaiGoiDichVuId = 1
	WHERE GoiSoSinh = 1

	UPDATE dbo.ChuongTrinhGoiDichVu
	SET	LoaiGoiDichVuId = 2
	WHERE LoaiGoiDichVuId is null

	-- bổ sung 27/12/2021
INSERT dbo.DonViMau
(
    Ten,
    Ma,
    IsDefault,
    CreatedById,
    LastUserId,
    LastTime,
    CreatedOn
)
VALUES
(   N'Việt Đức',       -- Ten - nvarchar(250)
    N'01',       -- Ma - nvarchar(50)
    1,      -- IsDefault - bit
    1,         -- CreatedById - bigint
    1,         -- LastUserId - bigint
    GETDATE(), -- LastTime - datetime
    GETDATE()  -- CreatedOn - datetime
    )

	INSERT dbo.DonViMau
(
    Ten,
    Ma,
    IsDefault,
    CreatedById,
    LastUserId,
    LastTime,
    CreatedOn
)
VALUES
(   N'Thẩm mỹ BS Tú',       -- Ten - nvarchar(250)
    N'02',       -- Ma - nvarchar(50)
    1,      -- IsDefault - bit
    1,         -- CreatedById - bigint
    1,         -- LastUserId - bigint
    GETDATE(), -- LastTime - datetime
    GETDATE()  -- CreatedOn - datetime
    )

	INSERT dbo.DonViMau
(
    Ten,
    Ma,
    IsDefault,
    CreatedById,
    LastUserId,
    LastTime,
    CreatedOn
)
VALUES
(   N'Thẩm mỹ',       -- Ten - nvarchar(250)
    N'03',       -- Ma - nvarchar(50)
    1,      -- IsDefault - bit
    1,         -- CreatedById - bigint
    1,         -- LastUserId - bigint
    GETDATE(), -- LastTime - datetime
    GETDATE()  -- CreatedOn - datetime
    )
Update dbo.CauHinh
Set [Value] = '3.2.8' where [Name] = 'CauHinhHeThong.DatabaseVesion'