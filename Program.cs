using Microsoft.EntityFrameworkCore;
using study_ia_web_app.Data;
using study_ia_web_app.Repositories.archivo;
using study_ia_web_app.Repositories.planstudio;
using study_ia_web_app.Repositories.quiz;
using study_ia_web_app.Repositories.resumen;
using study_ia_web_app.Services;
using study_ia_web_app.Services.quiz;
using study_ia_web_app.Services.studio;
using study_ia_web_app.Utils;
using System.Text.Json.Serialization;


namespace study_ia_web_app
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddScoped<IFileRepository, FileRepository>();
            builder.Services.AddScoped<GoogleDriveService>();
            builder.Services.AddHttpClient<IAiService, AiService>();
            builder.Services.AddScoped<IResumenRepository, ResumenRepository>();
            // Imporante orden quiz
            builder.Services.AddScoped<IQuizService, QuizService>();
            builder.Services.AddScoped<IAiQuizGenerator, AiQuizGenerator>();
            builder.Services.AddScoped<IQuizBuilderService, QuizBuilderService>();
            builder.Services.AddScoped<IQuizRepository, QuizRepository>();
            // plan de estudio
            builder.Services.AddScoped<IPlanEstudioService, PlanEstudioService>();
            builder.Services.AddScoped<IPlanEstudioRepository, PlanEstudioRepository>();
            builder.Services.AddScoped<IAiPlanService, AiPlanService>();
            
            
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Console.WriteLine("Excepción no controlada: " + e.ExceptionObject.ToString());
            };

            // API IA
            builder.Services.Configure<OpenRouterSettings>
                (builder.Configuration.GetSection("OpenRouter"));

            // API EVELABS VOICE
            builder.Services.Configure<ElevenLabsSettings>
                (builder.Configuration.GetSection("ElevenLabs"));


            // API-DRIVE
            builder.Services.Configure<GoogleDriveSettings>(
                builder.Configuration.GetSection("GoogleDrive"));

            // GEMINI
            builder.Services.Configure<GeminiSettings>
                (builder.Configuration.GetSection("GeminiIA"));

            builder.Services.AddHttpClient();


            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("EFCoreDBConnection")));

            // Add services to the container.
            builder.Services.AddControllers()
            .AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            // CORS: permitir cualquier origen (solo para pruebas)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("PermitirTodo", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();

                });
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Study IA Web API V1");
                });
            }

            //app.UseHttpsRedirection();

            // CORS
            app.UseCors("PermitirTodo");

            // Configure the HTTP request pipeline.

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
