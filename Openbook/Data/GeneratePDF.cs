using System.Diagnostics;

namespace Openbook.Data
{
	public class GeneratePDF
	{
		private string url { get; set; }
		public GeneratePDF(string _url)
		{
			this.url = _url;
		}
		public byte[] GetPdf()
		{
			var switches = $"-q {url} -";

			string rotativePath = Path.Combine(Directory.GetCurrentDirectory(), "Rotativa", "wkhtmltopdf.exe");
			//Path.Combine(Directory.GetCurrentDirectory(), "Rotativa", "wkhtmltopdf.exe");

			using (var proc = new Process())
			{
				try
				{
					proc.StartInfo = new ProcessStartInfo
					{
						FileName = rotativePath,
						Arguments = switches,
						UseShellExecute = false,
						RedirectStandardOutput = true,
						RedirectStandardError = true,
						RedirectStandardInput = true,
						CreateNoWindow = true
					};
					proc.Start();
				}
				catch (Exception ex)
				{
					throw ex;
				}

				using (var ms = new MemoryStream())
				{
					proc.StandardOutput.BaseStream.CopyTo(ms);
					return ms.ToArray();
				}
			}
		}
	}
}
