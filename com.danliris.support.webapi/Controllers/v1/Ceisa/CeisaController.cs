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
        private readonly IdentityService identityService;

        public CeisaController(ICeisaService ceisaService, IServiceProvider serviceProvider, IMapper mapper)
        {
            this.ceisaService = ceisaService;
            this.serviceProvider = serviceProvider;
            this.identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
            this.Mapper = mapper;

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
    }
}
