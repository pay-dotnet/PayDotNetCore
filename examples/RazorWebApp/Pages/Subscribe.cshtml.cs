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

        public void OnGet(string priceId)
        {
            SelectedPlan = Data.Plans[priceId];
        }

        public Data.Plan SelectedPlan { get; set; }

        [BindProperty]
        public string Price { get; set; }

        [BindProperty]
        public string PaymentProcessor { get; set; } = "stripe";

        [BindProperty]
        public string Email { get; set; }

        public async Task<IActionResult> OnPostAsync(string priceId)
        {
            SelectedPlan = Data.Plans[priceId];
            try
            {
                PayCustomer payCustomer = await _billableManager.GetOrCreateCustomerAsync(Email, PaymentProcessor);
                PaySubscription paySubscription = await _billableManager.SubscribeAsync(payCustomer, Price, SelectedPlan.Name);
                return RedirectToPage("Success");
            }
            catch (ActionRequiredPayDotNetException e)
            {
                return RedirectToPage("Pay", new { id = e.Payment.Id });
            }
            // TODO: this doesn't make sense.
            catch (InvalidPaymentPayDotNetException e)
            {
                return RedirectToPage("Pay", new { id = e.Payment.Id });
            }
            catch (PayDotNetException e)
            {
                ModelState.AddModelError("", e.Message);
                return Page();
            }
        }
    }
}