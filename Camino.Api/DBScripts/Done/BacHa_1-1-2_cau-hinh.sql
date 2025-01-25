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
           (N'CauHinhGachNo.TaiKhoan',10,N'Tài khoản',N'[{"KeyId":"11201","DisplayName":"11201","DataType":3,"GhiChu":"Tiền Việt Nam"},{"KeyId":"11202","DisplayName":"11202","DataType":3,"GhiChu":"Tiền USD"},{"KeyId":"1111","DisplayName":"1111","DataType":3,"GhiChu":"Tiền mặt"}]',1,1,GETDATE(),GETDATE()),
		   (N'CauHinhGachNo.SoTaiKhoanNganHang',10,N'Số tài khoản ngân hàng',N'[{"KeyId":"01864874846548","DisplayName":"01864874846548","DataType":3,"GhiChu":""},{"KeyId":"043524874846548","DisplayName":"043524874846548","DataType":3,"GhiChu":""}]',1,1,GETDATE(),GETDATE()),
		   (N'CauHinhGachNo.LoaiThuChi',10,N'Loại thu chi',N'[{"KeyId":"Thu tiền viện phí BN","DisplayName":"Thu tiền viện phí BN","DataType":3,"GhiChu":""},{"KeyId":"Thu tiền bán thuốc","DisplayName":"Thu tiền bán thuốc","DataType":3,"GhiChu":""}]',1,1,GETDATE(),GETDATE()),
		   (N'CauHinhGachNo.TyGiaUSD',4,N'Tỷ giá USD',N'23270',1,1,GETDATE(),GETDATE())
GO
Update CauHinh
Set [Value] = '1.1.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'