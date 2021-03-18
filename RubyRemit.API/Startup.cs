using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RubyRemit.Business.Contracts;
using RubyRemit.Business.Services;
using RubyRemit.Domain.Interfaces;
using RubyRemit.Infrastructure;
using RubyRemit.Infrastructure.AutoMapperSettings.Profiles;
using RubyRemit.Infrastructure.Repositories;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RubyRemit.Api
{
    public class Startup
    {
        private readonly IWebHostEnvironment _hostingEnv;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _hostingEnv = env;
        }

        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(setupAction =>
            {
                setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status200OK));
                setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
                setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));
                setupAction.Filters.Add(new ProducesAttribute("application/json"));
                setupAction.Filters.Add(new ConsumesAttribute("application/json"));
            });

            services
                .AddDbContext<RubyRemitContext>(options => options
                .UseSqlServer(Configuration.GetConnectionString("DefaultConnStr")));

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IPaymentStateRepository, PaymentStateRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IValidator, Validator>();
            services.AddScoped<IOrchestrator, Orchestrator>();

            services.AddAutoMapper(c => c.AddProfile<DefaultProfile>(), typeof(Startup));

            services.AddHttpClient("paymentGateway", c =>
            {
                if (_hostingEnv.IsDevelopment())
                    c.BaseAddress = new Uri("https://localhost:2000/");
                else
                    c.BaseAddress = new Uri("https://gateways.rubyremit.herokuapp.com/");
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.Timeout = new TimeSpan(0, 0, 30);
            });

            services.AddSwaggerGen(setupAction =>
            {
                // Create and configure OpenAPI specification document with basic information 
                setupAction.SwaggerDoc("RubyRemitOpenAPISpecs",
                    new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Title = "RubyRemit API",
                        Version = "1",
                        Description = "RubyRemit API enables users to submit payment information for processing via an external gateway. " +
                        "This is a work in progress. Consequently, certain features such as 'authentication' will be available in a future version.",
                        Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                        {
                            Email = "philipeano@gmail.com",
                            Name = "Philip Newman",
                            Url = new Uri("https://www.twitter.com/philipeano")
                        },
                        License = new Microsoft.OpenApi.Models.OpenApiLicense()
                        {
                            Name = "MIT License",
                            Url = new Uri("https://opensource.org/licenses/MIT")
                        }
                    });

                //Fetch all XML output documents, and include their content in the OpenAPI specification
                var currentAssembly = Assembly.GetExecutingAssembly();
                var linkedAssemblies = currentAssembly.GetReferencedAssemblies();
                var fullAssemblyList = linkedAssemblies.Union(new AssemblyName[] { currentAssembly.GetName() });
                var xmlCommentFiles = fullAssemblyList
                    .Select(a => Path.Combine(AppContext.BaseDirectory, $"{a.Name}.xml"))
                    .Where(f => File.Exists(f))
                    .ToArray();

                foreach (string xmlFile in xmlCommentFiles)
                {
                    setupAction.IncludeXmlComments(xmlFile);
                }
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseSwagger();
            app.UseSwaggerUI(setupAction =>
            {
                setupAction.SwaggerEndpoint("/swagger/RubyRemitOpenAPISpecs/swagger.json", "RubyRemit API");
                setupAction.RoutePrefix = "";
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}