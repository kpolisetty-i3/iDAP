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
        public string spacy_model = "en_core_web_md";

        public SpacyProcessor(string endpoint)
        {
            this.EndpointUrl = endpoint;
        }

        private static readonly HttpClient _httpClient = new HttpClient();

        public IList<NLPResult> GetEntities(string text)
        {
            var json = new NLPRequest() { model = spacy_model, text = text };
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
            var json = new NLPRequest() { model = spacy_model, text = text };
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

        public HashSet<string> GetPersons(string content)
        {
            IList<NLPResult> entities = GetEntities(content);
            StringBuilder result = new StringBuilder();
            HashSet<string> persons = new HashSet<string>();
            if (entities.Count > 0)
            {
                var filter = entities.Where(sel => sel.Type == "PERSON").ToList();
                foreach (NLPResult avalue in filter)
                    persons.Add(avalue.Text.Trim().Replace("\n", string.Empty));
                //result.AppendLine($"{avalue.Text}");                                    
                return persons;
            }
            return persons;
        }

        public HashSet<string> GetOrgs(string content)
        {
            IList<NLPResult> entities = GetEntities(content);
            StringBuilder result = new StringBuilder();
            HashSet<string> orgs = new HashSet<string>();
            if (entities.Count > 0)
            {
                var filter = entities.Where(sel => sel.Type == "ORG").ToList();
                foreach (NLPResult avalue in filter)
                    orgs.Add(avalue.Text.Trim().Replace("\n", string.Empty));
                //result.AppendLine($"{avalue.Text}");                                    
                return orgs;
            }
            return orgs;
        }
    }
}
