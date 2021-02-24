using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaHua.ADRMS {
    public class CaseModel
    {

        public Guid Id { get; set; }
        public string FCaseName { get; set; }
        public bool FIsFullControl { get; set; }
        public bool FIsRead { get; set; }
        public bool FIsPrint { get; set; }
        public bool FIsSave { get; set; }
        public bool FIsWrite { get; set; }
        public List<UserModel> UserModels { get; set; }
    }
}
