using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.danliris.support.lib;
using com.danliris.support.lib.Helpers;
using com.danliris.support.lib.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using IdentityServer4.AccessTokenValidation;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using com.danliris.support.lib.Interfaces;
using Com.DanLiris.Service.support.lib.Services;
using Com.DanLiris.Service.support.lib.Interfaces;
using com.danliris.support.lib.Services.Ceisa;
using AutoMapper;
using com.danliris.support.lib.Interfaces.Ceisa;
using com.danliris.support.lib.Services.Ceisa;
using com.danliris.support.lib.Services.Ceisa.TPB;
using com.danliris.support.lib.Interfaces.Ceisa.TPB;

namespace com.danliris.support.webapi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void RegisterEndpoint()
        {
            APIEndpoint.Core = Configuration.GetValue<string>("CoreEndpoint") ?? Configuration["CoreEndpoint"];
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection") ?? Configuration["DefaultConnection"];
			string LocalDbConnectionString = Configuration.GetConnectionString("LocalDbProductionConnection") ?? Configuration["LocalDbProductionConnection"];
			APIEndpoint.ConnectionString = Configuration.GetConnectionString("DefaultConnection") ?? Configuration["DefaultConnection"];
            APIEndpoint.LocalConnectionString = Configuration.GetConnectionString("LocalDbProductionConnection") ?? Configuration["LocalDbProductionConnection"];
            APIEndpoint.HostToHost = Configuration.GetValue<string>("HostToHostEndpoint") ?? Configuration["HostToHostEndpoint"];

            //Get Credential Ceisa
            CredentialCeisa.Username = Configuration.GetValue<string>("UsernameCeisa") ?? Configuration["UsernameCeisa"];
            CredentialCeisa.Password = Configuration.GetValue<string>("PasswordCeisa") ?? Configuration["PasswordCeisa"];

            services
				.AddDbContext<SupportDbContext>(options => options.UseSqlServer(connectionString))
				.AddApiVersioning(options =>
                {
                    options.ReportApiVersions = true;
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                });
            services.AddTransient<ILocalDbProductionDBContext>(s => new LocalDbProductionDBContext(LocalDbConnectionString));
            services
                .AddTransient<FactBeacukaiService>();
			services
				.AddTransient<ScrapService>();
            services
                .AddTransient<FactItemMutationService>();
			services
				.AddTransient<WIPService>();
			services
				.AddTransient<FinishedGoodService>();
			services
			.AddTransient<MachineMutationService>();
            services
                .AddTransient<HOrderService>();
            services
                .AddTransient<ExpenditureGoodsService>();
            services.AddTransient<TraceableInService>();
            services.AddTransient<TraceableOutService>();
            services.AddTransient<IViewFactBeacukaiService, ViewFactBeacukaiService>();
            services
                .AddTransient<IBeacukaiTempService, BeacukaiTempService>();
            services.AddTransient<IMachineService, MachineService>();
            services.AddTransient<IPEBService, PEBService>();
            services.AddTransient<ICeisaService, CeisaService>();
            services.AddTransient<IBC40, BC40Service>();
            services.AddTransient<IBC261, BC261Service>();
            services.AddTransient<IBC23, BC23Service>();
            services.AddTransient<IBC25, BC25Service>();
            services.AddTransient<IBC262, BC262Service>();
            services.AddTransient<ITPBService, TPBService>();
            services.AddTransient<IHSService, HSService>();
            services.AddAutoMapper();
            services.AddTransient<LogHistoriesService>();
            var Secret = Configuration.GetValue<string>("Secret") ?? Configuration["Secret"];
            var Key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Secret));

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateLifetime = false,
                        IssuerSigningKey = Key
                    };
                });

            services
                .AddMvcCore()
                .AddApiExplorer()
                .AddAuthorization()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver())
                .AddJsonFormatters();

            services.AddCors(options => options.AddPolicy("SupportPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .WithExposedHeaders("Content-Disposition", "api-version", "content-length", "content-md5", "content-type", "date", "request-id", "response-time");
            }));

            //#region Swagger
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new Info() { Title = "My API", Version = "v1" });
            //    c.AddSecurityDefinition("Bearer", new ApiKeyScheme()
            //    {
            //        In = "header",
            //        Description = "Please enter into field the word 'Bearer' following by space and JWT",
            //        Name = "Authorization",
            //        Type = "apiKey",
            //    });
            //    c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>()
            //    {
            //        {
            //            "Bearer",
            //            Enumerable.Empty<string>()
            //        }
            //    });
            //    c.CustomSchemaIds(i => i.FullName);
            //});
            //#endregion
            //string env = Configuration.GetValue<string>(APIEndpoint.ConnectionString);
            RegisterEndpoint();
            RegisterServices(services);
        }
        private void RegisterServices(IServiceCollection services)
        {
            services
                .AddScoped<IdentityService>()
                .AddScoped<ValidateService>()
                //.AddScoped<IHttpClientService, HttpClientService>()
                .AddScoped<IValidateService, ValidateService>();

            //if (isTest == false)
            //{
            //    services.AddScoped<IHttpClientService, HttpClientService>();
            //}
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseCors("SupportPolicy");
            app.UseMvc();
            //app.UseSwagger();
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
            //});
        }
    }
}