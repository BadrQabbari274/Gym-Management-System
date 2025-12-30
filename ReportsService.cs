using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Forma_System
{

    public class ReportsService
    {
        private readonly DataBaseForma2025Entities _db = new DataBaseForma2025Entities();

        public (DateTime From, DateTime To) GetPeriod(PeriodType type)
        {
            DateTime today = DateTime.Today;
            DateTime from, to;

            // استبدل الـ switch expression بـ switch statement
            switch (type)
            {
                case PeriodType.Today:
                    from = today;
                    to = today.AddDays(1).AddTicks(-1);
                    break;

                case PeriodType.ThisWeek:
                    int diff = ((int)today.DayOfWeek + 6) % 7;
                    from = today.AddDays(-diff);
                    to = DateTime.Now;
                    break;

                case PeriodType.ThisMonth:
                    from = new DateTime(today.Year, today.Month, 1);
                    to = DateTime.Now;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("type");
            }

            return (from, to);
        }

        // بقية الدوال ثابتة، لا تستخدم أي C#8 features
        public int GetAttendanceCount(DateTime from, DateTime to)
        {
            return _db.Attendees.Count(a => a.DateTime >= from && a.DateTime <= to);
        }

        public int GetExpiringSubscriptions(int daysAgo)
        {
            DateTime cutoff = DateTime.Today.AddDays(-daysAgo);
            return _db.Subscribes.Count(s => s.SubscribeDate <= cutoff);
        }

        public List<AttendanceDto> GetAttendanceDetails(DateTime from, DateTime to, string search = "")
        {
            return _db.Attendees
                      .Include(a => a.Customer)
                      .Where(a => a.DateTime >= from && a.DateTime <= to
                               && (string.IsNullOrEmpty(search)
                                   || a.Customer.Name.Contains(search)))
                      .Select(a => new AttendanceDto
                      {
                          Date = a.DateTime ?? DateTime.MinValue,
                          Customer = a.Customer.Name,
                          Notes = a.Notes
                      })
                      .ToList();
        }

        public List<SubscriptionDto> GetSubscriptionDetails(string search = "")
        {
            // 1. استعلم من الداتا بيز أولًا (تنفيذ SQL)
            var subs = _db.Subscribes
                          .Include(s => s.Customer)
                          .Include(s => s.Baqat)
                          .Where(s => string.IsNullOrEmpty(search)
                                   || s.Customer.Name.Contains(search)
                                   || s.Baqat.Name.Contains(search))
                          .ToList();  // هنا تُنفّذ الكويري فعليًا

            // 2. الآن نُحوِّل القائمة في الذاكرة ونحسب الحالة
            var result = subs.Select(s => new SubscriptionDto
            {
                SubscribeDate = s.SubscribeDate,
                Customer = s.Customer.Name,
                Package = s.Baqat.Name,
                Status = DetermineStatus(s)    // هذه المناداة في الذاكرة ولن تخرب LINQ-to-Entities
            })
            .ToList();

            return result;
        }


        private string DetermineStatus(Subscribe s)
        {
            var baqa = _db.Baqats.Find(s.BaqaId);
            if (baqa == null) return "باقة مفقودة";

            DateTime now = DateTime.Now;
            DateTime weekStart = now.AddDays(-((int)now.DayOfWeek + 6) % 7);
            DateTime monthStart = new DateTime(now.Year, now.Month, 1);

            int weekCount = _db.Attendees.Count(a =>
                            a.CustomerId == s.CustomerId && a.DateTime >= weekStart);
            int monthCount = _db.Attendees.Count(a =>
                            a.CustomerId == s.CustomerId && a.DateTime >= monthStart);

            return (weekCount >= baqa.MaxDaysPerWeek || monthCount >= baqa.DaysPerMonth)
                 ? "منتهي"
                 : "ساري";
        }
    }

    public class AttendanceDto
    {
        public DateTime? Date { get; set; }
        public string Customer { get; set; }
        public string Notes { get; set; }
    }

    public class SubscriptionDto
    {
        public DateTime SubscribeDate { get; set; }
        public string Customer { get; set; }
        public string Package { get; set; }
        public string Status { get; set; }
    }
}
