using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;

namespace ServiceA.API
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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ServiceA.API", Version = "v1" });
            });

            services.AddHttpClient<ProductService>(opt =>
            {
                opt.BaseAddress = new Uri("https://localhost:5002/api/Products/");
            }).AddPolicyHandler(GetRetryPolicy());
        }

        //CircuitBraker'ın daha az gelişmiş versiyonu.
        //Aşağıdaki configurasyona göre uygulama 3 hatalı request ten sonra
        //10 sn boyunca açık devre şekline girerek reuqesti atmadan hata mesajı döner.
        //10 sn bittikten sonra 1 request atar eğer yine başarısız ise tekrar 10 sn açık devre haline döner.
        private IAsyncPolicy<HttpResponseMessage> GetCircuitBraker()
        {
            return HttpPolicyExtensions.HandleTransientHttpError()
                .CircuitBreakerAsync(3
                    , TimeSpan.FromSeconds(10)
                    ,onBreak:(arg1,arg2)=>{ Debug.WriteLine("Circuit Braker Status => On Break"); }
                    ,onReset:()=>{ Debug.WriteLine("Circuit Braker Status => On Braker Reset"); }
                    ,onHalfOpen:()=>{ Debug.WriteLine("Circuit Braker Status => On Braker Half Open"); });
        }

        //CircuitBraker'ın advence versiyonu.
        //Aşağıdaki configurasyona göre uygulama 30 saniye boyunca hata alınan istekleri sayar.
        //Hata alan istekler toplam istek sayısının Yüzde 10'nundan fazla ise ve hata alan istek sayısı
        //5 den büyük ise 10 saniye boyunca uygulama açık devre haline geçer. 10 sn'den sonra hatalı 1 istek alırsa
        //tekrar açık devre hale geçer.
        public IAsyncPolicy<HttpResponseMessage> GetAdvenceCircuitBraker()
        {
            return HttpPolicyExtensions.HandleTransientHttpError()
                .AdvancedCircuitBreakerAsync(0.1, //Hatalı istekler yüzde 10 ve fazlası ise
                    TimeSpan.FromSeconds(30), // 30 saniye boyunca gelen istekleri takip eder.
                    5, //Hatalı istek sayısı 5 ten fazla ise
                    TimeSpan.FromSeconds(10)  //10 sn boyunca açık devre haline geçer.
                    , onBreak: (arg1, arg2) => { Debug.WriteLine("Circuit Braker Status => On Break"); }
                    , onReset: () => { Debug.WriteLine("Circuit Braker Status => On Braker Reset"); }
                    , onHalfOpen: () => { Debug.WriteLine("Circuit Braker Status => On Braker Half Open"); } );
        }

        private IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
           return  HttpPolicyExtensions.HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
                .WaitAndRetryAsync(5, retryAttempt =>
                {
                    Console.WriteLine($"Retry count:{retryAttempt}");
                    return TimeSpan.FromSeconds(10);
                }, onRetryAsync: onRetryAsync);
        }

        public Task onRetryAsync(DelegateResult<HttpResponseMessage> arg1, TimeSpan arg2)
        {
            Console.WriteLine($"Request is made again:{arg2.TotalMilliseconds}");

            return Task.CompletedTask;
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ServiceA.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}