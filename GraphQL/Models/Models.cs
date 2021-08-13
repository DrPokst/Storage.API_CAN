using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQLRequests.GraphQL.Models
{

    public class GraphQLData
    {
        public Search search { get; set; }
    }
    public class Search
    {
        public Result[] results { get; set; }
    }

    public class Result
    {
        public Part part { get; set; }
    }

    public class Part
    {
        public string mpn { get; set; }
        public Category category { get; set; }
        public Manufacturer manufacturer { get; set; }
        public string manufacturer_url { get; set; }
        public Best_Datasheet best_datasheet { get; set; }
        public string short_description { get; set; }
        public string octopart_url { get; set; }
        public Image[]? images { get; set; }
        public Spec[] specs { get; set; }
        public Series series { get; set; }
        public Seller[] sellers { get; set; }
    }

    public class Category
    {
        public string name { get; set; }
    }

    public class Manufacturer
    {
        public string name { get; set; }
    }

    public class Best_Datasheet
    {
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Series
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Image
    {
        public string url { get; set; }
    }

    public class Spec
    {
        public Attribute attribute { get; set; }
        public string display_value { get; set; }
    }

    public class Attribute
    {
        public string name { get; set; }
        public string group { get; set; }
    }

    public class Seller
    {
        public Company company { get; set; }
        public Offer[] offers { get; set; }
    }

    public class Company
    {
        public string name { get; set; }
    }

    public class Offer
    {
        public string sku { get; set; }
        public int? moq { get; set; }
        public string click_url { get; set; }
        public DateTime updated { get; set; }
        public int inventory_level { get; set; }
        public Price[] prices { get; set; }
    }

    public class Price
    {
        public float price { get; set; }
        public string currency { get; set; }
        public int quantity { get; set; }
        public float converted_price { get; set; }
        public string converted_currency { get; set; }
    }


}

