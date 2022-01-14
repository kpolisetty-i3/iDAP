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

        private AzureKeyCredential  credentials { get; set; }
        private TextAnalyticsClient client { get; set; }

        public CognitiveLanguage(string endpointUrl, string accessKey)
        {
            AzureKeyCredential credentials = new AzureKeyCredential(accessKey);
            client = new TextAnalyticsClient(new Uri(endpointUrl), credentials);
        }

        public StringBuilder GetKeyPhrases(StringBuilder text)
        {
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
            StringBuilder keyPhrases = new StringBuilder();

            var response = client.RecognizeEntities(text.ToString());
            foreach (CategorizedEntity keyphrase in response.Value)
            {
                keyPhrases.Append($" {keyphrase.Text} - {keyphrase.Category},");
            }
            return keyPhrases;
        }

        public StringBuilder GetLinkedEntities(StringBuilder text)
        {
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
