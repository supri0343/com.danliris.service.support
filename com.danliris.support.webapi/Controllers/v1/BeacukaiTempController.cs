﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.danliris.support.lib.Services;
using com.danliris.support.lib.ViewModel;
using com.danliris.support.webapi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace com.danliris.support.webapi.Controllers.v1
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/beacukaitemps")]
    [Authorize]
    public class BeacukaiTempController : Controller
    {
        private static readonly string ApiVersion = "1.0";
        private readonly IBeacukaiTempService Service;

        public BeacukaiTempController(IBeacukaiTempService service)
        {
            Service = service;
        }

        [HttpGet]
        public IActionResult Get(int size = 25, string keyword = null)
        {
            try
            {
                var data = Service.Get(size, keyword);
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data,
                    info = new
                    {
                        size,
                        count = data.Count
                    }
                });
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("to-delete")]
        public async Task<IActionResult> GetDataToDelete(string jenis, string type, string nomor)
        {
            try
            {
                var data = await Service.GetDataForDelete(jenis,type,nomor);
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data,
                    info = new
                    {
                      
                    }
                });
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpPut("delete")]
        public async Task<IActionResult> DeleteData([FromBody]List<BEACUKAI_ToDeleteViewModel> data)
        {
            try
            {
                await Service.DeleteData(data);
                return NoContent();
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

    }
}