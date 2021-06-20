using System.Collections.Generic;

namespace QuartzServiceWithDBLogs.Config
{
    public class QuartzSettings
    {
        public Dictionary<string, string> ConnectionStrings { get; set; }
        public string HelloWorldSchedule { get; set; }
        public string InstanceName { get; set; }
    }
}
