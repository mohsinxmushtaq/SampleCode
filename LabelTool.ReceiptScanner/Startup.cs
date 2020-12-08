using System;
using LabelTool.ReceiptScanner.Migrations;
using LabelTool.ReceiptScanner.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.ResourceManagement;

namespace LabelTool.ReceiptScanner
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IDataMigration, LabelProjectMigrations>();

            services.AddScoped<ILabelToolServices, LabelToolServices>();

            //Admin Menu
            services.AddScoped<INavigationProvider, LabelToolMenu>();
        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            routes.MapAreaControllerRoute(
                name: "Home",
                areaName: "LabelTool.ReceiptScanner",
                pattern: "Home/Index",
                defaults: new { controller = "Home", action = "Index" }
            );
        }
    }
}