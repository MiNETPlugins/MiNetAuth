using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace MiNetAuth.Config
{
    public class ConfigManager : MiNETAuth
    {
        protected override void OnEnable()
        {
            _log.Info("ConfigManager has been Loaded...");
        }
        public override void OnDisable()
        {
            _log.Info("ConfigManager has been Disable...");
        }
    }
}
