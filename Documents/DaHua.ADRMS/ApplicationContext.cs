using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaHua.ADRMS {
	public class ApplicationContext {

	public static string UserName {
			get {
				return AppDomain.CurrentDomain.GetData("RmsUserId").ToString(); 
            }
			set {
				AppDomain.CurrentDomain.SetData("RmsUserId",value);
			}
		}
	}
}
