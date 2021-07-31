using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using MGP.Template.APIService.Data;
using MGP.Template.APIService.Jwt;
using MGP.Template.APIService.Models;
using MGP.Template.APIService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace MGP.Template.APIService.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TemplateController : ControllerBase
    {
        private ClsServices _ClsServices;
        private ClsGlobal _ClsGlobal;
        private ParameterConfig _parameterConfig;

        public TemplateController(Variable variable
            , IConfiguration config
            , IHttpContextAccessor httpContextAccessor
            , TemplateContext authContext
            , LogContext logContext)
        {
            _parameterConfig = new ParameterConfig();
            _parameterConfig.PrmVariable = variable;
            _parameterConfig.PrmTemplateContext = authContext;
            _parameterConfig.PrmLogContext = logContext;
            _parameterConfig.PrmIConfiguration = config;
            _parameterConfig.PrmIHttpContextAccessor = httpContextAccessor;
            _ClsServices = new ClsServices(_parameterConfig);
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(_ClsGlobal.SetSuccessStatus("Template API"));
            }
            catch (Exception)
            {
                return StatusCode(500, _ClsGlobal.SetErrorStatus("Error Exception!"));
            }
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}