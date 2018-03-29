using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistrSystems2
{
    class History
    {
        public  List<BankAccount> BankAccounts { get; set; }
        public TimeSpanAction Action { get; set; }

        public History(List<BankAccount> bankAccounts, TimeSpanAction action)
        {
            BankAccounts = bankAccounts;
            Action = action;
        }
    }
}
