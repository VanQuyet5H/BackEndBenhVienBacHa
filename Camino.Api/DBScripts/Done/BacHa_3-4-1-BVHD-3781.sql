
INSERT INTO [dbo].[CauHinh]([Name],[DataType],[Description],[Value],[CreatedById],[LastUserId],[LastTime],[CreatedOn])
     VALUES (N'CauHinhCDHA.CauHinhInKyThuat',3,N'In nhóm dịch vụ bệnh viện chọn sẳn cho CĐHA',N'[{"NhomDichVuBenhVienId":"227","Ten":"Khoa GMHS"},{"NhomDichVuBenhVienId":"226","Ten":"XQUANG THƯỜNG"},{"NhomDichVuBenhVienId":"230","Ten":"CTSCANNER"},{"NhomDichVuBenhVienId":"231","Ten":"MRI"}]',1,1,GETDATE(),GETDATE())
GO

Update dbo.CauHinh
Set [Value] = '3.4.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'


