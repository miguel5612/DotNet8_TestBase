using FrameworkBase.Automation.Api.Models;

namespace FrameworkBase.Automation.Api.Rules;

/// <summary>
/// Calculates retail pricing results using deterministic mock rules.
/// Input: a <see cref="RetailPriceQuoteRequest"/> with subtotal, customer segment, coupon, and fulfillment options.
/// Output: a <see cref="RetailPriceQuote"/> ready to be returned by the mock API.
/// Business case: the framework can validate commercial rules such as discounts and free shipping without calling a real retail backend.
/// </summary>
public sealed class RetailPricingEngine
{
    private const decimal BaseShippingFee = 12.50m;

    /// <summary>
    /// Calculates a retail quote from the provided cart request.
    /// Input: a retail price quote request.
    /// Output: a quote with promotions, shipping fees, and final total.
    /// Business case: simulate the checkout rules that matter to merchandising, loyalty, and fulfillment teams.
    /// </summary>
    /// <param name="request">The retail request to evaluate.</param>
    /// <returns>The simulated retail quote.</returns>
    public RetailPriceQuote CalculateQuote(RetailPriceQuoteRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateRequest(request);

        var couponDiscount = GetCouponDiscount(request);
        var loyaltyDiscount = GetLoyaltyDiscount(request);
        var discountAmount = Math.Max(couponDiscount, loyaltyDiscount);
        var promotionApplied = GetPromotionName(couponDiscount, loyaltyDiscount);
        var discountedSubtotal = request.Subtotal - discountAmount;
        var shippingFee = GetShippingFee(request, discountedSubtotal);
        var finalTotal = discountedSubtotal + shippingFee;

        return new RetailPriceQuote
        {
            QuoteId = $"QUOTE-{request.OrderId}",
            Status = "Quoted",
            DiscountAmount = discountAmount,
            ShippingFee = shippingFee,
            FinalTotal = finalTotal,
            PromotionApplied = promotionApplied,
            FulfillmentType = request.UseStorePickup ? "StorePickup" : "HomeDelivery",
            BusinessMessage = BuildBusinessMessage(promotionApplied, shippingFee),
        };
    }

    private static void ValidateRequest(RetailPriceQuoteRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.OrderId))
        {
            throw new ArgumentException("The order identifier is required.", nameof(request));
        }

        if (request.Subtotal <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(request), "The subtotal must be greater than zero.");
        }

        if (request.ItemsCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request), "The item count must be greater than zero.");
        }
    }

    private static decimal GetCouponDiscount(RetailPriceQuoteRequest request)
    {
        return string.Equals(request.CouponCode, "WELCOME10", StringComparison.OrdinalIgnoreCase) && request.Subtotal >= 50m
            ? Math.Round(request.Subtotal * 0.10m, 2, MidpointRounding.AwayFromZero)
            : 0m;
    }

    private static decimal GetLoyaltyDiscount(RetailPriceQuoteRequest request)
    {
        return string.Equals(request.CustomerSegment, "Gold", StringComparison.OrdinalIgnoreCase) && request.Subtotal >= 200m
            ? Math.Round(request.Subtotal * 0.15m, 2, MidpointRounding.AwayFromZero)
            : 0m;
    }

    private static string GetPromotionName(decimal couponDiscount, decimal loyaltyDiscount)
    {
        if (loyaltyDiscount > couponDiscount)
        {
            return "Gold15";
        }

        if (couponDiscount > 0m)
        {
            return "Welcome10";
        }

        return "None";
    }

    private static decimal GetShippingFee(RetailPriceQuoteRequest request, decimal discountedSubtotal)
    {
        if (request.UseStorePickup)
        {
            return 0m;
        }

        if (string.Equals(request.CustomerSegment, "Gold", StringComparison.OrdinalIgnoreCase))
        {
            return 0m;
        }

        return discountedSubtotal >= 120m ? 0m : BaseShippingFee;
    }

    private static string BuildBusinessMessage(string promotionApplied, decimal shippingFee)
    {
        return promotionApplied switch
        {
            "Gold15" when shippingFee == 0m => "Gold loyalty pricing applied with free shipping.",
            "Gold15" => "Gold loyalty pricing applied.",
            "Welcome10" when shippingFee == 0m => "Coupon pricing applied with free shipping.",
            "Welcome10" => "Coupon pricing applied.",
            _ when shippingFee == 0m => "Standard pricing applied with free shipping.",
            _ => "Standard pricing applied.",
        };
    }
}
