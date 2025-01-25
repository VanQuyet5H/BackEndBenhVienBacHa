

ALTER TABLE YeuCauTiepNhan
ADD BieuHienLamSang [nvarchar] (Max) NULL
GO
ALTER TABLE YeuCauTiepNhan
ADD DichTeSarsCoV2 [nvarchar] (Max) NULL
GO

ALTER TABLE YeuCauDichVuKyThuat
ADD LoaiKitThu NVARCHAR(250) NULL
GO

INSERT INTO [dbo].[CauHinh]([Name],[DataType],[Description],[Value],[CreatedById],[LastUserId],[LastTime],[CreatedOn])
     VALUES (N'CauHinhTiepNhan.DichVuTestSarsCovid',3,N'SARS-CoV',N'[{"DichVuKyThuatBenhVienId":"3887","TenDichVu":"SARS-CoV-2 Ag (test nhanh)"}]',1,1,GETDATE(),GETDATE())
GO

INSERT INTO [dbo].[CauHinh]([Name],[DataType],[Description],[Value],[CreatedById],[LastUserId],[LastTime],[CreatedOn])
     VALUES (N'CauHinhTiepNhan.LoaiKitTestSarsCovid',3,N'Loại kit test SARS-CoV',N'[{"LoaiKitThuId":"1","LoaiKitThu":"Flowflex SARSCoV-2 Antigen Rapid Test"},
{"LoaiKitThuId":"2","LoaiKitThu":"Trueline Covid-19 Ag Rapid Test"},
{"LoaiKitThuId":"3","LoaiKitThu":"Biosynex Covid-19 Ag BSS"},
{"LoaiKitThuId":"4","LoaiKitThu":"V Trust Covid-19 Antigen Rapid Test"},
{"LoaiKitThuId":"5","LoaiKitThu":"CareStart Covid-19 Antigen"},
{"LoaiKitThuId":"6","LoaiKitThu":"Espline SARS-CoV-2"},
{"LoaiKitThuId":"7","LoaiKitThu":"GenBody Covid-19 Ag"},
{"LoaiKitThuId":"8","LoaiKitThu":"Humasis Covid-19 Ag Test"},
{"LoaiKitThuId":"9","LoaiKitThu":"Asan Easy Test Covid-19 Ag"},
{"LoaiKitThuId":"10","LoaiKitThu":"SARSCoV-2 Rapid Antigen Test"},
{"LoaiKitThuId":"11","LoaiKitThu":"Panbio Covid-19 Ag Rapid Test Device"},
{"LoaiKitThuId":"12","LoaiKitThu":"Panbio™ Covid-19 Ag Rapid Test Device (Nasal)"},
{"LoaiKitThuId":"13","LoaiKitThu":"Standard Q Covid-19 Ag)"},
{"LoaiKitThuId":"14","LoaiKitThu":"SGTi-flex Covid-19 Ag"},
{"LoaiKitThuId":"15","LoaiKitThu":"BioCredit Covid-19 Ag"},
{"LoaiKitThuId":"16","LoaiKitThu":"COVID-19 Ag"}]',1,1,GETDATE(),GETDATE())
GO

Update dbo.CauHinh
Set [Value] = '3.3.6' where [Name] = 'CauHinhHeThong.DatabaseVesion'
