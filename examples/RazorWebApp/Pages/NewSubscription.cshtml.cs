using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PayDotNet.Core;
using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;

namespace RazorWebApp.Pages
{
    public class NewSubscriptionModel : PageModel
    {
        public NewSubscriptionModel(IBillableManager billableManager)
        {
            _billableManager = billableManager;
        }

        public void OnGet()
        {
            Price = Plans.First().PriceId;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                PayCustomer payCustomer = await _billableManager.GetOrCreateCustomerAsync(Email, PaymentProcessor);
                PaySubscription paySubscription = await _billableManager.SubscribeAsync(payCustomer, "Basic", Price);
                return RedirectToPage("Success");
            }
            catch (ActionRequiredPayDotNetException e)
            {
                return RedirectToPage("Pay", new { id = e.Payment.Id });
            }
            catch (PayDotNetException e)
            {
                ModelState.AddModelError("", e.Message);
                return Page();
            }
        }

        [BindProperty]
        public string Price { get; set; }

        [BindProperty]
        public string PaymentProcessor { get; set; } = "stripe";

        [BindProperty]
        public string Email { get; set; }

        public Plan[] Plans = new[]
        {
            new Plan("Basic", "price_1MyDj2JUAL06t0UNphFwwc6l", 249m),
            new Plan("Premium", "price_1MyDj2JUAL06t0UNjnmx5za5", 749m),
            new Plan("Gold", "price_1MyI7PJUAL06t0UNO12eNG9M", 999m)
        };

        private readonly IBillableManager _billableManager;
    }

    public record Plan(string Name, string PriceId, decimal Price);
}