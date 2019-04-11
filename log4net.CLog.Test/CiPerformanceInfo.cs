using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log4net.CLog.Test
{
    [Serializable]
    public class CiPerformanceInfo
    {
        [JsonProperty("application")]
        public string Application { get; set; }

        [JsonProperty("applicationName")]
        public string ApplicationName { get; set; }

        [JsonProperty("deploymentId")]
        public long DeploymentId { get; set; }

        [JsonProperty("step")]
        public string Step { get; set; }

        [JsonProperty("stepName")]
        public string StepName { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("actionName")]
        public string ActionName { get; set; }

        [JsonProperty("logTime")]
        public DateTime LogTime { get; set; }

        [JsonProperty("duration")]
        public double Duration { get; set; }

        [JsonProperty("description")]
        public String Description { get; set; }
    }

    public class Inner
    {
        public string Title { get; set; }
        public string Content { get; set; }

    }
}
