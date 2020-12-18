using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ImperitWASM.Server.Pages
{
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ErrorModel : PageModel
	{
		public string? RequestId { get; set; }
		public void OnGet() => RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
	}
}
