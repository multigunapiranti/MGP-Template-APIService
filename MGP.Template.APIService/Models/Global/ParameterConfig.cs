using MGP.Template.APIService.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGP.Template.APIService.Models
{
    public class ParameterConfig
    {
        public TemplateContext PrmTemplateContext { get; set; }
        public LogContext PrmLogContext { get; set; }
        public IConfiguration PrmIConfiguration { get; set; }
        public IHttpContextAccessor PrmIHttpContextAccessor { get; set; }
        public Variable PrmVariable { get; set; }
    }
}
