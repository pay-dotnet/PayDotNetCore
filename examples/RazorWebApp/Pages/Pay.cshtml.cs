using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PayDotNet.Core;
using PayDotNet.Core.Abstraction;
using PayDotNet.Stores;

namespace RazorWebApp.Pages
{
    public class PayModel : PageModel
    {
        private readonly IBillableManager _billableManager;
        private readonly IPaymentProcessorService _paymentProcessorService;

        public PayModel(
            IBillableManager billableManager,
            IPaymentProcessorService paymentProcessorService)
        {
            _billableManager = billableManager;
            _paymentProcessorService = paymentProcessorService;
        }

        [BindProperty]
        public string Email { get; set; }

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
                PayCustomer payCustomer = await _billableManager.GetOrCreateCustomerAsync("dotnetfromthemountain@gmail.com", new(PaymentProcessors.Stripe));
                Payment = await _paymentProcessorService.GetPaymentAsync(payCustomer, id);
            }
            return Page();
        }

        public async Task<IActionResult> OnPost(
            [FromForm(Name = "payment_method_id")] string paymentMethodId)
        {
            await _billableManager.GetOrCreateCustomerAsync(Email, new(PaymentProcessors.Stripe, paymentMethodId));
            return Page();
        }
    }
}