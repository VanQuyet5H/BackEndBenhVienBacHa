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
	       (N'CauHinhNoiTru.PhieuKhaiThacTienSuDiUng',3,N'Phiếu khai thác tiền sử dị ứng',N'[{"STT":"0","NoiDung":"Loại thuốc hoặc dị nguyên nào đã gây dị ứng?","TenThuoc":"","DiNguyenGayDiUng":"", "CoKhong":false,"SoLan":"0","BieuHienLamSang":"","XuTri":""},  {"STT":"1","NoiDung":"Dị ứng với loại côn trùng nào?","TenThuoc":"","DiNguyenGayDiUng":"", "CoKhong":false,"SoLan":"0","BieuHienLamSang":"","XuTri":""},  {"STT":"2","NoiDung":"Dị ứng với loại thực phẩm nào?","TenThuoc":"","DiNguyenGayDiUng":"", "CoKhong":false,"SoLan":"0","BieuHienLamSang":"","XuTri":""},  {"STT":"3","NoiDung":"Dị ứng với các tác nhân khác: phấn hoa, bụi nhà,hóa chất, mỹ phẩm...?","TenThuoc":"","DiNguyenGayDiUng":"", "CoKhong":false,"SoLan":"0","BieuHienLamSang":"","XuTri":""},  {"STT":"4","NoiDung":"Tiền sử cá nhân có bệnh dị ứng nào?(viêm mủi dị ứng, hen phế quản ... )","TenThuoc":"","DiNguyenGayDiUng":"", "CoKhong":false,"SoLan":"0","BieuHienLamSang":"","XuTri":""},  {"STT":"5","NoiDung":"Tiền sử gia đình có bệnh dự ứng?(Bố mẹ, con, anh chị em ruột, có ai bị các bệnh di ứng trên không)","TenThuoc":"","DiNguyenGayDiUng":"", "CoKhong":false,"SoLan":"0","BieuHienLamSang":"","XuTri":""}]',1,1,GETDATE(),GETDATE())


          
GO

UPDATE CauHinh
Set [Value] = '2.0.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'

