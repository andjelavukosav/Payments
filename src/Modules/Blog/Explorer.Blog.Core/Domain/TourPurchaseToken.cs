using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.Core.Domain
{
    public class TourPurchaseToken : Entity
    {
        public string TouristId { get; set; }
        public string TourId { get; set; }
        public TokenStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpiredDate { get; set; }
       // public String jwtToken { get; private set; }

        public TourPurchaseToken() { }
        public TourPurchaseToken(string touristId, string tourId/*, String jwtToken*/)
        {
            TouristId = touristId;
            TourId = tourId;
            Status = TokenStatus.Active;
            CreatedDate = DateTime.UtcNow;
            ExpiredDate = CreatedDate.AddYears(1);  // Token važi godinu dana od datuma kreiranja
           // this.jwtToken = jwtToken;
        }


        public enum TokenStatus
        {
            Active,   // Token je aktivan i može se koristiti
            Used,     // Token je iskorišćen i više nije važeći
            Expired   // Token je istekao i više ne može biti iskorišćen
        }

    }
}
