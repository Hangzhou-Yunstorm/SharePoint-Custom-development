using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DaHua.ADRMS {
	public class PathUtility {

		public const string CaseJsonFileName = "case.json";
		public static string BaseDirectory {
			get {
				return AppDomain.CurrentDomain.BaseDirectory;
			}
		}
		public static string BaseStoragePath {
			get {
				return IOUtility.EnsureDirectoryExists(BaseDirectory,"Storages");
			}
		}
		public static string BaseStorageFolderPath {
			get {
				return IOUtility.EnsureDirectoryExists(BaseStoragePath,DateTime.Now.ToString("yyyymmhhssfff"));
			}
		}

		public static string CaseJosnPath
		{
		get {
				return Path.Combine(BaseStorageFolderPath,CaseJsonFileName);
		}
		
		}

	}
}
