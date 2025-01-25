INSERT INTO [dbo].[CauHinh]
           ([Name]
           ,[DataType]
           ,[Description]
           ,[Value]
           ,[CreatedById]
           ,[LastUserId]
           ,[LastTime]
           ,[CreatedOn])
     VALUES
           ('CauHinhCDHA.CauHinhIn'
           ,3
           ,N'Cấu hình in'
           ,'[{"DichVuKyThuatBenhVienId":98,"CauHinhIn":{"InLogo":false,"InBarcode":false,"InTieuDe":true,"InHoVaTen":true,"InNamSinh":true,"InGioiTinh":true,"InDiaChi":true,"InBSChiDinh":false,"InNgayChiDinh":false,"InNoiChiDinh":false,"InSoBenhAn":false,"InChuanDoan":false,"InDienGiai":false,"InChiDinh":false,"InThanhNgang":false,"InKyThuat":false}}]'
           ,1
           ,1
           ,getdate()
           ,getdate())

Update dbo.CauHinh
Set [Value] = '2.6.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'
