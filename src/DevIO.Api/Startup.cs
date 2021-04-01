using DevIO.Api.Configuration;
using DevIO.Api.Configurations;
using DevIO.Api.Extensions;
using DevIO.Data.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
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
            services.AddSwaggerConfig();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
#if DEBUG
            app.UseDeveloperExceptionPage();
#else
            app.UseHsts();
#endif
            app.UseSwaggerConfig(provider);
            app.UseAuthentication();
            app.UseConfiguration();
        }
    }
}
