begin tran
   update [CauHinh] with (serializable) set [Value] = N'true'
   where [Name] = N'CauHinhChung.UuTienXuatKhoTheoHanSuDung'

   if @@rowcount = 0
   begin
	INSERT [CauHinh] ([Name], [DataType], [Description], [Value], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) 
	VALUES (N'CauHinhChung.UuTienXuatKhoTheoHanSuDung', 1, N'Xuất kho theo hạn sử dụng', N'true', 1, 1, '2022-02-17', '2022-02-17')
   end
commit tran
GO 

Update dbo.CauHinh
Set [Value] = '3.5.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'
