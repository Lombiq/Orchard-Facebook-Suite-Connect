using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;
using Piedone.Facebook.Suite.Models;

namespace Piedone.Facebook.Suite.Migrations
{
    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public class FacebookConnectMigrations : DataMigrationImpl
    {
        public int Create()
        {
            // Creating table FacebookConnectPartRecord
            SchemaBuilder.CreateTable(typeof(FacebookConnectSettingsPartRecord).Name,
                table => table
                    .ContentPartRecord()
                    .Column<string>("Permissions")
                    .Column<bool>("OnlyAllowVerified")
                    .Column<bool>("SimpleRegistration")
            );

            ContentDefinitionManager.AlterTypeDefinition("FacebookConnectWidget",
                cfg => cfg
                    .WithPart(typeof(FacebookConnectWidgetPart).Name)
                    .WithPart("WidgetPart")
                    .WithPart("CommonPart")
                    .WithSetting("Stereotype", "Widget"));

            // Creating table FacebookUserPartRecord
            SchemaBuilder.CreateTable(typeof(FacebookUserPartRecord).Name,
                table => table
                    .ContentPartRecord()
                    .Column<long>("FacebookUserId")
                    .Column<string>("Name")
                    .Column<string>("FirstName")
                    .Column<string>("LastName")
                    .Column<string>("Link")
                    .Column<string>("FacebookUserName")
                    .Column<string>("Gender")
                    .Column<int>("TimeZone")
                    .Column<string>("Locale")
                    .Column<bool>("IsVerified")
            ).AlterTable(typeof(FacebookUserPartRecord).Name,
                table => table
                    .CreateIndex("FacebookUser", new string[] { "FacebookUserId" })
                );


            return 6;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(FacebookUserPartRecord).Name,
                table => table
                    .CreateIndex("FacebookUser", new string[] { "FacebookUserId" })
                );

            return 2;
        }

        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(FacebookUserPartRecord).Name,
                table => table
                    .AlterColumn("Locale", column => column.WithType(System.Data.DbType.String))
                );

            return 3;
        }

        public int UpdateFrom3()
        {
            // Because of the splitting of Connect and the rest of Facebook Suite, this rename is necessary
            SchemaBuilder.ExecuteSql("EXEC sp_rename 'Piedone_Facebook_Suite_FacebookConnectPartRecord', 'Piedone_Facebook_Suite_Connect_FacebookConnectPartRecord'");
            SchemaBuilder.ExecuteSql("EXEC sp_rename 'Piedone_Facebook_Suite_FacebookUserPartRecord', 'Piedone_Facebook_Suite_Connect_FacebookUserPartRecord'");

            return 4;
        }

        public int UpdateFrom4()
        {
            SchemaBuilder.CreateTable(typeof(FacebookConnectSettingsPartRecord).Name,
                table => table
                    .ContentPartRecord()
                    .Column<string>("Permissions")
                    .Column<bool>("AutoLogin")
                    .Column<bool>("OnlyAllowVerified")
                    .Column<bool>("SimpleRegistration")
            );

            ContentDefinitionManager.AlterTypeDefinition("FacebookConnectWidget",
                cfg => cfg
                    .RemovePart("FacebookConnectPart")
                    .WithPart(typeof(FacebookConnectWidgetPart).Name));

            ContentDefinitionManager.DeletePartDefinition("FacebookConnectPart");

            SchemaBuilder.DropTable("FacebookConnectPartRecord");

            return 5;
        }

        public int UpdateFrom5()
        {
            SchemaBuilder.AlterTable(typeof(FacebookConnectSettingsPartRecord).Name,
                table => table
                    .DropColumn("AutoLogin")
                );

            return 6;
        }
    }
}