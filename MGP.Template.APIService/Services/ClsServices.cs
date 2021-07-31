using MGP.Template.APIService.Data;
using MGP.Template.APIService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using MGP.NetCoreLibrary;
using MGP.Models.NetCoreLibrary31.Log;
using MGP.Models.NetCoreLibrary31.BackOffice;
using MGP.Models.NetCoreLibrary31.Auth;

namespace MGP.Template.APIService.Services
{
    public class ClsServices : IServices
    {
        private IConfiguration _config;
        private IHttpContextAccessor _ihttpContextAccessor;
        private TemplateContext _TemplateContext;
        private LogContext _logContext;
        private MGPLogin_v _UserLogin;
        private Variable _variable;
        private ClsGlobal _ClsGlobal;

        private DateTime _nOw;

        private string[] _strModifyStatus = { "I", "U" };

        public ClsServices(ParameterConfig param)
        {
            _config = param.PrmIConfiguration;
            _TemplateContext = param.PrmTemplateContext;
            _logContext = param.PrmLogContext;
            _ihttpContextAccessor = param.PrmIHttpContextAccessor;
            _variable = param.PrmVariable;
            _UserLogin = new MGPLogin_v();

            _nOw = DateTime.Now;

            _ClsGlobal = new ClsGlobal(param);
        }
        private APIMessage<Object> GetUser(bool cekToken)
        {
            try
            {
                APIMessage<Object> Header = _ClsGlobal.CheckHeaderAndUser(cekToken);
                if (!Header.IsSuccess || Header.Data == null)
                    return Header;

                if (cekToken)
                    _UserLogin = null;

                if (Header.Data != null)
                    _UserLogin = (MGPLogin_v)Header.Data;

                return _ClsGlobal.SetSuccessStatus(_UserLogin, "Successfully.");
            }
            catch (Exception exc)
            {
                _ClsGlobal.SaveLog(exc, "GetUser");
                return _ClsGlobal.SetErrorExcStatus(new MGPLogin_v(), "Error, Try again.");
            }
        }
    }
}
