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
        public int FullDaysRemaining
        {
            get
            {
                var lastDay = DateTime.Parse(term.last_final_exam_date).Date;
                var remaining = lastDay - DateTime.Now.Date;
                return (int)remaining.TotalDays;
            }
        }

        public int AdjustedDaysRemaining(IList<DateTime> toRemove)
        {
            for (int i = 0; i < toRemove.Count; i++)
            {
                for (int j = 0; j < toRemove.Count; j++)
                {
                    if(j!= i && toRemove[i].Date.Equals(toRemove[j].Date))
                        toRemove[j]= new DateTime(0);
                }
            }

            int count = toRemove.Count(IsInCurrentTerm);

            return FullDaysRemaining - count;
        }

        /// <summary>
        /// Checks if the date is between now and end of quarter
        /// </summary>
        /// <param name="date">Date to check</param>
        /// <returns></returns>
        private bool IsInCurrentTerm(DateTime date)
        {
            var endTicks = DateTime.Parse(term.last_final_exam_date).Ticks;
            var nowTicks = DateTime.Now.Ticks;
            if (date.Ticks > nowTicks && date.Ticks < endTicks)
            {
                return true;
            }
            return false;
        }

        public double TrueDaysRemaining
        {
            get
            {
                var lastDay = DateTime.Parse(term.last_final_exam_date);
                var remaining = lastDay - DateTime.Now;
                return remaining.TotalDays;
            }
        }
    }

    public class Term
    {
        public string grade_submission_deadline { get; set; }
        public string quarter { get; set; }
        public string last_final_exam_date { get; set; }
        public int year { get; set; }
    }
    
}
