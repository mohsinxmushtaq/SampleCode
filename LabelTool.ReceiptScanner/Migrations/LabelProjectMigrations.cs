using LabelTool.ReceiptScanner.Models;
using OrchardCore.Data.Migration;
using System;
using System.Collections.Generic;
using System.Text;

namespace LabelTool.ReceiptScanner.Migrations
{
    class LabelProjectMigrations : DataMigration
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(nameof(LabelProject), table => table
                 .Column<int>(nameof(LabelProject.Id), col => col.PrimaryKey().Identity())
                 .Column<string>(nameof(LabelProject.DisplayName))
                 .Column<string>(nameof(LabelProject.AzureConnection))
                 .Column<string>(nameof(LabelProject.FolderPath))
                 .Column<string>(nameof(LabelProject.Owner), col => col.NotNull())
                 .Column<DateTime>(nameof(LabelProject.CreatedUtc), col => col.NotNull())
            );

            return 1;
        }
    }
}
