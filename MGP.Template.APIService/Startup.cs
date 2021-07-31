using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MGP.Template.APIService.Data;
using MGP.Template.APIService.Middleware;
using MGP.Template.APIService.Models;
using MGP.Template.APIService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
//Begin, Linux
using Microsoft.AspNetCore.HttpOverrides;
//End, Linux

namespace MGP.Template.APIService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string _strDefaultConnection = Configuration.GetConnectionString("DefaultConnection");
            string _strLogtConnection = Configuration.GetConnectionString("LogConnection");
            string _strUrlEPV = Configuration["UrlEPV"];
            string _strDatabaseName = Configuration["DatabaseName"];
            string _strAPIService = Configuration["APIService"];
            int _nTimeOut = Convert.ToInt32(Configuration["QueryTimeOut"]);
            int _nMaxRetryEPV = Convert.ToInt16(Configuration["MaxRetryEPV"]);
            bool _bIsEPV = Convert.ToBoolean(Configuration["IsEPV"]);

            Action<Variable> global = (opt =>
            {
                opt.TemplateConnectionString = _strDefaultConnection;
                opt.LogConnectionString = _strLogtConnection;
                opt.UrlEPV = _strUrlEPV;
                opt.DatabaseName = _strDatabaseName;
                opt.APIService = _strAPIService;
                opt.QueryTimeOut = _nTimeOut;
                opt.IsEPV = _bIsEPV;
            });

            services.Configure(global);
            services.AddScoped(resolver => resolver.GetRequiredService<IOptions<Variable>>().Value);

            if (_bIsEPV)
            {
                _strDefaultConnection = "";
                _strLogtConnection = "";
            }

            //services.AddDbContext<TemplateContext>(options =>
            //     options.UseMySQL(_strDefaultConnection));
            //services.AddDbContext<LogContext>(options =>
            //     options.UseMySQL(_strLogtConnection));

            services.AddScoped(resolver => resolver.GetRequiredService<IOptions<ParameterConfig>>().Value);
            services.AddScoped<IServices, ClsServices>();
            services.AddScoped<IGlobal, ClsGlobal>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<ClsAPILogService>();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyOrigin",
                   builder => builder.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader());
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("AllowAnyOrigin");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            #region Middleware
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
            #endregion Middleware

            //Begin, Linux
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            //End, Linux
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
