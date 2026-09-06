using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace YourNamespace.Controllers
{
    public class PdfMergeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> MergePDFs(List<IFormFile> files)
        {
            if (files == null || files.Count < 2)
            {
                TempData["Error"] = "Please upload at least two PDF files.";
                return RedirectToAction("Index");
            }

            using var outputDocument = new PdfDocument();

            foreach (var file in files)
            {
                if (file.Length > 0 && Path.GetExtension(file.FileName).ToLower() == ".pdf")
                {
                    using var stream = file.OpenReadStream();
                    var inputDocument = PdfReader.Open(stream, PdfDocumentOpenMode.Import);

                    for (int idx = 0; idx < inputDocument.PageCount; idx++)
                    {
                        outputDocument.AddPage(inputDocument.Pages[idx]);
                    }
                }
            }

            using var outputStream = new MemoryStream();
            outputDocument.Save(outputStream, false);
            outputStream.Position = 0;

            return File(outputStream.ToArray(), "application/pdf", "Merged.pdf");
        }
    }
}
