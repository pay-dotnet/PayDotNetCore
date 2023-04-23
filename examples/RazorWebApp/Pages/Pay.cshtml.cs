using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe;

namespace RazorWebApp.Pages
{
    public class PayModel : PageModel
    {
        public PayModel()
        {
        }

        public PaymentIntent PaymentIntent { get; private set; }

        public async Task OnGetAsync(string id)
        {
            var service = new PaymentIntentService();
            PaymentIntent = await service.GetAsync(id);
        }

        public async Task OnPostAsync(string id)
        {
            var service = new PaymentIntentService();
            PaymentIntent = await service.GetAsync(id);
        }
    }
}