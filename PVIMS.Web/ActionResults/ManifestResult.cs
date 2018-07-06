using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Offline.WebUI.ActionResults
{
    public class ManifestResult : FileResult
    {
        public ManifestResult(string version)
            :base("text/cache-manifest")
        {
            Version = version;
            CacheResources = new List<string>();
            NetworkResources = new List<string>();
            FallbackResources = new Dictionary<string, string>();
            Settings = new Dictionary<string, string>();
        }

        public string Version { get; set; }
        public IEnumerable<string> CacheResources { get; set; }
        public IEnumerable<string> NetworkResources { get; set; }
        public Dictionary<string, string> FallbackResources { get; set; }
        public Dictionary<string, string> Settings { get; set; }

        protected override void WriteFile(HttpResponseBase response)
        {
            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.Cache.SetExpires(DateTime.MinValue);

            WriteManifestHeader(response);
            WriteCacheResources(response);
            WriteNetworkResources(response);
            WriteFallbackResources(response);
            WriteSettings(response);
        }

        private void WriteManifestHeader(HttpResponseBase response)
        {
            response.Output.WriteLine("CACHE MANIFEST");
            response.Output.WriteLine(string.Format("#V{0}", Version ?? string.Empty));
        }

        private void WriteCacheResources(HttpResponseBase response)
        {
            response.Output.WriteLine("CACHE:");

            foreach (var cacheResource in CacheResources)
            {
                response.Output.WriteLine(cacheResource);
            }
        }

        private void WriteNetworkResources(HttpResponseBase response)
        {
            response.Output.WriteLine();
            response.Output.WriteLine("NETWORK:");

            foreach (var networkResource in NetworkResources)
            {
                response.Output.WriteLine(networkResource);
            }
        }

        private void WriteFallbackResources(HttpResponseBase response)
        {
            response.Output.WriteLine();
            response.Output.WriteLine("FALLBACK:");

            foreach (var fallbackResource in FallbackResources)
            {
                response.Output.WriteLine(string.Format("{0} {1}", fallbackResource.Key, fallbackResource.Value));
            }
        }

        private void WriteSettings(HttpResponseBase response)
        {
            response.Output.WriteLine();
            response.Output.WriteLine("SETTINGS:");

            foreach (var setting in Settings)
            {
                response.Output.WriteLine(string.Format("{0} {1}", setting.Key, setting.Value));
            }
        }
    }
}