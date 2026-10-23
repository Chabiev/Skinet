using API.Errors;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Order = Core.Entities.OrderAggregate.Order;

namespace API.Controllers;

public class PaymentsController : BaseApiController
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentsController> _logger;
    private const string WhSecret = "whsec_68d86104e23636f7d57207c9f990351cbd0caee3a0f2d9f1241d7c76f6619c8f";

    public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    [Authorize]
    [HttpPost("{basketId}")]
    public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
    {
        var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);
        
        if (basket == null) return BadRequest(new ApiResponse(400, "Problem with your basket"));

        return basket;
    }

    // [HttpPost("webhook")]
    // public async Task<ActionResult> StripeWebhook()
    // {
    //     var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
    //     
    //     var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"],
    //         WhSecret);
    //     PaymentIntent intent;
    //     Order order;
    //
    //     switch (stripeEvent.Type)
    //     {
    //         case "payment_intent.succeeded":
    //             intent = (PaymentIntent)stripeEvent.Data.Object;
    //             _logger.LogInformation("Payment Succeeded: ", intent.Id);
    //             order = await _paymentService.UpdateOrderPaymentSucceeded(intent.Id);
    //             _logger.LogInformation("Order updated to payment received: ", order.Id);
    //             break;
    //         case "payment_intent.payment_failed":
    //             intent = (PaymentIntent)stripeEvent.Data.Object;
    //             _logger.LogInformation("Payment Failed: ", intent.Id);
    //             order = await _paymentService.UpdateOrderPaymentFailed(intent.Id);
    //             _logger.LogInformation("Payment Failed: ", order.Id);
    //             break;
    //     }
    //     
    //     return new EmptyResult();
    // }
    
    [HttpPost("webhook")]
public async Task<ActionResult> StripeWebhook()
{
    var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

    try
    {
        // 1. Pass throwOnApiVersionMismatch: false to ignore version mismatches
        var stripeEvent = EventUtility.ConstructEvent(
            json, 
            Request.Headers["Stripe-Signature"],
            WhSecret,
            throwOnApiVersionMismatch: false
        );

        PaymentIntent intent;
        Order order;

        switch (stripeEvent.Type)
        {
            case "payment_intent.succeeded":
                intent = (PaymentIntent)stripeEvent.Data.Object;
                _logger.LogInformation("Payment Succeeded for Intent ID: {Id}", intent.Id);
                
                order = await _paymentService.UpdateOrderPaymentSucceeded(intent.Id);
                
                if (order == null)
                {
                    _logger.LogError("Order not found for PaymentIntent ID: {Id}", intent.Id);
                    return BadRequest(new ApiResponse(400, "Order not found for this payment intent"));
                }
                
                _logger.LogInformation("Order {OrderId} updated to PaymentReceived", order.Id);
                break;

            case "payment_intent.payment_failed":
                intent = (PaymentIntent)stripeEvent.Data.Object;
                _logger.LogWarning("Payment Failed for Intent ID: {Id}", intent.Id);
                
                order = await _paymentService.UpdateOrderPaymentFailed(intent.Id);
                
                if (order == null)
                {
                    _logger.LogError("Order not found for PaymentIntent ID: {Id}", intent.Id);
                    return BadRequest(new ApiResponse(400, "Order not found for this payment intent"));
                }
                
                _logger.LogInformation("Order {OrderId} updated to PaymentFailed", order.Id);
                break;

            default:
                _logger.LogInformation("Unhandled event type: {Type}", stripeEvent.Type);
                break;
        }

        return new EmptyResult();
    }
    catch (StripeException e)
    {
        _logger.LogError(e, "Stripe Webhook Signature Verification Failed");
        return BadRequest(new ApiResponse(400, "Webhook Error"));
    }
    catch (Exception e)
    {
        _logger.LogError(e, "An unexpected error occurred while processing the Stripe Webhook");
        return StatusCode(500, new ApiResponse(500, e.Message));
    }
}
}