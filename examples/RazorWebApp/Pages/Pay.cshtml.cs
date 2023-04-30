using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PayDotNet.Core.Models;
using PayDotNet.Core.Services;

namespace RazorWebApp.Pages
{
    public class PayModel : PageModel
    {
        private readonly IPaymentProcessorService _paymentProcessorService;

        public PayModel(IPaymentProcessorService paymentProcessorService)
        {
            _paymentProcessorService = paymentProcessorService;
        }

        public IPayment? Payment { get; set; }

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToPage(new { id = "setup" });
            }
            if (id == "setup")
            {
                return Page();
            }
            if (!string.IsNullOrEmpty(id))
            {
                Payment = await _paymentProcessorService.GetPaymentAsync(id);
            }
            return Page();
        }

        public void OnPost()
        {
        }
    }
}