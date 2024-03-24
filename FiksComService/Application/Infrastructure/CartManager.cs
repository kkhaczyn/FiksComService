using FiksComService.Application.Helpers;
using FiksComService.Models.Cart;
using FiksComService.Repositories;

namespace FiksComService.Application.Infrastructure
{
    public class CartManager()
    {
        private static readonly string cartSessionKey = "Cart";

        public static int RemoveFromCart(ISession session, int componentId)
        {
            var cart = GetItems(session);

            var thisComponent = cart.Find(x => x.Component.ComponentId == componentId);

            int quantity = 0;

            if (thisComponent == null)
            {
                return quantity;
            }
            if(thisComponent.Quantity > 1)
            {
                thisComponent.Quantity--;

                quantity = thisComponent.Quantity;
            }
            else
            {
                cart.Remove(thisComponent);
            }

            session.SetObjectAsJson(cartSessionKey, cart);

            return quantity;
        }

        public static decimal GetCartValue(ISession session)
        {
            var items = GetItems(session);
            return items.Sum(x => x.Quantity * x.Value);
        }

        public static int GetCartQuantity(ISession session)
        {
            var cart = GetItems(session);

            return cart.Sum(x => x.Quantity);
        }

        public static List<CartItem> GetItems(ISession session)
        {
            var cart = SessionHelper.GetObjectFromJson<List<CartItem>>(session, cartSessionKey);

            if (cart == null)
            {
                cart = [];
            }
            return cart;
        }

        public static void AddToCart(ISession session, IComponentRepository componentRepository, int componentId)
        {
            var cart = GetItems(session);
            var thisComponent = cart.Find(x => x.Component.ComponentId == componentId);
            if (thisComponent != null)
            {
                thisComponent.Quantity++;
            }
            else
            {
                var newCartItem = componentRepository.GetComponentById(componentId);
                if (newCartItem != null)
                {
                    var cartItem = new CartItem
                    {
                        Component = newCartItem,
                        Quantity = 1,
                        Value = newCartItem.Price
                    };

                    cart.Add(cartItem);
                }
            }
            SessionHelper.SetObjectAsJson(session, cartSessionKey, cart);
        }
    }
}
