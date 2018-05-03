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
	[Route("v{version:apiVersion}/customs-report")]
	[Authorize]
	public class CustomsReportController : Controller
	{
		private static readonly string ApiVersion = "1.0";
		private WIPService wipService { get; }
		private ScrapService scrapService { get; }
		public CustomsReportController(ScrapService scrapService)
		{
			this.scrapService = scrapService;
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
	}
}