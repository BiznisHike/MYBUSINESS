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
    
    public partial class PO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PO()
        {
            this.PODs = new HashSet<POD>();
        }
    
        public string Id { get; set; }
        public Nullable<int> POSerial { get; set; }
        public decimal BillAmount { get; set; }
        public decimal BillPaid { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<decimal> Balance { get; set; }
        public Nullable<decimal> PrevBalance { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<bool> PurchaseReturn { get; set; }
        public Nullable<decimal> SupplierId { get; set; }
        public Nullable<decimal> PODId { get; set; }
        public Nullable<decimal> PurchaseOrderAmount { get; set; }
        public Nullable<decimal> PurchaseReturnAmount { get; set; }
        public Nullable<decimal> PurchaseOrderQty { get; set; }
        public Nullable<decimal> PurchaseReturnQty { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentDetail { get; set; }
        public string Remarks { get; set; }
        public string Remarks2 { get; set; }
        public Nullable<decimal> EmployeeId { get; set; }
    
        public virtual Employee Employee { get; set; }
        public virtual Supplier Supplier { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<POD> PODs { get; set; }
    }
}
