using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Student
    {
        public int? Id { get; set; }
        public bool Verified { get; set; }
        public User UserInfo { get; set; }
    }
}
