namespace InfoJenkins.Dto
{
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("{Key}")]
    public class JobItem
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public IEnumerable<JobItem> Childs { get; set; }

        public IEnumerable<JobItem> Parents { get; set; }
    }
}