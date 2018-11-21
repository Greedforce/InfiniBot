using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfiniBot
{
    public class TokenContainer
    {
        public string name { get; set; }
        public string token { get; set; }

        public TokenContainer(string name, string token)
        {
            this.name = name;
            this.token = token;
        }

        public override string ToString()
        {
            return $"{name}:{token}";
        }
    }
}
