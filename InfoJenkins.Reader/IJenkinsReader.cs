namespace InfoJenkins.Reader
{
    using InfoJenkins.Dto;
    using System;
    using System.Collections.Generic;

    public interface IJenkinsReader
    {
        IEnumerable<Niles.Model.Job> GetJobs(Uri baseUri);
    }
}