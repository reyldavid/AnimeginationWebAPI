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
        [SwaggerOperation("GetOrderItems")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IHttpActionResult> GetOrderItems(int id)
        {
            if (string.IsNullOrEmpty(Request.UserId()))
            {
                return Unauthorized();
            }
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

        // GET: api/OrderItems/id/5
        [Route("api/OrderItems/id/{id}", Name = "GetOrderItemById")]
        [JwtTokenFilter]
        [SwaggerOperation("GetOrderItemById")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IHttpActionResult> GetOrderItemById(int id)
        {
            if (string.IsNullOrEmpty(Request.UserId()))
            {
                return Unauthorized();
            }
            string userId = Request.UserId();

            if (!this.OrderItemExists(id))
            {
                return NotFound();

            }
            var orderItem = db.OrderItems.SingleOrDefault(item => item.OrderItemID == id);

            if (orderItem == null)
            {
                return NotFound();
            }
            var response = new
            {
                OrderItemID = orderItem.OrderItemID,
                OrderID = orderItem.OrderID,
                ProductID = orderItem.ProductID,
                Quantity = orderItem.Quantity,
                UnitPrice = orderItem.FinalUnitPrice
            };

            return Ok(response);
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
        public async Task<IHttpActionResult> PostOrderItem([FromBody] OrderItemModel orderItemInput)
        {
            if (string.IsNullOrEmpty(Request.UserId()))
            {
                return Unauthorized();
            }

            Order order = db.Orders.SingleOrDefault(ord => ord.OrderID == orderItemInput.orderid);

            if (order == null)
            {
                return NotFound();
            }
            if (!this.OrderItemExists(orderItemInput.orderitemid))
            {
                return NotFound();
            }
            OrderItem orderItem = db.OrderItems.SingleOrDefault(item => 
                    item.OrderItemID == orderItemInput.orderitemid);            
            if (orderItem == null)
            {
                return NotFound();
            }
            orderItem.ProductID = orderItemInput.productid;
            orderItem.Quantity = orderItemInput.quantity;
            orderItem.FinalUnitPrice = Double.Parse(orderItemInput.unitprice.ToString());

            db.SaveChanges();

            var response = new
            {
                OrderItemID = orderItem.OrderItemID,
                OrderID = orderItem.OrderItemID,
                ProductID = orderItem.ProductID,
                Quantity = orderItem.Quantity,
                FinalUnitPrice = orderItem.FinalUnitPrice
            };
            return Ok(response);
        }

        // GET: api/OrderItems/move/5/wish
        [HttpGet]
        [Route("api/OrderItems/move/{id}/{cartType}", Name = "MoveOrderItem")]
        [JwtTokenFilter]
        [SwaggerOperation("MoveOrderItem")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public HttpResponseMessage MoveOrderItem(int id, string cartType)
        {
            if (string.IsNullOrEmpty(Request.UserId()))
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            string userId = Request.UserId();

            OrderType orderType = db.OrderTypes.SingleOrDefault(
                ordtype => ordtype.OrderName.ToLower().Equals(cartType.ToLower()));

            if (!this.OrderItemExists(id))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);

            }
            OrderItem orderItem = db.OrderItems.SingleOrDefault(
                oi => oi.OrderItemID.Equals(id));

            Order order = db.Orders.SingleOrDefault(ord => ord.OrderID == orderItem.OrderID);

            if (order == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            Order newOrder = db.Orders.SingleOrDefault(ord => ord.UserId.Equals(userId) &&
                                ord.OrderType.OrderTypeID.Equals(orderType.OrderTypeID));
            if (newOrder == null)
            {
                newOrder = new Order
                {
                    UserId = Request.UserId(),
                    OrderDate = DateTime.Now,
                    OrderType = orderType
                };
                db.Orders.Add(newOrder);

                db.SaveChanges();
            }

            //var productId = orderItem.ProductID;
            //var quantity = orderItem.Quantity;
            //var unitPrice = orderItem.FinalUnitPrice;
            //Product product = db.Products.SingleOrDefault(prod => prod.ProductID == productId);

            //OrderItem newOrderItem = new OrderItem
            //{
            //    Order = newOrder,
            //    OrderID = newOrder.OrderID,
            //    Product = product,
            //    ProductID = product.ProductID,
            //    Quantity = quantity,
            //    FinalUnitPrice = unitPrice
            //};

            //db.OrderItems.Add(newOrderItem);

            //newOrder.OrderItems.Add(newOrderItem);

            //order.OrderItems.Remove(orderItem);


            //OrderItem newOrderItem = new OrderItem
            //{
            //    Order = newOrder,
            //    OrderID = newOrder.OrderID,
            //    Product = orderItem.Product,
            //    ProductID = orderItem.ProductID,
            //    Quantity = orderItem.Quantity,
            //    FinalUnitPrice = orderItem.FinalUnitPrice
            //};
            //db.SaveChanges();

            //db.OrderItems.Remove(orderItem);

            orderItem.Order = newOrder;
            orderItem.OrderID = newOrder.OrderID;

            //OrderItem newOrderItem = new OrderItem();
            //db.OrderItems.Add(newOrderItem);
            //db.Entry(newOrderItem).CurrentValues.SetValues( db.Entry(orderItem).CurrentValues );

            //newOrderItem.Order = newOrder;
            //newOrderItem.OrderID = newOrder.OrderID;
            //db.OrderItems.Remove(orderItem);

            db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, orderItem);
        }

        //[SwaggerResponse(HttpStatusCode.InternalServerError)]
        //public HttpResponseMessage PostOrder([FromBody] OrderModel orderInput)
        //{
        //    string userId = Request.UserId();

        //    if (!userId.Equals(orderInput.userid))
        //    {
        //        return Request.CreateResponse(HttpStatusCode.Unauthorized);
        //    }
        //    Order order = db.Orders.SingleOrDefault(ord => ord.OrderID == orderInput.orderid);

        //    if (order == null)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.NotFound);
        //    }

        //    OrderType orderType = db.OrderTypes.SingleOrDefault(
        //        ordtype => ordtype.OrderName.ToLower().Equals(orderInput.ordertype.ToLower()));

        //    double subTotal = order.OrderItems.Sum(item => item.Product.YourPrice * item.Quantity);

        //    double shipping = Math.Round(Helpers.GetShippingAndHandling(subTotal, order.OrderItems.Sum(item => item.Quantity)), 2);
        //    double taxes = Math.Round(Helpers.GetTaxes(subTotal), 2);
        //    double discounts = Math.Round(Helpers.GetDiscounts(subTotal), 2);

        //    order.ShippingHandling = orderInput.shipping == 0 ?
        //        shipping : Double.Parse(orderInput.shipping.ToString());
        //    order.Taxes = orderInput.taxes == 0 ?
        //        taxes : Double.Parse(orderInput.taxes.ToString());
        //    order.Discounts = orderInput.discounts == 0 ?
        //        discounts : Double.Parse(orderInput.discounts.ToString());

        //    order.TrackingNumber = string.IsNullOrEmpty(orderInput.trackingnumber) ?
        //        Helpers.GetTrackingNumber() : orderInput.trackingnumber;
        //    order.OrderDate = DateTime.Now;
        //    order.IsPurchased = orderInput.ispurchased;

        //    order.OrderTypeID = string.IsNullOrEmpty(orderInput.ordertype) ?
        //        order.OrderTypeID : orderType.OrderTypeID;

        //    db.SaveChanges();

        //    return Request.CreateResponse(HttpStatusCode.OK, order);
        //}

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

            Order order = db.Orders.SingleOrDefault(ord => ord.OrderID == orderItem.OrderID);

            if (order == null)
            {
                return NotFound();
            }

            db.OrderItems.Remove(orderItem);

            if (order.OrderItems.Count == 0)
            {
                db.Orders.Remove(order);
            }
            //await db.SaveChangesAsync();
            db.SaveChanges();

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