using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using StockConsumer.Consumer.Services;
using StockData.CacheService.Redis;
using StockData.Repositories.Concrete;
using StockData.Repositories.Interfaces;
using StockService.Helpers;
using StockService.Interfaces;
using System;
using System.Text.Json.Serialization;

namespace StockAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            #region swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("BeymenStockAPISwagger", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Stock Api",
                    Description = "An API to perform Stock operations"
                });
            });
            #endregion

            #region ModelState Error Disabled

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            #endregion

            #region json

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

            });
            #endregion

            services.AddControllers(options =>
            {
                options.ReturnHttpNotAcceptable = true;
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            });

            services.AddCors(o => o.AddPolicy("myclients", builder =>
            {
                builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
            }));

            services.AddHostedService<StockConsumer.Consumer.StockConsumer>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


            #region AutoFac

            var builder = new ContainerBuilder();
            builder.Populate(services);

            builder.RegisterType<StockService.Services.StockService>().As<IStockService>().SingleInstance();
            builder.RegisterGeneric(typeof(BaseMongoRepository<>)).As(typeof(IBaseMongoRepository<>)).InstancePerLifetimeScope();
            builder.RegisterType<StockRepository>().As<IStockRepository>().SingleInstance();
            builder.RegisterType<RedisRepository>().As<IRedisRepository>().SingleInstance();
            builder.RegisterType<RedisServer>().SingleInstance();
            builder.RegisterType<StockConsumerService>().SingleInstance();
            builder.RegisterType<RabbitHelper>().InstancePerLifetimeScope();
            #endregion

            var appContainer = builder.Build();
            return new AutofacServiceProvider(appContainer);     

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("myclients");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/BeymenStockAPISwagger/swagger.json", "BEYMEN");
            });
        }
    }
}
