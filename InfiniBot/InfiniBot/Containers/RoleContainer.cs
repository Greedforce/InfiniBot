using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfiniBot
{
    public class RoleContainer
    {
        public string name { get; set; }
        public bool joinable { get; set; }
        public RoleType roleType { get; set; }

        public RoleContainer(string name, bool joinable, RoleType roleType)
        {
            this.name = name;
            this.joinable = joinable;
            this.roleType = roleType;
        }

        public override string ToString()
        {
            return $"{name}:{joinable}:{roleType}";
        }

        public override bool Equals(object obj)
        {
            if (obj is RoleContainer)
            {
                return name == (obj as RoleContainer).name && joinable == (obj as RoleContainer).joinable && roleType == (obj as RoleContainer).roleType;
            }
            return base.Equals(obj);
        }
    }
}
