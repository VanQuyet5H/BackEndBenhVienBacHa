
declare @nhomChaId BIGINT = (SELECT TOP 1 Id FROM NhomDichVuBenhVien WHERE Ma = N'PTTT')

INSERT INTO [dbo].[NhomDichVuBenhVien]
           ([Ma],[Ten],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn],[NhomDichVuBenhVienChaId])
     VALUES (IDENT_CURRENT(N'NhomDichVuBenhVien') + 1,N'PHẪU THUẬT KHÁC',1,1,1,GETDATE(),GETDATE(),@nhomChaId)

INSERT INTO [dbo].[NhomDichVuBenhVien]
           ([Ma],[Ten],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn],[NhomDichVuBenhVienChaId])
	VALUES (IDENT_CURRENT(N'NhomDichVuBenhVien') + 1,N'THỦ THUẬT DA LIỄU',1,1,1,GETDATE(),GETDATE(),@nhomChaId)

INSERT INTO [dbo].[NhomDichVuBenhVien]
           ([Ma],[Ten],[IsDefault],[CreatedById],[LastUserId],[LastTime],[CreatedOn],[NhomDichVuBenhVienChaId])
	VALUES (IDENT_CURRENT(N'NhomDichVuBenhVien') + 1,N'THỦ THUẬT Y HỌC DÂN TỘC- PHỤC HỒI CHỨC NĂNG',1,1,1,GETDATE(),GETDATE(),@nhomChaId)
GO

Update dbo.CauHinh
Set [Value] = '0.5.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'