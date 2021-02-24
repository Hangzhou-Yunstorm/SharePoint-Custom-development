using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentsSP.Model
{
    public class GroupModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string members { get; set; }
        public string icon { get; set; }
        public string hideName { get; set; }
        public string operation { get; set; }
    }
}
