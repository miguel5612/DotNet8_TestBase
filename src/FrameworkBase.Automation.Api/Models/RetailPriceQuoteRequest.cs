using System.Text.Json.Serialization;

namespace FrameworkBase.Automation.Api.Models;

/// <summary>
/// Describes the input used to simulate retail cart pricing.
/// Input: cart amount, item count, customer segment, coupon, and fulfillment choices.
/// Output: a request payload for the retail mock API.
/// Business case: pricing APIs determine whether discounts and shipping policies align with the active promotion rules.
/// </summary>
public sealed class RetailPriceQuoteRequest
{
    /// <summary>
    /// Gets the order identifier associated with the cart.
    /// Input: a cart or order reference from the retail channel.
    /// Output: a value echoed in artifacts and downstream logs.
    /// Business case: a quote should stay tied to the same commercial transaction across systems.
    /// </summary>
    [JsonPropertyName("orderId")]
    public string OrderId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the customer segment used by the pricing engine.
    /// Input: values such as Standard, Gold, or VIP.
    /// Output: a segment used to evaluate promotions and shipping benefits.
    /// Business case: loyalty programs commonly drive discounts and delivery benefits.
    /// </summary>
    [JsonPropertyName("customerSegment")]
    public string CustomerSegment { get; init; } = "Standard";

    /// <summary>
    /// Gets the cart subtotal before discounts and shipping.
    /// Input: the merchandise total calculated by the storefront.
    /// Output: the base amount used by the pricing engine.
    /// Business case: promotions and thresholds usually depend on the pre-shipping subtotal.
    /// </summary>
    [JsonPropertyName("subtotal")]
    public decimal Subtotal { get; init; }

    /// <summary>
    /// Gets the number of items in the cart.
    /// Input: a positive item count.
    /// Output: a basic data quality value for acceptance tests.
    /// Business case: an empty cart should not produce a valid quote.
    /// </summary>
    [JsonPropertyName("itemsCount")]
    public int ItemsCount { get; init; }

    /// <summary>
    /// Gets the shipping country associated with the quote.
    /// Input: a country or market code.
    /// Output: a value available for regional business rules.
    /// Business case: shipping fees and allowed fulfillment paths may vary by market.
    /// </summary>
    [JsonPropertyName("shippingCountry")]
    public string ShippingCountry { get; init; } = "CO";

    /// <summary>
    /// Gets the optional coupon code applied by the customer.
    /// Input: a coupon string or null when no coupon is used.
    /// Output: a value evaluated by the pricing rules.
    /// Business case: promotional campaigns depend on valid coupon eligibility.
    /// </summary>
    [JsonPropertyName("couponCode")]
    public string? CouponCode { get; init; }

    /// <summary>
    /// Gets a value indicating whether the customer selected store pickup.
    /// Input: <c>true</c> when the order should be collected in store.
    /// Output: a flag evaluated by the shipping policy.
    /// Business case: pickup orders should avoid delivery fees.
    /// </summary>
    [JsonPropertyName("useStorePickup")]
    public bool UseStorePickup { get; init; }
}
