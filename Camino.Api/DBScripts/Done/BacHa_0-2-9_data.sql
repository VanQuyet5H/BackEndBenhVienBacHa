
update DonViHanhChinh set TenVietTat=
 dbo.fnFirsties(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(TenDonViHanhChinh,N'Tỉnh ',''),N'Thành phố ',''),N'Huyện ',''),N'Thị xã ',''),N'Xã ',''),N'Thị trấn ',''));