using AutoMapper;
using com.danliris.support.lib.Helpers;
using com.danliris.support.lib.Interfaces.Ceisa;
using com.danliris.support.lib.Models.Ceisa.TPB;
using com.danliris.support.lib.ViewModel.Ceisa.TPBViewModel;
using com.danliris.support.webapi.Controllers.v1.Ceisa;
using com.danliris.support.webapi.Controllers.v1.Ceisa.TPBController;
using Com.DanLiris.Service.support.lib.Services;
using Com.Moonlay.NetCore.Lib;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace com.danliris.support.test
{
    public class BC23ControllerTests
    {
        private readonly Mock<IBC23> _mockBC23Service;
        //private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly Mock<IMapper> _mockMapper;
        private readonly BC23Controller _controller;

        private Mock<IServiceProvider> _mockServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService());

            return serviceProvider;
        }

        public BC23ControllerTests()
        {
            _mockBC23Service = new Mock<IBC23>();
            //_mockServiceProvider = new Mock<IServiceProvider>();
            _mockMapper = new Mock<IMapper>();
            _controller = new BC23Controller(_mockBC23Service.Object, _mockServiceProvider().Object, _mockMapper.Object);

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user.Object }
            };

            _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
            _controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");
            _controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "7";

        }

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }


        [Fact]
        public void Get_ReturnsOkResult()
        {
            // Arrange
            int page = 1;
            int size = 10;
            string keyword = "test";
            string filter = "filter";
            string order = "{}";

            var data = new ReadResponse<object>(new List<object>(), 1, new Dictionary<string, string>());
            _mockBC23Service.Setup(s => s.Read(page, size, order, keyword, filter)).Returns(data);
           
            // Act
            var result = _controller.Get(page, size, keyword, filter, order);

            // Assert
            Assert.Equal(200, GetStatusCode(result));
        }

        [Fact]
        public void Get_ThrowsException_ReturnsStatusCode500()
        {
            // Arrange
            int page = 1;
            int size = 10;
            string keyword = "test";
            string filter = "filter";
            string order = "{}";

            _mockBC23Service.Setup(s => s.Read(page, size, order, keyword, filter)).Throws(new Exception("Error"));

            // Act
            var result = _controller.Get(page, size, keyword, filter, order);

            // Assert
            var statusCodeResult = GetStatusCode(result);
            Assert.Equal(500, statusCodeResult);
        }

        [Fact]
        public void GetUrut_ReturnsOkResult()
        {
            // Arrange
            var data = "urut1";
            _mockBC23Service.Setup(s => s.Urut()).Returns(data);

            // Act
            var result = _controller.GetUrut();

            // Assert
            Assert.Equal(200, GetStatusCode(result));
        }

        [Fact]
        public void GetUrut_ThrowsException_ReturnsStatusCode500()
        {
            // Arrange
            _mockBC23Service.Setup(s => s.Urut()).Throws(new Exception("Error"));

            // Act
            var result = _controller.GetUrut();

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Post_ReturnsCreatedResult()
        {
            // Arrange
            var model = new TPBHeader();

            // Act
            var result = await _controller.Post(model);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result);
            var modelValue = Assert.IsType<Dictionary<string, object>>(createdResult.Value);
            Assert.Equal("1.0", modelValue["apiVersion"]);
            Assert.Equal("OK", modelValue["message"]);
            Assert.Equal(201, modelValue["statusCode"]);
        }

        //[Fact]
        //public async Task Post_ThrowsServiceValidationExeption_ReturnsBadRequestResult()
        //{
        //    // Arrange
        //    var model = new TPBHeader();
        //    _mockBC23Service.Setup(s => s.Create(model)).Throws(new ServiceValidationExeption("Validation Error"));

        //    // Act
        //    var result = await _controller.Post(model);

        //    // Assert
        //    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        //    var modelValue = Assert.IsType<Dictionary<string, object>>(badRequestResult.Value);
        //    Assert.Equal("1.0", modelValue["apiVersion"]);
        //    Assert.Equal("Bad Request", modelValue["message"]);
        //    Assert.Equal(400, modelValue["statusCode"]);
        //}

        [Fact]
        public async Task Post_ThrowsException_ReturnsStatusCode500()
        {
            // Arrange
            var model = new TPBHeader();
            _mockBC23Service.Setup(s => s.Create(model)).Throws(new Exception("Error"));

            // Act
            var result = await _controller.Post(model);

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsOkResult()
        {
            // Arrange
            long id = 1;
            var data = new TPBHeader();
            var viewModel = new TPBHeaderViewModel();
            _mockBC23Service.Setup(s => s.ReadById(id)).ReturnsAsync(data);
            _mockMapper.Setup(m => m.Map<TPBHeaderViewModel>(data)).Returns(viewModel);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<Dictionary<string, object>>(okResult.Value);
            Assert.Equal("1.0", model["apiVersion"]);
            Assert.Equal(viewModel, model["data"]);
            Assert.Equal("OK", model["message"]);
            Assert.Equal(200, model["statusCode"]);
        }

        [Fact]
        public async Task GetById_ThrowsException_ReturnsStatusCode500()
        {
            // Arrange
            long id = 1;
            _mockBC23Service.Setup(s => s.ReadById(id)).Throws(new Exception("Error"));

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Put_ReturnsOkResult()
        {
            // Arrange
            int id = 1;
            var viewModel = new TPBHeader();

            // Act
            var result = await _controller.Put(id, viewModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<Dictionary<string, object>>(okResult.Value);
            Assert.Equal("1.0", model["apiVersion"]);
            Assert.Equal("OK", model["message"]);
            Assert.Equal(200, model["statusCode"]);
        }

        [Fact]
        public async Task Put_ThrowsException_ReturnsStatusCode500()
        {
            // Arrange
            int id = 1;
            var viewModel = new TPBHeader();
            _mockBC23Service.Setup(s => s.UpdateAsync(id, viewModel)).Throws(new Exception("Error"));

            // Act
            var result = await _controller.Put(id, viewModel);

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsOkResult()
        {
            // Arrange
            int id = 1;

            // Act
            var result = await _controller.Delete(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<Dictionary<string, object>>(okResult.Value);
            Assert.Equal("1.0", model["apiVersion"]);
            Assert.Equal("OK", model["message"]);
            Assert.Equal(200, model["statusCode"]);
        }

        [Fact]
        public async Task Delete_ThrowsException_ReturnsStatusCode500()
        {
            // Arrange
            int id = 1;
            _mockBC23Service.Setup(s => s.Delete(id)).Throws(new Exception("Error"));

            // Act
            var result = await _controller.Delete(id);

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task PostToSupport_ReturnsOkResult()
        {
            // Arrange
            int id = 1;
            var viewModel = new TPBViewModelList();

            // Act
            var result = await _controller.PostToSupport(id, viewModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<Dictionary<string, object>>(okResult.Value);
            Assert.Equal("1.0", model["apiVersion"]);
            Assert.Equal("OK", model["message"]);
            Assert.Equal(200, model["statusCode"]);
        }

        [Fact]
        public async Task PostToSupport_ThrowsException_ReturnsStatusCode500()
        {
            // Arrange
            int id = 1;
            var viewModel = new TPBViewModelList();
            _mockBC23Service.Setup(s => s.PostToSupportTPB(id, viewModel)).Throws(new Exception("Error"));

            // Act
            var result = await _controller.PostToSupport(id, viewModel);

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
    }
}
