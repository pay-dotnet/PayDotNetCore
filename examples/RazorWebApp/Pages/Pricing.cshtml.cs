using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorWebApp.Pages
{
    public class PricingModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}

public static class Data
{
    public record Plan(string Name, string PriceId, decimal Price);

    private static Plan[] _plans = new[]
    {
        new Plan("Basic", "price_1MyDj2JUAL06t0UNphFwwc6l", 249m),
        new Plan("Premium", "price_1MyDj2JUAL06t0UNjnmx5za5", 749m),
        new Plan("Gold", "price_1MyI7PJUAL06t0UNO12eNG9M", 999m)
    };

    public static IDictionary<string, Plan> Plans => _plans.ToDictionary(p => p.PriceId, p => p);
}