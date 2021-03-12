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
using RubyRemit.Infrastructure.Repositories;

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

            services.AddHttpClient("paymentGateway", c =>
            {
                if (_hostingEnv.IsDevelopment())
                    c.BaseAddress = new System.Uri("https://localhost:2000/");
                else
                    c.BaseAddress = new System.Uri("https://gateways.rubyremit.herokuapp.com");
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.Timeout = new System.TimeSpan(0, 0, 30);
            });

            services.AddAutoMapper(c => c.AddProfile<AutoMapping>(), typeof(Startup));
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
