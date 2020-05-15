using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Http;
using AnimeginationApi.Models;
using Swashbuckle.Swagger.Annotations;
using AnimeginationApi.Filters;
using System.Threading.Tasks;
using AnimeginationApi.Services;
using System.Web.Http.Cors;
using System.Collections.Generic;

namespace AnimeginationApi.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
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
                        oi.FinalUnitPrice,
                        oi.ItemDate
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
                            p.YourPrice,
                            oi.ItemDate
                        })
                        .Where(p => p.UserId == userId &&
                                p.OrderName.ToLower().Equals(carttype.ToLower()))
                        .OrderByDescending(oid => oid.ItemDate)
                        .AsEnumerable();

            return Ok(items);
        }

        // GET: api/CartItems/item/5
        [Route("api/cartitems/item/{id}", Name = "GetCartItem")]
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
        [Route("api/carttotals/{cartType}")]
        [JwtTokenFilter]
        [SwaggerOperation("GetCartTotals")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IHttpActionResult> GetCartTotals(string cartType)
        {
            string userId = Request.UserId();

            var orders = db.Orders.Where(ord => ord.UserId.Equals(userId) &&
                ord.OrderType.OrderName.ToLower().Equals(cartType.ToLower()));

            var firstOrder = db.Orders.Where(ord => ord.UserId.Equals(userId) &&
                ord.OrderType.OrderName.ToLower().Equals(cartType.ToLower())).FirstOrDefault();

            var duplicateOrders = db.Orders.Where(ord => ord.UserId.Equals(userId) &&
                ord.OrderType.OrderName.ToLower().Equals(cartType.ToLower()))
                .GroupBy(ord => ord.OrderType.OrderName).Skip(1);

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

        // GET: api/additem/5/visited
        [HttpGet]
        [Route("api/additem/{productId}/{cartType}", Name = "AddCartItem")]
        [JwtTokenFilter]
        [SwaggerOperation("AddCartItem")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> AddCartItem(int productId, string cartType)
        {
            if (string.IsNullOrEmpty(Request.UserId()))
            {
                return Unauthorized();
            }
            string userId = Request.UserId();

            OrderType orderType = db.OrderTypes.SingleOrDefault(
                ordtype => ordtype.OrderName.ToLower().Equals(cartType.ToLower()));

            Product product = db.Products.SingleOrDefault(prod => prod.ProductID == productId);

            Order order = db.Orders.SingleOrDefault(ord => ord.UserId.Equals(userId) &&
                                ord.OrderType.OrderTypeID.Equals(orderType.OrderTypeID));
            if (order == null)
            {
                order = new Order
                {
                    UserId = Request.UserId(),
                    OrderDate = DateTime.Now,
                    OrderType = orderType
                };
                db.Orders.Add(order);

                db.SaveChanges();
            }

            OrderItem orderItem = order.OrderItems.SingleOrDefault(
                oi => oi.ProductID.Equals(productId));

            if (orderItem == null)
            {
                orderItem = new OrderItem
                {
                    Order = order,
                    OrderID = order.OrderID,
                    Product = product,
                    ProductID = product.ProductID,
                    Quantity = 1,
                    FinalUnitPrice = product.YourPrice,
                    ItemDate = DateTime.Now
                };
                db.OrderItems.Add(orderItem);
                order.OrderItems.Add(orderItem);
            }
            else
            {
                orderItem.ItemDate = DateTime.Now;
            }
            db.SaveChanges();

            int skip = "VisitedMaxNumber".GetConfigurationNumericValue();

            IEnumerable<OrderItem> excessItems = order.OrderItems.OrderByDescending(oi => oi.ItemDate).Skip(skip);

            foreach (OrderItem excess in excessItems)
            {
                db.OrderItems.Remove(excess);
            }
            db.SaveChanges();

            return Ok(orderItem);
        }


        [HttpPut]
        [JwtTokenFilter]
        [SwaggerOperation("PutCartItem")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> PutCartItem([FromBody] CartItemModel itemInput)
        {
            if (string.IsNullOrEmpty(Request.UserId()))
            {
                return Unauthorized();
            }
            string userId = Request.UserId();

            OrderType orderType = db.OrderTypes.SingleOrDefault(ot => 
                ot.OrderName.ToLower().Equals(itemInput.ordertype));

            Order order = db.Orders.Where(ord => ord.UserId.Equals(userId) &&
                ord.OrderType.OrderTypeID.Equals(orderType.OrderTypeID)).FirstOrDefault();

            if (order == null)
            {
                order = new Order
                {
                    UserId = Request.UserId(),
                    OrderDate = DateTime.Now,
                    OrderType = orderType,
                    OrderTypeID = orderType.OrderTypeID,
                };
                db.Orders.Add(order);

                db.SaveChanges();
            }

            Product product = db.Products.SingleOrDefault(prod => prod.ProductID == itemInput.productid);

            OrderItem orderItem = db.OrderItems.SingleOrDefault(oi => oi.OrderID.Equals(order.OrderID) 
                                    && oi.ProductID.Equals(itemInput.productid));

            if (orderItem == null)
            {
                orderItem = new OrderItem
                {
                    Order = order,
                    OrderID = order.OrderID,
                    Product = product,
                    ProductID = product.ProductID,
                    Quantity = itemInput.quantity,
                    FinalUnitPrice = Double.Parse(itemInput.unitprice.ToString()),
                    ItemDate = DateTime.Now
                };
                db.OrderItems.Add(orderItem);
            }
            else
            {
                orderItem.Quantity += itemInput.quantity;
            }
            //db.SaveChanges();

            Helpers.UpdateOrder(ref order);
            db.SaveChanges();

            return Ok(orderItem);
        }

        // POST: api/CartItems/list
        [HttpPost]
        [Route("api/cartitems/list", Name = "GetCartItemsList")]
        [SwaggerOperation("GetCartItemsList")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IHttpActionResult> GetCartItemsList([FromBody] int[] productIds)
        {
            var items = db.Products
            .Where(prod => productIds.Contains(prod.ProductID))
            .Select(p => new
            {
                categoryName = p.Category.CategoryName,
                finalUnitPrice = p.YourPrice,
                mediumName = p.Medium.MediumName,
                orderID = 0,
                orderItemID = 0,
                orderName = "visited",
                orderTypeID = 4,
                productAgeRating = p.ProductAgeRating,
                productCode = p.ProductCode,
                productID = p.ProductID,
                productLength = p.ProductLength,
                productTitle = p.ProductTitle,
                productYearCreated = p.ProductYearCreated,
                publisherName = p.Publisher.PublisherName,
                quantity = 1,
                ratingID = p.RatingID,
                unitPrice = p.UnitPrice,
                userId = "",
                yourPrice = p.YourPrice
            });

            return Ok(items);
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