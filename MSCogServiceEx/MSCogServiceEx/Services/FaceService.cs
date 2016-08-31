using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MSCogServiceEx.Services
{
    public class FaceService
    {
        public static async Task<FaceRectangle[]> UploadAndDetectFaces(Stream stream)
        {
            var fClient = new FaceServiceClient(CognitiveServicesKeys.FaceKey);

            var faces = await fClient.DetectAsync(stream);
            var faceRects = faces.Select(face => face.FaceRectangle);

            if (faceRects == null || faceRects.Count() == 0)
            {
                throw new Exception("Can't detect the faces");
            }
            return faceRects.ToArray(); 
        }
    }
}
