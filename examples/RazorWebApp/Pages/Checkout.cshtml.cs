using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PayDotNet.Core;
using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;

namespace RazorWebApp.Pages
{
    public class CheckoutModel : PageModel
    {
        private readonly IBillableManager _billableManager;

        public CheckoutModel(IBillableManager billableManager)
        {
            _billableManager = billableManager;
        }

        public void OnGet()
        {
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Mode { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            PayCustomer payCustomer = await _billableManager.GetOrCreateCustomerAsync(Email, new(PaymentProcessors.Stripe));

            PayCheckoutResult result = Mode == "checkout"
                ? await _billableManager.CheckoutChargeAsync(payCustomer, new PayCheckoutChargeOptions(name: "T-Shirt", unitAmount: 15_00, currency: "eur", quantity: 2))
                : await _billableManager.CheckoutChargeAsync(payCustomer, new PayCheckoutChargeOptions(LineItems: new() { new(Data.Plans.FirstOrDefault().Key) }, Mode: "subscription"));

            return Redirect(result.CheckoutUrl.ToString());
        }
    }
}