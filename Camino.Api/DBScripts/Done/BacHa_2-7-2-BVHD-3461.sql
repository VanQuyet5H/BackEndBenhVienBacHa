declare @templateNoi NVARCHAR(MAX) = N'{"ComponentDynamics":[{"Type":3,"Id":"TuanHoan","Label":"Tuần hoàn","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"HoHap","Label":"Hô hấp","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"TieuHoa","Label":"Tiêu hóa","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"ThanTietNieuSinhDuc","Label":"Thận - Tiết niệu - Sinh dục","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"ThanKinh","Label":"Thần kinh","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"CoXuongKhop","Label":"Cơ - Xương - Khớp","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"DaLieu","Label":"Da liễu","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true}]}',
		@templateNgoai NVARCHAR(MAX) = N'{"ComponentDynamics":[{"Type":3,"Id":"TuanHoan","Label":"Tuần hoàn","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"HoHap","Label":"Hô hấp","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"TieuHoa","Label":"Tiêu hóa","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"ThanTietNieuSinhDuc","Label":"Thận - Tiết niệu - Sinh dục","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"CoXuongKhop","Label":"Cơ - Xương - Khớp","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true}]}',
		@templateNhi NVARCHAR(MAX) = N'{"ComponentDynamics":[{"Type":3,"Id":"TuanHoan","Label":"Tuần hoàn","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"HoHap","Label":"Hô hấp","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"TieuHoa","Label":"Tiêu hóa","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"ThanTietNieuSinhDuc","Label":"Thận - Tiết niệu - Sinh dục","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"ThanKinh","Label":"Thần kinh","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true}]}',

		@DichVuKhamNoiId BIGINT = 0,
		@DichVuKhamNgoaiId BIGINT = 0,
		@DichVuKhamNhiId BIGINT = 0

SELECT TOP 1 @DichVuKhamNoiId = Id 
FROM DichVuKhamBenhBenhVien
WHERE Ma = N'02.1897' AND Ten = N'Khám Nội'

SELECT TOP 1 @DichVuKhamNgoaiId = Id 
FROM DichVuKhamBenhBenhVien
WHERE Ma = N'10.1897' AND Ten = N'Khám Ngoại'

SELECT TOP 1 @DichVuKhamNhiId = Id 
FROM DichVuKhamBenhBenhVien
WHERE Ma = N'03.1897' AND Ten = N'Khám Nhi'

-- xử lý update template trong yêu cầu khám
IF @DichVuKhamNoiId <> 0
BEGIN
	INSERT INTO TemplateKhamBenhTheoDichVu(DichVuKhamBenhBenhVienId, Ten, TieuDe, ComponentDynamics, CreatedById, LastUserId, LastTime, CreatedOn)
	VALUES (@DichVuKhamNoiId, N'Khám Nội', N'Khám Nội', @templateNoi, 1, 1, GETDATE(), GETDATE())

	UPDATE yc
	SET ThongTinKhamTheoDichVuTemplate = @templateNoi
	FROM YeuCauKhamBenh yc
	LEFT JOIN YeuCauTiepNhan tn on yc.YeuCauTiepNhanId = tn.Id
	WHERE DichVuKhamBenhBenhVienId = @DichVuKhamNoiId AND tn.LoaiYeuCauTiepNhan <> 6
END

IF @DichVuKhamNgoaiId <> 0
BEGIN
	INSERT INTO TemplateKhamBenhTheoDichVu(DichVuKhamBenhBenhVienId, Ten, TieuDe, ComponentDynamics, CreatedById, LastUserId, LastTime, CreatedOn)
	VALUES (@DichVuKhamNgoaiId, N'Khám Ngoại', N'Khám Ngoại', @templateNgoai, 1, 1, GETDATE(), GETDATE())

	UPDATE yc
	SET ThongTinKhamTheoDichVuTemplate = @templateNgoai
	FROM YeuCauKhamBenh yc
	LEFT JOIN YeuCauTiepNhan tn on yc.YeuCauTiepNhanId = tn.Id
	WHERE DichVuKhamBenhBenhVienId = @DichVuKhamNgoaiId AND tn.LoaiYeuCauTiepNhan <> 6
END

IF @DichVuKhamNhiId <> 0
BEGIN
	INSERT INTO TemplateKhamBenhTheoDichVu(DichVuKhamBenhBenhVienId, Ten, TieuDe, ComponentDynamics, CreatedById, LastUserId, LastTime, CreatedOn)
	VALUES (@DichVuKhamNhiId, N'Khám Nhi', N'Khám Nhi', @templateNhi, 1, 1, GETDATE(), GETDATE())

	UPDATE yc
	SET ThongTinKhamTheoDichVuTemplate = @templateNhi
	FROM YeuCauKhamBenh yc
	LEFT JOIN YeuCauTiepNhan tn on yc.YeuCauTiepNhanId = tn.Id
	WHERE DichVuKhamBenhBenhVienId = @DichVuKhamNhiId AND tn.LoaiYeuCauTiepNhan <> 6
END

GO
Update dbo.CauHinh
Set [Value] = '2.7.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'