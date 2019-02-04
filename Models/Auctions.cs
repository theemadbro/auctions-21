using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace beltfix.Models
{
    public class CurrentDateAttribute : ValidationAttribute
    {
        public CurrentDateAttribute()
        {}


        protected override ValidationResult IsValid(object value, ValidationContext ValidationContext)
        {
            var dt = (DateTime)value;
            if(dt <= DateTime.Today)
            {
                return new ValidationResult("An Auction can't start today!!");
            }
            return ValidationResult.Success;
        }

    }
    public class MoreThan0Attribute : ValidationAttribute
    {
        public MoreThan0Attribute()
        {}

        protected override ValidationResult IsValid(object value, ValidationContext ValidationContext)
        {
            var inp = (int)value;
            if(inp <= 0)
            {
                return new ValidationResult("Bid must be greater than 0!");
            }
            return ValidationResult.Success;
        }
    }
    public class Auctions
    {
        [Key]
        public int id { get; set; }


        [Required(ErrorMessage="Product Name is required!")]
        [MinLength(3, ErrorMessage="Product Name Length must be greater than 2")]
        public string itemname { get; set; }


        [Required(ErrorMessage="Description is required!")]
        [MinLength(10, ErrorMessage="Description Length must be greater than 9")]
        public string  description { get; set; }


        [Required(ErrorMessage="Bid Amount is required!")]
        [MoreThan0]
        public int bid { get; set; }


        [Required(ErrorMessage="End Date is required!")]
        [DataType(DataType.Date)]
        [CurrentDate]
        public DateTime endDate { get; set; }


        [ForeignKey("users")]
        public int sellerid {get; set; }
        public Users seller { get; set; }


        public List<Bidders> bidders { get; set; }
        public Auctions()
        {
            bidders = new List<Bidders>();
        }


        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime created_at { get; set; }


        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime updated_at { get; set; }
    }
}