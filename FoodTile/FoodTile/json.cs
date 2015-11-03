using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodTile
{

    public class Rootobject
    {
        public HuskyCard employee_husky_card { get; set; }
        public HuskyCard resident_dining { get; set; }
        public HuskyCard student_husky_card { get; set; }
    }

    public class HuskyCard
    {
        public float balance { get; set; }
        public string last_updated { get; set; }
        public string add_funds_url { get; set; }
    }
}
