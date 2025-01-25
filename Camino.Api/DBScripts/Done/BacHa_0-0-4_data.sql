update [ICD] set LoiDanCuaBacSi = 
N'Những điều nên thực hiện 
- Ăn điều độ đúng giờ, phụ thuộc vào giờ uống thuốc hoặc chích insulin. 
- Ăn chậm, nhai kỹ. 
- Nên ăn 3 bữa chính và 1 – 2 bữa phụ. - Ăn 5 bữa: sáng 30%, phụ sáng 5%, trưa 30%, chiều 25%, tối 10%. 
- Bữa ăn có đa dạng các loại thực phẩm. 
- Giảm bớt lượng tinh bột trong khẩu phần (cơm, mì, bánh mì, khoai tây...), thay các  loại thực phẩm tinh chế như gạo trắng, bún, phở, bánh mì trắng trong bữa ăn hằng ngày bằng các ngũ cốc thô như gạo lực, bánh mì đen, bắp, khoai củ. 
- Chọn trái cây có chỉ số đường huyết và lượng tải đường huyết thấp. 
- Nên ăn cá thay thịt tối thiểu 3 lần/ tuần nhất là cá biển sâu vì có nhiều Omega 3 có lợi cho tim mạch (cá thu, cá trích, cá hồi, cá basa...) 
- Chế độ ăn nhiều rau xanh. Lượng rau quả tươi nên dùng mỗi ngày 400 – 500g/ ngày. 
- Ăn vừa đủ loại trái cây (1- 2 suất/ ngày) khoảng 200g mỗi ngày. 
- Hạn chế muối < 6g/ ngày. Chú ý ăn nhạt hơn nếu có kèm cao huyết áp. 
- Tập thể dục thường xuyên: nên tập 30 -45 phút mỗi ngày. Người lớn tuổi nên chọn hình thức đi bộ, đạp xe. 
Những điều cần tránh 
- Bỏ bữa ăn, ăn dồn vào bữa sau. 
- Các thức ăn có nhiều đường và muối  
- Ăn nhiều thực phẩm nhiều cholesteron và chất béo no: đồ lòng, phomai, bơ, mỡ... 
- Uống rượu bia vì có nguy cơ gây hạ đường huyết, đặc biệt uống rượu mà không ăn.'
where LoaiICDId in (SELECT Id FROM [LoaiICD] where [NhomICDID] in (SELECT [Id] FROM [BvBacHa].[dbo].[NhomICD] where TenTiengViet like '%dai thao duong%'))
go
update [ICD] set LoiDanCuaBacSi = 
N'Những điều nên làm 
- Theo dõi cân nặng thường xuyên ít nhất mỗi tháng 1 lần để điều chỉnh kịp thời. Đối với những người thừa cân/ béo phì (chỉ số BMI > 25, hoặc vòng eo nam > 90cm, ở nữ > 80cm); cần thực hiện chế độ giảm cân, nên ăn những loại thực phẩm ít đường, ít béo phối hợp với tập thể dục đều đặn. - Đối với những người có cân nặng chuẩn BMI < 23, nên theo dõi cân nặng thường xuyên để duy trì trọng lượng của mình không để tăng cân. 
- Chế độ ăn giàu rau xanh, nên dùng mỗi ngày 400 – 500g/ ngày. Rau là nguồn cung cấp kali, canxi, magne, các loại vitamin, chất xơ đồng thời giúp giảm cholesterol toàn phần trong thực phẩm. 
- Ăn nhiều quả chín.
- Tập thể dục thường xuyên: nên tập thể dục từ 30 – 45 phút mỗi ngày. Người lớn tuổi nên chọn hình thức đi bộ, đạp xe đạp. - Uống thêm 1- 2 ly sữa mỗi ngày: sữa ít béo, sữa đậu nành. 
Những điều cần tránh 
- Hút thuốc lá: cần ngưng hút thuốc lá. 
- Uống quá nhiều rượu bia. Nam: chỉ nên uống lối đa 2 lon bia (khoảng 720ml bia), 300ml rượu nho hoặc 60ml rượu mạnh/ ngày, Nữ: chỉ nên uống tối đa 1 lon bia, 150ml rượng nho hoặc 30 ml rượu mạnh/ ngày. 
- Thói quen ăn mặn, dùng thêm nước chấm mặn trong bữa ăn. 
- Các thực phẩm chế biến sẵn có nhiều muối, bột nêm, chả giò, xúc xích, thịt hộp, chà bông, dưa muối chua, mì gói... 
- Ăn quá nhiều chất béo bão hòa và cholesterol: hạn chế các chất béo bão hòa và cholesterol góp phần phòng tránh bệnh tim mạch. 
- Các chất có cafein gây kích thích thần kinh như trà đặc, cà phê.
Tái khám ngay nếu đau đầu, chóng mặt, buồn nôn, nôn, đau ngực, khó thở, rối loạn ý thức, nói ngọng, méo miệng, giảm vận động và cảm giác chân tay...
Khám chuyên khoa Tim mạch vào đầu tuần sau để theo dõi và điều trị chuyên khoa về tăng huyết áp'
where LoaiICDId in (SELECT Id FROM [LoaiICD] where [NhomICDID] in (SELECT [Id] FROM [BvBacHa].[dbo].[NhomICD] where TenTiengViet like '%benh ly tang huyet ap%'))
go
update [ICD] set LoiDanCuaBacSi = 
N'- Nên dùng protid dễ hấp thu, ít sinh hơi (đè ép gây khó thở):  sữa, sữa chua, cá.
- Hạn chế mỡ động vật, bơ, phomat...
- Chất bột đường: Chiếm 60 – 65% tổng năng lượng
- Hạn chế thực phẩm chế biến sẵn có nhiều muối,  Natri ≤ 2000m
- Thức ăn chế biến mềm, dễ tiêu hóa, ít sợi xơ. 
- Trong ngày nên ăn 5 – 6 bữa nhỏ. 
- Sau khi ăn không nên nằm ngay, nên nghỉ 30 – 40 phút.
- Nhu cầu nước: 1 – 1.5 lít, đảm bảo cân bằng lượng xuất nhập. 
- Hạn chế cafe, rượu, nước có gas. 
- Không hút thuốc lá.'
where LoaiICDId in (SELECT Id FROM [BvBacHa].[dbo].[LoaiICD] where TenTiengViet like '%suy tim%')
go
update [ICD] set LoiDanCuaBacSi = 
N'Thực phẩm nên dùng 
- Sữa và các sản phẩm từ sữa (sữa tươi, sữa công thức, pho mai, yaourt, bơ...), trứng. 
- Rau củ được nấu chín hoặc chế biến dưới dạng súp. 
- Các tinh bột phức tạp được nấu chín, mềm như cơm, xôi, bánh mì sandwich, khoai.
- Chống tăng tiết dịch vị và HCl: không để bụng đói; Không nên ăn quá no mà chia thành nhiều bữa (4- 5 bữa)., nước luộc, nước hầm thịt nguyên chất 
- Nên ăn chậm, nhai kỹ. 
- không ăn thức ăn quá nóng hay quá lạnh
- Trong cơn đau: uống nước, sữa, cháo
Những điều cần tránh 
- Không nên: dùng các loại thực phẩm nhiều mùi vị, được chế biến bằng cách chiên, nướng, quay, dùng nhiều dầu mỡ. 
- Các loại thịt nguội chế biến sẵn như dăm bông, lạp xưởng, xúc xích, nước sốt. 
- Các loại thực phẩm quá cứng như gân sụn, rau quả sống.
- Quả xanh, chua,… Gia vị: chua, cay, nồng, … 
- Không nên ăn quá nhiều gia vị: tỏi, dấm, tiêu, ớt, muối... 
- Không nên uống rượu, bia, cà phê.
- Ngưng hoàn toàn thuốc lá.'
where LoaiICDId in (SELECT Id FROM [BvBacHa].[dbo].[LoaiICD] where TenTiengViet like '%loet da day%' or TenTiengViet like '%viem da day va ta trang%')
