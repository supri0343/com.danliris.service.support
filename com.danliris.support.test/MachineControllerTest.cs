using com.danliris.support.lib.Interfaces;
using com.danliris.support.lib.Models.Machine;
using com.danliris.support.lib.ViewModel;
using com.danliris.support.webapi.Controllers.v1;
using Com.DanLiris.Service.support.lib.Services;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Security.Claims;
using Xunit;

namespace com.danliris.support.test
{
    public class MachineControllerTest
    {
        private Machine model
        {
            get
            {
                return new Machine
                {
                   Classification="",
                   MachineBrand="",
                   MachineCategory="",
                };
            }
        }

        private MutationMachine MutationMachine
        {
            get
            {
                return new MutationMachine
                {
                    TransactionDate =DateTime.Now,
                    MachineBrand = "",
                    MachineCategory = "",
                };
            }
        }

        private Mock<IServiceProvider> GetServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });


            return serviceProvider;
        }

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        [Fact]
        public void Should_Success_Get_ReportMachine()
        {
            var mockFacade = new Mock<IMachineService>();
            mockFacade.Setup(x => x.GetMachines(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<Machine> { model });

        

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            MachineController controller = new MachineController(mockFacade.Object, GetServiceProvider().Object);
            var response = controller.GetMachiness( It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Report_DataMachine()
        {
            var mockFacade = new Mock<IMachineService>();
            mockFacade.Setup(x => x.GetMachines(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<Machine> { model });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            MachineController controller = new MachineController(mockFacade.Object, GetServiceProvider().Object);
            var response = controller.GetXlsMachine(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }


        [Fact]
        public void Should_Success_GetXls_ReportMachine()
        {
            var mockFacade = new Mock<IMachineService>();
            mockFacade.Setup(x => x.GetXlsMachine(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new MemoryStream());

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };

            user.Setup(u => u.Claims).Returns(claims);
            MachineController controller = new MachineController(mockFacade.Object, GetServiceProvider().Object);
            var response = controller.GetXlsMachine(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.GetType().GetProperty("ContentType").GetValue(response, null));

        }

        [Fact]
        public void Should_Error_GetXls_ReportMachine()
        {
            var mockFacade = new Mock<IMachineService>();
            mockFacade.Setup(x => x.GetMachines(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<Machine> { model});

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };

            user.Setup(u => u.Claims).Returns(claims);
            MachineController controller = new MachineController(mockFacade.Object, GetServiceProvider().Object);
            var response = controller.GetXlsMachine(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));

        }

        [Fact]
        public void Should_Success_Get_ReportMachineMutation()
        {
            var mockFacade = new Mock<IMachineService>();
            mockFacade.Setup(x => x.GetMachineMutation(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<MutationMachine> { MutationMachine });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            MachineController controller = new MachineController(mockFacade.Object, GetServiceProvider().Object);
            var response = controller.GetMachineMutation(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        //[Fact]
        //public void Should_Error_Get_Report_DataMachineMutation()
        //{
        //    var mockFacade = new Mock<IMachineService>();
        //    mockFacade.Setup(x => x.GetXlsMachineMutation(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        //          .Returns(new MemoryStream());

        //    var user = new Mock<ClaimsPrincipal>();
        //    var claims = new Claim[]
        //    {
        //        new Claim("username", "unittestusername")
        //    };
        //    user.Setup(u => u.Claims).Returns(claims);
        //    MachineController controller = new MachineController(mockFacade.Object, GetServiceProvider().Object);
        //    var response = controller.GetMachineMutation(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        //    Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        //}


        [Fact]
        public void Should_Success_GetXls_ReportMachineMutation()
        {
            var mockFacade = new Mock<IMachineService>();
            mockFacade.Setup(x => x.GetXlsMachineMutation(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new MemoryStream());

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };

            user.Setup(u => u.Claims).Returns(claims);
            MachineController controller = new MachineController(mockFacade.Object, GetServiceProvider().Object);
            var response = controller.GetXlsMachineMutation(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.GetType().GetProperty("ContentType").GetValue(response, null));

        }

        [Fact]
        public void Should_Error_GetXls_ReportMachineMutation()
        {
            var mockFacade = new Mock<IMachineService>();
            mockFacade.Setup(x => x.GetMachines(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<Machine> { model });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };

            user.Setup(u => u.Claims).Returns(claims);
            MachineController controller = new MachineController(mockFacade.Object, GetServiceProvider().Object);
            var response = controller.GetXlsMachine(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));

        }
    }
}
