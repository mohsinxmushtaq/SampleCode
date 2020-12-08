using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using OrchardCore.Navigation;

namespace LabelTool.ReceiptScanner
{
    public class LabelToolMenu : INavigationProvider
    {
        private readonly IStringLocalizer S;

        public LabelToolMenu(IStringLocalizer<LabelToolMenu> localizer)
        {
            S = localizer;
        }

        public Task BuildNavigationAsync(string name, NavigationBuilder builder)
        {
            // We want to add our menus to the "admin" menu only.
            if (!String.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return Task.CompletedTask;
            }

            // Adding our menu items to the builder.
            // The builder represents the full admin menu tree.
            builder
                .Add(S["Label Tool"], S["Label Tool"].PrefixPosition(), labelTool => labelTool
                .AddClass("label-tool").Id("label-tool")
                .Add(S["Create Project"], S["Create Project"].PrefixPosition(), receipts => receipts
                       .Action("Create", "Projects", "LabelTool.ReceiptScanner"))
                .Add(S["Edit Project"], S["Edit Project"].PrefixPosition(), editor => editor
                       .Action("Editor", "Projects", "LabelTool.ReceiptScanner")));

            return Task.CompletedTask;
        }
    }
}
