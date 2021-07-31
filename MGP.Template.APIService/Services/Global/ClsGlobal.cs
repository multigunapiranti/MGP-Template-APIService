using MGP.Template.APIService.Data;
using MGP.Template.APIService.Models;
using MGP.Models.NetCoreLibrary31.BackOffice;
using MGP.Models.NetCoreLibrary31.Log;
using MGP.NetCoreLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using MGP.Models.NetCoreLibrary31.Auth;

namespace MGP.Template.APIService.Services
{
    public class ClsGlobal : IGlobal
    {
        private IConfiguration _config;
        private IHttpContextAccessor _ihttpContextAccessor;
        private TemplateContext _TemplateContext;
        private LogContext _logContext;
        private StringValues _strHeaderValue;
        private MGPLogin_v _UserLogin;
        private DateTime _nOw;

        private string[] _strModifyStatus = { "I", "U" };

        public ClsGlobal()
        {
            _nOw = DateTime.Now;
        }
        public ClsGlobal(ParameterConfig param)
        {
            _config = param.PrmIConfiguration;
            _TemplateContext = param.PrmTemplateContext;
            _logContext = param.PrmLogContext;
            _ihttpContextAccessor = param.PrmIHttpContextAccessor;

            _nOw = DateTime.Now;
        }

        public void SaveLog(Exception exc, string Action)
        {
            try
            {
                if (exc == null)
                    exc = new Exception();

                string strErrMsg = "ERROR : " + exc.Message;
                string strStackTrace = "STACK : " + exc.StackTrace;
                string strInnerExcp = (exc.InnerException != null ? "ERROR : " + exc.InnerException.Message + " stack " + exc.InnerException.StackTrace : "");

                ErrorException_TH log = new ErrorException_TH();
                log.ip_address = IPAddress();
                log.action = Action;
                log.error_message = strErrMsg;
                log.stack_trace = strStackTrace;
                log.inner_exception = strInnerExcp;
                log.system_name = "MGP.Template.APIService";
                log.last_modified_time = DateTime.Now;
                _logContext.ErrorException_THSet.Add(log);
                _logContext.SaveChanges(true);
            }
            catch (Exception ex)
            {
                var provider = new Log4NetProvider("log4net.config");
                var logger = provider.CreateLogger(ex.Message);
            }
        }

        public APIMessage<Object> CheckHeaderAndUser(bool cekToken)
        {
            try
            {
                // Get Header Client //
                if (IsClient().IsSuccess)
                {
                    if (!(bool)IsClient().Data.GetType().GetProperty("isAny").GetValue(IsClient().Data))
                        return SetErrorStatus(null, "No Authorization!");
                }
                else
                    return SetErrorStatus(null, "No Authorization!");

                if (!cekToken)
                    return SetSuccessStatus(null, "Ok fine!");

                // Get User By token //
                GetUserAccess();

                if (_UserLogin == null)
                    return SetErrorStatus(null, "User not found!");

                return SetSuccessStatus(_UserLogin, "Ok fine!");
            }
            catch (Exception exc)
            {
                SaveLog(exc, "CekHeaderAndUser");
                return SetErrorStatus(null, "User not found!");
            }
        }

        public MGPLogin_v GetUserAccess()
        {
            _UserLogin = null;
            try
            {
                if (string.IsNullOrEmpty(GetHeader(APIHeader.AUTHORIZATION)))
                    return _UserLogin;

                var token = new JwtSecurityToken(jwtEncodedString: GetHeader(APIHeader.AUTHORIZATION));

                string UserGuid = token.Claims.FirstOrDefault(x => x.Type == "guid").Value;

                var qID = from l in _TemplateContext.MGPLogin_vSet
                          where l.id == UserGuid
                          && _strModifyStatus.Contains(l.modify_status)
                          select l;

                if (qID.Any())
                    _UserLogin = qID.FirstOrDefault();
            }
            catch (Exception exc)
            {
                _UserLogin = null;
                SaveLog(exc, "GetUserAccess");
            }
            return _UserLogin;
        }

        public APIMessage<Object> IsClient()
        {
            APIMessage<Object> _result = new APIMessage<Object>();
            var response = new { isAny = false };
            _result = SetSuccessStatus(response, "Platform not registered!");
            try
            {
                if (!string.IsNullOrEmpty(GetHeader(APIHeader.X_CLIENT)))
                {
                    response = new { isAny = true };
                    _result = SetSuccessStatus(response, "Platform registered!");
                }
            }
            catch (Exception exc)
            {
                _result = SetErrorStatus(response, "Error cek platform!");
                SaveLog(exc, "IsClient");
            }
            return _result;
        }

        public string IPAddress()
        {
            return _ihttpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
        }

        public string GetHeader(string header)
        {
            string strHeader = header.ToString();
            switch (strHeader)
            {
                case APIHeader.AUTHORIZATION:
                    if (!_ihttpContextAccessor.HttpContext.Request.Headers.TryGetValue(APIHeader.AUTHORIZATION, out _strHeaderValue))
                        _strHeaderValue = string.Empty;
                    break;
                case APIHeader.X_CLIENT:
                    if (!_ihttpContextAccessor.HttpContext.Request.Headers.TryGetValue(APIHeader.X_CLIENT, out _strHeaderValue))
                        _strHeaderValue = string.Empty;
                    break;
                case APIHeader.X_OUTLET:
                    if (!_ihttpContextAccessor.HttpContext.Request.Headers.TryGetValue(APIHeader.X_OUTLET, out _strHeaderValue))
                        _strHeaderValue = string.Empty;
                    break;
                case APIHeader.X_OTHER:
                    if (!_ihttpContextAccessor.HttpContext.Request.Headers.TryGetValue(APIHeader.X_OTHER, out _strHeaderValue))
                        _strHeaderValue = string.Empty;
                    break;
            }
            return _strHeaderValue;
        }

        public APIMessage<Object> SetErrorStatus(string _StrMessage = "")
        {
            return ClsLibrary.SetErrorStatus(_StrMessage);
        }

        public APIMessage<Object> SetErrorStatus(Object _Data = null, string _StrMessage = "")
        {
            return ClsLibrary.SetErrorStatus(_Data, _StrMessage);
        }

        public APIMessage<Object> SetErrorStatusCode(Object _Data = null, string _StrStatusCode = "99", string _StrMessage = "")
        {
            return ClsLibrary.SetErrorStatus(_Data, _StrStatusCode, _StrMessage);
        }

        public APIMessage<Object> SetSuccessStatus(string _StrMessage = "")
        {
            return ClsLibrary.SetSuccessStatus(_StrMessage);
        }

        public APIMessage<Object> SetSuccessStatus(Object _Data = null, string _StrMessage = "")
        {
            return ClsLibrary.SetSuccessStatus(_Data, _StrMessage);
        }

        public APIMessage<Object> SetSuccessStatusCode(Object _Data = null, string _StrStatusCode = "00", string _StrMessage = "")
        {
            return ClsLibrary.SetSuccessStatus(_Data, _StrStatusCode, _StrMessage);
        }

        public APIMessage<Object> SetErrorExcStatus(Object _Data = null, string _StrMessage = "")
        {
            return ClsLibrary.SetErrorExcStatus(_Data, _StrMessage);
        }

        public APIMessage<Object> SetErrorExcStatus(string _StrMessage = "")
        {
            return ClsLibrary.SetErrorExcStatus(_StrMessage);
        }
    }
}
