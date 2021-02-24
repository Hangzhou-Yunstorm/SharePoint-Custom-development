using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DaHua.ADRMS {
	public class IOUtility {
		public static void EnsureDirectoryExists(string path) {
			if(!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
		}
		public static string EnsureDirectoryExists(params string[] paths) {
			var path = Path.Combine(paths);
			EnsureDirectoryExists(path);
			return path;
		}
		public static void DeleteDirectory(string physicalPath,bool recursive) {
		
			int count = 0;
			DELETE:
			try {
				if(Directory.Exists(physicalPath)) {
					try {

						Directory.Delete(physicalPath,recursive);
				
					} catch(IOException) {
						Directory.Delete(physicalPath,recursive);
					} catch(InvalidProgramException) {
						Directory.Delete(physicalPath,recursive);
					}
				}
			} catch(Exception exception) {
				count++;
				if(count < 21) {
					goto DELETE;
				}
			}

		}
		public static void DeleteFileIfExists(string physicalPath) {
			var sb = new StringBuilder();
			sb.Append("");
			int count = 0;
			DELETE:
			try {
				if(File.Exists(physicalPath)) {
					try {

						File.Delete(physicalPath);

					} catch(IOException) {
						File.Delete(physicalPath);
					} catch(InvalidProgramException) {
						//Mono does not support FileSystem.DeleteDirectory
						File.Delete(physicalPath);
					}
				}
			} catch {
				count++;
				if(count < 21) {
					goto DELETE;
				}

			}

		}

		public static IEnumerable<DirectoryInfo> EnumerateDirectoriesExludeHidden(string path) {
			var dir = new DirectoryInfo(path);
			return dir.EnumerateDirectories().Where(it => (it.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden);
		}
		public static IEnumerable<FileInfo> EnumerateFilesExludeHidden(string path) {
			var dir = new DirectoryInfo(path);
			return dir.EnumerateFiles().Where(it => (it.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden);
		}
	}
}
