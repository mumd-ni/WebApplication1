
 using Microsoft.AspNetCore.Builder;
 using Microsoft.AspNetCore.Hosting;
 using Microsoft.Extensions.DependencyInjection;
 using Microsoft.Extensions.Hosting;
 using Microsoft.EntityFrameworkCore;
 using Microsoft.Extensions.Configuration;
 using Microsoft.AspNetCore.Authentication.JwtBearer;
 using Microsoft.IdentityModel.Tokens;
 using System.Text;
using WebApplication1.Services;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
           
            var builder = WebApplication.CreateBuilder(args);

            // ✅ جلب إعدادات الاتصال من `appsettings.json`
            var configuration = builder.Configuration;

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // ✅ إضافة قاعدة البيانات (Entity Framework Core - SQL Server)
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IEmailService, EmailService>(); // تسجيل EmailService
            // ✅ تفعيل `Controllers`
            builder.Services.AddControllers();

            // ✅ تمكين CORS (للسماح للـ Flutter App بالاتصال)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader());
            });

            // ✅ تمكين المصادقة باستخدام `JWT` (اختياري)
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "YourIssuer",
                        ValidAudience = "YourAudience",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSuperSecretKey"))
                    };
                });

            // ✅ إضافة `Swagger` لتوثيق API
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // ✅ استخدام `Swagger` في وضع التطوير
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                 {
                 options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
                 });
            }

            // ✅ تفعيل CORS
            app.UseCors("AllowAll");

            // ✅ تفعيل المصادقة (إذا كنت تستخدم JWT)
            app.UseAuthentication();
            app.UseAuthorization();

            // ✅ توجيه الطلبات إلى الـ API Controllers
            //  app.MapControllers();
            app.MapControllers();

            // ✅ تشغيل التطبيق
            app.Run();

        }
    }
}
