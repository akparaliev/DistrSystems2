using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistrSystems2
{
    public class BankAccount
    {
        public BankAccount(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; set; }
        public string Name { get; set; }

        private long _money;
        public long Money
        {
            get { return _money; }
            set
            {
                if (value < 0)
                {
                    throw new Exception("money cannot be negative amount");
                }
                _money = value;
            }
        }
    }
}
