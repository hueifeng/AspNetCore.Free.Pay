using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OmniPay.Alipay.Extensions;
using OmniPay.Core.Configuration.DependencyInjection;
using OmniPay.Unionpay.Extensions;
using OmniPay.Wechatpay.Extensions;
using System.Net.Http;
using System.Text;

namespace OmniPay.Pay
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
            //΢��֧������Щ�ӿ���Ҫ��֤������
            //var clientCertificate =
            //    new X509Certificate2(
            //        "Certs/apiclient_cert.p12", "1233410002");
            var handler = new HttpClientHandler();
            //handler.ClientCertificates.Add(clientCertificate);
            //������ʽ�ͻ��˴����������������
            services.AddHttpClient("WeChatPaysHttpClientName", c =>
            {
            }).ConfigurePrimaryHttpMessageHandler(() => handler);

            services.AddOmniPayService(builder =>
            {
                builder.Services.AddWeChatPay(options =>
                {
                    Configuration.GetSection("WeChatPays").Bind(options);
                }).AddValidators();
                builder.Services.AddAliPay(options =>
                {
                    Configuration.GetSection("AliPays").Bind(options);
                });
				builder.Services.AddUnionPay(options =>
                {
                    Configuration.GetSection("Unionpays").Bind(options);
                });
            });

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}");
            });

            // app.UseOmniPay();   
            // app.UseAliOmniPay();
            // app.UseUnionPay();

        }
    }
}
