using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;

namespace asp.net_core_client_mvc
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
            services.AddMvc();

            // Turn off the JWT claim type mapping to allow well-known claims (e.g 'sub' and 'idp') to flow through unmolested.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            // Adds the authentication services to DI
            // Here we are using a cookie as the primary means to authenticate a user.
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            // Add the handler that can process cookies
            .AddCookie("Cookies")
            // Used to configure the handler that perform the OpenID Connect protocol.
            .AddOpenIdConnect("oidc", options =>
            {
                // Used to persist the tokens from IdentityServer in the cookie
                options.SignInScheme = "Cookies";

                // Indicates that we are trusting IdentityServer
                options.Authority = "http://localhost:5000";
                options.RequireHttpsMetadata = false;
                
                options.ClientId = "mvc";
                // Has to match secret on Identity
                options.ClientSecret = "secret";
                // code id_token basically means "use hybrid flow"
                options.ResponseType = "code id_token";

                // The OpenID Connect middleware saves the tokens automatically for us
                // Tokens are stored inside the properties section of the cookie.
                // The easiest way to access them is by using extension methods from the Microsoft.AspNetCore.Authentication namespace
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;

                options.Scope.Add("administrationApi");
                options.Scope.Add("offline_access");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            // Ensure the authentication services execute on each request.
            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();

            app.UseMvc(routes =>
            {
                routes.MapRoute("signin-oidc", "signin-oidc", defaults: new { controller = "Home", action = "Account"});
                routes.MapRoute("signout-callback-oidc", "signout-callback-oidc", defaults: new { controller = "Home", action = "LoggedOut" });
            });
        }
    }
}
