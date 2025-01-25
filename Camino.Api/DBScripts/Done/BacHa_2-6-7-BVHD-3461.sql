declare @templateNoi NVARCHAR(MAX) = N'{"ComponentDynamics":[{"Type":3,"Id":"TuanHoan","Label":"Tuần hoàn","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"HoHap","Label":"Hô hấp","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"TieuHoa","Label":"Tiêu hóa","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"ThanTietNieuSinhDuc","Label":"Thận - Tiết niệu - Sinh dục","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"ThanKinh","Label":"Thần kinh","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"CoXuongKhop","Label":"Cơ - Xương - Khớp","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"DaLieu","Label":"Da liễu","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true}]}',
		@DichVuKhamNoiId BIGINT = 0

SELECT TOP 1 @DichVuKhamNoiId = Id 
FROM DichVuKhamBenhBenhVien
WHERE Ma = N'02.1897' AND Ten = N'Khám Nội'

-- xử lý update template trong yêu cầu khám
IF @DichVuKhamNoiId <> 0
BEGIN
	UPDATE yc
	SET ThongTinKhamTheoDichVuTemplate = @templateNoi
	FROM YeuCauKhamBenh yc
	LEFT JOIN YeuCauTiepNhan tn on yc.YeuCauTiepNhanId = tn.Id
	WHERE DichVuKhamBenhBenhVienId = @DichVuKhamNoiId AND tn.LoaiYeuCauTiepNhan <> 6
END