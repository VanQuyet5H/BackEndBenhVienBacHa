INSERT INTO [dbo].[InputStringStored]
           ([Set]
           ,[Value]
           ,[CreatedById]
           ,[LastUserId]
           ,[LastTime]
           ,[CreatedOn])
     VALUES
           (3,N'Chụp CTscanner sọ não độ dày lớp cắt 3 mm dưới lều, 8mm trên lều, không tiêm thuốc cản quang.' ,1,1,GETDATE(),GETDATE()),
		   (3,N'Chụp CTscanner hệ thống xoang hàm mặt độ dày lớp cắt 3 mm, theo mặt phẳng axial và coronal, không tiêm thuốc cản quang.' ,1,1,GETDATE(),GETDATE()),
		   (3,N'Chụp CTscanner lồng ngực độ dày lớp cắt 6 mm, không  tiêm thuốc cản quang.' ,1,1,GETDATE(),GETDATE()),
		   (3,N'Chụp CTscanner ổ bụng độ dày lớp cắt 6 mm, trước và sau tiêm thuốc cản quang.' ,1,1,GETDATE(),GETDATE()),
		   (3,N'Chụp CT khớp gối, độ dày 3mm theo mặt phẳng Axial, tái tạo theo mặt phẳng Coronal, không tiêm thuốc cản quang.' ,1,1,GETDATE(),GETDATE()),
		   (3,N'Chụp CTscanner xương đá, độ dày lớp cắt 0,75 mm, theo mặt phẳng axial và coronal, không tiêm thuốc cản quang.' ,1,1,GETDATE(),GETDATE())
GO

ALTER TABLE YeuCauDichVuKyThuat
ADD MayTraKetQuaId BIGINT NULL
GO

Update dbo.CauHinh
Set [Value] = '1.4.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'