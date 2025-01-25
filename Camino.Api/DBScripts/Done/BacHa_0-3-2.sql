ALTER TABLE [YeuCauKhamBenh]
ADD 
	[TomTatKetQuaCLS] nvarchar(4000) NULL,
	[GhiChuICDChinh] nvarchar(4000) NULL,
	[CachGiaiQuyet] nvarchar(4000) NULL;
GO

ALTER TABLE [NhomDichVuBenhVien]
ADD 
	[NhomDichVuBenhVienChaId] bigint NULL;
GO
ALTER TABLE [dbo].[NhomDichVuBenhVien]  WITH CHECK ADD  CONSTRAINT [FK__NhomDichVuBenhVien__NhomDichVuBenhVien] FOREIGN KEY([NhomDichVuBenhVienChaId])
REFERENCES [dbo].[NhomDichVuBenhVien] ([Id])
GO

ALTER TABLE [dbo].[NhomDichVuBenhVien] CHECK CONSTRAINT [FK__NhomDichVuBenhVien__NhomDichVuBenhVien]
GO

delete from [dbo].[NhomDichVuBenhVien]
SET IDENTITY_INSERT [dbo].[NhomDichVuBenhVien] ON 
Go
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('1','PTTT',N'Phẩu thuật thủ thuật',NULL,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('2','XN',N'Xét nghiệm',NULL,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('3','CDHA',N'Chẩn đoán hình ảnh',NULL,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('4','TDCN',N'Thăm dò chức năng',NULL,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('5','TYC',N'DỊCH VỤ THEO YÊU CẦU',NULL,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('6','KH',N'DỊCH VỤ KHÁC',NULL,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('7','07',N'CT-SCANNER',3,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('8','08',N'ĐIỆN CƠ',4,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('9','09',N'ĐIỆN NÃO',4,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('10','10',N'ĐIỆN TIM',4,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('11','11',N'ĐO HÔ HẤP',4,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('12','12',N'ĐO LOÃNG XƯƠNG',4,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('13','13',N'Hollter huyet ap',4,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('14','14',N'MRI',3,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('15','15',N'NỘI SOI',1,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('16','16',N'NỘI SOI TMH',1,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('17','17',N'PHẪU THUẬT CHẤN THƯƠNG - CHỈNH HÌNH',1,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('18','18',N'PHẪU THUẬT DA LIỄU',1,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('19','19',N'PHẪU THUẬT MẮT',1,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('20','20',N'PHẪU THUẬT NGOẠI KHOA',1,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('21','21',N'PHẪU THUẬT NHI KHOA',1,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('22','22',N'PHẪU THUẬT NỘI SOI',1,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('23','23',N'PHẪU THUẬT RĂNG HÀM MẶT',1,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('24','24',N'PHẪU THUẬT SẢN KHOA',1,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('25','25',N'PHẪU THUẬT SẢN PHỤ KHOA',1,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('26','26',N'PHẪU THUẬT TAI - MŨI - HỌNG',1,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('27','27',N'PHẪU THUẬT TẠO HÌNH',1,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('28','28',N'PHẪU THUẬT UNG BƯỚU',1,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('29','29',N'SIÊU ÂM',3,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('30','30',N'THỦ THUẬT CÁC LOẠI THỦ THUẬT KHÁC',1,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('31','31',N'THỦ THUẬT HỒI SỨC - CẤP CỨU',1,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('32','32',N'THỦ THUẬT KHOA CĐHA - TDCN',1,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('33','33',N'THỦ THUẬT MẮT',1,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('34','34',N'THỦ THUẬT NGOẠI KHOA',1,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('35','35',N'THỦ THUẬT NỘI KHOA',1,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('36','36',N'THỦ THUẬT NỘI SOI',1,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('37','37',N'THỦ THUẬT RĂNG HÀM MẶT',1,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('38','38',N'THỦ THUẬT SẢN KHOA',1,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('39','39',N'THỦ THUẬT SẢN PHỤ KHOA',1,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('40','40',N'THỦ THUẬT TAI - MŨI - HỌNG',1,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('41','41',N'THUÊ PHÒNG MỔ ĐẠI PHẪU',5,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('42','42',N'THUÊ PHÒNG MỔ GÂY MÊ',5,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('43','43',N'THUÊ PHÒNG MỔ TIỀN MÊ',5,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('44','44',N'TIÊM CHỦNG',5,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('45','45',N'XN HUYẾT HỌC',2,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('46','46',N'XN NƯỚC TIỂU',2,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('47','47',N'XN SH PHÂN TỬ',2,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('48','48',N'XN SINH HÓA',2,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('49','49',N'XN TẾ BÀO - GIẢI PHẪU BỆNH',2,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('50','50',N'XN VI SINH',2,NULL,1,1,1,'1/1/2020','1/1/2020')
INSERT INTO [dbo].[NhomDichVuBenhVien] ([Id],[Ma],[Ten],[NhomDichVuBenhVienChaId],[MoTa],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) VALUES('51','51',N'X-QUANG',3,NULL,1,1,1,'1/1/2020','1/1/2020')

GO
SET IDENTITY_INSERT [dbo].[NhomDichVuBenhVien] OFF 

ALTER TABLE [DichVuKyThuatBenhVien]
ADD 
	[NhomDichVuBenhVienId] bigint NULL;
GO
ALTER TABLE [dbo].[DichVuKyThuatBenhVien]  WITH CHECK ADD  CONSTRAINT [FK__DichVuKyThuatBenhVien__NhomDichVuBenhVien] FOREIGN KEY([NhomDichVuBenhVienId])
REFERENCES [dbo].[NhomDichVuBenhVien] ([Id])
GO

ALTER TABLE [dbo].[DichVuKyThuatBenhVien] CHECK CONSTRAINT [FK__DichVuKyThuatBenhVien__NhomDichVuBenhVien]
GO

UPDATE A SET  A.NhomDichVuBenhVienId = case when A.DichVuKyThuatId is not null AND B.NhomDichVuKyThuatId = 23 then 22 END FROM [DichVuKyThuatBenhVien] AS A left join [DichVuKyThuat] B on A.DichVuKyThuatId = B.Id WHERE  B.NhomDichVuKyThuatId = 23
UPDATE A SET  A.NhomDichVuBenhVienId = case when A.DichVuKyThuatId is not null AND B.NhomDichVuKyThuatId = 29 then 50 END FROM [DichVuKyThuatBenhVien] AS A left join [DichVuKyThuat] B on A.DichVuKyThuatId = B.Id WHERE  B.NhomDichVuKyThuatId = 29
UPDATE A SET  A.NhomDichVuBenhVienId = case when A.DichVuKyThuatId is not null AND B.NhomDichVuKyThuatId = 15 then 39 END FROM [DichVuKyThuatBenhVien] AS A left join [DichVuKyThuat] B on A.DichVuKyThuatId = B.Id WHERE  B.NhomDichVuKyThuatId = 15
UPDATE A SET  A.NhomDichVuBenhVienId = case when A.DichVuKyThuatId is not null AND B.NhomDichVuKyThuatId = 3 then 51 END FROM [DichVuKyThuatBenhVien] AS A left join [DichVuKyThuat] B on A.DichVuKyThuatId = B.Id WHERE  B.NhomDichVuKyThuatId = 3
UPDATE A SET  A.NhomDichVuBenhVienId = case when A.DichVuKyThuatId is not null AND B.NhomDichVuKyThuatId = 32 then 4 END FROM [DichVuKyThuatBenhVien] AS A left join [DichVuKyThuat] B on A.DichVuKyThuatId = B.Id WHERE  B.NhomDichVuKyThuatId = 32
UPDATE A SET  A.NhomDichVuBenhVienId = case when A.DichVuKyThuatId is not null AND B.NhomDichVuKyThuatId = 26 then 45 END FROM [DichVuKyThuatBenhVien] AS A left join [DichVuKyThuat] B on A.DichVuKyThuatId = B.Id WHERE  B.NhomDichVuKyThuatId = 26
UPDATE A SET  A.NhomDichVuBenhVienId = case when A.DichVuKyThuatId is not null AND B.NhomDichVuKyThuatId = 12 then 18 END FROM [DichVuKyThuatBenhVien] AS A left join [DichVuKyThuat] B on A.DichVuKyThuatId = B.Id WHERE  B.NhomDichVuKyThuatId = 12
UPDATE A SET  A.NhomDichVuBenhVienId = case when A.DichVuKyThuatId is not null AND B.NhomDichVuKyThuatId = 6 then 12 END FROM [DichVuKyThuatBenhVien] AS A left join [DichVuKyThuat] B on A.DichVuKyThuatId = B.Id WHERE  B.NhomDichVuKyThuatId = 6
UPDATE A SET  A.NhomDichVuBenhVienId = case when A.DichVuKyThuatId is not null AND B.NhomDichVuKyThuatId = 21 then 15 END FROM [DichVuKyThuatBenhVien] AS A left join [DichVuKyThuat] B on A.DichVuKyThuatId = B.Id WHERE  B.NhomDichVuKyThuatId = 21
UPDATE A SET  A.NhomDichVuBenhVienId = case when A.DichVuKyThuatId is not null AND B.NhomDichVuKyThuatId = 27 then 2 END FROM [DichVuKyThuatBenhVien] AS A left join [DichVuKyThuat] B on A.DichVuKyThuatId = B.Id WHERE  B.NhomDichVuKyThuatId = 27
UPDATE A SET  A.NhomDichVuBenhVienId = case when A.DichVuKyThuatId is not null AND B.NhomDichVuKyThuatId = 7 then 1 END FROM [DichVuKyThuatBenhVien] AS A left join [DichVuKyThuat] B on A.DichVuKyThuatId = B.Id WHERE  B.NhomDichVuKyThuatId = 7
UPDATE A SET  A.NhomDichVuBenhVienId = case when A.DichVuKyThuatId is not null AND B.NhomDichVuKyThuatId = 18 then 23 END FROM [DichVuKyThuatBenhVien] AS A left join [DichVuKyThuat] B on A.DichVuKyThuatId = B.Id WHERE  B.NhomDichVuKyThuatId = 18
UPDATE A SET  A.NhomDichVuBenhVienId = case when A.DichVuKyThuatId is not null AND B.NhomDichVuKyThuatId = 30 then 30 END FROM [DichVuKyThuatBenhVien] AS A left join [DichVuKyThuat] B on A.DichVuKyThuatId = B.Id WHERE  B.NhomDichVuKyThuatId = 30
UPDATE A SET  A.NhomDichVuBenhVienId = case when A.DichVuKyThuatId is not null AND B.NhomDichVuKyThuatId = 10 then 31 END FROM [DichVuKyThuatBenhVien] AS A left join [DichVuKyThuat] B on A.DichVuKyThuatId = B.Id WHERE  B.NhomDichVuKyThuatId = 10
UPDATE A SET  A.NhomDichVuBenhVienId = case when A.DichVuKyThuatId is not null AND B.NhomDichVuKyThuatId = 4 then 51 END FROM [DichVuKyThuatBenhVien] AS A left join [DichVuKyThuat] B on A.DichVuKyThuatId = B.Id WHERE  B.NhomDichVuKyThuatId = 4
UPDATE A SET  A.NhomDichVuBenhVienId = case when A.DichVuKyThuatId is not null AND B.NhomDichVuKyThuatId = 19 then 30 END FROM [DichVuKyThuatBenhVien] AS A left join [DichVuKyThuat] B on A.DichVuKyThuatId = B.Id WHERE  B.NhomDichVuKyThuatId = 19
UPDATE A SET  A.NhomDichVuBenhVienId = case when A.DichVuKyThuatId is not null AND B.NhomDichVuKyThuatId = 13 then 35 END FROM [DichVuKyThuatBenhVien] AS A left join [DichVuKyThuat] B on A.DichVuKyThuatId = B.Id WHERE  B.NhomDichVuKyThuatId = 13
UPDATE A SET  A.NhomDichVuBenhVienId = case when A.DichVuKyThuatId is not null AND B.NhomDichVuKyThuatId = 5 then 7 END FROM [DichVuKyThuatBenhVien] AS A left join [DichVuKyThuat] B on A.DichVuKyThuatId = B.Id WHERE  B.NhomDichVuKyThuatId = 5
UPDATE A SET  A.NhomDichVuBenhVienId = case when A.DichVuKyThuatId is not null AND B.NhomDichVuKyThuatId = 16 then 33 END FROM [DichVuKyThuatBenhVien] AS A left join [DichVuKyThuat] B on A.DichVuKyThuatId = B.Id WHERE  B.NhomDichVuKyThuatId = 16
UPDATE A SET  A.NhomDichVuBenhVienId = case when A.DichVuKyThuatId is not null AND B.NhomDichVuKyThuatId = 2 then 29 END FROM [DichVuKyThuatBenhVien] AS A left join [DichVuKyThuat] B on A.DichVuKyThuatId = B.Id WHERE  B.NhomDichVuKyThuatId = 2
UPDATE A SET  A.NhomDichVuBenhVienId = case when A.DichVuKyThuatId is not null AND B.NhomDichVuKyThuatId = 17 then 40 END FROM [DichVuKyThuatBenhVien] AS A left join [DichVuKyThuat] B on A.DichVuKyThuatId = B.Id WHERE  B.NhomDichVuKyThuatId = 17
UPDATE A SET  A.NhomDichVuBenhVienId = case when A.DichVuKyThuatId is not null AND B.NhomDichVuKyThuatId = 31 then 30 END FROM [DichVuKyThuatBenhVien] AS A left join [DichVuKyThuat] B on A.DichVuKyThuatId = B.Id WHERE  B.NhomDichVuKyThuatId = 31
UPDATE A SET  A.NhomDichVuBenhVienId = case when A.DichVuKyThuatId is not null AND B.NhomDichVuKyThuatId = 11 then 35 END FROM [DichVuKyThuatBenhVien] AS A left join [DichVuKyThuat] B on A.DichVuKyThuatId = B.Id WHERE  B.NhomDichVuKyThuatId = 11
UPDATE A SET  A.NhomDichVuBenhVienId = case when A.DichVuKyThuatId is not null AND B.NhomDichVuKyThuatId = 20 then 28 END FROM [DichVuKyThuatBenhVien] AS A left join [DichVuKyThuat] B on A.DichVuKyThuatId = B.Id WHERE  B.NhomDichVuKyThuatId = 20
UPDATE A SET  A.NhomDichVuBenhVienId = case when A.DichVuKyThuatId is not null AND B.NhomDichVuKyThuatId = 28 then 48 END FROM [DichVuKyThuatBenhVien] AS A left join [DichVuKyThuat] B on A.DichVuKyThuatId = B.Id WHERE  B.NhomDichVuKyThuatId = 28
UPDATE A SET  A.NhomDichVuBenhVienId = case when A.DichVuKyThuatId is not null AND B.NhomDichVuKyThuatId = 14 then 34 END FROM [DichVuKyThuatBenhVien] AS A left join [DichVuKyThuat] B on A.DichVuKyThuatId = B.Id WHERE  B.NhomDichVuKyThuatId = 14
UPDATE A SET  A.NhomDichVuBenhVienId = case when A.DichVuKyThuatId is not null AND B.NhomDichVuKyThuatId = 37 then 6 END FROM [DichVuKyThuatBenhVien] AS A left join [DichVuKyThuat] B on A.DichVuKyThuatId = B.Id WHERE  B.NhomDichVuKyThuatId = 37
UPDATE A SET  A.NhomDichVuBenhVienId = case when A.DichVuKyThuatId is not null AND B.NhomDichVuKyThuatId = 8 then 1 END FROM [DichVuKyThuatBenhVien] AS A left join [DichVuKyThuat] B on A.DichVuKyThuatId = B.Id WHERE  B.NhomDichVuKyThuatId = 8
UPDATE [DichVuKyThuatBenhVien] SET  NhomDichVuBenhVienId = 6 WHERE  NhomDichVuBenhVienId IS NULL
GO
ALTER TABLE [DichVuKyThuatBenhVien]
ALTER COLUMN [NhomDichVuBenhVienId] bigint NOT NULL;
GO
ALTER TABLE [YeuCauDichVuKyThuat]
ADD 
	[NhomDichVuBenhVienId] bigint NULL;
GO
ALTER TABLE [dbo].[YeuCauDichVuKyThuat]  WITH CHECK ADD  CONSTRAINT [FK__YeuCauDichVuKyThuat__NhomDichVuBenhVien] FOREIGN KEY([NhomDichVuBenhVienId])
REFERENCES [dbo].[NhomDichVuBenhVien] ([Id])
GO
UPDATE A SET  A.NhomDichVuBenhVienId = B.NhomDichVuBenhVienId
FROM [YeuCauDichVuKyThuat] AS A inner join [DichVuKyThuatBenhVien] B on A.DichVuKyThuatBenhVienId = B.Id
GO
ALTER TABLE [YeuCauDichVuKyThuat]
ALTER COLUMN [NhomDichVuBenhVienId] bigint NOT NULL;
GO
Update CauHinh
Set [Value] = '0.3.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'

