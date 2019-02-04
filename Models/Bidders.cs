using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace beltfix.Models
{
    public class Bidders
    {
        [Key]
        public int id { get; set; }


        [ForeignKey("users")]
        public int bidderid { get; set; }
        public Users bidder { get; set; }


        [ForeignKey("auctions")]
        public int auctionid { get; set; }
        public Auctions auction { get; set; }


        [Required]
        public int bidamount { get; set; }
    }
}