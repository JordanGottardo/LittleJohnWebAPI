using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using LittleJohnWebAPI.Data.Tickers;
using LittleJohnWebAPI.Data.Users;
using LittleJohnWebAPI.Utils;

namespace LittleJohnWebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var fakeTickersService = new FakeTickersService();
            var tickersRepository = new TickersRepository(fakeTickersService);
            var fakeUsersService = new FakeUsersService();
            var usersRepository = new UsersRepository(fakeUsersService);
            var tokenAuthorizer = new TokenAuthorizer();

            services.AddSingleton<ITickersRepository, TickersRepository>(provider => tickersRepository);
            services.AddSingleton<IUsersRepository, UsersRepository>(provider => usersRepository);
            services.AddSingleton<ITokenAuthorizer, TokenAuthorizer>(provider => tokenAuthorizer);

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication("Bearer", options =>
                {
                    options.ApiName = "littlejohnapi";
                    options.Authority = "https://localhost:5201";
                });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "LittleJohnWebAPI", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LittleJohnWebAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
