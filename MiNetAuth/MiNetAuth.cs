using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;

using MiNET;
using MiNET.Net;
using MiNET.Security;
using MiNET.Plugins;
using MiNET.Plugins.Attributes;
using MiNetAuth.PackageHandler;

using log4net;
using Microsoft.AspNet.Identity;
using MiNET.Utils;

namespace MiNetAuth
{
    public class MiNETAuth : Plugin
    {
        protected static ILog _log = LogManager.GetLogger(typeof(MiNETAuth));

        protected override void OnEnable()
        {
            Context.Server.UserManager = new UserManager<User>(new DefaultUserStore());
        }

        
    }
}
