
namespace Systems.SteelToePOC.Web.Api
{
    using log4net;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.Mvc.Routing;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Middlewares;
    using SampleWebApiAspNetCore.Dtos;
    using SampleWebApiAspNetCore.Entities;
    using SampleWebApiAspNetCore.Repositories;
    using SampleWebApiAspNetCore.Services;
    using Steeltoe.Management.Endpoint.Env;
    using Steeltoe.Management.Endpoint.Health;
    using Steeltoe.Management.Endpoint.HeapDump;
    using Steeltoe.Management.Endpoint.Info;
    using Steeltoe.Management.Endpoint.Metrics;
    using Steeltoe.Management.Endpoint.Refresh;
    using Steeltoe.Management.Endpoint.ThreadDump;
    using Steeltoe.Management.Endpoint.Trace;
    using Swashbuckle.AspNetCore.Swagger;

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
            services.AddThreadDumpActuator(Configuration);
            services.AddHeapDumpActuator(Configuration);
            services.AddInfoActuator(Configuration);
            services.AddEnvActuator(Configuration);
            services.AddRefreshActuator(Configuration);
            services.AddHealthActuator(Configuration);
            services.AddTraceActuator(Configuration);
            services.AddMetricsActuator(Configuration);
            services.AddMvc().AddXmlSerializerFormatters()
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddOptions();
            services.AddDbContext<FoodDbContext>(opt => opt.UseInMemoryDatabase("FoodDatabase"));
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            services.AddSingleton<ISeedDataService, SeedDataService>();
            services.AddScoped<IFoodRepository, EfFoodRepository>();
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(implementationFactory =>
            {
                var actionContext = implementationFactory.GetService<IActionContextAccessor>().ActionContext;
                return new UrlHelper(actionContext);
            });

            
            services.AddSwaggerGen(
                options =>
                {
                    options.SwaggerDoc("v1", new Info { Title = "Systems.SteelToePOC", Version = "v1" });
                });
            services.AddSingleton<ILog>(LogManager.GetLogger(typeof(System.Object)));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app,
            IHostingEnvironment env)
        {
            app.UseThreadDumpActuator();
            app.UseHeapDumpActuator();
            app.UseInfoActuator();
            app.UseEnvActuator();
            app.UseRefreshActuator();
            app.UseHealthActuator();
            app.UseTraceActuator();
            app.UseMetricsActuator();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "text/plain";
                        var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                        if (errorFeature != null)
                        {

                        }
                        await context.Response.WriteAsync("There was an error").ConfigureAwait(false);
                    });
                });
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(
       options =>
       {
           options.SwaggerEndpoint("./swagger/v1/swagger.json", "Systems.SteelToePOC V1");
           options.RoutePrefix = ""; //serve the Swagger UI at the app's root

       });

            app.UseHttpsRedirection();
            app.UseCors("AllowAllOrigins");
            AutoMapper.Mapper.Initialize(mapper =>
            {
                mapper.CreateMap<FoodItem, FoodItemDto>().ReverseMap();
                mapper.CreateMap<FoodItem, FoodUpdateDto>().ReverseMap();
                mapper.CreateMap<FoodItem, FoodCreateDto>().ReverseMap();
            });
            app.UseMiddleware(typeof(ExceptionHandler));
            app.UseMvc();
        }
    }
}