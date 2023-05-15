using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PayDotNet.Core;
using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;

namespace RazorWebApp.Pages
{
    public class SubscribeModel : PageModel
    {
        private readonly IBillableManager _billableManager;

        public SubscribeModel(IBillableManager billableManager)
        {
            _billableManager = billableManager;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string PaymentProcessor { get; set; } = "stripe";

        [BindProperty]
        public string Price { get; set; }

        public Data.Plan SelectedPlan { get; set; }

        public void OnGet(string priceId)
        {
            SelectedPlan = Data.Plans[priceId];
        }

        public async Task<IActionResult> OnPostAsync(string priceId)
        {
            SelectedPlan = Data.Plans[priceId];
            try
            {
                PayCustomer payCustomer = await _billableManager.GetOrCreateCustomerAsync(Email, new(PaymentProcessor));
                IPayment payment = await _billableManager.SubscribeAsync(payCustomer, new PaySubscribeOptions(SelectedPlan.Name, Price));
                if (!payment.IsSucceeded())
                {
                    return RedirectToPage("Pay", new { id = payment.Id });
                }

                return RedirectToPage("Success");
            }
            catch (PayDotNetException e)
            {
                ModelState.AddModelError("", e.Message);
                return Page();
            }
        }
    }
}