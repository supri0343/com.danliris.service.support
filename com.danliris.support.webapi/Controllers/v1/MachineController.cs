using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.danliris.support.lib.Interfaces;
using com.danliris.support.lib.Models;
using com.danliris.support.lib.Models.Machine;
using com.danliris.support.lib.Services;
using com.danliris.support.lib.ViewModel;
using com.danliris.support.webapi.Helpers;
using Com.DanLiris.Service.support.lib.Interfaces;
using Com.DanLiris.Service.support.lib.Services;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace com.danliris.support.webapi.Controllers.v1
{
	[Produces("application/json")]
	[ApiVersion("1.0")]
	[Route("v{version:apiVersion}/machine")]
	[Authorize]
	public class MachineController : Controller
	{
		private static readonly string ApiVersion = "1.0";
		public readonly IServiceProvider serviceProvider;
		private IMachineService machineService;
		private readonly IdentityService identityService;

		public MachineController(IMachineService machineService, IServiceProvider serviceProvider)
		{
			this.machineService = machineService;
			this.serviceProvider = serviceProvider;
			this.identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));

		}

		[HttpGet("brand")]
		public IActionResult GetMachineBrand(string Keyword, int size = 25)
		{
			int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
			string accept = Request.Headers["Accept"];

			try
			{
				var data = machineService.GetMachineBrand(size, Keyword);
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

		[HttpPost("brand")]
		public async Task<IActionResult> PostBrand([FromBody] MachineBrand model)
		{
			int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
			string accept = Request.Headers["Accept"];
			

			try
            {
				var username = User.Claims.Single(p => p.Type.Equals("username")).Value;
				await machineService.CreateMachineBrand(model, username);

				Dictionary<string, object> Result =
				   new ResultFormatter(ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE)
				   .Ok();
				return Created(String.Concat(Request.Path, "/", 0), Result);
			}
			catch (Exception e)
			{
				Dictionary<string, object> Result =
					new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
					.Fail();
				return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
			}
		}
		[HttpGet("category")]
		public IActionResult GetMachineCategory(string Keyword, int size = 25)
		{
			int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
			string accept = Request.Headers["Accept"];

			try
			{
				var data = machineService.GetMachineCategory(size, Keyword);
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

		[HttpPost("category")]
		public async Task<IActionResult> PostMachineCategory([FromBody] MachineCategory model)
		{
			int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
			string accept = Request.Headers["Accept"];


			try
			{
				var username = User.Claims.Single(p => p.Type.Equals("username")).Value;
				await machineService.CreateMachineCategory(model, username);

				Dictionary<string, object> Result =
				   new ResultFormatter(ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE)
				   .Ok();
				return Created(String.Concat(Request.Path, "/", 0), Result);
			}
			catch (Exception e)
			{
				Dictionary<string, object> Result =
					new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
					.Fail();
				return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
			}
		}

		[HttpGet("machines")]
		public IActionResult GetMachines(int page = 1, int size = 25, string keyword = null, string filter = "{}")
		{
			int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
			string accept = Request.Headers["Accept"];

			try
			{
				var data = machineService.GetMachine(page,size,keyword,filter);
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

		[HttpGet("machiness")]
		public IActionResult GetMachiness(string tipe, string ctg, string serial)
		{


			try
			{
				var data = machineService.GetMachines(tipe, ctg,serial);
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

		[HttpGet("machinesByBrand")]
		public IActionResult GetMachinesByBrand(int page = 1, int size = 25, string keyword = null, string filter = "{}")
		{
	

			try
			{
				var data = machineService.GetMachineByBrand(page, size, keyword, filter);
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

		[HttpGet("machines/{id}")]
		public IActionResult GetMachinesById(string id)
		{
			int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
			string accept = Request.Headers["Accept"];

			try
			{
				var data = machineService.GetMachineById(id);
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

		[HttpPut("machines/{id}")]
		public async Task<IActionResult> PutMachinesById(string id, [FromBody] Machine machine)
		{
			int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
			string accept = Request.Headers["Accept"];

			try
			{
				identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
				identityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());

				IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

				validateService.Validate(machine);

				await machineService.UpdateMachine(id,machine, identityService.Username);

				Dictionary<string, object> Result =
				   new ResultFormatter(ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE)
				   .Ok();
				return Created(String.Concat(Request.Path, "/", 0), Result);
			}
			catch (ServiceValidationExeption e)
			{
				Dictionary<string, object> Result =
					new ResultFormatter(ApiVersion, General.BAD_REQUEST_STATUS_CODE, General.BAD_REQUEST_MESSAGE)
					.Fail(e);
				return BadRequest(Result);
			}
			catch (Exception e)
			{
				Dictionary<string, object> Result =
					new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
					.Fail();
				return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
			}
		}

		[HttpPost("machinesmutation")]
		public async Task<IActionResult> PostMachineMutation([FromBody] MachinesMutation model)
		{
			//int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
			//string accept = Request.Headers["Accept"];


			try
			{
				identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
				identityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());

				IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

				validateService.Validate(model);

				await machineService.CreateMachineAndMutation(model, identityService.Username);

				Dictionary<string, object> Result =
				   new ResultFormatter(ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE)
				   .Ok();
				return Created(String.Concat(Request.Path, "/", 0), Result);
			}
			catch (ServiceValidationExeption e)
			{
				Dictionary<string, object> Result =
					new ResultFormatter(ApiVersion, General.BAD_REQUEST_STATUS_CODE, General.BAD_REQUEST_MESSAGE)
					.Fail(e);
				return BadRequest(Result);
			}
			catch (Exception e)
			{
				Dictionary<string, object> Result =
					new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
					.Fail();
				return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
			}
		}

		[HttpPost("machinesmutation/out")]
		public async Task<IActionResult> PostMachineMutationOut([FromBody] MachinesMutation model)
		{
			try
			{
				identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
				identityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());

				IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

				validateService.Validate(model);

				await machineService.CreateMachineAndMutation(model, identityService.Username);

				Dictionary<string, object> Result =
				   new ResultFormatter(ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE)
				   .Ok();
				return Created(String.Concat(Request.Path, "/", 0), Result);
			}
			catch (ServiceValidationExeption e)
			{
				Dictionary<string, object> Result =
					new ResultFormatter(ApiVersion, General.BAD_REQUEST_STATUS_CODE, General.BAD_REQUEST_MESSAGE)
					.Fail(e);
				return BadRequest(Result);
			}
			catch (Exception e)
			{
				Dictionary<string, object> Result =
					new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
					.Fail();
				return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
			}
		}

		[HttpGet("mutation")]
		public IActionResult GetMachineMutation(string tipe, string ctg, string serial)
		{
			try
			{
				var data = machineService.GetMachineMutation(tipe, ctg, serial);
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
		[HttpGet("mutation/{id}")]
		public IActionResult GetMutationById(Guid id)
		{
			try
			{
				var data = machineService.GetMutationById(id);
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

		[HttpPut("mutation/{id}")]
		public async Task<IActionResult> PutMutationById(Guid id, [FromBody] MachineMutation machine)
		{
			int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
			string accept = Request.Headers["Accept"];

			try
			{
				identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
				identityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());

				IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

				validateService.Validate(machine);

				await machineService.UpdateMutation(id, machine, identityService.Username);

				Dictionary<string, object> Result =
				   new ResultFormatter(ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE)
				   .Ok();
				return Created(String.Concat(Request.Path, "/", 0), Result);
			}
			catch (ServiceValidationExeption e)
			{
				Dictionary<string, object> Result =
					new ResultFormatter(ApiVersion, General.BAD_REQUEST_STATUS_CODE, General.BAD_REQUEST_MESSAGE)
					.Fail(e);
				return BadRequest(Result);
			}
			catch (Exception e)
			{
				Dictionary<string, object> Result =
					new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
					.Fail();
				return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
			}
		}
		[HttpGet("mutation/download")]
		public IActionResult GetXlsMachineMutation(string tipe, string ctg, string serial)
		{
			byte[] xlsInBytes;
			try
			{
				var xls = machineService.GetXlsMachineMutation(tipe, ctg, serial);
				string filename = "Laporan Mutasi Mesin";
				if (tipe != "") filename += " " + tipe;
				if (ctg != "") filename += "_" + ctg;
				if (serial != "") filename += "_" + serial;
				filename += ".xlsx";


				xlsInBytes = xls.ToArray();
				var file = File(xlsInBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
				return file;
			}
			catch (Exception e)
			{
				Dictionary<string, object> Result =
					new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
					.Fail();
				return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
			}
		}

		[HttpGet("download")]
		public IActionResult GetXlsMachine(string tipe, string ctg, string serial)
		{
			byte[] xlsInBytes;
			try
			{
				var xls = machineService.GetXlsMachine(tipe, ctg, serial);
				string filename = "Laporan Mesin";
				if (tipe != "") filename += " " + tipe;
				if (ctg != "") filename += "_" + ctg;
				if (serial != "") filename += "_" + serial;
				filename += ".xlsx";


				xlsInBytes = xls.ToArray();
				var file = File(xlsInBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
				return file;
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
