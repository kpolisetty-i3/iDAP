using Newtonsoft.Json;
using OCDETF.iDAP.Core.Library.NLP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace OCDETF.iDAP.Core.Library
{
    public class SpacyProcessor : INaturalLangProcessor
    {
        public string EndpointUrl { get; set; }

        public SpacyProcessor(string endpoint)
        {
            this.EndpointUrl = endpoint;
        }

        private static readonly HttpClient _httpClient = new HttpClient();

        public IList<NLPResult> GetEntities(string text)
        {
            var json = new NLPRequest() { Model = "en", Text = text };
            var returnValue = new List<NLPResult>();
            using (var content = new StringContent(JsonConvert.SerializeObject(json), System.Text.Encoding.UTF8, "application/json"))
            {
                try
                {
                    HttpResponseMessage result = _httpClient.PostAsync(EndpointUrl, content).Result;
                    returnValue = JsonConvert.DeserializeObject<List<NLPResult>>(result.Content.ReadAsStringAsync().Result);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
                return returnValue;
            }
        }

        public string GetPersons(IList<NLPResult> entities)
        {
            StringBuilder result = new StringBuilder();
            if (entities.Count > 0)
            {
                var filter = entities.Where(sel => sel.Type == "PERSON" || sel.Type == "NORP").ToList();
                foreach (NLPResult avalue in filter)
                {
                    result.Append($"{avalue.Text} - ");
                }
                return result.ToString().Trim();
            }
            else
                return string.Empty;
        }

        public string GetOrgs(IList<NLPResult> entities)
        {
            StringBuilder result = new StringBuilder();
            if (entities.Count > 0)
            {
                var filter = entities.Where(sel => sel.Type == "ORG").ToList();
                foreach (NLPResult avalue in filter)
                {
                    result.Append($"{avalue.Text}, ");
                }
                return result.ToString().Trim();
            }
            else
                return string.Empty;
        }

        public IList<NLPResult> GetAll(string text)
        {
            var json = new NLPRequest() { Model = "en", Text = text };
            var returnValue = new List<NLPResult>();

            using (var content = new StringContent(JsonConvert.SerializeObject(json), System.Text.Encoding.UTF8, "application/json"))
            {
                try
                {
                    HttpResponseMessage result = _httpClient.PostAsync(EndpointUrl, content).Result;
                    returnValue = JsonConvert.DeserializeObject<List<NLPResult>>(result.Content.ReadAsStringAsync().Result);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
                return returnValue;
            }
        }
    }
}
