using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.danliris.support.lib.Services;
using com.danliris.support.webapi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace com.danliris.support.webapi.Controllers.v1
{
	[Produces("application/json")]
	[ApiVersion("1.0")]
	[Route("v{version:apiVersion}/customs-reports")]
	[Authorize]
	public class CustomsReportController : Controller
	{
		private static readonly string ApiVersion = "1.0";
		private ScrapService scrapService { get; }
        private FactBeacukaiService factBeacukaiService { get; }
        private FactItemMutationService factItemMutationService { get; }
        private WIPService wipService { get; }
		private FinishedGoodService finishedGoodService { get; }
		private MachineMutationService machineMutationService { get; }

		public CustomsReportController(ScrapService scrapService, WIPService wipService, FactBeacukaiService factBeacukaiService, FactItemMutationService factItemMutationService,FinishedGoodService finishedGoodService, MachineMutationService machineMutationService)
		{
			this.scrapService = scrapService;
            this.factBeacukaiService = factBeacukaiService;
            this.factItemMutationService = factItemMutationService;
			this.wipService = wipService;
			this.finishedGoodService = finishedGoodService;
			this.machineMutationService = machineMutationService;
		}

        [HttpGet("in")]
        public IActionResult GetIN(string type, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order = "{}")
        {
            int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            string accept = Request.Headers["Accept"];

            try
            {

                var data = factBeacukaiService.GetReportIN(type, dateFrom, dateTo, page, size, Order, offset);

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

        [HttpGet("scrap")]
		public IActionResult Get(  DateTime? dateFrom, DateTime? dateTo)
		{
			int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
			string accept = Request.Headers["Accept"];
			 
			try
			{
				var data = scrapService.GetScrapReport(dateFrom,dateTo, offset);
				return Ok(new
				{
					apiVersion = ApiVersion,
					data = data
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
		[HttpGet("finished-good")]
		public IActionResult GetFinishedGood(DateTime? dateFrom, DateTime? dateTo)
		{
			int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
			string accept = Request.Headers["Accept"];

			try
			{
				var data = finishedGoodService.GetFinishedGoodReport(dateFrom, dateTo, offset);
				return Ok(new
				{
					apiVersion = ApiVersion,
					data = data
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
		[HttpGet("machine-mutation")]
		public IActionResult GetMachineReport(DateTime? dateFrom, DateTime? dateTo)
		{
			int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
			string accept = Request.Headers["Accept"];

			try
			{
				var data = machineMutationService.GetMachineMutationReport(dateFrom, dateTo, offset);
				return Ok(new
				{
					apiVersion = ApiVersion,
					data = data
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
		[HttpGet("bbUnits")]
        public IActionResult GetBBUnit(int unit, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order = "{}")
        {
            int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            string accept = Request.Headers["Accept"];

            try
            {
                var data = factItemMutationService.GetReportBBUnit(unit, dateFrom, dateTo, page, size, Order, offset);
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

        [HttpGet("bpUnits")]
        public IActionResult GetBPUnit(int unit, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order = "{}")
        {
            int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            string accept = Request.Headers["Accept"];

            try
            {
                var data = factItemMutationService.GetReportBPUnit(unit, dateFrom, dateTo, page, size, Order, offset);
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

		[HttpGet("wip")]
		public IActionResult GetWIP(DateTime? date)
		{
			int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
			string accept = Request.Headers["Accept"];

			try
			{
				var data = wipService.GetWIPReport (date, offset);
				return Ok(new
				{
					apiVersion = ApiVersion,
					data = data
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