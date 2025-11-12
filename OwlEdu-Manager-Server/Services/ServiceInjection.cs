namespace OwlEdu_Manager_Server.Services
{
    public static class ServiceInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<StudentService>();
            services.AddScoped<AccountService>();
            services.AddScoped<AttendanceService>();
            services.AddScoped<ClassAssignmentService>();
            services.AddScoped<CourseService>();
            services.AddScoped<ClassService>();
            services.AddScoped<EnrollmentService>();
            services.AddScoped<PaymentService>();
            services.AddScoped<ScheduleService>();
            services.AddScoped<ScoreService>();
            services.AddScoped<TeacherService>();
            return services;
        }
    }
}
