using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorWebApp.Pages
{
    public class PayHandleModel : PageModel
    {
        public IActionResult OnGet(
            [FromQuery(Name = "redirect_status")] string redirectStatus,
            [FromQuery(Name = "payment_intent")] string? paymentIntentId,
            [FromQuery(Name = "setup_intent")] string? setupIntentId)
        {
            if (redirectStatus == "succeeded")
            {
                return RedirectToPage("Success");
            }
            else
            {
                string id = !string.IsNullOrEmpty(paymentIntentId)
                    ? paymentIntentId
                    : setupIntentId;
                return RedirectToPage("Pay", new { id = id });
            }
        }
    }
}