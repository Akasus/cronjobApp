using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Jobs
{
    internal interface ISchedulerJob
    {
        Task ExecuteAsync();
    }
}
