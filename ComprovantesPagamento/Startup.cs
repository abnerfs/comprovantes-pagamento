using ComprovantesPagamento.Contracts;
using ComprovantesPagamento.Domain.Models;
using ComprovantesPagamento.Domain.Responses;
using ComprovantesPagamento.Repositories;
using ComprovantesPagamento.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ComprovantesPagamento
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var dbConfig = Configuration.GetSection("MongoDB").Get<DatabaseConfig>();
            var dropboxConfig = Configuration.GetSection("Dropbox").Get<DropboxConfig>();
            var jwtConfig = Configuration.GetSection("JWT").Get<JwtConfig>();

            var jwtService = new JwtService(jwtConfig);
            services.AddSingleton(jwtService);
            services.AddSingleton(jwtConfig);
            services.AddTransient<PaymentTypeRepository>();
            services.AddTransient<UserRepository>();
            services.AddSingleton<DropboxService>();
            services.AddSingleton<IDbService>(new DbService(dbConfig));
            services.AddSingleton(dropboxConfig);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = jwtService.GetTokenValidationParams();
            });

            #region Automapper
            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PaymentType, PaymentTypeResponse>();
            });

            services.AddSingleton(config.CreateMapper());
            #endregion

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
