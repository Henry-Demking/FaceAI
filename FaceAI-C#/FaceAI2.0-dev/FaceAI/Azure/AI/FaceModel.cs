using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FaceAI.Classes;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace FaceAI.Azure.AI
{
    class FaceModel
    {
        // The client for connecting to azure
        private IFaceClient client;
        // Recognition model
        private string RECOGNITION_MODEL4;

        // Link to image base
        private readonly string IMAGE_BASE_URL = ConfigurationManager.AppSettings.Get("IMAGE_BASE_URL");
        private readonly string FACE_URL = ConfigurationManager.AppSettings.Get("FACE_URL");
        private readonly string PATH_TO_TEMP = Path.GetTempPath() + "FaceAI\\";


        public FaceModel()
        {
            this.client = AzureConnection.Authenticate();
            this.RECOGNITION_MODEL4 = RecognitionModel.Recognition04;
        }

        public async Task<bool> ImageisFaceAsync(string url)
        {
            // Detect faces from image URL. Since only recognizing, use the recognition model 1.
            // We use detection model 3 because we are not retrieving attributes.
            IList <DetectedFace> detectedFaces = await this.client.Face.DetectWithUrlAsync(url, recognitionModel: RECOGNITION_MODEL4, detectionModel: DetectionModel.Detection03);

            return detectedFaces.Count > 0;
        }

        public async Task<List<DetectedFace>> DetectFaceRecognize(string url)
        {
            // Detect faces from image URL. Since only recognizing, use the recognition model 1.
            // We use detection model 3 because we are not retrieving attributes.
            IList<DetectedFace> detectedFaces = await this.client.Face.DetectWithUrlAsync(url, recognitionModel: this.RECOGNITION_MODEL4, detectionModel: DetectionModel.Detection03);
            return detectedFaces.ToList();
        }

        public async Task<IList<SimilarFace>> FindSimilar(DetectedFace detected, IList<Face> targets)
        {
            IList<Guid?> targetGuids = GetIds(targets);

           return await client.Face.FindSimilarAsync(detected.FaceId.Value, null, null, targetGuids);
        }

        private IList<Guid?> GetIds(IList<Face> targets)
        {
            IList<Guid?> guids = new List<Guid?>();

            foreach(Face face in targets)
            {
                guids.Add(face.ImageGuid);
            }

            return guids;
        }
    }
}
