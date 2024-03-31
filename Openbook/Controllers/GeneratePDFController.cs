using Microsoft.AspNetCore.Mvc;
using Openbook.Data;

namespace Openbook.Controllers
{
	public class GeneratePDFController : Controller
	{
		[HttpGet]
		[Route("DownloadPdf")]
		public IActionResult DownloadPdf(string pageName)
		{
			var pdf = new GeneratePDF($"http://localhost:5022/{pageName}");
			var pdFile = pdf.GetPdf();

			var pdfStream = new MemoryStream(pdFile);
			return new FileStreamResult(pdfStream, "application/pdf");
		}
	}
}
