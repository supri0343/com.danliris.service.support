using com.danliris.support.lib.Services;
using com.danliris.support.webapi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.danliris.support.webapi.Controllers.v1
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/fact-beacukai-reports")]
    [Authorize]
    public class FactBeacukaiReportController : Controller
    {
        private static readonly string ApiVersion = "1.0";
        private FactBeacukaiService factBeacukaiService { get; }

        public FactBeacukaiReportController(FactBeacukaiService factBeacukaiService)
        {
            this.factBeacukaiService = factBeacukaiService;
        }

        [HttpGet("in")]
        public IActionResult GetIN(string type, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order = "{}")
        {
            int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            string accept = Request.Headers["Accept"];

            try
            {

                var data = factBeacukaiService.GetReportIN(type, dateFrom, dateTo, page, size, Order, offset);
                var docNo = data.Item1.ToArray();
                var index = 0;
                foreach (var a in data.Item1)
                {
                    var dup = Array.Find(docNo, o => o.BCType == a.BCType && o.BCNo == a.BCNo);
                    if (dup != null)
                    {
                        if (dup.count == 0)
                        {
                            index++;
                            dup.count = index;
                        }
                    }
                    a.count = dup.count;
                }
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = data.Item1,
                    info = new { total = data.Item2 }
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

        [HttpGet("out")]
        public IActionResult GetOUT(string type, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order = "{}")
        {
            int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            string accept = Request.Headers["Accept"];

            try
            {

                var data = factBeacukaiService.GetReportOUT(type, dateFrom, dateTo, page, size, Order, offset);
                
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = data.Item1,
                    info = new { total = data.Item2 }
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
    }
}
