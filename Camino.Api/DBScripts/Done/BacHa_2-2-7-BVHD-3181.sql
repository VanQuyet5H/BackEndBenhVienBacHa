declare @templateMat NVARCHAR(MAX) = N'{"ComponentDynamics":[{"Type":4,"Id":"group","Label":"Mắt","fxFlex":"100%","fxFlexSm":"100%","groupItems":[{"Type":1,"Id":"ThiLucKhongKinh","Label":"Thị lực không kính","Value":null,"fxFlex":"20%","fxFlexSm":"20%"},{"Type":2,"Id":"ThiLucKhongKinhMatTrai","Label":"MT","Value":null,"fxFlex":"40%","fxFlexSm":"40%", "maxlength":1000, "placeholder":null},{"Type":2,"Id":"ThiLucKhongKinhMatPhai","Label":"MP","Value":null,"fxFlex":"40%","fxFlexSm":"40%", "maxlength":1000, "placeholder":null},{"Type":1,"Id":"ThiLucCoKinh","Label":"Thị lực có kính","Value":null,"fxFlex":"20%","fxFlexSm":"20%"},{"Type":2,"Id":"ThiLucCoKinhTrai","Label":"MT","Value":null,"fxFlex":"40%","fxFlexSm":"40%", "maxlength":1000, "placeholder":null}, {"Type":2,"Id":"ThiLucCoKinhPhai","Label":"MP","Value":null,"fxFlex":"40%","fxFlexSm":"40%", "maxlength":1000, "placeholder":null},{"Type":1,"Id":"NhanAp","Label":"Nhãn áp","Value":null,"fxFlex":"20%","fxFlexSm":"20%"},{"Type":2,"Id":"NhanApMatTrai","Label":"MT","Value":null,"fxFlex":"40%","fxFlexSm":"40%", "maxlength":1000, "placeholder":""},{"Type":2,"Id":"NhanApMatPhai","Label":"MP","Value":null,"fxFlex":"40%","fxFlexSm":"40%", "maxlength":1000, "placeholder":""},{"Type":3,"Id":"KhamMatNoiDung","Label":"Khám mắt","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "placeholder":"Nội dung"},{"Type":3,"Id":"SoiHienVi","Label":"Sinh hiển vi","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"SoiDayMat","Label":"Soi đáy mắt","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true}]}]}',
		@templateRHM NVARCHAR(MAX) = N'{"ComponentDynamics":[{"Type":4,"Id":"group","Label":"Răng - Hàm - Mặt","fxFlex":"100%","fxFlexSm":"100%","groupItems":[{"Type":3,"Id":"Rang","Label":"Răng","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"Ham","Label":"Hàm","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"Mat","Label":"Mặt","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true}]}]}',
		@templateTMH NVARCHAR(MAX) = N'{"ComponentDynamics":[{"Type":4,"Id":"group","Label":"Tai - Mũi - Họng","fxFlex":"100%","fxFlexSm":"100%","groupItems":[{"Type":3,"Id":"Tai","Label":"Tai","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"Mui","Label":"Mũi","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"Hong","Label":"Họng","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true}]}]}',

		@DichVuKhamMatId BIGINT = 0,
		@DichVuKhamRHMId BIGINT = 0,
		@DichVuKhamTMHId BIGINT = 0

SELECT TOP 1 @DichVuKhamMatId = DichVuKhamBenhBenhVienId 
FROM TemplateKhamBenhTheoDichVu
WHERE Ten = N'. Khám Mắt' AND TieuDe = N'Khám Mắt'

SELECT TOP 1 @DichVuKhamRHMId = DichVuKhamBenhBenhVienId 
FROM TemplateKhamBenhTheoDichVu
WHERE Ten = N'. Khám Răng Hàm Mặt' AND TieuDe = N'Khám Răng Hàm Mặt'

SELECT TOP 1 @DichVuKhamTMHId = DichVuKhamBenhBenhVienId 
FROM TemplateKhamBenhTheoDichVu
WHERE Ten = N'. Khám Tai Mũi Họng' AND TieuDe = N'Khám Tai Mũi Họng'

-- xử lý update template default
UPDATE TemplateKhamBenhTheoDichVu
SET ComponentDynamics = (Case Ten When N'. Khám Mắt' THEN @templateMat
								  WHEN N'. Khám Răng Hàm Mặt' THEN @templateRHM
								  WHEN N'. Khám Tai Mũi Họng' THEN @templateTMH
								  ELSE ComponentDynamics END)

-- xử lý update template trong yêu cầu khám
IF @DichVuKhamMatId <> 0
BEGIN
	UPDATE yc
	SET ThongTinKhamTheoDichVuTemplate = @templateMat
	FROM YeuCauKhamBenh yc
	LEFT JOIN YeuCauTiepNhan tn on yc.YeuCauTiepNhanId = tn.Id
	WHERE DichVuKhamBenhBenhVienId = @DichVuKhamMatId AND tn.LoaiYeuCauTiepNhan <> 6
END

IF @DichVuKhamRHMId <> 0
BEGIN
	UPDATE yc
	SET ThongTinKhamTheoDichVuTemplate = @templateRHM
	FROM YeuCauKhamBenh yc
	LEFT JOIN YeuCauTiepNhan tn on yc.YeuCauTiepNhanId = tn.Id
	WHERE DichVuKhamBenhBenhVienId = @DichVuKhamRHMId AND tn.LoaiYeuCauTiepNhan <> 6
END

IF @DichVuKhamTMHId <> 0
BEGIN
	UPDATE yc
	SET ThongTinKhamTheoDichVuTemplate = @templateTMH
	FROM YeuCauKhamBenh yc
	LEFT JOIN YeuCauTiepNhan tn on yc.YeuCauTiepNhanId = tn.Id
	WHERE DichVuKhamBenhBenhVienId = @DichVuKhamTMHId AND tn.LoaiYeuCauTiepNhan <> 6
END

GO
Update dbo.CauHinh
Set [Value] = '2.2.7' where [Name] = 'CauHinhHeThong.DatabaseVesion'