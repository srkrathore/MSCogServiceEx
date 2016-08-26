using MSCogServiceEx.Model;
using MSCogServiceEx.Services;
using MSCogServiceEx.Services.BingSearch;
using MvvmHelpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MSCogServiceEx.ViewModel
{
    public class ImageSearchViewModel : INotifyPropertyChanged
    {

        string searchString = "Cute Monkey";
        public string SearchString
        {
            get { return searchString; }
            set
            {
                searchString = value;
                OnPropertyChanged();
            }
        }
        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { isBusy = value; OnPropertyChanged(); }
        }

        List<ImageResult> searchResult;
        public List<ImageResult> Images
        {
            get { return searchResult; }
            set { searchResult = value; OnPropertyChanged(); }
        }
        ICommand getImages;
        public ICommand GetImagesCommand =>
                getImages ??
                (getImages = new Command(async () => await SearchForImages()));

        public async Task SearchForImages()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            //Bing Image API
            var url = $"https://api.cognitive.microsoft.com/bing/v5.0/images/" +
                      $"search?q={searchString}" +
                      $"&count=20&offset=0&mkt=en-us&safeSearch=Strict";

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", CognitiveServicesKeys.BingSearch);

                    var json = await client.GetStringAsync(url);

                    var result = JsonConvert.DeserializeObject<SearchResult>(json);

                    Images = result.Images.Select(i => new ImageResult
                    {
                        ContextLink = i.HostPageUrl,
                        FileFormat = i.EncodingFormat,
                        ImageLink = i.ContentUrl,
                        ThumbnailLink = i.ThumbnailUrl,
                        Title = i.Name
                    }).ToList();
                  
                }
            }
            catch (Exception ex)
            {
                //  ("Unable to query images: " + ex.Message);               
            }
            finally
            {
                IsBusy = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    //public class ImageSearchViewModel
    //{
    //    public ObservableRangeCollection<ImageResult> Images { get; }

    //    public async Task AnalyzeImageAsync(string imageUrl)
    //    {
    //        var result = string.Empty;
    //        try
    //        {
    //            using (var client = new HttpClient())
    //            {
    //                var stream = await client.GetStreamAsync(imageUrl);

    //                var emotion = await EmotionService.GetAverageHappinessScoreAsync(stream);

    //                result = EmotionService.GetHappinessMessage(emotion);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            result = "Unable to analyze image";
    //        }

    //        await UserDialogs.Instance.AlertAsync(result);

    //    }






    //    public async Task TakePhotoAndAnalyzeAsync(bool useCamera = true)
    //    {
    //        string result = "Error";
    //        MediaFile file = null;
    //        try
    //        {

    //            await CrossMedia.Current.Initialize();


    //            if (useCamera)
    //            {
    //                file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
    //                {
    //                    Directory = "Samples",
    //                    Name = "test.jpg",
    //                    SaveToAlbum = true
    //                });
    //            }
    //            else
    //            {
    //                file = await CrossMedia.Current.PickPhotoAsync();
    //            }


    //            if (file == null)
    //                result = "No photo taken.";
    //            else
    //            {
    //                var emotion = await EmotionService.GetAverageHappinessScoreAsync(file.GetStream());

    //                result = EmotionService.GetHappinessMessage(emotion);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            result = ex.Message;
    //        }

    //        await UserDialogs.Instance.AlertAsync(result, "Emotion", "OK");
    //    }

    //}
}
