namespace InfoJenkins.Reader
{
    using InfoJenkins.Dto;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    public class JenkinsReader : IJenkinsReader
    {
        private List<Niles.Model.Job> allJobs = new List<Niles.Model.Job>();
        private List<Niles.Model.Job> filteredJobs = new List<Niles.Model.Job>();

        private static object LockObject = new object();

        public IEnumerable<Niles.Model.Job> GetJobs(Uri baseUri)
        {
            var client = new Niles.Client.JsonJenkinsClient();
            var node = client.GetResource<Niles.Model.Node>(baseUri);
            var jobsDefinitions = node.Jobs;

            Console.WriteLine($"There are {jobsDefinitions.Count} in the provided address" + Environment.NewLine);
            int currentJob = 0;
            jobsDefinitions.AsParallel().ForAll(jobDefinition =>
            {
                Console.Write($"\rGathering job {currentJob} out of {jobsDefinitions.Count}");
                Niles.Model.Job jobToAdd = client.GetResource<Niles.Model.Job>(jobDefinition.Url, 1);
                lock (LockObject)
                {
                    allJobs.Add(jobToAdd);
                    currentJob++;
                }
            });

            Console.WriteLine(Environment.NewLine + Environment.NewLine + "  All jobs have been gathered, proceeding to filter them");

            var originJob = allJobs.FirstOrDefault(job => job.Name.Equals(ConfigurationManager.AppSettings["StartingFilteringJobName"] ?? string.Empty));

            if (originJob == null)
            {
                Console.WriteLine(Environment.NewLine + $"There specified starting job is not found within the retrieved ones");
                return filteredJobs;
            }
            else
            {
                AddJobRecursive(originJob);

                Console.WriteLine(Environment.NewLine + $"There are {filteredJobs.Count} jobs that match the filtering criteria");

                return filteredJobs;
            }
        }

        private void AddJobRecursive(Niles.Model.Job job)
        {
            filteredJobs.Add(job);
            if (job.Name.Equals(ConfigurationManager.AppSettings["EndingFilteringJobName"] ?? string.Empty))
                return;
            if (job.DownstreamProjects.Any())
            {
                foreach (var downstreamProject in job.DownstreamProjects)
                {
                    AddJobRecursive(allJobs.FirstOrDefault(downstreamJob => downstreamJob.Name.Equals(downstreamProject.Name)));
                }
            }
        }

        private JobItem GetFullItem(Niles.Model.Job job, IEnumerable<Niles.Model.Job> childs, IEnumerable<Niles.Model.Job> parents)
        {
            var result = ParseItem(job);
            result.Childs = ParseCollection(childs);
            result.Parents = ParseCollection(parents);
            return result;
        }

        private IEnumerable<JobItem> ParseCollection(IEnumerable<Niles.Model.Job> items)
        {
            ICollection<JobItem> result = null;

            if (items != null && items.Any())
            {
                result = new List<JobItem>(items.Count());
                foreach (var item in items)
                {
                    result.Add(ParseItem(item));
                }
            }

            return result;
        }

        private JobItem ParseItem(Niles.Model.Job job)
        {
            var result = new JobItem();
            result.Key = job.Name;
            result.Name = job.Name;
            return result;
        }
    }
}