using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ShopAppBackend.Models.Context;
using ShopAppBackend.Services;
using ShopAppBackend.Settings;
using ShopAppBackend.Settings.Interfaces;

namespace ShopAppBackend
{
    public class Startup
    {
        private readonly IWebHostEnvironment _currentEnvironment;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _currentEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(Configuration["ConnectionString"]));

            services.Configure<ImageSettings>(
                Configuration.GetSection(nameof(ImageSettings)));

            services.AddSingleton<IImageSettings>(sp =>
                sp.GetRequiredService<IOptions<ImageSettings>>().Value);

            var userSettingsSection = Configuration.GetSection(nameof(UserSettings));
            services.Configure<UserSettings>(userSettingsSection);

            services.AddSingleton<IUserSettings>(sp =>
                sp.GetRequiredService<IOptions<UserSettings>>().Value);

            // configure jwt authentication
            var userSettings = userSettingsSection.Get<UserSettings>();
            var key = Encoding.ASCII.GetBytes(userSettings.Secret);
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = !_currentEnvironment.IsDevelopment();
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                // options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore; 
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

            services.AddSingleton<ImageService>();
            services.AddSingleton<AuthService>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            app.UseDeveloperExceptionPage();
            //}

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
