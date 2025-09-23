using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.API.Dtos
{
    public class ShoppingCartItemDTO
    {
        public string TourId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public long ShoppingCartId { get; set; } // Foreign Key
        //public ShoppingCart ShoppingCart { get; set; }

        // Konstruktor sa ShoppingCartId
        public ShoppingCartItemDTO(string tourId, string name, double price)
        {
            TourId = tourId;
            Name = name;
            Price = price;
        }

    }
}
