using FluentMigrator;

namespace QuartzServiceWithDBLogs.Migrations
{
    [Migration(0001)]
    public class Version0001_Setup : Migration
    {
        public override void Up()
        {
            // Create the Quartz Tables
            Execute.EmbeddedScript("QuartzServiceWithDBLogs.Migrations.QuartzTables.sql");

            // Create the log table
            Create.Table("ApplicationLog")
                .WithColumn("LogId").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("Level").AsString(50).NotNullable()
                .WithColumn("Date").AsDateTime().NotNullable()
                .WithColumn("Source").AsString(50).NotNullable()
                .WithColumn("Message").AsString(int.MaxValue).Nullable()
                .WithColumn("Data").AsString(int.MaxValue).Nullable();
        }

        public override void Down()
        {
            Delete.Table("ApplicationLog");
            Delete.Table("QRTZ_CALENDARS");
            Delete.Table("QRTZ_CRON_TRIGGERS");
            Delete.Table("QRTZ_FIRED_TRIGGERS");
            Delete.Table("QRTZ_PAUSED_TRIGGER_GRPS");
            Delete.Table("QRTZ_SCHEDULER_STATE");
            Delete.Table("QRTZ_LOCKS");
            Delete.Table("QRTZ_JOB_DETAILS");
            Delete.Table("QRTZ_SIMPLE_TRIGGERS");
            Delete.Table("QRTZ_SIMPROP_TRIGGERS");
            Delete.Table("QRTZ_BLOB_TRIGGERS");
            Delete.Table("QRTZ_TRIGGERS");
        }
    }
}
