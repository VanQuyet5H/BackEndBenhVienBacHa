DECLARE @DichVuKhamId BIGINT = NULL,
		@TempLateTheoDichVuId BIGINT = NULL,
		@Content NVARCHAR(MAX) = N'{"ComponentDynamics":[{"Type":4,"Id":"group","Label":"Mắt","fxFlex":"100%","fxFlexSm":"100%","groupItems":[{"Type":1,"Id":"ThiLucKhongKinh","Label":"Thị lực không kính","Value":null,"fxFlex":"20%","fxFlexSm":"20%"},{"Type":2,"Id":"ThiLucKhongKinhMatPhai","Label":"Mắt phải","Value":null,"fxFlex":"40%","fxFlexSm":"40%", "placeholder":null},{"Type":2,"Id":"ThiLucKhongKinhMatTrai","Label":"Mắt trái","Value":null,"fxFlex":"40%","fxFlexSm":"40%", "placeholder":null},{"Type":1,"Id":"NhanAp","Label":"Nhãn áp","Value":null,"fxFlex":"20%","fxFlexSm":"20%"},{"Type":2,"Id":"NhanApMatPhai","Label":"Mắt phải","Value":null,"fxFlex":"40%","fxFlexSm":"40%", "placeholder":"mmhg"},{"Type":2,"Id":"NhanApMatTrai","Label":"Mắt trái","Value":null,"fxFlex":"40%","fxFlexSm":"40%", "placeholder":"mmhg"},{"Type":1,"Id":"ThiLucCoKinh","Label":"Thị lực có kính","Value":null,"fxFlex":"20%","fxFlexSm":"20%"},{"Type":2,"Id":"ThiLucCoKinhPhai","Label":"Mắt phải","Value":null,"fxFlex":"40%","fxFlexSm":"40%", "placeholder":null},{"Type":2,"Id":"ThiLucCoKinhTrai","Label":"Mắt trái","Value":null,"fxFlex":"40%","fxFlexSm":"40%", "placeholder":null}, {"Type":3,"Id":"SoiHienVi","Label":"Sinh hiển vi","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"SoiDayMat","Label":"Soi đáy mắt","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true}]},{"Type":3,"Id":"TuanHoan","Label":"Tuần hoàn","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"HoHap","Label":"Hô hấp","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"TieuHoa","Label":"Tiêu hóa","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"ThanTietNieuSinhDuc","Label":"Thận - Tiết niệu - Sinh dục","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"ThanKinh","Label":"Thần kinh","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"CoXuongKhop","Label":"Cơ - Xương - Khớp","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"RangHamMat","Label":"Răng - Hàm - Mặt","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"NoiTietDinhDuong","Label":"Nội tiết - Dinh dưỡng","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"SanPhuKhoa","Label":"Sản phụ khoa","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true},{"Type":3,"Id":"DaLieu","Label":"Da liễu","Value":null,"fxFlex":"100%","fxFlexSm":"100%", "maxlength":4000, "minHeight": 22, "useVoice": true}]}'


SELECT TOP 1 @DichVuKhamId = DichVuKhamBenhBenhVienId, @TempLateTheoDichVuId = Id
FROM TemplateKhamBenhTheoDichVu WHERE TieuDe = N'Khám Mắt'

UPDATE TemplateKhamBenhTheoDichVu 
SET ComponentDynamics = @Content
WHERE Id = @TempLateTheoDichVuId

UPDATE YeuCauKhamBenh
SET ThongTinKhamTheoDichVuTemplate = @Content
WHERE DichVuKhamBenhBenhVienId = @DichVuKhamId
	AND ThongTinKhamTheoDichVuTemplate IS NOT NULL


Update CauHinh
Set [Value] = '0.9.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'