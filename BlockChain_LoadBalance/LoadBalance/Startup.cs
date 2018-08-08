using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LoadBalance.Db;
using LoadBalance.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace LoadBalance
{
    public class Startup {
        private static JObject ConfigJson = null;


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            
        }

        public static IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            Init();
        }

        public static void Init() {

            var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

            ConfigJson = JObject.Parse(File.ReadAllText(jsonPath));

            
            DbMgr.Instance.Init();
            RedisHelper.Instance.Init();
            NodeChecker.Instance.Start();


        }

        public static T GetConfig<T>(string path) {
            return ConfigJson.SelectToken(path).ToObject<T>();
        }
    }
}
