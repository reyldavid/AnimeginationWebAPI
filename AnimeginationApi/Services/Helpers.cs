using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AnimeginationApi.Models;

namespace AnimeginationApi.Services
{
    public static class Helpers
    {
        public static JwtKeys GetJwtKeys()
        {
            JwtKeys keys = new JwtKeys()
            {
                InPrivateKey = "InPrivateKey".GetConfigurationValue(),
                InPublicKey = "InPublicKey".GetConfigurationValue(),
                OutPrivateKey = "OutPrivateKey".GetConfigurationValue(),
                OutPublicKey = "OutPublicKey".GetConfigurationValue()
            };

            return keys;
        }

        public static double GetTaxes(double subTotal)
        {
            double taxRate = "CurrentTaxRate".GetConfigurationDoubleValue();
            double taxes = subTotal * taxRate / 100;
            return taxes;
        }

        public static double GetDiscounts(double subTotal)
        {
            double discountRate = "TodaysDiscountRate".GetConfigurationDoubleValue();
            double discounts = subTotal * discountRate / 100;
            return discounts;
        }

        public static double GetShippingAndHandling(double subtotal, int quantity)
        {
            double shippingAndHandling = 0.0;
            double shippingFirstItem = "ShippingFirstItem".GetConfigurationDoubleValue();
            double shippingAdditionalItems = "ShippingAdditionalItems".GetConfigurationDoubleValue();
            double shippingFreeThreshold = "ShippingFreeThreshold".GetConfigurationDoubleValue();

            if (quantity > 0)
            {
                shippingAndHandling = shippingFirstItem + ((quantity - 1) * shippingAdditionalItems);
            }
            if ((shippingFreeThreshold > 0) &&
                (subtotal > shippingFreeThreshold))
            {
                shippingAndHandling = 0.0;
            }
            return shippingAndHandling;
        }

        public static string GetTrackingNumber()
        {
            string trackingNumber = "TrackingNumberPrefix".GetConfigurationValue() + new Random().Next(1, 99999999).ToString("D8");
            return trackingNumber;
        }

        public static void UpdateOrder(ref Order order)
        {
            double subTotal = order.OrderItems.Sum(item => item.Product.YourPrice * item.Quantity);

            order.ShippingHandling = Math.Round(Helpers.GetShippingAndHandling(subTotal, order.OrderItems.Sum(item => item.Quantity)), 2);
            order.Taxes = Math.Round(Helpers.GetTaxes(subTotal), 2);
            order.Discounts = Math.Round(Helpers.GetDiscounts(subTotal), 2);

            order.Totals = Math.Round(subTotal + order.ShippingHandling + order.Taxes - order.Discounts, 2);
            order.TrackingNumber = Helpers.GetTrackingNumber();
            order.OrderDate = DateTime.Now;
            order.IsPurchased = order.OrderTypeID == 2;
        }

    }
}