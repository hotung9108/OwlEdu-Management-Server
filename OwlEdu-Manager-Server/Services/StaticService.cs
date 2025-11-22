using Microsoft.EntityFrameworkCore;
using OwlEdu_Manager_Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OwlEdu_Manager_Server.Services
{
    public class StaticService
    {
        private readonly EnglishCenterManagementContext _context;

        public StaticService(EnglishCenterManagementContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalStudentCountAsync()
        {
            return await _context.Students.CountAsync();
        }

        public async Task<int> GetTotalTeacherCountAsync()
        {
            return await _context.Teachers.CountAsync();
        }

        public async Task<int> GetTotalClassCountAsync()
        {
            return await _context.Classes.CountAsync();
        }

        public async Task<int> GetTotalCourseCountAsync()
        {
            return await _context.Courses.CountAsync();
        }

        public async Task<decimal> GetTotalRevenueByDateAsync(DateTime date)
        {
            var targetDate = DateOnly.FromDateTime(date);
            return await _context.Payments.Where(p => p.PaymentDate.HasValue && p.PaymentDate.Value == targetDate).SumAsync(p => p.Amount ?? 0);
        }

        public async Task<decimal> GetTotalRevenueByMonthAsync(int year, int month)
        {
            return await _context.Payments.Where(p => p.PaymentDate.HasValue && p.PaymentDate.Value.Year == year && p.PaymentDate.Value.Month == month)
                .SumAsync(p => p.Amount ?? 0);
        }

        public async Task<decimal> GetTotalRevenueByYearAsync(int year)
        {
            return await _context.Payments.Where(p => p.PaymentDate.HasValue && p.PaymentDate.Value.Year == year).SumAsync(p => p.Amount ?? 0);
        }
        public async Task<decimal> GetPaidRevenueByYearAsync(int year)
        {
            return await _context.Payments
                .Where(p => p.PaymentDate.HasValue && p.PaymentDate.Value.Year == year && p.Status == "Paid")
                .SumAsync(p => p.Amount ?? 0);
        }

        public async Task<decimal> GetPendingRevenueByYearAsync(int year)
        {
            return await _context.Payments
                .Where(p => p.PaymentDate.HasValue && p.PaymentDate.Value.Year == year && p.Status == "Pending")
                .SumAsync(p => p.Amount ?? 0);
        }
        public async Task<decimal> GetPaidRevenueByDateAsync(DateTime date)
        {
            var targetDate = DateOnly.FromDateTime(date);
            return await _context.Payments.Where(p => p.PaymentDate.HasValue && p.PaymentDate.Value == targetDate && p.Status == "Paid").SumAsync(p => p.Amount ?? 0);
        }

        public async Task<decimal> GetPendingRevenueByDateAsync(DateTime date)
        {
            var targetDate = DateOnly.FromDateTime(date);
            return await _context.Payments.Where(p => p.PaymentDate.HasValue && p.PaymentDate.Value == targetDate && p.Status == "Pending").SumAsync(p => p.Amount ?? 0);
        }
        public async Task<decimal> GetPendingRevenueByMonthAsync(int year, int month)
        {
            return await _context.Payments
                .Where(p => p.PaymentDate.HasValue && p.PaymentDate.Value.Year == year && p.PaymentDate.Value.Month == month && p.Status == "Pending")
                .SumAsync(p => p.Amount ?? 0);
        }

        //public async Task<decimal> GetPendingRevenueByYearAsync(int year)
        //{
        //    return await _context.Payments
        //        .Where(p => p.PaymentDate.HasValue && p.PaymentDate.Value.Year == year && p.Status == "Pending")
        //        .SumAsync(p => p.Amount ?? 0);
        //}

        public async Task<int> GetEnrollmentCountByDateAsync(DateTime date)
        {
            var targetDate = DateOnly.FromDateTime(date);
            return await _context.Enrollments
                .Where(e => e.EnrollmentDate.HasValue && e.EnrollmentDate.Value == targetDate)
                .CountAsync();
        }
        public async Task<int> GetEnrollmentCountByMonthAsync(int year, int month)
        {
            return await _context.Enrollments
                .Where(e => e.EnrollmentDate.HasValue && e.EnrollmentDate.Value.Year == year && e.EnrollmentDate.Value.Month == month)
                .CountAsync();
        }

        public async Task<int> GetEnrollmentCountByYearAsync(int year)
        {
            return await _context.Enrollments
                .Where(e => e.EnrollmentDate.HasValue && e.EnrollmentDate.Value.Year == year)
                .CountAsync();
        }
        public async Task<List<dynamic>> GetEnrollmentByCourseAndDateAsync(DateTime date)
        {
            var targetDate = DateOnly.FromDateTime(date);
            return await _context.Courses
                .GroupJoin(
                    _context.Enrollments.Where(e => e.EnrollmentDate.HasValue && e.EnrollmentDate.Value == targetDate),
                    c => c.Id,
                    e => e.CourseId,
                    (course, enrollments) => new
                    {
                        CourseName = course.Name,
                        TotalEnrollments = enrollments.Count()
                    })
                .OrderByDescending(x => x.TotalEnrollments)
                .ToListAsync<dynamic>();
        }
        public async Task<List<dynamic>> GetEnrollmentByCourseAndMonthAsync(int year, int month)
        {
            return await _context.Courses
                .GroupJoin(
                    _context.Enrollments.Where(e => e.EnrollmentDate.HasValue && e.EnrollmentDate.Value.Year == year && e.EnrollmentDate.Value.Month == month),
                    c => c.Id,
                    e => e.CourseId,
                    (course, enrollments) => new
                    {
                        CourseName = course.Name,
                        TotalEnrollments = enrollments.Count()
                    })
                .OrderByDescending(x => x.TotalEnrollments)
                .ToListAsync<dynamic>();
        }

        public async Task<List<dynamic>> GetEnrollmentByCourseAndYearAsync(int year)
        {
            return await _context.Courses
                .GroupJoin(
                    _context.Enrollments.Where(e => e.EnrollmentDate.HasValue && e.EnrollmentDate.Value.Year == year),
                    c => c.Id,
                    e => e.CourseId,
                    (course, enrollments) => new
                    {
                        CourseName = course.Name,
                        TotalEnrollments = enrollments.Count()
                    })
                .OrderByDescending(x => x.TotalEnrollments)
                .ToListAsync<dynamic>();
        }

        public async Task<List<dynamic>> GetAttendanceStatusByDateAsync(DateTime date)
        {
            var targetDate = DateOnly.FromDateTime(date);
            return await _context.Classes
                .GroupJoin(
                    _context.Schedules.Where(s => s.SessionDate.HasValue && s.SessionDate.Value == targetDate),
                    c => c.Id,
                    s => s.ClassId,
                    (classEntity, schedules) => new
                    {
                        ClassName = classEntity.Name,
                        TotalStudents = schedules.SelectMany(s => s.Attendances).Count(),
                        Present = schedules.SelectMany(s => s.Attendances).Count(a => a.Status == "present"),
                        Absent = schedules.SelectMany(s => s.Attendances).Count(a => a.Status == "absent"),
                        Late = schedules.SelectMany(s => s.Attendances).Count(a => a.Status == "late"),
                        Excused = schedules.SelectMany(s => s.Attendances).Count(a => a.Status == "excused")
                    })
                .OrderBy(x => x.ClassName)
                .ToListAsync<dynamic>();
        }
        public async Task<List<dynamic>> GetAttendanceStatusByMonthAsync(int year, int month)
        {
            return await _context.Classes
                .GroupJoin(
                    _context.Schedules.Where(s => s.SessionDate.HasValue && s.SessionDate.Value.Year == year && s.SessionDate.Value.Month == month),
                    c => c.Id,
                    s => s.ClassId,
                    (classEntity, schedules) => new
                    {
                        ClassName = classEntity.Name,
                        TotalStudents = schedules.SelectMany(s => s.Attendances).Count(),
                        Present = schedules.SelectMany(s => s.Attendances).Count(a => a.Status == "present"),
                        Absent = schedules.SelectMany(s => s.Attendances).Count(a => a.Status == "absent"),
                        Late = schedules.SelectMany(s => s.Attendances).Count(a => a.Status == "late"),
                        Excused = schedules.SelectMany(s => s.Attendances).Count(a => a.Status == "excused")
                    })
                .OrderBy(x => x.ClassName)
                .ToListAsync<dynamic>();
        }

        public async Task<List<dynamic>> GetAttendanceStatusByYearAsync(int year)
        {
            return await _context.Classes
                .GroupJoin(
                    _context.Schedules.Where(s => s.SessionDate.HasValue && s.SessionDate.Value.Year == year),
                    c => c.Id,
                    s => s.ClassId,
                    (classEntity, schedules) => new
                    {
                        ClassName = classEntity.Name,
                        TotalStudents = schedules.SelectMany(s => s.Attendances).Count(),
                        Present = schedules.SelectMany(s => s.Attendances).Count(a => a.Status == "present"),
                        Absent = schedules.SelectMany(s => s.Attendances).Count(a => a.Status == "absent"),
                        Late = schedules.SelectMany(s => s.Attendances).Count(a => a.Status == "late"),
                        Excused = schedules.SelectMany(s => s.Attendances).Count(a => a.Status == "excused")
                    })
                .OrderBy(x => x.ClassName)
                .ToListAsync<dynamic>();
        }
        public async Task<List<dynamic>> GetPaymentDetailsByDateAsync(DateTime date)
        {
            var targetDate = DateOnly.FromDateTime(date);
            return await _context.Payments
                .Where(p => p.PaymentDate.HasValue && p.PaymentDate.Value == targetDate)
                .Select(p => new
                {
                    StudentName = p.Enrollment != null && p.Enrollment.Student != null ? p.Enrollment.Student.FullName : null,
                    CourseName = p.Enrollment != null && p.Enrollment.Course != null ? p.Enrollment.Course.Name : null,
                    PaymentDate = p.PaymentDate,
                    Amount = p.Amount,
                    Status = p.Status
                })
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync<dynamic>();
        }

        public async Task<List<dynamic>> GetPaymentDetailsByMonthAsync(int year, int month)
        {
            return await _context.Payments
                .Where(p => p.PaymentDate.HasValue && p.PaymentDate.Value.Year == year && p.PaymentDate.Value.Month == month)
                .Select(p => new
                {
                    StudentName = p.Enrollment != null && p.Enrollment.Student != null ? p.Enrollment.Student.FullName : null,
                    CourseName = p.Enrollment != null && p.Enrollment.Course != null ? p.Enrollment.Course.Name : null,
                    PaymentDate = p.PaymentDate,
                    Amount = p.Amount,
                    Status = p.Status
                })
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync<dynamic>();
        }

        public async Task<List<dynamic>> GetPaymentDetailsByYearAsync(int year)
        {
            return await _context.Payments
                .Where(p => p.PaymentDate.HasValue && p.PaymentDate.Value.Year == year)
                .Select(p => new
                {
                    StudentName = p.Enrollment != null && p.Enrollment.Student != null ? p.Enrollment.Student.FullName : null,
                    CourseName = p.Enrollment != null && p.Enrollment.Course != null ? p.Enrollment.Course.Name : null,
                    PaymentDate = p.PaymentDate,
                    Amount = p.Amount,
                    Status = p.Status
                })
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync<dynamic>();
        }

        public async Task<List<dynamic>> GetEnrollmentDetailsByDateAsync(DateTime date)
        {
            var targetDate = DateOnly.FromDateTime(date);
            return await _context.Enrollments
                .Where(e => e.EnrollmentDate.HasValue && e.EnrollmentDate.Value == targetDate)
                .Select(e => new
                {
                    StudentName = e.Student != null ? e.Student.FullName : null,
                    CourseName = e.Course != null ? e.Course.Name : null,
                    EnrollmentDate = e.EnrollmentDate
                })
                .OrderByDescending(e => e.EnrollmentDate)
                .ToListAsync<dynamic>();
        }

        public async Task<List<dynamic>> GetEnrollmentDetailsByMonthAsync(int year, int month)
        {
            return await _context.Enrollments
                .Where(e => e.EnrollmentDate.HasValue && e.EnrollmentDate.Value.Year == year && e.EnrollmentDate.Value.Month == month)
                .Select(e => new
                {
                    StudentName = e.Student != null ? e.Student.FullName : null,
                    CourseName = e.Course != null ? e.Course.Name : null,
                    EnrollmentDate = e.EnrollmentDate
                })
                .OrderByDescending(e => e.EnrollmentDate)
                .ToListAsync<dynamic>();
        }

        public async Task<List<dynamic>> GetEnrollmentDetailsByYearAsync(int year)
        {
            return await _context.Enrollments
                .Where(e => e.EnrollmentDate.HasValue && e.EnrollmentDate.Value.Year == year)
                .Select(e => new
                {
                    StudentName = e.Student != null ? e.Student.FullName : null,
                    CourseName = e.Course != null ? e.Course.Name : null,
                    EnrollmentDate = e.EnrollmentDate
                })
                .OrderByDescending(e => e.EnrollmentDate)
                .ToListAsync<dynamic>();
        }

        public async Task<List<dynamic>> GetAttendanceDetailsByDateAsync(DateTime date)
        {
            var targetDate = DateOnly.FromDateTime(date);
            return await _context.Attendances
                .Where(a => a.Schedule != null && a.Schedule.SessionDate.HasValue && a.Schedule.SessionDate.Value == targetDate)
                .Select(a => new
                {
                    ClassName = a.Schedule != null && a.Schedule.Class != null ? a.Schedule.Class.Name : null,
                    ScheduleDate = a.Schedule != null ? a.Schedule.SessionDate : null,
                    StudentName = a.Student != null ? a.Student.FullName : null,
                    AttendanceStatus = a.Status,
                    Note = a.Note
                })
                .OrderBy(a => a.ClassName).ThenBy(a => a.ScheduleDate).ThenBy(a => a.StudentName)
                .ToListAsync<dynamic>();
        }

        public async Task<List<dynamic>> GetAttendanceDetailsByMonthAsync(int year, int month)
        {
            return await _context.Attendances
                .Where(a => a.Schedule != null && a.Schedule.SessionDate.HasValue && a.Schedule.SessionDate.Value.Year == year && a.Schedule.SessionDate.Value.Month == month)
                .Select(a => new
                {
                    ClassName = a.Schedule != null && a.Schedule.Class != null ? a.Schedule.Class.Name : null,
                    ScheduleDate = a.Schedule != null ? a.Schedule.SessionDate : null,
                    StudentName = a.Student != null ? a.Student.FullName : null,
                    AttendanceStatus = a.Status,
                    Note = a.Note
                })
                .OrderBy(a => a.ClassName).ThenBy(a => a.ScheduleDate).ThenBy(a => a.StudentName)
                .ToListAsync<dynamic>();
        }

        public async Task<List<dynamic>> GetAttendanceDetailsByYearAsync(int year)
        {
            return await _context.Attendances
                .Where(a => a.Schedule != null && a.Schedule.SessionDate.HasValue && a.Schedule.SessionDate.Value.Year == year)
                .Select(a => new
                {
                    ClassName = a.Schedule != null && a.Schedule.Class != null ? a.Schedule.Class.Name : null,
                    ScheduleDate = a.Schedule != null ? a.Schedule.SessionDate : null,
                    StudentName = a.Student != null ? a.Student.FullName : null,
                    AttendanceStatus = a.Status,
                    Note = a.Note
                })
                .OrderBy(a => a.ClassName).ThenBy(a => a.ScheduleDate).ThenBy(a => a.StudentName)
                .ToListAsync<dynamic>();
        }

        public async Task<List<dynamic>> GetScoreDetailsByDateAsync(string classId, DateTime date)
        {
            var targetDate = date.Date;
            // compute average score per record using available score parts
            var scores = await _context.Scores
                .Where(s => s.ClassId == classId && s.CreatedAt.HasValue && s.CreatedAt.Value.Date == targetDate)
                .Select(s => new
                {
                    Average = ((s.Lisening ?? 0) + (s.Speaking ?? 0) + (s.Reading ?? 0) + (s.Writing ?? 0)) / 4,
                    CreatedAt = s.CreatedAt
                })
                .ToListAsync();

            var total = scores.Count;
            var excellent = scores.Count(s => s.Average >= 9);
            var good = scores.Count(s => s.Average >= 7 && s.Average < 9);
            var fair = scores.Count(s => s.Average >= 5 && s.Average < 7);
            var weak = scores.Count(s => s.Average < 5);

            return new List<dynamic> {
                new { TotalScores = total, Xuatsac = excellent, Tot = good, TrungBinh = fair, Yeu = weak }
            };
        }

        public async Task<List<dynamic>> GetScoreDetailsByMonthAsync(string classId, int year, int month)
        {
            var scores = await _context.Scores
                .Where(s => s.ClassId == classId && s.CreatedAt.HasValue && s.CreatedAt.Value.Year == year && s.CreatedAt.Value.Month == month)
                .Select(s => new
                {
                    Average = ((s.Lisening ?? 0) + (s.Speaking ?? 0) + (s.Reading ?? 0) + (s.Writing ?? 0)) / 4
                })
                .ToListAsync();

            var total = scores.Count;
            var excellent = scores.Count(s => s.Average >= 9);
            var good = scores.Count(s => s.Average >= 7 && s.Average < 9);
            var fair = scores.Count(s => s.Average >= 5 && s.Average < 7);
            var weak = scores.Count(s => s.Average < 5);

            return new List<dynamic> {
                new { TotalScores = total, Xuatsac = excellent, Tot = good, TrungBinh = fair, Yeu = weak }
            };
        }

        public async Task<List<dynamic>> GetScoreDetailsByYearAsync(string classId, int year)
        {
            var scores = await _context.Scores
                .Where(s => s.ClassId == classId && s.CreatedAt.HasValue && s.CreatedAt.Value.Year == year)
                .Select(s => new
                {
                    Average = ((s.Lisening ?? 0) + (s.Speaking ?? 0) + (s.Reading ?? 0) + (s.Writing ?? 0)) / 4
                })
                .ToListAsync();

            var total = scores.Count;
            var excellent = scores.Count(s => s.Average >= 9);
            var good = scores.Count(s => s.Average >= 7 && s.Average < 9);
            var fair = scores.Count(s => s.Average >= 5 && s.Average < 7);
            var weak = scores.Count(s => s.Average < 5);

            return new List<dynamic> {
                new { TotalScores = total, Xuatsac = excellent, Tot = good, TrungBinh = fair, Yeu = weak }
            };
        }

        public async Task<List<dynamic>> GetAverageScoreByClassAndDateAsync(DateTime date)
        {
            var targetDate = date.Date;
            return await _context.Classes
                .GroupJoin(
                    _context.Scores.Where(s => s.CreatedAt.HasValue && s.CreatedAt.Value.Date == targetDate),
                    c => c.Id,
                    s => s.ClassId,
                    (cls, scores) => new
                    {
                        ClassName = cls.Name,
                        AverageScore = scores.Any() ? scores.Average(s => ((s.Lisening ?? 0) + (s.Speaking ?? 0) + (s.Reading ?? 0) + (s.Writing ?? 0)) / 4) : 0
                    })
                .OrderBy(x => x.ClassName)
                .ToListAsync<dynamic>();
        }

        public async Task<List<dynamic>> GetAverageScoreByClassAndMonthAsync(int year, int month)
        {
            return await _context.Classes
                .GroupJoin(
                    _context.Scores.Where(s => s.CreatedAt.HasValue && s.CreatedAt.Value.Year == year && s.CreatedAt.Value.Month == month),
                    c => c.Id,
                    s => s.ClassId,
                    (cls, scores) => new
                    {
                        ClassName = cls.Name,
                        AverageScore = scores.Any() ? scores.Average(s => ((s.Lisening ?? 0) + (s.Speaking ?? 0) + (s.Reading ?? 0) + (s.Writing ?? 0)) / 4) : 0
                    })
                .OrderBy(x => x.ClassName)
                .ToListAsync<dynamic>();
        }

        public async Task<List<dynamic>> GetAverageScoreByClassAndYearAsync(int year)
        {
            return await _context.Classes
                .GroupJoin(
                    _context.Scores.Where(s => s.CreatedAt.HasValue && s.CreatedAt.Value.Year == year),
                    c => c.Id,
                    s => s.ClassId,
                    (cls, scores) => new
                    {
                        ClassName = cls.Name,
                        AverageScore = scores.Any() ? scores.Average(s => ((s.Lisening ?? 0) + (s.Speaking ?? 0) + (s.Reading ?? 0) + (s.Writing ?? 0)) / 4) : 0
                    })
                .OrderBy(x => x.ClassName)
                .ToListAsync<dynamic>();
        }
    }
}
