using DevIO.Api.Configurations;
using DevIO.Api.Extensions;
using DevIO.Data.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevIO.Api
{
  public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MeuDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentityConfiguration(Configuration);
            services.AddControllers();
            services.AddAutoMapper(typeof(Startup));
            services.ResolveDependencies();

            services.AddWebApiConfig();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
#if DEBUG
            app.UseDeveloperExceptionPage();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DevIO.Api v1"));
#else
            app.UseHsts();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DevIO.Api Release v1"));
#endif
            app.UseSwagger();
            app.UseAuthentication();
            app.UseConfiguration();
        }
    }
}
