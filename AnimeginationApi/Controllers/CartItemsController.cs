﻿using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Http;
using AnimeginationApi.Models;
using Swashbuckle.Swagger.Annotations;
using AnimeginationApi.Filters;
using System.Threading.Tasks;
using AnimeginationApi.Services;

namespace AnimeginationApi.Controllers
{
    public class CartItemsController : ApiController
    {
        private AnimeDB db = new AnimeDB();

        // GET: api/CartItems
        [JwtTokenFilter]
        [SwaggerOperation("GetCartItems")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IHttpActionResult> GetCartItems()
        {
            string userId = Request.UserId();

            var items = 
                db.Orders
                .Join(
                    db.OrderTypes,
                    o => o.OrderTypeID, ot => ot.OrderTypeID, (o, ot) => new
                    {
                        o.OrderID,
                        o.UserId,
                        ot.OrderTypeID,
                        ot.OrderName
                    })
                    .Join(
                    db.OrderItems,
                    o => o.OrderID, oi => oi.OrderID, (o, oi) => new
                    {
                        o.OrderID,
                        o.UserId,
                        o.OrderTypeID,
                        o.OrderName,
                        oi.OrderItemID,
                        oi.ProductID,
                        oi.Quantity,
                        oi.FinalUnitPrice
                    })
                    .Join(
                        db.Products,
                        oi => oi.ProductID, p => p.ProductID, (oi, p) => new
                        {
                            oi.OrderID,
                            oi.UserId,
                            oi.OrderTypeID,
                            oi.OrderName,
                            oi.OrderItemID,
                            oi.ProductID,
                            oi.Quantity,
                            oi.FinalUnitPrice,
                            p.Category.CategoryName,
                            p.Medium.MediumName,
                            p.ProductAgeRating,
                            p.ProductCode,
                            p.ProductLength,
                            p.ProductTitle,
                            p.ProductYearCreated,
                            p.Publisher.PublisherName,
                            p.RatingID,
                            p.UnitPrice,
                            p.YourPrice
                        })
                        .Where(p => p.UserId == userId)
                        .AsEnumerable();

            return Ok(items);
        }

        // GET: api/CartItems/{carttype:string}
        [Route("api/cartitems/{carttype}", Name = "GetCartItemsByType")]
        [JwtTokenFilter]
        [SwaggerOperation("GetCartItemsByType")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IHttpActionResult> GetCartItems(string carttype)
        {
            string userId = Request.UserId();

            // Clean up the Duplicates first!
            var totals = from os in db.Orders
                         join ois in db.OrderItems on os.OrderID equals ois.OrderID
                         where os.UserId == userId && os.OrderType.OrderName == carttype.ToLower()
                         group ois by ois.ProductID into agg
                         select new
                         {
                             ProductID = agg.Key,
                             Quantity = agg.Sum(ois => ois.Quantity)
                         };

            var firstOrder = db.Orders.Where(ord => ord.UserId.Equals(userId) &&
                ord.OrderType.OrderName.ToLower().Equals(carttype.ToLower()))
                .OrderBy(ord => ord.OrderID).FirstOrDefault();

            IQueryable<Order> duplicateOrders = db.Orders.Where(ord => ord.UserId.Equals(userId) &&
                ord.OrderType.OrderName.ToLower().Equals(carttype.ToLower()))
                .OrderBy(ord => ord.OrderID).Skip(1);

            foreach (var total in totals)
            {
                OrderItem firstItem = db.OrderItems.Where(fi => fi.OrderID.Equals(firstOrder.OrderID) &&
                    fi.ProductID.Equals(total.ProductID)).OrderBy(fi => fi.OrderItemID).FirstOrDefault();

                if (firstItem != null)
                {
                    firstItem.Quantity = total.Quantity;
                }
            }

            foreach (var total in totals)
            {
                IQueryable<OrderItem> duplicateItems = db.OrderItems.Where(fi => fi.OrderID.Equals(firstOrder.OrderID) &&
                fi.ProductID.Equals(total.ProductID)).OrderBy(fi => fi.OrderItemID).Skip(1);

                foreach (OrderItem dupe in duplicateItems)
                {
                    db.OrderItems.Remove(dupe);
                }
            }

            foreach (Order dupe in duplicateOrders)
            {
                db.Orders.Remove(dupe);
            }

            await db.SaveChangesAsync();

            // Duplicates have been removed at this point
            var items = 
                db.Orders
                .Join(
                    db.OrderTypes,
                    o => o.OrderTypeID, ot => ot.OrderTypeID, (o, ot) => new
                    {
                        o.OrderID,
                        o.UserId,
                        ot.OrderTypeID,
                        ot.OrderName
                    })
                    .Join(
                    db.OrderItems,
                    o => o.OrderID, oi => oi.OrderID, (o, oi) => new
                    {
                        o.OrderID,
                        o.UserId,
                        o.OrderTypeID,
                        o.OrderName,
                        oi.OrderItemID,
                        oi.ProductID,
                        oi.Quantity,
                        oi.FinalUnitPrice
                    })
                    .Join(
                        db.Products,
                        oi => oi.ProductID, p => p.ProductID, (oi, p) => new
                        {
                            oi.OrderID,
                            oi.UserId,
                            oi.OrderTypeID,
                            oi.OrderName,
                            oi.OrderItemID,
                            oi.ProductID,
                            oi.Quantity,
                            oi.FinalUnitPrice,
                            p.Category.CategoryName,
                            p.Medium.MediumName,
                            p.ProductAgeRating,
                            p.ProductCode,
                            p.ProductLength,
                            p.ProductTitle,
                            p.ProductYearCreated,
                            p.Publisher.PublisherName,
                            p.RatingID,
                            p.UnitPrice,
                            p.YourPrice
                        })
                        .Where(p => p.UserId == userId &&
                                p.OrderName.ToLower().Equals(carttype.ToLower()))
                        .AsEnumerable();

            return Ok(items);
        }

        // GET: api/CartItems/5
        [JwtTokenFilter]
        [SwaggerOperation("GetCartItem")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IHttpActionResult> GetCartItems(int id)
        {
            string userId = Request.UserId();

            var items =
                db.Orders
                .Join(
                    db.OrderItems,
                    o => o.OrderID, oi => oi.OrderID, (o, oi) => new
                    {
                        o.OrderID,
                        o.UserId,
                        o.OrderTypeID,
                        oi.OrderItemID,
                        oi.ProductID,
                        oi.Quantity,
                        oi.FinalUnitPrice
                    })
                    .Join(
                        db.Products,
                        oi => oi.ProductID, p => p.ProductID, (oi, p) => new
                        {
                            oi.OrderID,
                            oi.UserId,
                            oi.OrderTypeID,
                            oi.OrderItemID,
                            oi.ProductID,
                            oi.Quantity,
                            oi.FinalUnitPrice,
                            p.Category.CategoryName,
                            p.Medium.MediumName,
                            p.ProductAgeRating,
                            p.ProductCode,
                            p.ProductLength,
                            p.ProductTitle,
                            p.ProductYearCreated,
                            p.Publisher.PublisherName,
                            p.RatingID,
                            p.UnitPrice,
                            p.YourPrice
                        })
                        .Where(p => p.UserId == userId && p.OrderTypeID == id)
                        .AsEnumerable();

            return Ok(items);
        }

        // GET: api/CartTotals/5
        [Route("api/carttotals")]
        [JwtTokenFilter]
        [SwaggerOperation("GetCartTotals")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IHttpActionResult> GetCartTotals(string cartType)
        {
            string userId = Request.UserId();

            //OrderType orderType = db.OrderTypes.SingleOrDefault(
            //    ordtype => ordtype.OrderName.ToLower().Equals(cartType.ToLower()));

            var orders = db.Orders.Where(ord => ord.UserId.Equals(userId) &&
                ord.OrderType.OrderName.ToLower().Equals(cartType.ToLower()));

            foreach (var ord in orders) {
                var orderItems = db.OrderItems.Where(oi => oi.OrderID.Equals(ord.OrderID));

                foreach (var oi in orderItems) {
                    // HEY REY!! REMOVE DUPLICATE ORDER ITEMS HERE!
                }
            }

            var firstOrder = db.Orders.Where(ord => ord.UserId.Equals(userId) &&
                ord.OrderType.OrderName.ToLower().Equals(cartType.ToLower())).FirstOrDefault();

            var duplicateOrders = db.Orders.Where(ord => ord.UserId.Equals(userId) &&
                ord.OrderType.OrderName.ToLower().Equals(cartType.ToLower()))
                .GroupBy(ord => ord.OrderType.OrderName);

            foreach(var dupe in duplicateOrders) {
                var nani = dupe;
            }

            foreach(var ord in orders)
            {
                Order order = db.Orders.Where(o => o.OrderID == ord.OrderID).FirstOrDefault();

                Helpers.UpdateOrder(ref order);
            }
            db.SaveChanges();

            var totals = db.Orders.Select(ord => new
            {
                OrderID = ord.OrderID,
                UserId = ord.UserId,
                ShippingHandling = ord.ShippingHandling,
                Taxes = ord.Taxes,
                Discounts = ord.Discounts,
                Totals = ord.Totals,
                OrderDate = ord.OrderDate,
                IsPurchased = ord.IsPurchased,
                TrackingNumber = ord.TrackingNumber,
                OrderType = ord.OrderType.OrderName,
                Quantity = (int?)ord.OrderItems.Sum(item => item.Quantity) ?? 0
            })
            .Where(ord => ord.UserId.Equals(userId) && 
                ord.OrderType.ToLower().Equals(cartType.ToLower()));

            return Ok(totals);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}