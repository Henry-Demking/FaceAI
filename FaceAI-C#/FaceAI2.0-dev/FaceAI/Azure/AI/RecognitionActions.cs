using FaceAI.Classes;
using FaceAI.Exceptions;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Console = FaceAI.Classes.Console;

namespace FaceAI.Azure.AI
{
    class RecognitionActions
    {
        private FaceModel model;
        // Path to temporary folder
        string tempPath;
        private Console console;
        private double threashold;
        private List<Face> cache;
        public RecognitionActions(string tempPath, Console console, List<Face> cache,double threashold = 0.75)
        {
            this.tempPath = tempPath;
            this.threashold = threashold;
            this.model = new FaceModel();
            this.console = console;
            this.cache = cache;
        }

        public async Task<bool> ImageisFaceAsync(Bitmap image)
        {
            console.Out("> Saving image");
            BlobImage blobImage = await BlobCommonActions.SaveImageAsync(tempPath, image);
            console.Out("> Image saved");
            console.Out("> Checking face is viable");
            bool isFace = await model.ImageisFaceAsync(blobImage.Url);
            console.Out($"> Face viable: {isFace}");
            // Delete the image for privacy
            console.Out($"> Deleting image");
            BlobCommonActions.DeleteImage(blobImage);
            console.Out($"> Image deleted");
            return isFace;
        }

        public async Task<bool> ImageisFaceAsync(BlobImage blobImage)
        {
            console.Out("> Checking face is viable");
            bool isFace = await model.ImageisFaceAsync(blobImage.Url);
            console.Out($"> Face viable: {isFace}");

            return isFace;
        }

        public async Task<List<List<Face>>> FindSimilar(List<Face> parents, ProgressBar pbar, int incrimentVal)
        {
            // Get file names
            console.Out($"> Retreiving files");
            List<string> targetImageFileNames = await BlobCommonActions.GetFilesAsync();
            List<Face> targetFaces = new List<Face>();
            List<List<Face>> finalResults = new List<List<Face>>();
            int incrimentable;
            if (cache.Count < targetImageFileNames.Count)
            {

                incrimentable = 50 / targetImageFileNames.Count();
                // Create ckass for each face
                int z = 1;
                foreach (string targetImageFileName in targetImageFileNames)
                {
                    console.Out($"> {z}/{targetImageFileNames.Count} Creating face info for : {targetImageFileName}");
                    console.Out($"...");
                    string url = "https://6221faces.blob.core.windows.net/faces/" + targetImageFileName;
                    // Detect faces from target image url.
                    var faces = await model.DetectFaceRecognize(url);
                    // Add detected faceId to list of values about this face.
                    int incri2 = incrimentable / faces.Count;
                    foreach (var face in faces)
                    {
                        Face faceVal = new Face(targetImageFileName, url, 0, face.FaceId.Value, face.FaceRectangle);
                        targetFaces.Add(faceVal);
                        pbar.Value = pbar.Value + incri2;
                    }
                    z++;
                }
                console.Out($"> Saving to cache");
                cache = targetFaces.ConvertAll(face => new Face(face.Filename, face.Path, face.Similarity, face.ImageGuid, face.Rectangle, face.DetectedFace));
            } else
            {
                console.Out($"> Retrieving from cache");
                targetFaces = cache.ConvertAll(face => new Face(face.Filename, face.Path, face.Similarity, face.ImageGuid, face.Rectangle, face.DetectedFace));
                pbar.Value = pbar.Value + 50;
            }
            console.Out($"> Creating face sublist");
            int splitRation;
            if (targetFaces.Count <= 10)
            {
                splitRation = 10;
            } else if(targetFaces.Count <= 20){
                splitRation = 20;
            } else
            {
                splitRation = Math.Min(50, targetFaces.Count/20);
            }
            
            incrimentable = (100-(pbar.Value+30)) / splitRation;
            int j = 0;
            foreach (List<Face> subList in SplitList(targetFaces, splitRation))
            {
                console.Out($"> Getting results for sublist {j}");
                List<List<Face>> results = await GetResultsAsync(subList, parents, pbar, incrimentable);
                if (finalResults.Count == 0)
                {
                    finalResults = results;
                } else
                {
                    if (results.Count == 0)
                    {
                        continue;
                    }
                    for (int i =0; i<results.Count; i++)
                    {
                        foreach (Face face in results[i])
                        {
                            finalResults[i].Add(face);
                        }
                    }
                }
                j++;
            }

            foreach(Face face in cache)
            {
                face.Similarity = 0;
            }
            console.Out($"> Found all results");
            return finalResults;
        }

        public async Task<List<Face>> FindParent(Bitmap image)
        {
            console.Out($"> Finding parents");
            console.Out($"> Saving image");
            BlobImage blobImage = await BlobCommonActions.SaveImageAsync(tempPath, image);
            console.Out($"> Detecting faces");
            List<DetectedFace> detected = await model.DetectFaceRecognize(blobImage.Url);
            console.Out($"> Image saved");

            List<Face> allFaces = new List<Face>();
            foreach (var face in detected)
            {
                Face thisFace = new Face(blobImage.Filename, blobImage.Url, 1, face.FaceId.Value, face.FaceRectangle, face);
                allFaces.Add(thisFace);
            }
            console.Out($"> Parents found");
            return allFaces;
        }

        private async Task<List<List<Face>>> GetResultsAsync(List<Face> targetSnip, List<Face> parents, ProgressBar pbar, int incri)
            {
            List<List<Face>> results = new List<List<Face>>();
            
            try {
                int incrimentable = incri / 2;
                int i = 1;
                foreach (Face parent in parents)
                {
                    console.Out($"\tParent {i}/{parents.Count}");
                    console.Out($"\t\tGetting similarity");
                    IList<SimilarFace> vals = await model.FindSimilar(parent.DetectedFace, targetSnip);

                    List<Face> parentSimilar = targetSnip.ConvertAll(face => new Face(face.Filename, face.Path, face.Similarity, face.ImageGuid, face.Rectangle, face.DetectedFace));
                    foreach (SimilarFace face in vals)
                    {
                        foreach (Face aFace in parentSimilar.Where(x =>
                            {return (x.ImageGuid != parent.DetectedFace.FaceId) && (x.ImageGuid == face.FaceId);
                            }))
                        {
                            aFace.Similarity = face.Confidence >= this.threashold ? face.Confidence : 0;
                        }
                    }
                    results.Add(parentSimilar);
                    pbar.Value = pbar.Value + incrimentable;
                    i++;
                }
            
            } catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return results;
        }

        public static IEnumerable<List<T>> SplitList<T>(List<T> bigList, int nSize = 3)
        {
            for (int i = 0; i < bigList.Count; i += nSize)
            {
                yield return bigList.GetRange(i, Math.Min(nSize, bigList.Count - i));
            }
        }
    }
}
