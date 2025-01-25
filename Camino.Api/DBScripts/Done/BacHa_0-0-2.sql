USE [BvBacHa]
GO
TRUNCATE TABLE TemplateKhamBenhTheoKhoa
GO
EXEC sp_rename 'TemplateKhamBenhTheoKhoa', 'TemplateKhamBenhTheoDichVu'; 
GO
EXEC sp_rename 'TemplateKhamBenhTheoDichVu.KhoaPhongId', 'DichVuKhamBenhBenhVienId', 'COLUMN';
GO
ALTER TABLE TemplateKhamBenhTheoDichVu
ALTER COLUMN DichVuKhamBenhBenhVienId BIGINT not null
GO
ALTER TABLE TemplateKhamBenhTheoDichVu
ADD FOREIGN KEY (DichVuKhamBenhBenhVienId) REFERENCES DichVuKhamBenhBenhVien(Id);
GO
SET IDENTITY_INSERT [dbo].[TemplateKhamBenhTheoDichVu] ON 
GO
INSERT [dbo].[TemplateKhamBenhTheoDichVu] ([Id], [DichVuKhamBenhBenhVienId], [Ten], [TieuDe], [ComponentDynamics], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (1, 7, N'Khám Tai - Mũi - Họng', N'Khám Tai - Mũi - Họng', N'{"ComponentDynamics":[{"Type":1,"Id":"Tai","Label":"Tai","Value":null,"fxFlex":"100%","fxFlexSm":"100%"},
					  {"Type":1,"Id":"Mui","Label":"Mũi","Value":null,"fxFlex":"100%","fxFlexSm":"100%"},
					  {"Type":1,"Id":"Vom","Label":"Vòm","Value":null,"fxFlex":"100%","fxFlexSm":"100%"},
					  {"Type":1,"Id":"Hong","Label":"Họng","Value":null,"fxFlex":"100%","fxFlexSm":"100%"},
					  {"Type":1,"Id":"ThanhQuan","Label":"Thanh quản","Value":null,"fxFlex":"100%","fxFlexSm":"100%"}]}', 1, 1, CAST(N'2020-03-03T00:00:00.000' AS DateTime), CAST(N'2020-03-03T00:00:00.000' AS DateTime))
GO
INSERT [dbo].[TemplateKhamBenhTheoDichVu] ([Id], [DichVuKhamBenhBenhVienId], [Ten], [TieuDe], [ComponentDynamics], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (2, 6, N'Khám Răng - Hàm - Mặt', N'Khám Răng - Hàm - Mặt', N'{"ComponentDynamics":[{"Type":1,"Id":"KhamToanThan","Label":"Khoám toàn thân","Value":null,"fxFlex":"100%","fxFlexSm":"100%"},
					  {"Type":1,"Id":"Rang","Label":"Răng","Value":null,"fxFlex":"100%","fxFlexSm":"100%"},
					  {"Type":1,"Id":"Ham","Label":"Hàm","Value":null,"fxFlex":"100%","fxFlexSm":"100%"},
					  {"Type":1,"Id":"Mat","Label":"Mặt","Value":null,"fxFlex":"100%","fxFlexSm":"100%"}]}', 1, 1, CAST(N'2020-03-03T00:00:00.000' AS DateTime), CAST(N'2020-03-03T00:00:00.000' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[TemplateKhamBenhTheoDichVu] OFF
GO

Update CauHinh
Set [Value] = '0.0.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'