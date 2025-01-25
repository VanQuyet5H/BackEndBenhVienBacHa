DECLARE @DichVuKhamMatId BIGINT = 0,
		@Template NVARCHAR(MAX) = N'{"ComponentDynamics":[{"Type":4,"Id":"group","Label":"Mắt","fxFlex":"100%","fxFlexSm":"100%","groupItems":[{"Type":1,"Id":"ThiLucKhongKinh","Label":"Thị lực không kính","Value":null,"fxFlex":"20%","fxFlexSm":"20%"},{"Type":2,"Id":"ThiLucKhongKinhMatPhai","Label":"Mắt phải","Value":null,"fxFlex":"40%","fxFlexSm":"40%", "placeholder":null},{"Type":2,"Id":"ThiLucKhongKinhMatTrai","Label":"Mắt trái","Value":null,"fxFlex":"40%","fxFlexSm":"40%", "placeholder":null},{"Type":1,"Id":"NhanAp","Label":"Nhãn áp","Value":null,"fxFlex":"20%","fxFlexSm":"20%"},{"Type":2,"Id":"NhanApMatPhai","Label":"Mắt phải","Value":null,"fxFlex":"40%","fxFlexSm":"40%", "placeholder":"mmhg"},{"Type":2,"Id":"NhanApMatTrai","Label":"Mắt trái","Value":null,"fxFlex":"40%","fxFlexSm":"40%", "placeholder":"mmhg"},{"Type":1,"Id":"ThiLucCoKinh","Label":"Thị lực có kính","Value":null,"fxFlex":"20%","fxFlexSm":"20%"},{"Type":2,"Id":"ThiLucCoKinhPhai","Label":"Mắt phải","Value":null,"fxFlex":"40%","fxFlexSm":"40%", "placeholder":null},{"Type":2,"Id":"ThiLucCoKinhTrai","Label":"Mắt trái","Value":null,"fxFlex":"40%","fxFlexSm":"40%", "placeholder":null}, {"Type":3,"Id":"SoiHienVi","Label":"Sinh hiển vi","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"SoiDayMat","Label":"Soi đáy mắt","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true}]}]}'
SELECT TOP 1 @DichVuKhamMatId = DichVuKhamBenhBenhVienId 
FROM TemplateKhamBenhTheoDichVu
WHERE Ten = N'. Khám Mắt' AND TieuDe = N'Khám Mắt'

IF @DichVuKhamMatId <> 0
BEGIN
	UPDATE TemplateKhamBenhTheoDichVu 
	SET ComponentDynamics = @Template
	WHERE Ten = N'. Khám Mắt' AND TieuDe = N'Khám Mắt'

	UPDATE yc
	SET ThongTinKhamTheoDichVuTemplate = @Template
	FROM YeuCauKhamBenh yc
	LEFT JOIN YeuCauTiepNhan tn on yc.YeuCauTiepNhanId = tn.Id
	WHERE DichVuKhamBenhBenhVienId = @DichVuKhamMatId AND tn.LoaiYeuCauTiepNhan <> 6
END

GO
Update dbo.CauHinh
Set [Value] = '2.1.8' where [Name] = 'CauHinhHeThong.DatabaseVesion'
