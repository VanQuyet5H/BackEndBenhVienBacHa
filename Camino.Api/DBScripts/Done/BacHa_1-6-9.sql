
INSERT [dbo].[CauHinh] ([Name], [DataType], [Description], [Value], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) 
VALUES (N'CauHinhNoiTru.ThoiGianMacDinhTrongNgay', 10, N'Thời gian mặc định trong ngày', N'[{"KeyId":"1","DisplayName":"Sáng","Value":"25200","GhiChu":"Sáng","DataType":6,"IsDisabled":false},{"KeyId":"2","DisplayName":"Trưa","Value":"39600","GhiChu":"Trưa","DataType":6,"IsDisabled":false},{"KeyId":"3","DisplayName":"Chiều","Value":"61200","GhiChu":"Chiều","DataType":6,"IsDisabled":false},{"KeyId":"4","DisplayName":"Tối","Value":"75600","GhiChu":"Tối","DataType":6,"IsDisabled":false}]', 1, 1, CAST(N'2020-11-10T15:28:51.597' AS DateTime), CAST(N'2020-11-10T15:28:51.597' AS DateTime))

GO
Update CauHinh
Set [Value] = '1.6.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'