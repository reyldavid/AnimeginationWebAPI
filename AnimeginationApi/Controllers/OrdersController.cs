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

namespace AnimeginationApi.Controllers
{
    public class OrdersController : ApiController
    {
        private AnimeDB db = new AnimeDB();

        // GET: api/Orders
        [AdminRoleFilter]
        [SwaggerOperation("GetOrders")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IHttpActionResult> GetOrders()
        {
            var orders = 
            db.Orders.Select(ord => new
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
                OrderType = ord.OrderType.OrderName
            }).AsEnumerable();

            return Ok(orders);
        }

        // GET: api/Orders/5
        [Route("api/Orders/id/{id}", Name = "GetOrdersByID")]
        [JwtTokenFilter]
        [SwaggerOperation("GetOrdersById")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IHttpActionResult> GetOrdersById(int id)
        {
            string userId = Request.UserId();

            var order =
            db.Orders.Select(ord => new
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
                OrderType = ord.OrderType.OrderName
            })
            .Where(ord => ord.OrderID == id);

            return Ok(order);
        }

        // GET: api/Orders/{carttype:string}
        [Route("api/Orders/{carttype}", Name = "GetOrdersByType")]
        [JwtTokenFilter]
        [SwaggerOperation("GetOrdersByType")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IHttpActionResult> GetOrdersByType(string carttype)
        {
            string userId = Request.UserId();
            //OrderType orderType = db.OrderTypes.SingleOrDefault(
            //    ordtype => ordtype.OrderName.ToLower().Equals(carttype.ToLower()));

            var orders = db.Orders.Where(ord => ord.UserId.Equals(userId) &&
                ord.OrderType.OrderName.ToLower().Equals(carttype.ToLower()));

            foreach (var ord in orders)
            {
                Order order = db.Orders.Where(o => o.OrderID == ord.OrderID).FirstOrDefault();

                Helpers.UpdateOrder(ref order);
            }
            db.SaveChanges();

            var totals =
                orders.Select(ord => new
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
                ItemQuantity = ord.OrderItems.Sum(item => item.Quantity),
                ProductQuantity = ord.OrderItems.Count,
                SubTotal = Math.Round(ord.OrderItems.Sum(item => item.Product.YourPrice * item.Quantity), 2)
            })
            .Where(ord => ord.OrderType.ToLower().Equals(carttype.ToLower()));

            return Ok(totals);
        }

        [HttpPut]
        [JwtTokenFilter]
        [SwaggerOperation("PutOrder")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> PutOrder([FromBody] OrderModel orderInput)
        {
            string userId = Request.UserId();

            if (!userId.Equals(orderInput.userid))
            {
                return Unauthorized();
            }

            OrderType orderType = db.OrderTypes.SingleOrDefault(
                ordtype => ordtype.OrderName.ToLower().Equals(orderInput.ordertype.ToLower()));

            Order order = new Order
            {
                UserId = orderInput.userid,
                ShippingHandling = Double.Parse(orderInput.shipping.ToString()),
                Taxes = Double.Parse(orderInput.taxes.ToString()),
                Discounts = Double.Parse(orderInput.discounts.ToString()),
                Totals = Double.Parse(orderInput.totals.ToString()),
                OrderDate = DateTime.Now,
                IsPurchased = orderInput.ispurchased,
                TrackingNumber = orderInput.trackingnumber,
                OrderTypeID = orderType.OrderTypeID
            };

            db.Orders.Add(order);
            db.SaveChanges();

            var response = new
            {
                ShippingHandling = order.ShippingHandling,
                Taxes = order.Taxes,
                Discounts = order.Discounts,
                Totals = order.Totals,
                OrderDate = order.OrderDate,
                IsPurchased = order.IsPurchased,
                TrackingNumber = order.TrackingNumber,
                OrderType = order.OrderType.OrderName
            };
            return Ok(response);
        }

        [HttpPost]
        [JwtTokenFilter]
        [SwaggerOperation("PostOrder")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> PostOrder([FromBody] OrderModel orderInput)
        {
            string userId = Request.UserId();

            if (!userId.Equals(orderInput.userid))
            {
                return Unauthorized();
            }
            Order order = db.Orders.SingleOrDefault(ord => ord.OrderID == orderInput.orderid);

            if (order == null)
            {
                return NotFound();
            }

            OrderType orderType = db.OrderTypes.SingleOrDefault(
                ordtype => ordtype.OrderName.ToLower().Equals(orderInput.ordertype.ToLower()));

            // HEY REY!!  USE HELPER!!
            double subTotal = order.OrderItems.Sum(item => item.Product.YourPrice * item.Quantity);

            double shipping = Math.Round(Helpers.GetShippingAndHandling(subTotal, order.OrderItems.Sum(item => item.Quantity)), 2);
            double taxes = Math.Round(Helpers.GetTaxes(subTotal), 2);
            double discounts = Math.Round(Helpers.GetDiscounts(subTotal), 2);

            order.ShippingHandling = orderInput.shipping == 0 ? 
                shipping : Double.Parse(orderInput.shipping.ToString());
            order.Taxes = orderInput.taxes == 0 ? 
                taxes : Double.Parse(orderInput.taxes.ToString());
            order.Discounts = orderInput.discounts == 0 ? 
                discounts : Double.Parse(orderInput.discounts.ToString());

            order.Totals = subTotal + order.ShippingHandling + order.Taxes - order.Discounts;

            order.TrackingNumber = string.IsNullOrEmpty(orderInput.trackingnumber) ? 
                Helpers.GetTrackingNumber() : orderInput.trackingnumber;
            order.OrderDate = DateTime.Now;
            order.IsPurchased = orderInput.ispurchased;
            order.OrderTypeID = orderType.OrderTypeID;

            db.SaveChanges();

            var response = new
            {
                ShippingHandling = order.ShippingHandling,
                Taxes = order.Taxes,
                Discounts = order.Discounts,
                Totals = order.Totals,
                OrderDate = order.OrderDate,
                IsPurchased = order.IsPurchased,
                TrackingNumber = order.TrackingNumber,
                OrderType = order.OrderType.OrderName
            };
            return Ok(response);
        }

        // DELETE: api/orders/guid
        [HttpDelete]
        [AdminRoleFilter]
        [SwaggerOperation("DeleteOrder")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        public async Task<IHttpActionResult> DeleteOrder(int id)
        {
            // Delete Order Record
            // Order order = await db.Orders.FindAsync(id);
            Order order = db.Orders.SingleOrDefault(ord => ord.OrderID == id);

            if (order == null)
            {
                return NotFound();
            }

            db.Orders.Remove(order);
            await db.SaveChangesAsync();

            return Ok(order);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderExists(int id)
        {
            return db.Orders.Count(e => e.OrderID == id) > 0;
        }
    }
}