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
    public class FaceViewModel : INotifyPropertyChanged
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
                    Name = "FaceCheckPic.jpg",
                    SaveToAlbum = true
                });

                if (file == null)
                    return;

                SelectedImage = ImageSource.FromStream(() =>
                {
                    return file.GetStream();
                });
                selectedPicStream = file.GetStream();
                var vFaces = await FaceService.UploadAndDetectFaces(selectedPicStream);
                Message = "There are " + vFaces.Length.ToString() + " faces in picture";
                file.Dispose();

            }
            catch (Exception ex)
            {
                Message = "Uh oh :( Something went wrong \n " + ex.Message;
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
                SelectedImage = ImageSource.FromStream(() => file.GetStream());
                selectedPicStream = file.GetStream();
                var vFaces = await FaceService.UploadAndDetectFaces(selectedPicStream);
                Message = "There are " + vFaces.Length.ToString() + " faces in picture";
                file.Dispose();

            }
            catch (Exception ex)
            {
                Message = "Uh oh :( Something went wrong \n Error Message : " + ex.Message ;
                IsBusy = false;
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
