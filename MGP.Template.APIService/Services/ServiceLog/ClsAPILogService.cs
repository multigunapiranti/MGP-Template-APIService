using MGP.Models.NetCoreLibrary31.Log;
using MGP.Template.APIService.Data;
using MGP.Template.APIService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGP.Template.APIService.Services
{
    public class ClsAPILogService
    {
        private readonly LogContext _db;
        private Variable _variable;
        public ClsAPILogService(LogContext db, Variable variable)
        {
            _db = db;
            _variable = variable;
        }

        public async Task Log(LogAPI_TH apiLogItem)
        {
            _db.LogAPI_THSet.Add(apiLogItem);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<LogAPI_TH>> Get()
        {
            var items = from i in _db.LogAPI_THSet
                        orderby i.id descending
                        select i;

            return await items.ToListAsync();
        }
    }
}
