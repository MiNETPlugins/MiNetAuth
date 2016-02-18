using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MiNetAuth.Config
{
    [DataContract]
    public class Config
    {
        [DataMember]
        public static string Lang { get; set; }



        public Config()
        {

        }
    }
}
