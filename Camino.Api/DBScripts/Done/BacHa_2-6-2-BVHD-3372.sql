ALTER TABLE KetQuaXetNghiemChiTiet
  ALTER COLUMN GiaTriNhapTay NVARCHAR(500) NULL;

ALTER TABLE [PhienXetNghiem] DROP COLUMN [BarCodeId];
ALTER TABLE [PhienXetNghiem] 
ADD 
[BarCodeId]  AS (case when [CreatedOn] > '2021-07-28 14:00:00.00' --set bằng thời điểm up code
then concat(right('00'+ltrim(str(datepart(day,[ThoiDiemBatDau]))),(2)),right('00'+ltrim(str(datepart(month,[ThoiDiemBatDau]))),(2)),right(datepart(year,[ThoiDiemBatDau]),(2)),right('0000'+ltrim(str([MaSo])),(4)))
else concat(right(datepart(year,[ThoiDiemBatDau]),(2)),right('00'+ltrim(str(datepart(month,[ThoiDiemBatDau]))),(2)),right('00'+ltrim(str(datepart(day,[ThoiDiemBatDau]))),(2)),right('0000'+ltrim(str([MaSo])),(4))) end)

GO
update template 
set Body = N'<style>
    body {
        margin: 0px;
    }
    * {
        font-size: 14px;
    }
    p {
        margin: 0;
    }
</style>
<div style="position: relative; width: 229px; overflow: hidden; padding-top: 3px;">
    <div style="position: relative; float: left; width: 10px; height: 1px;"><p style="transform: rotateZ(270deg) !important; position: absolute; top: 10px; font-size: 13px; width: 30px;">{{GioiTinhFotmat}}</p></div>
    <div style="position: relative; float: left; width: 200px; text-align: center; overflow: hidden;">
        <div style="float: left; width: 100%; text-align: center;"><img style="width: 80%;" src="data:image/png;base64,{{BarcodeByBarcodeId}}" /></div>
        <p style="font-weight: bold; font-size: 17px; text-align: center; width: 100%; float: left;">{{BarcodeId}}</p>
    </div>
    <div style="position: relative; float: left; width: 19px; height: 1px;"><p style="transform: rotateZ(90deg) !important; position: absolute; top: 20px; font-size: 13px; right: -8px; width: 61px;">{{GioCapCode}}</p></div>
    <div style="position: relative; float: left; width: 100%; text-align: center;"><p style="font-size: 13px; font-weight: bold;">{{TenBenhNhan}}</p></div>
    <div style="position: relative; float: left; width: 100%; text-align: center;"><p style="font-size: 13px;">{{NgaySinhFotmat}}</p></div>
</div>'
where Name = N'LayMauXetNghiemBarcode'

GO
Update dbo.CauHinh
Set [Value] = '2.6.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'
