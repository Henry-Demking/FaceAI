using Microsoft.Azure.CognitiveServices.Vision.Face;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceAI.Azure.AI
{
    class AzureConnection
    {
        private const string SUBSCRIPTION_KEY = "87f845c1b6f845c383164289c8ae42fb";
        private const string ENDPOINT = "https://csci6221.cognitiveservices.azure.com/";

        public static IFaceClient Authenticate()
        {
            return new FaceClient(new ApiKeyServiceClientCredentials(SUBSCRIPTION_KEY)) { Endpoint = ENDPOINT };
        }
    }
}
