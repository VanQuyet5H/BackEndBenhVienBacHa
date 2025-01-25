ALTER TABLE [dbo].[ChuongICD]
	ADD	
		[Stt] [nvarchar](20) NULL,
		[TenTiengVietTheoBenhVien] [nvarchar](250) NULL,
		[TenTiengAnhTheoBenhVien] [nvarchar](250) NULL,
		[CoBaoCao] [bit] NULL
		
GO
CREATE TABLE [dbo].[NhomICDTheoBenhVien](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ChuongICDId] [bigint] NOT NULL,
	[Stt] [nvarchar](20) NOT NULL,
	[Ma] [nvarchar](500) NOT NULL,
	[TenTiengViet] [nvarchar](250) NOT NULL,
	[TenTiengAnh] [nvarchar](250) NOT NULL,
	[HieuLuc] [bit] NOT NULL,
	[MoTa] [nvarchar](4000) NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__NhomICDTheoBenhVien] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[NhomICDTheoBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_NhomICDTheoBenhVien_ChuongICD] FOREIGN KEY([ChuongICDId])
REFERENCES [dbo].[ChuongICD] ([Id])
GO

ALTER TABLE [dbo].[NhomICDTheoBenhVien] CHECK CONSTRAINT [FK_NhomICDTheoBenhVien_ChuongICD]
GO

CREATE TABLE [dbo].[NhomICDLienKetICDTheoBenhVien](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[NhomICDTheoBenhVienId] [bigint] NOT NULL,
	[ICDId] [bigint] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK__NhomICDLienKetICDTheoBenhVien] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[NhomICDLienKetICDTheoBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_NhomICDLienKetICDTheoBenhVien_ICD] FOREIGN KEY([ICDId])
REFERENCES [dbo].[ICD] ([Id])
GO

ALTER TABLE [dbo].[NhomICDLienKetICDTheoBenhVien] CHECK CONSTRAINT [FK_NhomICDLienKetICDTheoBenhVien_ICD]
GO

ALTER TABLE [dbo].[NhomICDLienKetICDTheoBenhVien]  WITH CHECK ADD  CONSTRAINT [FK_NhomICDLienKetICDTheoBenhVien_NhomICDTheoBenhVien] FOREIGN KEY([NhomICDTheoBenhVienId])
REFERENCES [dbo].[NhomICDTheoBenhVien] ([Id])
GO

ALTER TABLE [dbo].[NhomICDLienKetICDTheoBenhVien] CHECK CONSTRAINT [FK_NhomICDLienKetICDTheoBenhVien_NhomICDTheoBenhVien]
GO

update [ChuongICD] set Stt = N'C01', CoBaoCao = 1, TenTiengVietTheoBenhVien = N'Chương I: Bệnh nhiễm khuẩn và kí sinh vật', TenTiengAnhTheoBenhVien = N'Chapter I: Certain infectious and parasistic diseases.' where SoChuong = N'I'
update [ChuongICD] set Stt = N'C02', CoBaoCao = 1, TenTiengVietTheoBenhVien = N'Chương II: Khối u', TenTiengAnhTheoBenhVien = N'Chapter II: Neoplasms' where SoChuong = N'II'
update [ChuongICD] set Stt = N'C03', CoBaoCao = 1, TenTiengVietTheoBenhVien = N'Chương III: Bệnh của máu , cơ quan tạo máu và cơ chế miễn dịch.', TenTiengAnhTheoBenhVien = N'Chapter III: Diseases of the blood and blood - forming organ and disorders involving the immune mechanism' where SoChuong = N'III'
update [ChuongICD] set Stt = N'C04', CoBaoCao = 1, TenTiengVietTheoBenhVien = N'Chương IV: Bệnh nội tiết, dinh dưỡng chuyển hoá', TenTiengAnhTheoBenhVien = N'Chapter IV: Endocrine,Nutritional and metabolic diseases' where SoChuong = N'IV'
update [ChuongICD] set Stt = N'C05', CoBaoCao = 1, TenTiengVietTheoBenhVien = N'Chương V: Rối loạn tâm thần và hành vi', TenTiengAnhTheoBenhVien = N'Chapter V: Mental and behavioural disorders' where SoChuong = N'V'
update [ChuongICD] set Stt = N'C06', CoBaoCao = 1, TenTiengVietTheoBenhVien = N'Chương VI: Bệnh của hệ thống thần kinh', TenTiengAnhTheoBenhVien = N'Chapter VI: Diseases of the nervous system' where SoChuong = N'VI'
update [ChuongICD] set Stt = N'C07', CoBaoCao = 1, TenTiengVietTheoBenhVien = N'Chương VII: Bệnh của mắt và phần phụ', TenTiengAnhTheoBenhVien = N'Chapter VII: Diseases of the eye and adnexa' where SoChuong = N'VII'
update [ChuongICD] set Stt = N'C08', CoBaoCao = 1, TenTiengVietTheoBenhVien = N'Chương VIII: Bệnh của tai và xương chũm', TenTiengAnhTheoBenhVien = N'Chapter VIII: Diseases of the ear and mastoid process' where SoChuong = N'VIII'
update [ChuongICD] set Stt = N'C09', CoBaoCao = 1, TenTiengVietTheoBenhVien = N'Chương IX: Bệnh của hệ tuần hoàn', TenTiengAnhTheoBenhVien = N'Chapter IX: Diseases of the circulatory system' where SoChuong = N'IX'
update [ChuongICD] set Stt = N'C10', CoBaoCao = 1, TenTiengVietTheoBenhVien = N'Chương X: Bệnh của hệ hô hấp', TenTiengAnhTheoBenhVien = N'Chapter X: Diseases of the respiratory system' where SoChuong = N'X'
update [ChuongICD] set Stt = N'C11', CoBaoCao = 1, TenTiengVietTheoBenhVien = N'Chương XI: Bệnh của hệ tiêu hoá', TenTiengAnhTheoBenhVien = N'Chapter XI: Diseases of the digestive system' where SoChuong = N'XI'
update [ChuongICD] set Stt = N'C12', CoBaoCao = 1, TenTiengVietTheoBenhVien = N'Chương XII: Bệnh của da và tổ chức dưới da.', TenTiengAnhTheoBenhVien = N'Chapter XII: Diseases of skin and subcutanneous tissue' where SoChuong = N'XII'
update [ChuongICD] set Stt = N'C13', CoBaoCao = 1, TenTiengVietTheoBenhVien = N'Chương XIII: Bệnh của hệ thống cơ, xương và mô liên kết', TenTiengAnhTheoBenhVien = N'Chapter XIII: Diseases of the musculoskeletal system and connective tissue' where SoChuong = N'XIII'
update [ChuongICD] set Stt = N'C14', CoBaoCao = 1, TenTiengVietTheoBenhVien = N'Chương XIV: Bệnh của hệ tiết niệu sinh dục', TenTiengAnhTheoBenhVien = N'Chapter XIV: Diseases of the genitourinary system B212' where SoChuong = N'XIV'
update [ChuongICD] set Stt = N'C15', CoBaoCao = 1, TenTiengVietTheoBenhVien = N'Chương XV: Chửa,đẻ và sau đẻ', TenTiengAnhTheoBenhVien = N'Chapter XV: Pregnancy, childbirth and the puerperium' where SoChuong = N'XV'
update [ChuongICD] set Stt = N'C16', CoBaoCao = 1, TenTiengVietTheoBenhVien = N'Chương XVI: Một số bệnh trong thời kì chu sinh', TenTiengAnhTheoBenhVien = N'Chapter XVI: Certain conditions originating in the perinatal period' where SoChuong = N'XVI'
update [ChuongICD] set Stt = N'C17', CoBaoCao = 1, TenTiengVietTheoBenhVien = N'Chương XVII: Dị dạng bẩm sinh, biến dạng của cromosom', TenTiengAnhTheoBenhVien = N'Chapter XVII: Congenital malformations, deformations and chromosomal abnormalities' where SoChuong = N'XVII'
update [ChuongICD] set Stt = N'C18', CoBaoCao = 1, TenTiengVietTheoBenhVien = N'Chương XVIII: Triệu chứng, dấu hiệu và phát hiện bất thường lâm sàng, xét nghiệm', TenTiengAnhTheoBenhVien = N'Chapter XVIII: Symptoms, signs and abnormal clinical and laboratory findings, not elsewhere classified' where SoChuong = N'XVIII'
update [ChuongICD] set Stt = N'C19', CoBaoCao = 1, TenTiengVietTheoBenhVien = N'Chương XIX: Vết thương, ngộ độc và kết quả của các nguyên nhân bên ngoàiChapter', TenTiengAnhTheoBenhVien = N'XIX: Iinjury, poisoning and certain other consequences of external causes' where SoChuong = N'XIX'
update [ChuongICD] set Stt = N'C20', CoBaoCao = 1, TenTiengVietTheoBenhVien = N'Chương XX: Nguyên nhân bên ngoài của bệnh tật và tử vong', TenTiengAnhTheoBenhVien = N'Chapter XX: External causes of morbidity and mortality' where SoChuong = N'XX'
update [ChuongICD] set Stt = N'C21', CoBaoCao = 1, TenTiengVietTheoBenhVien = N'Chương XXI: Các yếu tố ảnh hưởng đến sức khoẻ người khám nghiệm và điều tra', TenTiengAnhTheoBenhVien = N'Chapter XXI: Person encountering health services for examination and investigation.' where SoChuong = N'XXI'
GO

