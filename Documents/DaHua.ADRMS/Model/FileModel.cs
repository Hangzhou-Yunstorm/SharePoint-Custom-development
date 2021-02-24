using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaHua.ADRMS {
	public class FileModel {

	public DateTime CreateTime { get; set; }
	public string FFileName { get; set; }
	public string FFolderName { get; set; }
	public bool FIsFullControl { get; set; }
	public bool FIsRead { get; set; }
	public bool FIsPrint { get; set; }
	public bool FIsSave { get; set; }
	public bool FIsWrite { get; set; }
	
	}
}
