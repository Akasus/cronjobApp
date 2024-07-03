using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace JobApplication.Jobs
{
    public class EPAJob : ISchedulerJob
    {
        private readonly ILogger<EPAJob> _logger;
        private readonly IJobEventListener _eventListener;

        public EPAJob() { }
        public EPAJob(ILogger<EPAJob>) {


    }
}
