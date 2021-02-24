using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentsSP.Model
{
    public class MembersModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
    }

    public class EditMembersModel
    {
        public string account { get; set; }
        public string name { get; set; }
    }
}
