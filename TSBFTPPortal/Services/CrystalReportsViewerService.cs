using Serilog;
using System;
using System.Diagnostics;
using System.IO;

namespace TSBFTPPortal.Services
{
	public class CrystalReportsViewerService
	{
		private string _filePath { get; set; }
		private string _programPath { get; set; }

		public CrystalReportsViewerService(string filePath)
		{
			_filePath = filePath;
			_programPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources", "CrystalReportsViewer_02.exe");
		}

		public void ExecuteProgram()
		{
			string arguments = $"\"{_filePath}\"";

			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.FileName = _programPath;
			startInfo.Arguments = arguments;

			try
			{
				using (Process? process = Process.Start(startInfo))
				{
					if (process != null)
					{
						process.WaitForExit();
						int exitCode = process.ExitCode;
						Log.Information($"Crystal Reports Viewer successful");
					}
					else
					{
						Log.Error("Crystal Report Viewer process is null!");
					}
				}
			}
			catch (Exception ex)
			{
				// Handle any exceptions that occur during process execution
				Log.Error($"Error executing Crystal Reports Viewer: {ex.Message}");
			}
		}

	}
}
