
INSERT INTO [dbo].[CauHinh]([Name],[DataType],[Description],[Value],[CreatedById],[LastUserId],[LastTime],[CreatedOn])
     VALUES (N'CauHinhNoiTru.DanhSachKhoaKhongRaVien',3,N'Danh sách khoa không cho ra viện',N'[{"KhoaId":"2","Ten":"Khoa GMHS"}]',1,1,GETDATE(),GETDATE())
GO

Update dbo.CauHinh
Set [Value] = '3.4.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'

