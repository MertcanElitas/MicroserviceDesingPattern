using Common;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SagaStateMachineWorkerService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SagaStateMachineWorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {

                    services.AddMassTransit(configure =>
                    {
                        configure.AddSagaStateMachine<OrderStateMachine, OrderStateInstance>()
                        .EntityFrameworkRepository(opt =>
                        {
                            opt.AddDbContext<DbContext, OrderStateDbContext>((provider, builder) =>
                            {
                                builder.UseSqlServer(hostContext.Configuration.GetConnectionString("SqlConnectionString"),
                                    m => m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name));
                            });
                        });

                        configure.AddBus(prv => Bus.Factory.CreateUsingRabbitMq(cfg =>
                        {
                            cfg.Host(hostContext.Configuration["RabbitMQ:Url"], "/", host =>
                            {
                                host.Username(hostContext.Configuration["RabbitMQ:Username"]);
                                host.Password(hostContext.Configuration["RabbitMQ:Password"]);
                            });

                            cfg.ReceiveEndpoint(RabbitMQConstants.OrderSaga, e =>
                            {
                                e.ConfigureSaga<OrderStateInstance>(prv);
                            });
                        }));
                    });

                    services.AddMassTransitHostedService();
                    services.AddHostedService<Worker>();
                });
    }
}
