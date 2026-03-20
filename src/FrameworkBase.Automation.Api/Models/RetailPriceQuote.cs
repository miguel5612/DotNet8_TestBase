using System.Text.Json.Serialization;

namespace FrameworkBase.Automation.Api.Models;

/// <summary>
/// Represents the simulated outcome of a retail pricing request.
/// Input: created by the retail pricing engine after processing a cart.
/// Output: a response body that explains the commercial quote.
/// Business case: pricing tests should prove that promotions, shipping, and totals match the business expectation.
/// </summary>
public sealed class RetailPriceQuote
{
    /// <summary>
    /// Gets the identifier assigned to the generated quote.
    /// Output: a traceable quote reference.
    /// Business case: retail platforms use quote identifiers to connect pricing, checkout, and order management.
    /// </summary>
    [JsonPropertyName("quoteId")]
    public string QuoteId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the business status for the quote.
    /// Output: values such as Quoted or Rejected.
    /// Business case: the checkout flow needs an explicit pricing outcome before continuing to payment.
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Gets the discount amount applied to the subtotal.
    /// Output: the monetary value discounted from the cart.
    /// Business case: promotions should be measurable so they can be verified by automated tests and analytics.
    /// </summary>
    [JsonPropertyName("discountAmount")]
    public decimal DiscountAmount { get; init; }

    /// <summary>
    /// Gets the shipping fee charged to the customer.
    /// Output: the delivery charge after applying fulfillment rules.
    /// Business case: free shipping is often a critical acceptance criterion in retail campaigns.
    /// </summary>
    [JsonPropertyName("shippingFee")]
    public decimal ShippingFee { get; init; }

    /// <summary>
    /// Gets the final total after discounts and shipping.
    /// Output: the amount the customer is expected to pay.
    /// Business case: automation must prove that the checkout total respects both discount and shipping policies.
    /// </summary>
    [JsonPropertyName("finalTotal")]
    public decimal FinalTotal { get; init; }

    /// <summary>
    /// Gets the promotion label applied by the pricing engine.
    /// Output: a promotion identifier or None.
    /// Business case: marketing teams need to confirm which campaign was triggered for a quote.
    /// </summary>
    [JsonPropertyName("promotionApplied")]
    public string PromotionApplied { get; init; } = string.Empty;

    /// <summary>
    /// Gets the fulfillment type selected for the order.
    /// Output: values such as StorePickup or HomeDelivery.
    /// Business case: shipping assertions are easier to understand when the response states the active fulfillment path.
    /// </summary>
    [JsonPropertyName("fulfillmentType")]
    public string FulfillmentType { get; init; } = string.Empty;

    /// <summary>
    /// Gets the business message shown to the consumer.
    /// Output: a human-readable commercial summary.
    /// Business case: customer support and product flows can reuse the same message validated by automation.
    /// </summary>
    [JsonPropertyName("businessMessage")]
    public string BusinessMessage { get; init; } = string.Empty;
}
