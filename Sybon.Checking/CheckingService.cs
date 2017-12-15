using System;
using Microsoft.AspNetCore.Hosting;
using Sybon.Common;

namespace Sybon.Checking
{
    public class CheckingService : BaseService
    {
        public CheckingService(Func<string[], IWebHost> buildWebHostFunc) : base(buildWebHostFunc)
        {
        }

        public override string ServiceName => "Sybon.Checking";
    }
}