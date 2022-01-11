using Azure;
using Azure.AI.TextAnalytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCDETF.iDAP.Azure.Services
{
    public class CognitiveLanguage
    {
        public CognitiveLanguage() { }

        public string endpoint { get; set; }
        public string key { get; set; }

        public CognitiveLanguage(string endpointUrl, string accessKey)
        {
            this.endpoint = endpointUrl;
            this.key = accessKey;
        }
        public StringBuilder GetKeyPhrases(StringBuilder text)
        {

            AzureKeyCredential credentials = new AzureKeyCredential(key);
            var client = new TextAnalyticsClient(new Uri(endpoint), credentials);
            StringBuilder keyPhrases = new StringBuilder();

            var response = client.ExtractKeyPhrases(text.ToString());
            foreach (string keyphrase in response.Value)
            {
                keyPhrases.AppendLine(keyphrase);
            }
            return keyPhrases;

        }

        public StringBuilder GetKeyEntitites(StringBuilder text)
        {
            AzureKeyCredential credentials = new AzureKeyCredential(key);
            var client = new TextAnalyticsClient(new Uri(endpoint), credentials);
            StringBuilder keyPhrases = new StringBuilder();

            var response = client.RecognizeEntities(text.ToString());
            foreach (CategorizedEntity keyphrase in response.Value)
            {
                keyPhrases.AppendLine($" {keyphrase.Text} \t\t {keyphrase.ConfidenceScore} \t\t {keyphrase.Category} \t\t {keyphrase.SubCategory} ");
            }
            return keyPhrases;
        }

        public StringBuilder GetLinkedEntities(StringBuilder text)
        {
            AzureKeyCredential credentials = new AzureKeyCredential(key);
            var client = new TextAnalyticsClient(new Uri(endpoint), credentials);
            StringBuilder linkedEntities = new StringBuilder();

            var response = client.RecognizeLinkedEntities(text.ToString());
            foreach (var entity in response.Value)
            {
                linkedEntities.AppendLine($"\tName: {entity.Name},\tID: {entity.DataSourceEntityId},\tURL: {entity.Url}\tData Source: {entity.DataSource}");
                linkedEntities.AppendLine("\tMatches:");
                foreach (var match in entity.Matches)
                {
                    linkedEntities.AppendLine($"\t\tText: {match.Text}");
                    linkedEntities.AppendLine($"\t\tScore: {match.ConfidenceScore:F2}\n");
                }
            }
            return linkedEntities;
        }
    }
}
