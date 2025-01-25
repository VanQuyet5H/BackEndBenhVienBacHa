update [dbo].[LocaleStringResource] set [ResourceValue] = N'Có dịch vụ hưởng BHYT đã thu tiền, cần hủy phiếu thu để thực hiện cập nhật.' where [ResourceName] = N'YeuCauTiepNhanBase.CapNhatBHYTSauKhiThuTien'
GO
INSERT [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [Language], [CreatedById], [LastUserId], [LastTime], [CreatedOn])
VALUES (N'YeuCauTiepNhanBase.CapNhatBHYTSauKeDonThuoc', N'Bệnh nhân có đơn thuốc BHYT, cần hủy đơn thuốc BHYT để thực hiện cập nhật.', 1, 1, 1, CAST(N'2021-06-17T00:00:00.000' AS DateTime), CAST(N'2021-06-17T00:00:00.000' AS DateTime))
GO
INSERT [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [Language], [CreatedById], [LastUserId], [LastTime], [CreatedOn])
VALUES (N'YeuCauTiepNhanBase.CapNhatBHYTSauKhiYeuCauDuocPham', N'Bệnh nhân đã được kê dược phẩm BHYT, cần hủy dược phẩm BHYT để thực hiện cập nhật.', 1, 1, 1, CAST(N'2021-06-17T00:00:00.000' AS DateTime), CAST(N'2021-06-17T00:00:00.000' AS DateTime))
GO
INSERT [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [Language], [CreatedById], [LastUserId], [LastTime], [CreatedOn])
VALUES (N'YeuCauTiepNhanBase.CapNhatBHYTSauKhiYeuCauVatTu', N'Bệnh nhân đã được kê vật tư BHYT, cần hủy vật tư BHYT để thực hiện cập nhật.', 1, 1, 1, CAST(N'2021-06-17T00:00:00.000' AS DateTime), CAST(N'2021-06-17T00:00:00.000' AS DateTime))
GO
Update dbo.CauHinh
Set [Value] = '2.5.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'