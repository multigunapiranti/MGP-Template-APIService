using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGP.Template.APIService.Models
{
    public class Variable
    {
        public string TemplateConnectionString { get; set; }
        public string LogConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string UrlEPV { get; set; }
        public string APIService { get; set; }
        public int QueryTimeOut { get; set; }
        public bool IsEPV { get; set; }
    }
}
