using MSCogServiceEx.Services;
using Plugin.Media;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MSCogServiceEx.ViewModel
{
    public class EmotionViewModel : INotifyPropertyChanged
    {
        string msgString;
        public string Message
        {
            get { return msgString; }
            set
            {
                msgString = value;
                OnPropertyChanged();
            }
        }

        ImageSource imgSource;
        public ImageSource SelectedImage
        {
            get { return imgSource; }
            set
            {
                imgSource = value;
                OnPropertyChanged();
            }
        }
        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { isBusy = value; OnPropertyChanged(); }
        }
        Stream selectedPicStream = null;
        ICommand takePhoto;
        public ICommand TakePhotoCommand =>
                takePhoto ??
                (takePhoto = new Command(async () => await TakePhoto()));
        public async Task TakePhoto()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                Message = ":(No camera avaialble.";
                IsBusy = false;
                return;
            }
            try
            {
                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "Sample",
                    Name = "EmotionCheckPic.jpg",
                    SaveToAlbum = true
                });

                if (file == null)
                    return;

                //   await DisplayAlert("File Location", (saveToGallery.IsToggled ? file.AlbumPath : file.Path), "OK");

                SelectedImage = ImageSource.FromStream(() =>
                {
                    selectedPicStream = file.GetStream();
                    file.Dispose();
                    return selectedPicStream;
                });
            }
            catch (Exception ex)
            {
                Message = "Uh oh :( Something went wrong";
                IsBusy = false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        ICommand pickPhoto;
        public ICommand PickPhotoCommand =>
                pickPhoto ??
                (pickPhoto = new Command(async () => await SelectPhoto()));
        public async Task SelectPhoto()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                Message = "Permission not granted to photos.";
                IsBusy = false;
                return;
            }
            try
            {
                var file = await CrossMedia.Current.PickPhotoAsync().ConfigureAwait(true);


                if (file == null)
                    return;

                selectedPicStream = file.GetStream();
                file.Dispose();

                SelectedImage = ImageSource.FromStream(() => selectedPicStream);
            }
            catch (Exception ex)
            {
                Message = "Uh oh :( Something went wrong";
                IsBusy = false;
            }
            finally
            {
                IsBusy = false;
            }
        }
        ICommand checkEmotion;
        public ICommand CheckEmotionCommand =>
                checkEmotion ??
                (checkEmotion = new Command(async () => await CheckEmotionFrmService()));
        public async Task CheckEmotionFrmService()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            try
            {
                var emotion = await EmotionService.GetAverageHappinessScoreAsync(selectedPicStream);
                Message = EmotionService.GetHappinessMessage(emotion);
            }
            catch (Exception ex)
            {
                Message = "Uh oh :( Something went wrong \n Unable to analyze image";
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
}
