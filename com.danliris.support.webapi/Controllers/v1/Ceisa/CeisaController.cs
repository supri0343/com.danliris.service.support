using System;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Com.DanLiris.Service.support.lib.Services;
using com.danliris.support.lib.Interfaces;
using com.danliris.support.lib.Models.Ceisa.PEB;
using com.danliris.support.webapi.Helpers;
using com.danliris.support.lib.ViewModel.Ceisa.PEBViewModel;
using Newtonsoft.Json;
using com.danliris.support.lib.Interfaces.Ceisa;
using com.danliris.support.lib.Interfaces.Ceisa.TPB;
using System.IO;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;

namespace com.danliris.support.webapi.Controllers.v1.Ceisa
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/ceisa")]
    [Authorize]
    public class CeisaController:Controller
    {
        private static readonly string ApiVersion = "1.0";
        public readonly IServiceProvider serviceProvider;
        public readonly IMapper Mapper;
        private ICeisaService ceisaService;
        private IPEBService pEBService;
        private ITPBService TPBService;
        private readonly IdentityService identityService;

        public CeisaController(ICeisaService ceisaService, IServiceProvider serviceProvider, IMapper mapper, IPEBService pEBService, ITPBService tPBService)
        {
            this.ceisaService = ceisaService;
            this.serviceProvider = serviceProvider;
            this.identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
            this.Mapper = mapper;
            this.pEBService = pEBService;
            this.TPBService = tPBService;
        }

        [HttpGet("login")]
        public async Task<IActionResult> LoginCeisa()
        {
            try
            {
                var data = await ceisaService.Login();

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data.item,
                    data.message,
                    data.status,
                    statusCode = General.OK_STATUS_CODE
                }
                );
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("getRate")]
        public async Task<IActionResult> getRate(string kode)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                var authCeisa = Request.Headers["AuthorizationCeisa"].First();

                var data = await ceisaService.GetValutaRate(kode, authCeisa);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = data,
                    message = General.OK_MESSAGE,
                    statusCode = General.OK_STATUS_CODE
                }
                );
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }
        [HttpGet("getLartas")]
        public async Task<IActionResult> getLartas(string kode)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                var authCeisa = Request.Headers["AuthorizationCeisa"].First();

                var data = await ceisaService.GetLartas(kode, authCeisa);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = data,
                    message = General.OK_MESSAGE,
                    statusCode = General.OK_STATUS_CODE
                }
                );
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("getManifes")]
        public async Task<IActionResult> getManifes(string kodeKantor, string noHostBl, DateTime tglHostBl)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                var authCeisa = Request.Headers["AuthorizationCeisa"].First();

                var data = await ceisaService.GetManifestBC11(kodeKantor,noHostBl,tglHostBl, authCeisa);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = data,
                    message = General.OK_MESSAGE,
                    statusCode = General.OK_STATUS_CODE
                }
                );
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("getTarifHS")]
        public async Task<IActionResult> getTarifHS(string kode)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                var authCeisa = Request.Headers["AuthorizationCeisa"].First();

                var data = await ceisaService.GetTarifHS(kode, authCeisa);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = data,
                    message = General.OK_MESSAGE,
                    statusCode = General.OK_STATUS_CODE
                }
                );
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("GetBC-PEB/{id}")]
        public IActionResult GetPEBByIdToSupport(long id)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");
                var authCeisa = Request.Headers["AuthorizationCeisa"].First();
                var data = pEBService.ReadByIdToPushAsync(id, authCeisa);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = data,
                    message = General.OK_MESSAGE,
                    statusCode = General.OK_STATUS_CODE
                }
                );
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("GetBC-TPB/{id}")]
        public async Task<IActionResult> GetTPBByIdToSupportAsync(long id)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");
                var authCeisa = Request.Headers["AuthorizationCeisa"].First();
                var data = await TPBService.ReadByIdToPushAsync(id, authCeisa);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = data,
                    message = General.OK_MESSAGE,
                    statusCode = General.OK_STATUS_CODE
                }
                );
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }


        [HttpGet("tpb/download")]
        public async Task<IActionResult> GetXls(string noAju)
        {

            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);

                MemoryStream xls = await TPBService.GetExcel(noAju);


                string filename = noAju + ".xlsx";

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


        [HttpGet("peb/download")]
        public async Task<IActionResult> GetXlsPEB(string noAju)
        {

            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);

                MemoryStream xls = await pEBService.GetExcel(noAju);


                string filename = noAju + ".xlsx";

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