SET IDENTITY_INSERT [dbo].[NhomICDTheoBenhVien] ON 
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (1, 26, N'001', N'A00', N'Tả', N'Cholera', 1, N'A00', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (2, 39, N'212', N'N02-N08', N'Bệnh cầu thận khác', N'Other glomerular diseases', 1, N'N02;N03;N04;N05;N06;N07;N08', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (3, 39, N'211', N'N00-N01', N'Hội chứng viêm thận cấp và tiến triển nhanh', N'Acute and rapidly prograssive nephritis syndromes', 1, N'N00;N01', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (4, 38, N'210', N'M87-M99', N'Bệnh khác của hệ xương khớp, cơ và mô liên kết', N'Other diseases of the musculo', 1, N'M87;M88;M89;M90;M91;M92;M93;M94;M95;M96;M97;M98;M99', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (5, 38, N'209', N'M86', N'Viêm xương tuỷ', N'Osteomyelitis', 1, N'M86', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (6, 38, N'208', N'M80-M85', N'Di tật về mật độ và cấu trúc của xương', N'Disorders of bone density and structure', 1, N'M80;M81;M82;M83;M84;M85', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (7, 38, N'207', N'M60-M79', N'Tổn thương các mô mềm', N'Soft tissue disorders', 1, N'M60;M61;M62;M63;M64;M65;M66;M67;M68;M69;M70;M71;M72;M73;M74;M75;M76;M77;M78;M79', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (8, 38, N'206', N'M40-M49, M53-M54', N'Bệnh khác của cột sống', N'Other dorsopathies', 1, N'M40;M41;M42;M43;M44;M45;M46;M47;M48;M49;M53;M54', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (9, 38, N'205', N'M50-M51', N'Trật đốt sống cổ và các đốt sống khác', N'Cervical and other interverbral disc disorders', 1, N'M50;M51', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (10, 38, N'204', N'M30-M36', N'Bệnh của hệ thống tổ chức liên kết', N'Systematic connective tissue disorders', 1, N'M30;M31;M32;M33;M34;M35;M36', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (11, 38, N'203', N'M00-M03, M22-M25', N'Bệnh khác của khớp', N'Other joint disorders', 1, N'M00;M01;M02;M03;M22;M23;M24;M25', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (12, 38, N'202', N'M20- M21', N'Biến dạng các chi mắc phải', N'Accquired deformities of limbs', 1, N'M20;M21', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (13, 38, N'201', N'M15-M19', N'Bệnh thoái hoá khớp', N'Arthrosis', 1, N'M15;M16;M17;M18;M19', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (14, 38, N'200', N'M05-M14', N'Viêm khớp dạng thấp và viêm khớp khác', N'Rheumatoid arthritis, other inflamatory polyarthropaties', 1, N'M05;M06;M07;M08;M09;M10;M11;M12;M13;M14', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (15, 37, N'199', N'L10-L99', N'Bệnh khác của da và mô tế bào dưới da', N'Other diseases of skin and subcutaneous tissue', 1, N'L10;L11;L12;L13;L14;L15;L16;L17;L18;L19;L20;L21;L22;L23;L24;L25;L26;L27;L28;L29;L30;L31;L32;L33;L34;L35;L36;L37;L38;L39;L40;L41;L42;L43;L44;L45;L46;L47;L48;L49;L50;L51;L52;L53;L54;L55;L56;L57;L58;L59;L60;L61;L62;L63;L64;L65;L66;L67;L68;L69;L70;L71;L72;L73;L74;L75;L76;L77;L78;L79;L80;L81;L82;L83;L84;L85;L86;L87;L88;L89;L90;L91;L92;L93;L94;L95;L96;L97;L98;L99', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (16, 37, N'198', N'L00- L08', N'Bệnh nhiễm khuẩn da và mô tế bào dưới da', N'Infections of skin and subcutaneous tissue', 1, N'L00;L01;L02;L03;L04;L05;L06;L07;L08', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (17, 39, N'213', N'N10-N16', N'Bệnh ống thận kẽ', N'Renal tubulo', 1, N'N10;N11;N12;N13;N14;N15;N16', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (18, 39, N'214', N'N17-N19', N'Suy thận', N'Renal failure', 1, N'N17;N18;N19', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (19, 39, N'215', N'N20-N23', N'Sỏi tiết niệu', N'Urolithiasis', 1, N'N20;N21;N22;N23', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (20, 39, N'216', N'N30', N'Viêm bàng quang', N'Cystitis', 1, N'N30', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (21, 39, N'232', N'N97', N'Vô sinh nữ', N'Female infertility', 1, N'N97', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (22, 39, N'231', N'N95', N'Rối loạn mãn kinh và xung quanh mãn kinh khác', N'Menopausal other perimenopausal disorders', 1, N'N95', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (23, 39, N'230', N'N91-N92', N'Rối loạn kinh nguyệt', N'Disorders of menstruation', 1, N'N91;N92', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (24, 39, N'229', N'N83', N'Tổn thương không viêm của buồng trứng, vòi fallope và dây chằng rộng', N'Noinflammatory disorders of ovary, fallopian tube and broad ligament', 1, N'N83', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (25, 39, N'228', N'N81', N'Sa sinh dục nữ', N'Female genital prolapse', 1, N'N81', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (26, 39, N'227', N'N80', N'Viêm niêm mạc tử cung', N'Endometriosis', 1, N'N80', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (27, 39, N'226', N'N71, N73-N77', N'Tổn thương viêm khác của các cơ quan khung chậu nữ', N'Other inflamatory diseases of female pelvic organs', 1, N'N71;N73;N74;N75;N76;N77', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (28, 36, N'197', N'K32, K87-K93,K82-K83', N'Bệnh khác của bộ máy tiêu hoá', N'Other diseases of the digestive system', 1, N'K32;K87;K88;K89;K90;K91;K92;K93;K82;K83', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (29, 39, N'225', N'N72', N'Viêm nhiễm cổ tử cung', N'Inflamatory disease of cervix uteri', 1, N'N72', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (30, 39, N'223', N'N60-N64', N'Tổn thương của vú', N'Disorders of breast', 1, N'N60;N61;N62;N63;N64', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (31, 39, N'222', N'N44- N46, N48-N51', N'Bệnh khác của cơ quan sinh dục nam', N'Other diseases of male genital organs', 1, N'N44;N45;N46;N48;N49;N50;N51', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (32, 39, N'221', N'N47', N'Thừa bao qui đầu, hẹp và nghẹt bao qui đầu', N'Redundant prepuce, phimosis and paraphimosis', 1, N'N47', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (33, 39, N'220', N'N43', N'Tràn dịch tinhmạc,u nang tinhdịch Hydrocele and spermatocele', N'', 1, N'N43', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (34, 39, N'219', N'N41-N42', N'Tổn thương khác của tuyến tiền liệt', N'Other disorders of prostate', 1, N'N41;N42', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (35, 39, N'218', N'N40', N'Quá sản tuyến tiền liệt', N'Hyperplasia of prostate', 1, N'N40', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (36, 39, N'217', N'N25-N29,N31-N39', N'Bệnh khác của bộ máy tiết niệu', N'Other diseases of the urnary system', 1, N'N25;N26;N27;N28;N29;N31;N32;N33;N34;N35;N36;N37;N38;N39', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (37, 39, N'224', N'N70', N'Viêm vòi trứng và viêm buồng trứng', N'Salpingitis and oophoritis', 1, N'N70', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (38, 39, N'233', N'N82, N84-N90,N93-N94, N96, N98-N99', N'Bệnh khác của bộ máy sinh dục tiết niệu', N'Other disorders of genitourinary tract', 1, N'N82;N84;N85;N86;N87;N88;N89;N90;N93;N94;N96;N98;N99', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (39, 36, N'196', N'K85-K86', N'Viêm tuỵ cấp và bệnh khác của tuỵ', N'Acute pancreatitis and other diseases of pancreas', 1, N'K85;K86', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (40, 36, N'194', N'K71- K77', N'Các bệnh khác của gan', N'Other diseases of liver', 1, N'K71;K72;K73;K74;K75;K76;K77', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (41, 35, N'173', N'J35', N'Bệnh mạn tính của amidan và của VA', N'Chronic diseases of tonsils and adenoids', 1, N'J35', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (42, 35, N'172', N'J30- J31, J33-J34', N'Bệnh của mũi và các xoang phụ của mũi', N'Other diseases of nose and nasal sinuses', 1, N'J30;J31;J33;J34', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (43, 35, N'171', N'J32', N'Viêm xoang mạn tính', N'Chronic sinusitis', 1, N'J32', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (44, 35, N'170', N'J20- J21', N'Viêm phế quản và viêm tiểu phế quản cấp', N'Acute bronchitis and acute bronchiolitis', 1, N'J20;J21', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (45, 35, N'169', N'J12 -J18', N'Các bệnh viêm phổi', N'Pneumonia', 1, N'J12;J13;J14;J15;J16;J17;J18', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (46, 35, N'168', N'J10- J11', N'Cúm', N'Influenza', 1, N'J10;J11', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (47, 35, N'167', N'J00- J01, J05-J06', N'Viêm cấp đường hôhấp trên khác Other acute upper respiratory infections', N'', 1, N'J00;J01;J05;J06', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (48, 35, N'166', N'J04', N'Viêm thanh, khí quản cấp', N'Acute laryngitis and tracheitis', 1, N'J04', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (49, 35, N'165', N'J02- J03', N'Viêm họng và viêm amidan cấp', N'Acute pharyngitis and acute tonsillitis', 1, N'J02;J03', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (50, 34, N'164', N'I85-I99', N'Các bệnh khác của bộ máy tuần hoàn', N'Other diseases of the circulatory system', 1, N'I85;I86;I87;I88;I89;I90;I91;I92;I93;I94;I95;I96;I97;I98;I99', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (51, 34, N'163', N'I84', N'Trĩ', N'Haemorrhoids', 1, N'I84', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (52, 34, N'162', N'I83', N'Dẫn tĩnh mạch chi dưới', N'Varicose veins of lower extremities', 1, N'I83', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (53, 34, N'161', N'I80-I82', N'Viêm tĩnh mạch, viêm tĩnh mạch huyết khối, nghẽn mạch và huyết khối tĩnh mạch', N'Phlebitis, thrombophlebitis,venous embolism and thrombosis', 1, N'I80;I81;I82', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (54, 34, N'160', N'I71-I72, I77-I79', N'Bệnh khác của động mạch, tiểu động mạch và mao mạch', N'Other diseases of arteries, arterioles and capillaries', 1, N'I71;I72;I77;I78;I79', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (55, 34, N'159', N'I74', N'Nghẽn và huyết khối động mạch', N'Arterial embolism and thrombosis', 1, N'I74', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (56, 35, N'174', N'J36- J39', N'Bệnh khác đường hô hấp trên', N'Other diseases of upper respiratory tract', 1, N'J36;J37;J38;J39', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (57, 35, N'175', N'J40- J44', N'Viêm phế quản tràn khí và các bệnh phổi tắc nghẽn mạn tính', N'Bronchitis, emphysema and other chronic obstructive diseases', 1, N'J40;J41;J42;J43;J44', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (58, 35, N'176', N'J45- J46', N'Hen', N'Asthma', 1, N'J45;J46', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (59, 35, N'177', N'J47', N'Giãn phế quản', N'Bronchiectasis', 1, N'J47', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (60, 36, N'193', N'K70', N'Bệnh gan do rượu', N'Alcoholic liver disease', 1, N'K70', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (61, 36, N'192', N'K52-K55, K58-K67', N'Bệnh khác của ruột non và màng bụng', N'Other diseases of intestine peritoneum', 1, N'K52;K53;K54;K55;K58;K59;K60;K61;K62;K63;K64;K65;K66;K67', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (62, 36, N'191', N'K57', N'Bệnh túi thừa của ruột non', N'Diverticular disease of intestine', 1, N'K57', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (63, 36, N'190', N'K56', N'Tắc liệt ruột và tắc ruột không do thoát vị', N'Paralytic ileus, intestinal obstruction without hernia', 1, N'K56', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (64, 36, N'189', N'K50-K51', N'Bệnh Crohn (viêm ruột non từng vùng) và viêm loét đại tràng', N'Crohn’s disease and ulcerative colitis', 1, N'K50;K51', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (65, 36, N'188', N'K41-K46', N'Các thoát vị khác', N'Other hernia', 1, N'K41;K42;K43;K44;K45;K46', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (66, 36, N'187', N'K40', N'Thoát vị bẹn', N'Inguinal hernia', 1, N'K40', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (67, 36, N'195', N'K80-K81', N'Sỏi mật và viêm túi mật', N'Cholelithiasis and cholecystitis', 1, N'K80;K81', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (68, 36, N'186', N'K35- K38', N'Bệnh của ruột thừa', N'Diseases of appendix', 1, N'K35;K36;K37;K38', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (69, 36, N'184', N'K29', N'Viêm dạ dày và tá tràng Gastritis and duodenitis', N'', 1, N'K29', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (70, 36, N'183', N'K25- K27', N'Loét dạ dày và tá tràng', N'Gastric and duodenal ulcer', 1, N'K25;K26;K27', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (71, 36, N'182', N'K09- K14', N'Bệnh khác của khoang miệng, tuyến nước bọt và hàm', N'Other diseases of the oral cavity, salivary glands and jaws', 1, N'K09;K10;K11;K12;K13;K14', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (72, 36, N'181', N'K03- K08,K00- K01', N'Tổn thương khác liên quan đến răng và mô quanh răng', N'Other disorders of teeth and supporting structures', 1, N'K03;K04;K05;K06;K07;K08;K00;K01', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (73, 36, N'180', N'K02', N'Sâu răng', N'Dental caries', 1, N'K02', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (74, 35, N'179', N'J09, J22, J61-J65, J66-J99', N'Bệnh khác của bộ máy hô hấp', N'Other diseases of respiratory system.', 1, N'J09;J22;J61;J62;J63;J64;J65;J66;J67;J68;J69;J70;J71;J72;J73;J74;J75;J76;J77;J78;J79;J80;J81;J82;J83;J84;J85;J86;J87;J88;J89;J90;J91;J92;J93;J94;J95;J96;J97;J98;J99', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (75, 35, N'178', N'J60', N'Bệnh phổi không do phế cầu khuẩn', N'Pneumoconiosis', 1, N'J60', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (76, 36, N'185', N'K20-K23,K28, K30-K31', N'Bệnh khác của thực quản, dạ dày và tá tràng', N'Other diseases of oesophagus, stomach, duodenum', 1, N'K20;K21;K22;K23;K28;K30;K31', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (77, 34, N'158', N'I73', N'Bệnh mạch máu ngoại vi khác', N'Other peripheral vascular disease', 1, N'I73', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (78, 40, N'234', N'O03', N'Xẩy thai tự nhiên', N'Spontaneous abortion', 1, N'O03', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (79, 40, N'236', N'O00-O02,O05-O08', N'Xẩy thai khác', N'Other pregnancies with abortive outcome', 1, N'O00;O01;O02;O05;O06;O07;O08', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (80, 46, N'290', N'V01-V09, V10-V99, W00,W01 - W19', N'Tai nạn giao thông', N'Transport accidént', 1, N'V01;V02;V03;V04;V05;V06;V07;V08;V09;V10;V11;V12;V13;V14;V15;V16;V17;V18;V19;V20;V21;V22;V23;V24;V25;V26;V27;V28;V29;V30;V31;V32;V33;V34;V35;V36;V37;V38;V39;V40;V41;V42;V43;V44;V45;V46;V47;V48;V49;V50;V51;V52;V53;V54;V55;V56;V57;V58;V59;V60;V61;V62;V63;V64;V65;V66;V67;V68;V69;V70;V71;V72;V73;V74;V75;V76;V77;V78;V79;V80;V81;V82;V83;V84;V85;V86;V87;V88;V89;V90;V91;V92;V93;V94;V95;V96;V97;V98;V99;W00;W01;W02;W03;W04;W05;W06;W07;W08;W09;W10;W11;W12;W13;W14;W15;W16;W17;W18;W19', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (81, 44, N'289', N'T90-T98', N'Di chứng, thương tổn do chấn thương, do ngộ độc và hậu quả khác do nguyên nhân bên ngoài', N'Sequalae of injuries, of poisoning and of other consequences of external causes', 1, N'T90;T91;T92;T93;T94;T95;T96;T97;T98', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (82, 44, N'288', N'T79-T88', N'Một số biến chứng sớm của chấn thương, của chăm sóc ngoại khoa và y học, chưa xếp ở nơi khác', N'Certain early complications of trauma and complications of surgical and medical care, not elsewhere classified', 1, N'T79;T80;T81;T82;T83;T84;T85;T86;T87;T88', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (83, 44, N'287', N'T33- T35, T66-T73, T75-T78', N'Hiệu quả của các nguyên nhân bên ngoài khác và không xác định', N'Other and unspecified effects of external causes', 1, N'T33;T34;T35;T66;T67;T68;T69;T70;T71;T72;T73;T75;T76;T77;T78', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (84, 44, N'286', N'T74', N'Các hội chứng do điều trị xấu', N'Maltreatment syndromes', 1, N'T74', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (85, 44, N'285', N'T51- T65', N'Tác hại của các chất có nguồn gốc chủ yếu không phải thuốc', N'Toxic effects of substances chiefly nonmedical as to source', 1, N'T51;T52;T53;T54;T55;T56;T57;T58;T59;T60;T61;T62;T63;T64;T65', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (86, 44, N'284', N'T36- T50', N'Nhiễm độc thuốc và các sinh phẩm', N'Poisoning by drugs and biological substances', 1, N'T36;T37;T38;T39;T40;T41;T42;T43;T44;T45;T46;T47;T48;T49;T50', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (87, 44, N'283', N'T20-T32', N'Bỏng và sự ăn mòn', N'Burnss and corrosions', 1, N'T20;T21;T22;T23;T24;T25;T26;T27;T28;T29;T30;T31;T32', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (88, 44, N'282', N'T15-T19', N'Hậu quả do dị vật vào hốc tự nhiên', N'Effects of foreign body entert hrough natural orifice', 1, N'T15;T16;T17;T18;T19', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (89, 44, N'281', N'S00 - S01, S04, S09- S11, S14- S16, S19 - S21,S24-S25, S29-S31, S34-S35, S39-S41, S44-S46, S49-S51, S54-S56, S59-S61, S64-S66, S69-S71, S74-S76, S79-S81, S84-S86, S89-S91, S94-S96, S99, T00-T01, T06, T07, T09, T11, T13-T14', N'Các tổn thưng khác do chấn thưng xác định và ở nhiều nơi', N'Other injuries of specified, unspecified and multiple body regions', 1, N'S00;S01;S04;S09;S10;S11;S14;S15;S16;S19;S20;S21;S24;S25;S29;S30;S31;S34;S35;S39;S40;S41;S44;S45;S46;S49;S50;S51;S54;S55;S56;S59;S60;S61;S64;S65;S66;S69;S70;S71;S74;S75;S76;S79;S80;S81;S84;S85;S86;S89;S90;S91;S94;S95;S96;S99;T00;T01;T06;T07;T09;T11;T13;T14', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (90, 44, N'280', N'S07-S08, S17 - S18,S28, S38, S47-S48, S57-S58, S67-S68, S77-S78, S87-S88, S97 - S98, T04- T05', N'Chấn thương dập nát và cắt cụt đã xác định và nhiều vùng trong cơ thể', N'Crushing injuries and traumatic amputation or specified and multiple body regions', 1, N'S07;S08;S17;S18;S28;S38;S47;S48;S57;S58;S67;S68;S77;S78;S87;S88;S97;S98;T04;T05', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (91, 44, N'279', N'S26-S27,S36- S37', N'Thương tổn do chấn thương các nội tạng khác', N'Injury of other internal organs', 1, N'S26;S27;S36;S37', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (92, 44, N'278', N'S06', N'Thương tổn do chấn thương trong sọ', N'Intracranial injury', 1, N'S06', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (93, 44, N'277', N'S05', N'Thương tổn do chấn thương ở mắt và hốc mắt', N'Injury of eye and orbit', 1, N'S05', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (94, 44, N'276', N'S03, S13, S23, S33, S43, S53, S63, S73, S83, S93, T03', N'Sai khớp, bong gân, tổn thương khu trú và ở nhiều vùng cơ thể', N'Dislocations, sprains and strains of specified and multiple body regions', 1, N'S03;S13;S23;S33;S43;S53;S63;S73;S83;S93;T03', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (95, 46, N'291', N'W20-W64', N'Tai nạn do các nguyên nhân sức mạnh cơ học và không cố ý', N'exposure to inanimate mechanical forces', 1, N'W20;W21;W22;W23;W24;W25;W26;W27;W28;W29;W30;W31;W32;W33;W34;W35;W36;W37;W38;W39;W40;W41;W42;W43;W44;W45;W46;W47;W48;W49;W50;W51;W52;W53;W54;W55;W56;W57;W58;W59;W60;W61;W62;W63;W64', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (96, 46, N'292', N'W65-W84', N'Tai nạn chết đuối, chết chìm', N'Accident drowning and submersion', 1, N'W65;W66;W67;W68;W69;W70;W71;W72;W73;W74;W75;W76;W77;W78;W79;W80;W81;W82;W83;W84', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (97, 46, N'293', N'W85-W99', N'Tai nạn do dòng điện bức xạ, nhiệt độ và áp lực không khí quá cao', N'exposure to electric current radiation and extreme ambian air temperature and pressure', 1, N'W85;W86;W87;W88;W89;W90;W91;W92;W93;W94;W95;W96;W97;W98;W99', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (98, 46, N'294', N'X00-X09', N'Tai nạn do khói, lửa, đám cháy', N'expossure to smoke, fire and fpames', 1, N'X00;X01;X02;X03;X04;X05;X06;X07;X08;X09', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (99, 47, N'310', N'Z39', N'Chăm sóc và khám xét sau đẻ', N'Postpartum care and examination', 1, N'Z39', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (100, 47, N'309', N'Z38', N'Trẻ đẻ ra sống phân theo nơi sinh', N'Liveborn infants according to place of birth', 1, N'Z38', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (101, 47, N'308', N'Z34- Z36', N'Giám sát thai nghén và phát hiện trước đẻ', N'Antenatal screening and other supervision of pregnancy', 1, N'Z34;Z35;Z36', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (102, 47, N'307', N'Z30', N'Quản lí các biện pháp tránh thai', N'Contraceptive management', 1, N'Z30', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (103, 47, N'306', N'Z20, Z22- Z29', N'Người có nguy cơ liên quan đến bệnh truyền nhiễm', N'Other persons with potential health hazards related to communicable diseases', 1, N'Z20;Z22;Z23;Z24;Z25;Z26;Z27;Z28;Z29', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (104, 47, N'305', N'Z21', N'Nhiễm HIV không có triệu chứng', N'Asymptomatic human immuno deficiency virus infection status', 1, N'Z21', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (105, 47, N'304', N'Z00- Z01', N'Người tiếp xúc với các dịch vụ y tế làm các khám xét và điều tra', N'Person encountering health services for examination and investigation', 1, N'Z00;Z01', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (106, 44, N'275', N'T02', N'Gãy nhiều xương của cơ thể: do lao động và giao thông', N'Fractures involving multiple body regions', 1, N'T02', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (107, 46, N'303', N'Y31-Y36, Y83-Y89,Y90-Y98', N'Các yếu tố tăng cường cho nguyên nhân bệnh tật tử vong đã có trong phân loại', N'Supplementary factors related to cause of morbidity classified elswhere', 1, N'Y31;Y32;Y33;Y34;Y35;Y36;Y83;Y84;Y85;Y86;Y87;Y88;Y89;Y90;Y91;Y92;Y93;Y94;Y95;Y96;Y97;Y98', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (108, 46, N'301', N'Y60-Y69', N'Tai nạn rủi ro với người bệnh trong chăm sóc nội khoa', N'Misadventures to patients during surgical and medical care.', 1, N'Y60;Y61;Y62;Y63;Y64;Y65;Y66;Y67;Y68;Y69', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (109, 46, N'300', N'Y10-Y30,Y40-Y59', N'Tai biến của thuốc và các chất sinh học trong điều trị', N'Drugs medicament and biological substances causing adverseeffects in therapeutic use.', 1, N'Y10;Y11;Y12;Y13;Y14;Y15;Y16;Y17;Y18;Y19;Y20;Y21;Y22;Y23;Y24;Y25;Y26;Y27;Y28;Y29;Y30;Y40;Y41;Y42;Y43;Y44;Y45;Y46;Y47;Y48;Y49;Y50;Y51;Y52;Y53;Y54;Y55;Y56;Y57;Y58;Y59', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (110, 46, N'299', N'X85-Y09', N'Bạo lực đánh nhau', N'Assault', 1, N'X85;X86;X87;X88;X89;X90;X91;X92;X93;X94;X95;X96;X97;X98;X99;Y00;Y01;Y02;Y03;Y04;Y05;Y06;Y07;Y08;Y09', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (111, 46, N'298', N'X60-X84', N'Tự tử', N'Intentionnal sel', 1, N'X60;X61;X62;X63;X64;X65;X66;X67;X68;X69;X70;X71;X72;X73;X74;X75;X76;X77;X78;X79;X80;X81;X82;X83;X84', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (112, 46, N'297', N'X30-X39, X40-X49, X50-X59', N'Tai nạn ngộ độc do các chất độc', N'Accident poisoning by and exposure to noxious substances', 1, N'X30;X31;X32;X33;X34;X35;X36;X37;X38;X39;X40;X41;X42;X43;X44;X45;X46;X47;X48;X49;X50;X51;X52;X53;X54;X55;X56;X57;X58;X59', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (113, 46, N'296', N'X20-X29', N'Tai nạn do tiếp xúc với động vật và cây độc', N'Contact with venomous animals and plants', 1, N'X20;X21;X22;X23;X24;X25;X26;X27;X28;X29', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (114, 46, N'295', N'X10-X19', N'Tai nạn do tiếp xúc với các chất nóng', N'contact with heat and hot', 1, N'X10;X11;X12;X13;X14;X15;X16;X17;X18;X19', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (115, 46, N'302', N'Y70-Y82', N'Tai nạn do sử dụng thiết bị trong chẩn đoán và điều trị', N'Medical devices associated with adverse incidents in diagnostic and therapeutic use', 1, N'Y70;Y71;Y72;Y73;Y74;Y75;Y76;Y77;Y78;Y79;Y80;Y81;Y82', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (116, 40, N'235', N'O04', N'Xẩy thai do can thiệp y tế', N'Medical abortion', 1, N'O04', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (117, 44, N'274', N'S42, S52,S62,S82,S92,T10,T12', N'Gãy các phần khác của chi: do lao động và giao thông', N'Fracture of other lim bones', 1, N'S42;S52;S62;S82;S92;T10;T12', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (118, 44, N'272', N'S12,S22,S32,T08', N'Gãy xương cổ, ngực, khung chậu', N'Fracture of neck, thorax or pelvis.', 1, N'S12;S22;S32;T08', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (119, 41, N'251', N'P38-P39', N'Nhiễm khuẩn đặc hiệu khác thời kỳ chu sinh', N'Other infectious specific to the perinatal period', 1, N'P38;P39', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (120, 41, N'250', N'P35-P37', N'Nhiễm khuẩn và kí sinh vật bẩm sinh', N'Congenital infectious and parasitic diseases', 1, N'P35;P36;P37', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (121, 41, N'249', N'P22-P28', N'Các tổn thương hô hấp đặc hiệu khác của thời kỳ chu sinh', N'Other respiratory disorders originating in the perinatal period', 1, N'P22;P23;P24;P25;P26;P27;P28', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (122, 41, N'248', N'P20-P21', N'Thiếu ô xy trong tử cung và trong đẻ', N'Intrauterine hypoxis and birth asphyxia', 1, N'P20;P21', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (123, 41, N'247', N'P10-P15', N'Các chấn thương sản khoa', N'Birth trauma', 1, N'P10;P11;P12;P13;P14;P15', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (124, 41, N'246', N'P05-P07', N'Thai chậm phát triển, suy dinh dưỡng, rối loạn gắn liền với thai nghén và cân nặng không đủ khi sinh', N'Slow fetal growth, fetal malnutrition and disorders related to short gestation and low birth weight', 1, N'P05;P06;P07', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (125, 41, N'245', N'P00-P04', N'Bệnh lí thai nhi và sơ sinh do biến chứng thai nghén, chửa, đẻ', N'Fetus and newborn affected by maternal factors and by complications of pregnancy, labour and delivery', 1, N'P00;P01;P02;P03;P04', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (126, 40, N'244', N'O85-O99', N'Các biến chứng liên quan sinh đẻ và những vấn đề sản khoa chưa xếp ở chỗ khác', N'Complications predominantly related to the puerperium obstetric conditions, not elsewhere classified', 1, N'O85;O86;O87;O88;O89;O90;O91;O92;O93;O94;O95;O96;O97;O98;O99', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (127, 40, N'243', N'O80', N'Đẻ tự nhiên đn gin', N'Single spontaneous delivery', 1, N'O80', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (128, 40, N'242', N'O20-O29,O60-O63,O67- O71, O73-O75,O81-O84', N'Các biến chứng khác của chửa đẻ', N'Other complications pregnancy and delivery', 1, N'O20;O21;O22;O23;O24;O25;O26;O27;O28;O29;O60;O61;O62;O63;O67;O68;O69;O70;O71;O73;O74;O75;O81;O82;O83;O84', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (129, 40, N'241', N'O72', N'Chảy máu sau đẻ', N'Postpartum haemorrhage', 1, N'O72', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (130, 40, N'240', N'O64-O66', N'Đẻ khó do cn trở (vật chướng ngại)', N'Obstructed labour', 1, N'O64;O65;O66', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (131, 40, N'239', N'O30-O43,O47-O48', N'Chăm sóc khác cho người mẹ liên quan đến thai, buồng ối và những vấn đề có thể xy ra do đẻ', N'Other maternal care related to fetus and amniotic cavity and possible delivery problems', 1, N'O30;O31;O32;O33;O34;O35;O36;O37;O38;O39;O40;O41;O42;O43;O47;O48', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (132, 40, N'238', N'O44-O46', N'Rau tiền đạo, rau bong sớm (U máu sau rau) và chảy máu trước khi đẻ', N'Placeta praevia, premature separation of placenta and antepartum haemorrhage', 1, N'O44;O45;O46', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (133, 40, N'237', N'O10-O16', N'Phù nề, protein', N'niệu, tăng huyết áp, rối loạn thai nghén, đẻ và sau đẻ', 1, N'O10;O11;O12;O13;O14;O15;O16', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (134, 41, N'252', N'P55', N'Bệnh tan máu của thai và sơ sinh', N'Haemolytic disease of fetus and newborn.', 1, N'P55', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (135, 41, N'253', N'P08,P29, P50-P54,P56-P96', N'Tổn thương khác có nguồn gốc trong thời kỳ chu sinh', N'Other conditions originating in the perinatal period', 1, N'P08;P29;P50;P51;P52;P53;P54;P56;P57;P58;P59;P60;P61;P62;P63;P64;P65;P66;P67;P68;P69;P70;P71;P72;P73;P74;P75;P76;P77;P78;P79;P80;P81;P82;P83;P84;P85;P86;P87;P88;P89;P90;P91;P92;P93;P94;P95;P96', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (136, 42, N'254', N'Q05', N'Gai đôi cột sống', N'Spina bifida', 1, N'Q05', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (137, 42, N'255', N'Q00-Q04,Q06-Q07', N'Dị tật bẩm sinh khác của hệ thần kinh', N'Other congenital malforma', 1, N'Q00;Q01;Q02;Q03;Q04;Q06;Q07', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (138, 44, N'271', N'S02', N'Vỡ xương sọ và các xương mặt', N'Fracture of skull and facial bones', 1, N'S02', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (139, 43, N'270', N'R00- R09, R11 - R49, R51- R53, R55- R99', N'Các triệu chứng, dấu hiệu và kết quả bất thường về khám lâm sàng và xét nghiệm khác, chưa xếp ở chỗ khác', N'Other symptoms, signs and abnormal clinical and laboratory findings, not elsewhere classified', 1, N'R00;R01;R02;R03;R04;R05;R06;R07;R08;R09;R11;R12;R13;R14;R15;R16;R17;R18;R19;R20;R21;R22;R23;R24;R25;R26;R27;R28;R29;R30;R31;R32;R33;R34;R35;R36;R37;R38;R39;R40;R41;R42;R43;R44;R45;R46;R47;R48;R49;R51;R52;R53;R55;R56;R57;R58;R59;R60;R61;R62;R63;R64;R65;R66;R67;R68;R69;R70;R71;R72;R73;R74;R75;R76;R77;R78;R79;R80;R81;R82;R83;R84;R85;R86;R87;R88;R89;R90;R91;R92;R93;R94;R95;R96;R97;R98;R99', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (140, 43, N'269', N'R54', N'Lão suy', N'Senility', 1, N'R54', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (141, 43, N'268', N'R50', N'Sốt không rõ nguyên nhân', N'Fever of unknown origin', 1, N'R50', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (142, 43, N'267', N'R10', N'Đau bụng và khung chậu', N'Abdominal and pelvic pain', 1, N'R10', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (143, 42, N'266', N'Q90-Q99', N'Dị thường nhiễm sắc thể, chưa xếp ở chỗ khác', N'Chromosomal abnormalities, not elsewhere sclassified', 1, N'Q90;Q91;Q92;Q93;Q94;Q95;Q96;Q97;Q98;Q99', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (144, 42, N'265', N'Q10-Q13, Q14-Q18, Q30-Q34, Q80-Q89', N'Dị dạng bẩm sinh khác', N'Other congenital malformations', 1, N'Q10;Q11;Q12;Q13;Q14;Q15;Q16;Q17;Q18;Q30;Q31;Q32;Q33;Q34;Q80;Q81;Q82;Q83;Q84;Q85;Q86;Q87;Q88;Q89', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (145, 44, N'273', N'S72', N'Gãy xương đùi', N'Fracture of femur', 1, N'S72', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (146, 42, N'264', N'Q67-Q79', N'Dị dạng bẩm sinh khác của hệ xương và cơ', N'Other congenital malformations and deformations of the musculo skeletal system', 1, N'Q67;Q68;Q69;Q70;Q71;Q72;Q73;Q74;Q75;Q76;Q77;Q78;Q79', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (147, 42, N'262', N'Q65', N'Dị dạng bẩm sinh hông', N'Congenital deformities of hip', 1, N'Q65', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (148, 42, N'261', N'Q50-Q52,Q54-Q64', N'Dị dạng bẩm sinh của bộ máy sinh dục tiết niệu', N'Congenital malformations of genital organs', 1, N'Q50;Q51;Q52;Q54;Q55;Q56;Q57;Q58;Q59;Q60;Q61;Q62;Q63;Q64', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (149, 42, N'260', N'Q53', N'Tinh hoàn lạc chỗ', N'Undescended testicle', 1, N'Q53', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (150, 42, N'259', N'Q38-Q40,Q42-Q45', N'Dị tật bẩm sinh khác của bộ máy sinh dục tiết niệu', N'Other malformations of the genitourinary system', 1, N'Q38;Q39;Q40;Q42;Q43;Q44;Q45', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (151, 42, N'258', N'Q41', N'Không có, tịt hoặc hẹp ruột non', N'Absence, atresia and stenosis of small intestine', 1, N'Q41', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (152, 42, N'257', N'Q35-Q37', N'Sứt môi và hở hàm ếch', N'Cleft lip and cleft palate', 1, N'Q35;Q36;Q37', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (153, 42, N'256', N'Q20-Q28', N'Dị tật bẩm sinh khác của bộ máy tuần hoàn', N'Congenital malfor', 1, N'Q20;Q21;Q22;Q23;Q24;Q25;Q26;Q27;Q28', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (154, 42, N'263', N'Q66', N'Dị dạng bẩm sinh bàn chân', N'Congenital deformities of feet', 1, N'Q66', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (155, 34, N'157', N'I70', N'Xơ vữa độngmạch', N'Atherosclerosis', 1, N'I70', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (156, 34, N'156', N'I65-I69', N'Bệnh mạch máu não khác', N'Other cerebrovascular diseases', 1, N'I65;I66;I67;I68;I69', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (157, 34, N'155', N'I64', N'Tai biến mạch máu não, không xác định rõ chảy máu hoặc do nhồi máu', N'Stroke, not specified as haemorrhage or infarction', 1, N'I64', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (158, 26, N'055', N'B91', N'Di chứng viêm tuỷ xám cấp', N'Sequelae of poliomyelitis', 1, N'B91', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (159, 26, N'054', N'B90', N'Di chứng lao', N'Sequelae of tuberculosis', 1, N'B90', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (160, 26, N'053', N'B68-B71, B75, B77-B83', N'Bệnh giun sán khác', N'Other Helminthiases', 1, N'B68;B69;B70;B71;B75;B77;B78;B79;B80;B81;B82;B83', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (161, 26, N'052', N'B76', N'Giun móc', N'Hookworm diseases', 1, N'B76', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (162, 26, N'051', N'B74', N'Giun chỉ', N'Filariasis', 1, N'B74', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (163, 26, N'050', N'B73', N'Giun onchocerca', N'Onchocerciasis', 1, N'B73', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (164, 26, N'049', N'B72', N'Giun rồng', N'Dracunculiasis', 1, N'B72', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (165, 26, N'048', N'B67', N'Sán Echinococ', N'Echinococcosis', 1, N'B67', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (166, 26, N'047', N'B66', N'Các nhiễm khuẩn do sán lá', N'Other fluke infections', 1, N'B66', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (167, 26, N'046', N'B65', N'Sán máng', N'Schistosomiasis', 1, N'B65', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (168, 26, N'045', N'B56-B57', N'Trypanosomia', N'Trypanosomiasis', 1, N'B56;B57', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (169, 26, N'044', N'B55', N'Leishamania', N'Leishmaniasis', 1, N'B55', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (170, 26, N'043', N'B50-B54', N'Sốt rét', N'Malaria', 1, N'B50;B51;B52;B53;B54', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (171, 26, N'042', N'B35-B49', N'Nấm', N'Mycoses', 1, N'B35;B36;B37;B38;B39;B40;B41;B42;B43;B44;B45;B46;B47;B48;B49', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (172, 26, N'041', N'A81, A87-A89, B03-B04, B07-B09, B25, B27-B34', N'Bệnh virut khác', N'Other viral diseases', 1, N'A81;A87;A88;A89;B03;B04;B07;B08;B09;B25;B27;B28;B29;B30;B31;B32;B33;B34', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (173, 26, N'056', N'B92', N'Di chứng phong', N'Sequelae of leprosy', 1, N'B92', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (174, 26, N'057', N'A65-A67,A69-A70,A74,A77-A79,B58-B64,B85-B89,B94,B99, B95-B98', N'Bệnh nhiễm khuẩn và kí sinh vật khác', N'Other infectious and parasitic diseases', 1, N'A65;A66;A67;A69;A70;A74;A77;A78;A79;B58;B59;B60;B61;B62;B63;B64;B85;B86;B87;B88;B89;B94;B99;B95;B96;B97;B98', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (175, 27, N'058', N'C00-C14', N'U ác môi, khoang miệng, họng', N'Malignant neoplasm of lip, oral cavity and pharynx', 1, N'C00;C01;C02;C03;C04;C05;C06;C07;C08;C09;C10;C11;C12;C13;C14', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (176, 27, N'059', N'C15', N'U ác thực quản', N'Malignant neoplasm of oesophagus', 1, N'C15', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (177, 27, N'075', N'C53', N'U ác cổ tử cung', N'Maligant neoplasm of cervix uterus', 1, N'C53', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (178, 27, N'074', N'C51-C52', N'U ác khác cơ quan sinh dục nữ', N'Malignant neoplasms of female genital organs', 1, N'C51;C52', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (179, 27, N'073', N'C50', N'U ác vú', N'Malignant neoplasm of breast', 1, N'C50', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (180, 27, N'072', N'C45-C49', N'U ác mạc treo và các mô mềm', N'Malignantneoplasms of mesothelial and soft tissue', 1, N'C45;C46;C47;C48;C49', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (181, 27, N'071', N'C44', N'Các u ác khác của da', N'Other malignant neoplasms of skin', 1, N'C44', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (182, 27, N'070', N'C43', N'U ác hắc tố da', N'Malignant melanoma of skin', 1, N'C43', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (183, 27, N'069', N'C40-C41', N'U ác xương và sụn khớp', N'Malignant neoplasms of bone and articular cartilage', 1, N'C40;C41', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (184, 26, N'040', N'B26', N'Quai bị', N'Mumps', 1, N'B26', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (185, 27, N'068', N'C30-C31,C37-C39', N'Các u khác cơ quan hô hấp và lồng ngực', N'Other malignant neplasm of respiratory and intrathoracic', 1, N'C30;C31;C37;C38;C39', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (186, 27, N'066', N'C32', N'U ác thanh quản', N'Malignant neoplasm of larynx', 1, N'C32', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (187, 27, N'065', N'C17,C23-C24, C26', N'Các u khác cơ quan tiêu hoá', N'Other malignant neoplasms of digestive organs', 1, N'C17;C23;C24;C26', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (188, 27, N'064', N'C25', N'U ác tuỵ', N'Malignant neoplasm of pancreas', 1, N'C25', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (189, 27, N'063', N'C22', N'U ác gan và đường mật trong gan', N'Malignant neoplasm of liver and intrahepatic bile ducts', 1, N'C22', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (190, 27, N'062', N'C19-C21', N'U ác chỗ nối trực tràng sigma, trực tràng, hậu môn và ống hậu môn', N'Malignant neoplasm of rectosigmoid function, rectum, anus and anal canal', 1, N'C19;C20;C21', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (191, 27, N'061', N'C18', N'U ác đại tràng', N'Malignant neoplasm of colon', 1, N'C18', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (192, 27, N'060', N'C16', N'U ác dạ dày', N'Malignant neoplasm of stomach.', 1, N'C16', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (193, 27, N'067', N'C33-C34', N'U ác khí quản, phế quản và phổi', N'Malignant neoplasms of trachea, bronchus and lung', 1, N'C33;C34', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (194, 27, N'076', N'C54-C55, C56-C58', N'U ác các phần khác và không xác định của tử cung', N'Malignant neoplasms of other and unspecified parts of uterus', 1, N'C54;C55;C56;C57;C58', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (195, 26, N'039', N'B20-B24', N'Nhiễm HIV', N'Human immuno deficiency virus disease', 1, N'B20;B21;B22;B23;B24', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (196, 26, N'037', N'B16', N'Viêm gan B cấp', N'Acute hepatitis B', 1, N'B16', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (197, 26, N'016', N'A39', N'Nhiễm khuẩn não mô cầu', N'Meningococcal infection', 1, N'A39', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (198, 26, N'015', N'A37', N'Ho gà', N'Whooping cough', 1, N'A37', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (199, 26, N'014', N'A36', N'Bạch hầu', N'Diphtheria', 1, N'A36', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (200, 26, N'013', N'A34-A35', N'Các dạng uốn ván khác', N'Other tetanus', 1, N'A34;A35', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (201, 26, N'012', N'A33', N'Uốn ván sơ sinh', N'Tetanus neonatorum', 1, N'A33', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (202, 26, N'011', N'A30', N'Phong', N'Leprosy', 1, N'A30', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (203, 26, N'010', N'A23', N'Bệnh do Brucella', N'Brucellosis', 1, N'A23', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (204, 26, N'009', N'A20', N'Dịch hạch', N'Plague', 1, N'A20', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (205, 26, N'008', N'A17-A19', N'Các dạng lao khác', N'Other tuberculosis', 1, N'A17;A18;A19', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (206, 26, N'007', N'A15-A16', N'Lao bộ máy hô hấp', N'Respiratory tuberculosis', 1, N'A15;A16', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (207, 26, N'006', N'A02, A04-A05, A07-A08', N'Các bệnh nhiễm khuẩn ruột khác', N'Other intestinal infectious diseases', 1, N'A02;A04;A05;A07;A08', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (208, 26, N'005', N'A09', N'Iả chảy, viêm dạy dày, ruột non có nguồn gốc nhiễm khuẩn', N'Diarrhoea and gastroenteritis of presumed infectious origin.', 1, N'A09', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (209, 26, N'004', N'A06', N'Lỵ Amip', N'Amoebiasis', 1, N'A06', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (210, 26, N'003', N'A03', N'Iả chảy do Shigella', N'Shigellosis', 1, N'A03', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (211, 26, N'002', N'A01', N'Thương hàn, phó thương hàn', N'Typhoid and paratyphoid fevers', 1, N'A01', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (212, 26, N'017', N'A40-A41', N'Nhiễm khuẩn huyết', N'Septicemia', 1, N'A40;A41', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (213, 26, N'018', N'A21-A22, A24-A28, A31-A32, A38, A42-A49', N'Các bệnh do vi khuẩn khác', N'Other bacterial diseases', 1, N'A21;A22;A24;A25;A26;A27;A28;A31;A32;A38;A42;A43;A44;A45;A46;A47;A48;A49', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (214, 26, N'019', N'A50', N'Giang mai bẩm sinh', N'Congenital syphilis', 1, N'A50', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (215, 26, N'020', N'A51', N'Giang mai sớm (Giang mai I, II và kín)', N'Early syphilis', 1, N'A51', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (216, 26, N'036', N'B06', N'Rubêon', N'Rubella', 1, N'B06', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (217, 26, N'035', N'B05', N'Sởi', N'Measles', 1, N'B05', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (218, 26, N'034', N'B01-B02', N'Thuỷ đậu và zôna', N'Varicella and Zoster', 1, N'B01;B02', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (219, 26, N'033', N'B00', N'Nhiễm virut Héc', N'pét', 1, N'B00', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (220, 26, N'032', N'A90-A94,A96-A99', N'Sốt virut khác do tiết túc truyền và sốt virus xuất huyết', N'Other arthropod', 1, N'A90;A91;A92;A93;A94;A96;A97;A98;A99', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (221, 26, N'031', N'A95', N'Sốt vàng', N'Yellow fever', 1, N'A95', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (222, 26, N'030', N'A83-A86', N'Viêm não Virut', N'Viral encephalitis', 1, N'A83;A84;A85;A86', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (223, 26, N'038', N'B15, B17-B19', N'Viêm gan virut khác', N'Other viral hepatitis', 1, N'B15;B17;B18;B19', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (224, 26, N'029', N'A82', N'Dại', N'Rabies', 1, N'A82', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (225, 26, N'027', N'A75', N'Sốt Rickettsia', N'Typhus fever', 1, N'A75', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (226, 26, N'026', N'A71', N'Mắt hột', N'Trachoma', 1, N'A71', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (227, 26, N'025', N'A68', N'Sốt hồi quy', N'Relapsing fever', 1, N'A68', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (228, 26, N'024', N'A57-A64', N'Nhiễm khuẩn khác lây truyền qua đường tình dục', N'Other infection with a predominantly sexual mode of transmission', 1, N'A57;A58;A59;A60;A61;A62;A63;A64', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (229, 26, N'023', N'A55-A56', N'Nhiễm khuẩn Chlamydia lây truyền qua đường tình dục', N'Sexually transmitted chlamydial diseases', 1, N'A55;A56', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (230, 26, N'022', N'A54', N'Nhiễm lậu cầu khuẩn', N'Gonococcal infection', 1, N'A54', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (231, 26, N'021', N'A52-A53', N'Các loại giang mai khác', N'Other syphilis', 1, N'A52;A53', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (232, 26, N'028', N'A80', N'Bại liệt cấp', N'Acute poliomyelitis', 1, N'A80', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (233, 27, N'077', N'C61', N'U tiền liệt tuyến', N'Neoplasm neoplasm of prostate', 1, N'C61', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (234, 27, N'078', N'C60,C62-C63', N'U ác khác của cơ quan sinh dục nam', N'Other malignant neoplasms of male genital organs', 1, N'C60;C62;C63', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (235, 27, N'079', N'C67', N'U ác bàng quang', N'Malignant neoplasm of bladder', 1, N'C67', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (236, 32, N'134', N'H33', N'Bong và rách võng mạc', N'Retinal detachments and breaks', 1, N'H33', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (237, 32, N'133', N'H25-H28', N'Đục thể thuỷ tinh, tổn thương khác của thể thuỷ tinh', N'Cataract and other disorders of lens', 1, N'H25;H26;H27;H28', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (238, 32, N'132', N'H15-H19', N'Viêm giác mạc, tổn thương khác của củng mạc và giác mạc Keratitis and other disorders of sclera and cornea.', N'', 1, N'H15;H16;H17;H18;H19', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (239, 32, N'131', N'H10-H13', N'Viêm kết mạc ,tổn thương khác của kết mạc', N'Conjunctivitis and other disoders of conjunctiva', 1, N'H10;H11;H12;H13', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (240, 32, N'130', N'H00-H01', N'Viêm mi mắt', N'Inflammation of eyelid', 1, N'H00;H01', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (241, 31, N'129', N'G10-G13, G14, G21-G26,G31-G32,G36-G37, G46-G47, G60-G73, G90-G99', N'Bệnh khác của hệ thần kinh', N'Other diseases of the nervous system', 1, N'G10;G11;G12;G13;G14;G21;G22;G23;G24;G25;G26;G31;G32;G36;G37;G46;G47;G60;G61;G62;G63;G64;G65;G66;G67;G68;G69;G70;G71;G72;G73;G90;G91;G92;G93;G94;G95;G96;G97;G98;G99', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (242, 31, N'128', N'G80-G83', N'Liệt não, hội chứng liệt khác', N'Cerebral palsy and other paralytic syndromes', 1, N'G80;G81;G82;G83', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (243, 31, N'127', N'G50-G59', N'Tổn thương thần kinh, rễ và đám rối thần kinh', N'Nerve, nerve root and plexus disorders', 1, N'G50;G51;G52;G53;G54;G55;G56;G57;G58;G59', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (244, 31, N'126', N'G45', N'Cơn thiếu máu não thoáng qua và các hội chứng tương tự', N'Transient cerebral ischaemic attacks and related syndromes', 1, N'G45', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (245, 31, N'125', N'G43-G44', N'Đau nửa đầu và các hội chứng đau đầu khác', N'Migraine and other headache syndromes.', 1, N'G43;G44', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (246, 31, N'124', N'G40- G41', N'Động kinh', N'Epilepsy', 1, N'G40;G41', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (247, 31, N'123', N'G35', N'Xơ cứng nhiều nơi', N'Multiple sclerosis', 1, N'G35', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (248, 31, N'122', N'G30', N'Alzheimer', N'Alzheimer’s disease', 1, N'G30', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (249, 31, N'121', N'G20', N'Parkinson', N'Parkinson’s disease', 1, N'G20', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (250, 31, N'120', N'G00-G09', N'Viêm hệ thần kinh trung ương', N'Inflamatory diseases of the central nervous system', 1, N'G00;G01;G02;G03;G04;G05;G06;G07;G08;G09', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (251, 32, N'135', N'H40-H42', N'Glôcôm', N'Glaucoma', 1, N'H40;H41;H42', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (252, 32, N'136', N'H49-H50', N'Lác mắt', N'Strabismus', 1, N'H49;H50', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (253, 32, N'137', N'H52', N'Tật khúc xạ, các rối loạn điều tiết', N'Disorders of refraction and accomodation', 1, N'H52', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (254, 32, N'138', N'H54', N'Mù loà và giảm thị lực', N'Blindness and low vision', 1, N'H54', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (255, 34, N'154', N'I63', N'Nhồi máu não', N'Cerebral infarction', 1, N'I63', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (256, 34, N'153', N'I60-I62', N'Chảy máu não', N'Intracerebral haemorrhage', 1, N'I60;I61;I62', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (257, 34, N'152', N'I27- I43, I51-I52', N'Bệnh tim khác', N'Other heart diseases', 1, N'I27;I28;I29;I30;I31;I32;I33;I34;I35;I36;I37;I38;I39;I40;I41;I42;I43;I51;I52', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (258, 34, N'151', N'I50', N'Suy tim', N'Heart failure', 1, N'I50', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (259, 34, N'150', N'I44-I49', N'Rối loạn dẫn truyền và loạn nhịp tim', N'Conduction disorders and cardiac arrhythymias', 1, N'I44;I45;I46;I47;I48;I49', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (260, 34, N'149', N'I26', N'Tắc động mạch phổi', N'Pulmonary embolism', 1, N'I26', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (261, 34, N'148', N'I20, I23-I25', N'Bệnh tim thiếu máu cục bộ khác', N'Other ischaemic heart diseases', 1, N'I20;I23;I24;I25', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (262, 30, N'119', N'F04-F09 ,F50-F69, F80-F99', N'Rối loạn tâm thần và nhân cách khác', N'Other mental and behavioural disorders', 1, N'F04;F05;F06;F07;F08;F09;F50;F51;F52;F53;F54;F55;F56;F57;F58;F59;F60;F61;F62;F63;F64;F65;F66;F67;F68;F69;F80;F81;F82;F83;F84;F85;F86;F87;F88;F89;F90;F91;F92;F93;F94;F95;F96;F97;F98;F99', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (263, 34, N'147', N'I21-I22', N'Nhồi máu cơ tim', N'Acute myocardial infarction', 1, N'I21;I22', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (264, 34, N'145', N'I10', N'Tăng huyết áp nguyên phát', N'Essential (primary) hypertension', 1, N'I10', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (265, 34, N'144', N'I05-I09', N'Bệnh thấp tim mãn', N'Chronic rheumatic disease', 1, N'I05;I06;I07;I08;I09', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (266, 34, N'143', N'I00-I02', N'Thấp khớp cấp', N'Acute rheumatic heart disease', 1, N'I00;I01;I02', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (267, 33, N'142', N'H60-H62, H80-H83, H92-H95', N'Bệnh khác của tai và xương chũm', N'Other diseases of the ear and mastoid process', 1, N'H60;H61;H62;H80;H81;H82;H83;H92;H93;H94;H95', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (268, 33, N'141', N'H90-H91', N'Mất thính giác', N'Hearing loss', 1, N'H90;H91', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (269, 33, N'140', N'H65-H75', N'Viêm tai giữa , bệnh khác của tai giữa và xương chũm', N'Otitis media and other disorders of middle ear and mastoid', 1, N'H65;H66;H67;H68;H69;H70;H71;H72;H73;H74;H75', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (270, 32, N'139', N'H30-H32, H02-H22, H34-H36,H43-H48,H51,H53-H55,H57-H58,H59', N'Các bệnh khác của mắt và phần phụ mắt', N'Other diseases of the eye and adnexa', 1, N'H30;H31;H32;H02;H03;H04;H05;H06;H07;H08;H09;H14;H20;H21;H22;H34;H35;H36;H43;H44;H45;H46;H47;H48;H51;H53;H55;H57;H58;H59', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (271, 34, N'146', N'I11-I15', N'Bệnh tăng huyết áp khác', N'Other hypertensive diseases', 1, N'I11;I12;I13;I14;I15', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (272, 30, N'118', N'F70- F79', N'Chậm phát triển tâm thần', N'Mental retardation', 1, N'F70;F71;F72;F73;F74;F75;F76;F77;F78;F79', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (273, 30, N'117', N'F40- F48', N'Loạn thần kinh, rối loạn gắn liền với các yếu tố stress và các rối loạn thuộc thân thể', N'Neurotic, sterss', 1, N'F40;F41;F42;F43;F44;F45;F46;F47;F48', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (274, 30, N'116', N'F30- F39', N'Rối loạn khí sắc', N'Mood ( affective) disorders.', 1, N'F30;F31;F32;F33;F34;F35;F36;F37;F38;F39', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (275, 27, N'095', N'D33', N'U lành não và các phần khác của hệ thần kinh trung ương', N'Benign neoplasm of brain and other parts of central nervous system', 1, N'D33', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (276, 27, N'094', N'D30', N'U lành cơ quan tiết niệu', N'Benign neoplasm of urinary organs', 1, N'D30', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (277, 27, N'093', N'D27', N'U buồng trứng lành', N'Benign neoplasm of ovary', 1, N'D27', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (278, 27, N'092', N'D25', N'U cơ trơn tử cung', N'Leiomyoma of uterus', 1, N'D25', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (279, 27, N'091', N'D24', N'U vú lành', N'Benign neoplasm of breast', 1, N'D24', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (280, 27, N'090', N'D22-D23', N'U da lành', N'Benign neoplasm of skin', 1, N'D22;D23', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (281, 27, N'089', N'D06', N'Caxinom cổ tử cung', N'Carcinoma insitu of cervix uterus', 1, N'D06', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (282, 27, N'096', N'D00-D05, D07-D21,D26, D28-D29, D31-D32, D34-D48', N'U khác insitu, lành tính và các u tiến triển không chắc chắn hoặc chưa rõ', N'Other insitus and benign neoplasms and neoplasms of uncertain or unknown behaviour.', 1, N'D00;D01;D02;D03;D04;D05;D07;D08;D09;D10;D11;D12;D13;D14;D15;D16;D17;D18;D19;D20;D21;D26;D28;D29;D31;D32;D34;D35;D36;D37;D38;D39;D40;D41;D42;D43;D44;D45;D46;D47;D48', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (283, 27, N'088', N'C88-C89, C96, C86, C90', N'U ác limphô khác, cơ quan tạo máu và tổ chức có liên quan', N'Other malignant neoplasms of lymphoid, haematopoietic and related tissue', 1, N'C88;C89;C96;C86;C90', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (284, 27, N'086', N'C82-C85', N'U bạch huyết không phải Hodgkin Non', N'Hodgkin’s disease', 1, N'C82;C83;C84;C85', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (285, 27, N'085', N'C81', N'Bệnh Hodgkin', N'Hodgkin''s disease', 1, N'C81', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (286, 27, N'084', N'C73-C80,C97', N'U ác các khu trú khác, khó định nghĩa, thứ phát, không xác định rõ và phức hợp', N'Malignant neoplasm of other and ill', 1, N'C73;C74;C75;C76;C77;C78;C79;C80;C97', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (287, 27, N'083', N'C70,C72', N'U ác các phần khác của hệ thần kinh trung ương', N'Malignant neoplasm of other parts of central nervous system', 1, N'C70;C72', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (288, 27, N'082', N'C71', N'U ác não', N'Malignant neoplasm of brain', 1, N'C71', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (289, 27, N'081', N'C69', N'U ác mắt và các phần phụ', N'Malignant neoplasm of eye and adnexa', 1, N'C69', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (290, 27, N'080', N'C64-C66,C68', N'U ác khác của đường tiết niệu', N'Other malignant neoplasms of uterinary tract', 1, N'C64;C65;C66;C68', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (291, 27, N'087', N'C91-C95', N'Bệnh bạch cầu', N'Leukaemia', 1, N'C91;C92;C93;C94;C95', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (292, 47, N'311', N'Z12, Z40- Z54', N'Bệnh do tiếp xúc với dịch vụ y tế phải chăm sóc và khám xét đặc biệt', N'Persons encountering health services for specific procedures and health care', 1, N'Z12;Z40;Z41;Z42;Z43;Z44;Z45;Z46;Z47;Z48;Z49;Z50;Z51;Z52;Z53;Z54', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (293, 28, N'097', N'D50', N'Thiếu máu do thiếu sắt', N'Iron deficiency anaemia', 1, N'D50', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (294, 28, N'099', N'D65-D77', N'Tổn thương chảy máu, bệnh khác của máu và cơ quan tạo máu', N'Haemorrhagic conditions and other diseases of blood, blood', 1, N'D65;D66;D67;D68;D69;D70;D71;D72;D73;D74;D75;D76;D77', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (295, 30, N'115', N'F20- F29', N'Tâm thần phân liệt, rối loạn dạng phân liệt và hoang tưởng', N'Schizophrenia, schiztypal and delusional disorders', 1, N'F20;F21;F22;F23;F24;F25;F26;F27;F28;F29', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (296, 30, N'114', N'F11- F19', N'Rối loạn tâm thần và ứng xử liên quan dùng các chất kích thích tâm lí khác', N'Mental and behavioural disorders due to other psychoactive substances use', 1, N'F11;F12;F13;F14;F15;F16;F17;F18;F19', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (297, 30, N'113', N'F10', N'Rối loạn tâm thần và ứng xử liên quan uống rượu', N'Mental and behavioural disorders due to use of alcohol', 1, N'F10', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (298, 30, N'112', N'F00- F03', N'Sa sút trí tuệ', N'Dementia', 1, N'F00;F01;F02;F03', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (299, 29, N'111', N'E15-E35,E58-E63,E65-E67, E70-E78, E85-E87, E90, E68, E79-E80, E83-E84, E88-E89', N'Bệnh khác về nội tiết, dinh dưỡng và chuyển hoá', N'Other endocrine, nutritional and metabolic disorders', 1, N'E15;E16;E17;E18;E19;E20;E21;E22;E23;E24;E25;E26;E27;E28;E29;E30;E31;E32;E33;E34;E35;E58;E59;E60;E61;E62;E63;E65;E67;E70;E71;E72;E73;E74;E75;E76;E77;E78;E85;E87;E90;E68;E79;E80;E83;E84;E88;E89', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (300, 29, N'110', N'E86', N'Giảm lượng máu', N'Volume depletion', 1, N'E86', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (301, 29, N'109', N'E66', N'Béo phì', N'Obesity', 1, N'E66', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (302, 28, N'098', N'D51-D64', N'Thiếu máu khác', N'Other anaemias', 1, N'D51;D52;D53;D54;D55;D56;D57;D58;D59;D60;D61;D62;D63;D64', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (303, 29, N'108', N'E64', N'hậu quả của suy dinh dưỡng và thiếu chất dinh dưỡng khác', N'Sequalae of malnutrition and other nutritional deficiencies', 1, N'E64', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (304, 29, N'106', N'E50', N'Thiếu Vitamin A', N'Vitamin A deficiency', 1, N'E50', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (305, 29, N'105', N'E40-E46', N'Suy dinh dưỡng', N'Malnutrition', 1, N'E40;E41;E42;E43;E44;E45;E46', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (306, 29, N'104', N'E10-E14', N'Đái tháo đường', N'Diabetes mellitus.', 1, N'E10;E11;E12;E13;E14', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (307, 29, N'103', N'E03-E04, E06-E07', N'Tổn thương khác của tuyến giáp', N'Other disorders of thyroid.', 1, N'E03;E04;E06;E07', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (308, 29, N'102', N'E05', N'Nhiễm độc do tuyến giáp (cường giáp)', N'Thyrotoxicosis', 1, N'E05', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (309, 29, N'101', N'E00-E02', N'Tổn thương tuyến giáp liên quan đến thiếu iod', N'Iodine deficiency', 1, N'E00;E01;E02', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (310, 28, N'100', N'D80-D89', N'Một số rối loạn hệ miễn dịch', N'Certain disorders involving the immune mechanism', 1, N'D80;D81;D82;D83;D84;D85;D86;D87;D88;D89', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (311, 29, N'107', N'E51-E56', N'Thiếu vitamin khác', N'Other vitamin deficiencies.', 1, N'E51;E52;E53;E54;E55;E56', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
INSERT [dbo].[NhomICDTheoBenhVien] ([Id], [ChuongICDId], [Stt], [Ma], [TenTiengViet], [TenTiengAnh], [HieuLuc], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (312, 47, N'312', N'Z02-Z04, Z08-Z11, Z13, Z31- Z33, Z37,Z55-Z99', N'Bệnh do tiếp xúc với dịch vụ y tế vì những lý do khác', N'Person encoutering health services for other reasons', 1, N'Z02;Z03;Z04;Z08;Z09;Z10;Z11;Z13;Z31;Z32;Z33;Z37;Z55;Z56;Z57;Z58;Z59;Z60;Z61;Z62;Z63;Z64;Z65;Z66;Z67;Z68;Z69;Z70;Z71;Z72;Z73;Z74;Z75;Z76;Z77;Z78;Z79;Z80;Z81;Z82;Z83;Z84;Z85;Z86;Z87;Z88;Z89;Z90;Z91;Z92;Z93;Z94;Z95;Z96;Z97;Z98;Z99', 1, 1, CAST(N'2022-01-20T13:36:38.030' AS DateTime), CAST(N'2022-01-20T13:36:38.030' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[NhomICDTheoBenhVien] OFF
GO

Update dbo.CauHinh
Set [Value] = '3.4.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'

