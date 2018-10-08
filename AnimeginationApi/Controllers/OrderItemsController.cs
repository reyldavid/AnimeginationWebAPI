using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Http;
using AnimeginationApi.Models;
using Swashbuckle.Swagger.Annotations;
using AnimeginationApi.Filters;
using System.Threading.Tasks;
using System.Net.Http;
using AnimeginationApi.Services;

namespace AnimeginationApi.Controllers
{
    public class OrderItemsController : ApiController
    {
        private AnimeDB db = new AnimeDB();

        // GET: api/OrderItems
        [AdminRoleFilter]
        [SwaggerOperation("GetOrderItems")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IHttpActionResult> GetOrderItems()
        {
            var orderItems = db.OrderItems.Select(item => new
            {
                OrderItemID = item.OrderItemID,
                OrderID = item.OrderID,
                ProductID = item.ProductID,
                Quantity = item.Quantity,
                UnitPrice = item.FinalUnitPrice
            }).AsEnumerable();

            return Ok(orderItems);
        }

        // GET: api/OrderItems/5
        [JwtTokenFilter]
        [SwaggerOperation("GetOrderItem")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IHttpActionResult> GetOrderItems(int id)
        {
            string userId = Request.UserId();

            var orderItems =
                db.OrderItems.Select(item => new
                {
                    OrderItemID = item.OrderItemID,
                    OrderID = item.OrderID,
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    UnitPrice = item.FinalUnitPrice
                })
                .Where(item => item.OrderID == id);

            return Ok(orderItems);
        }

        [HttpPut]
        [JwtTokenFilter]
        [SwaggerOperation("PutOrderItem")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> PutOrderItem([FromBody] OrderItemModel itemInput)
        {
            if (string.IsNullOrEmpty(Request.UserId()))
            {
                return Unauthorized();
            }

            Order order = db.Orders.SingleOrDefault(ord => ord.OrderID == itemInput.orderid);

            if (order == null)
            {
                order = new Order
                {
                    UserId = Request.UserId(),
                    OrderDate = DateTime.Now
                };
                db.Orders.Add(order);

                db.SaveChanges();
            }

            Product product = db.Products.SingleOrDefault(prod => prod.ProductID == itemInput.productid);

            OrderItem item = new OrderItem
            {
                Order = order,
                OrderID = order.OrderID,
                Product = product,
                ProductID = product.ProductID,
                Quantity = itemInput.quantity,
                FinalUnitPrice = Double.Parse(itemInput.unitprice.ToString())
            };

            db.OrderItems.Add(item);
            db.SaveChanges();

            Helpers.UpdateOrder(ref order);
            db.SaveChanges();

            itemInput.orderitemid = item.OrderItemID;
            itemInput.orderid = item.OrderID;

            return Ok(itemInput);
        }

        [HttpPost]
        [JwtTokenFilter]
        [SwaggerOperation("PostOrderItem")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public HttpResponseMessage PostOrder([FromBody] OrderModel orderInput)
        {
            string userId = Request.UserId();

            if (!userId.Equals(orderInput.userid))
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            Order order = db.Orders.SingleOrDefault(ord => ord.OrderID == orderInput.orderid);

            if (order == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            OrderType orderType = db.OrderTypes.SingleOrDefault(
                ordtype => ordtype.OrderName.ToLower().Equals(orderInput.ordertype.ToLower()));

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

            order.TrackingNumber = string.IsNullOrEmpty(orderInput.trackingnumber) ?
                Helpers.GetTrackingNumber() : orderInput.trackingnumber;
            order.OrderDate = DateTime.Now;
            order.IsPurchased = orderInput.ispurchased;

            order.OrderTypeID = string.IsNullOrEmpty(orderInput.ordertype) ? 
                order.OrderTypeID : orderType.OrderTypeID;

            db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, order);
        }

        // DELETE: api/orderitemss/5
        [HttpDelete]
        [JwtTokenFilter]
        [SwaggerOperation("DeleteOrderItem")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        public async Task<IHttpActionResult> DeleteOrderItem(int id)
        {
            // Delete Order Item Record
            OrderItem orderItem = db.OrderItems.SingleOrDefault(item => item.OrderItemID == id);

            if (orderItem == null)
            {
                return NotFound();
            }

            db.OrderItems.Remove(orderItem);
            await db.SaveChangesAsync();

            return Ok(orderItem);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderItemExists(int id)
        {
            return db.OrderItems.Count(e => e.OrderItemID == id) > 0;
        }
    }
}