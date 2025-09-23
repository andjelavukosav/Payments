using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.Core.Domain
{
    public class ShoppingCart : Entity
    {
        public string TouristId { get; set; }

        // Samo ID-evi stavki
        public List<ShoppingCartItem> ShoppingCartItems { get; set; } 

        public double TotalPrice { get; set;  }

        public ShoppingCart() {
            ShoppingCartItems = new List<ShoppingCartItem>();
        }

        public ShoppingCart(string touristId)
        {
            TouristId = touristId;
            ShoppingCartItems = new List<ShoppingCartItem>();
            TotalPrice = 0;
        }
        public void AddItem(ShoppingCartItem item)
        {
            ShoppingCartItems.Add(item);
            TotalPrice += item.Price;
        }

    }


}
