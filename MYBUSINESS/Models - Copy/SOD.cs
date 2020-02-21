//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MYBUSINESS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SOD
    {
        public Nullable<int> SODId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public string SOId { get; set; }
        public Nullable<int> Quantity { get; set; }
        public decimal Auto { get; set; }
        public Nullable<decimal> SalePrice { get; set; }
        public Nullable<bool> SaleType { get; set; }
        public Nullable<decimal> PurchasePrice { get; set; }
        public string Remarks { get; set; }
        public Nullable<decimal> OpeningStock { get; set; }
        public Nullable<decimal> Profit { get; set; }
        public Nullable<decimal> PerPack { get; set; }
        public Nullable<bool> IsPack { get; set; }
    
        public virtual SO SO { get; set; }
        public virtual Product Product { get; set; }
    }
}
