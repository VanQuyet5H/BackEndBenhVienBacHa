using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public partial class UserController
    {
    //    [HttpGet("GetAllNotificationAndTaskByUserAsync")]
    //    [ClaimRequirement(Enums.SecurityOperation.None, Enums.DocumentType.None)]
    //    public async Task<User> GetAllNotificationAndTaskByUserAsync()
    //    {
    //        var currentUser = await _userService.GetCurrentUser();
    //        var userEntity = await _userService.GetAllNotificationAndTaskByUserAsync(currentUser.Id);
    //        return userEntity;
    //    }

    //    [HttpGet("GetAllThongBaoTheoUserAsync")]
    //    [ClaimRequirement(Enums.SecurityOperation.None, Enums.DocumentType.None)]
    //    public async Task<NotificationDetail> GetAllThongBaoTheoUserAsync(bool getAllNotification = true)
    //    {
    //        NotificationDetail chiTietThongBao = null;
    //        var listThongBao = await _userService.GetAllThongBaoTheoUserAsync();
    //        if (listThongBao != null)
    //        {
    //            chiTietThongBao = new NotificationDetail()
    //            {
    //                CountUnRead = listThongBao.Count(x => x.IsRead == false)
    //            };

    //            if (!getAllNotification)
    //            {
    //                chiTietThongBao.ListNotification = listThongBao.OrderByDescending(x => x.Id).Take(5).ToList();
    //            }
    //            else
    //            {
    //                chiTietThongBao.ListNotification = listThongBao.OrderByDescending(x => x.Id).ToList();
    //            }
    //        }
    //        return chiTietThongBao;
    //    }

    //    [HttpGet("GetAllCongViecTheoUserAsync")]
    //    [ClaimRequirement(Enums.SecurityOperation.None, Enums.DocumentType.None)]
    //    public async Task<TaskDetail> GetAllCongViecTheoUserAsync(bool getAllTask = true)
    //    {
    //        TaskDetail chiTietCongViec = null;
    //        var listCongViec = await _userService.GetAllCongViecTheoUserAsync();
    //        if (listCongViec != null)
    //        {
    //            chiTietCongViec = new TaskDetail()
    //            {
    //                CountUnRead = listCongViec.Count(x => x.IsRead == false)
    //            };
    //            if (!getAllTask)
    //            {
    //                chiTietCongViec.ListTask = listCongViec.OrderByDescending(x => x.Id).Take(5).ToList();
    //            }
    //            else
    //            {
    //                chiTietCongViec.ListTask = listCongViec.OrderByDescending(x => x.Id).ToList();
    //            }
    //        }
    //        return chiTietCongViec;
    //    }

    //    [HttpPut("CapNhatTrangThaiDaXemNotification")]
    //    [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.None)]
    //    public async Task<ActionResult> CapNhatTrangThaiDaXemNotificationVaTask(long id, Enums.MessagingType loaiThongBaoXuLy)
    //    {
    //        await _userService.CapNhatTrangThaiDaXemNotification(id, loaiThongBaoXuLy);
    //        return NoContent();
    //    }

    //    #region Task
    //    [HttpPost("GetDataForGridTaskAsync")]
    //    [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.User)]
    //    public async Task<ActionResult<GridDataSource>> GetDataForGridTaskAsync([FromBody]QueryInfo queryInfo)
    //    {

    //        var gridData = await _userService.GetDataForGridTaskAsync(queryInfo);
    //        return Ok(gridData);
    //    }


    //    [HttpPost("GetTotalPageForGridTaskAsync")]
    //    [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.User)]
    //    public async Task<ActionResult<GridDataSource>> GetTotalPageForGridTaskAsync([FromBody]QueryInfo queryInfo)
    //    {
    //        var gridData = await _userService.GetTotalPageForGridTaskAsync(queryInfo);
    //        return Ok(gridData);
    //    }

    //    #endregion

    //    #region Notification
    //    [HttpPost("GetDataForGridNotificationAsync")]
    //    [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.User)]
    //    public async Task<ActionResult<GridDataSource>> GetDataForGridNotificationAsync([FromBody]QueryInfo queryInfo)
    //    {

    //        var gridData = await _userService.GetDataForGridNotificationAsync(queryInfo);
    //        return Ok(gridData);
    //    }


    //    [HttpPost("GetTotalPageForGridNotificationAsync")]
    //    [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.User)]
    //    public async Task<ActionResult<GridDataSource>> GetTotalPageForGridNotificationAsync([FromBody]QueryInfo queryInfo)
    //    {
    //        var gridData = await _userService.GetTotalPageForGridNotificationAsync(queryInfo);
    //        return Ok(gridData);
    //    }
    //    #endregion
    }
}
