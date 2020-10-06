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
using Newtonsoft.Json;
using System;

namespace ComprovantesPagamento
{
    public class Startup
    {
        private IHostEnvironment _env;

        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            _env = env;

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

            DatabaseConfig dbConfig;
            DropboxConfig dropboxConfig;
            JwtConfig jwtConfig;
            
            if(_env.EnvironmentName == "Development")
            {
                dbConfig = Configuration.GetSection("MongoDB").Get<DatabaseConfig>();
                dropboxConfig = Configuration.GetSection("Dropbox").Get<DropboxConfig>();
                jwtConfig = Configuration.GetSection("JWT").Get<JwtConfig>();
            }
            else
            {
                dbConfig = JsonConvert.DeserializeObject<DatabaseConfig>(Environment.GetEnvironmentVariable("MONGODB_CONFIG"));
                dropboxConfig = JsonConvert.DeserializeObject<DropboxConfig>(Environment.GetEnvironmentVariable("DROPBOX_CONFIG"));
                jwtConfig = JsonConvert.DeserializeObject<JwtConfig>(Environment.GetEnvironmentVariable("JWT_CONFIG"));
            }


            var jwtService = new JwtService(jwtConfig);
            services.AddSingleton(jwtService);
            services.AddSingleton(jwtConfig);
            services.AddTransient<PaymentTypeRepository>();
            services.AddTransient<PaymentRepository>();
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
                cfg.CreateMap<Payment, PaymentResponse>();
                cfg.CreateMap<User, RegisterResponse>();
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
