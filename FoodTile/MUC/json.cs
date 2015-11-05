using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MUC
{
    public class UserHfsData
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


    public class TermInfo
    {
        public Term term { get; set; }
        public bool is_grad_student { get; set; }
        public string summer_term { get; set; }
        public int year { get; set; }
        public string quarter { get; set; }
    }

    public class Term
    {
        public string grade_submission_deadline { get; set; }
        public string quarter { get; set; }
        public string last_final_exam_date { get; set; }
        public int year { get; set; }
    }
    
}
