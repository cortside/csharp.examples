using System.Threading.Tasks;
using Cortside.AspNetCore.Builder;

namespace WeatherForecast.WebApi {
    /// <summary>
    /// Program
    /// </summary>
    public static class Program {
        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Task<int> Main(string[] args) {
            var builder = WebApiHost.CreateBuilder(args)
                //.ConfigureServices(a => {
                //    a.AddControllers();
                //})
                //.ConfigureApplication(a => {
                //    a.UseRouting();
                //    a.UseEndpoints(endpoints => {
                //        endpoints.MapControllers();
                //    });
                //})
                .UseStartup<Startup>();

            var api = builder.Build();

            //api.WebApplication.Environment.IsDevelopment();
            //api.WebApplication.UseSwagger();

            return api.StartAsync();
        }
    }

    //public class Program {
    //    public static void Main(string[] args) {
    //        var builder = WebApplication.CreateBuilder(args);

    //        // Add services to the container.

    //        builder.Services.AddControllers();
    //        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    //        builder.Services.AddEndpointsApiExplorer();
    //        builder.Services.AddSwaggerGen();

    //        var app = builder.Build();

    //        // Configure the HTTP request pipeline.
    //        if (app.Environment.IsDevelopment()) {
    //            app.UseSwagger();
    //            app.UseSwaggerUI();
    //        }

    //        app.UseHttpsRedirection();

    //        app.UseAuthorization();


    //        app.MapControllers();

    //        app.Run();
    //    }
}
