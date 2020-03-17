using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class ExpnseVM
    {
        public expense expense { get; set; }

        public List<ExpenseDetail> expenseDetail  { get; set; }
      
    }
}