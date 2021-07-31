using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MGP.Models.NetCoreLibrary31.Auth;
using MGP.Models.NetCoreLibrary31.BackOffice;
using MGP.NetCoreLibrary;

namespace MGP.Template.APIService.Services
{
    interface IGlobal
    {
        void SaveLog(Exception exc, string Action);
        APIMessage<Object> CheckHeaderAndUser(bool cekToken);
        MGPLogin_v GetUserAccess();
        APIMessage<Object> IsClient();
        string IPAddress();
        string GetHeader(string header);
        APIMessage<Object> SetErrorStatus(string Message);
        APIMessage<Object> SetErrorStatus(Object _Data, string Message);
        APIMessage<Object> SetErrorStatusCode(Object _Data, string _StrStatusCode, string _StrMessage);
        APIMessage<Object> SetSuccessStatus(string Message);
        APIMessage<Object> SetSuccessStatus(Object _Data, string Message);
        APIMessage<Object> SetSuccessStatusCode(Object _Data, string _StrStatusCode, string _StrMessage);
        APIMessage<Object> SetErrorExcStatus(Object _Data, string _StrMessage);
    }
}
