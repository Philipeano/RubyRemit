using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RubyRemit.Gateways.Services;
using RubyRemit.Infrastructure.PaymentGateways.Contracts;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RubyRemit.Gateways
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
            services.AddControllers(setupAction =>
            {
                setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status200OK));
                setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
                setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));
                setupAction.Filters.Add(new ProducesAttribute("application/json"));
                setupAction.Filters.Add(new ConsumesAttribute("application/json"));
            });

            services.AddScoped<ICheapPaymentGateway, BasicPaymentService>();
            services.AddScoped<IExpensivePaymentGateway, PremiumPaymentService>();
            services.AddSwaggerGen(setupAction =>
            {
                // Create and configure OpenAPI specification document with basic information 
                setupAction.SwaggerDoc("RubyRemitGatewaysOpenAPISpecs",
                    new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Title = "RubyRemit Gateways API",
                        Version = "1",
                        Description = "RubyRemit Gateways API accepts a payment processing request from any web client, processes it using one of its gateway implementations, and returns a response. " +
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
                setupAction.SwaggerEndpoint("/swagger/RubyRemitGatewaysOpenAPISpecs/swagger.json", "RubyRemit Gateways API");
                setupAction.RoutePrefix = "";
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
