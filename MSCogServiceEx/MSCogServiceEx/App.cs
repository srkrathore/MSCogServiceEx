
using MSCogServiceEx.View;
using MSCogServiceEx.ViewModel;
using Xamarin.Forms;

namespace MSCogServiceEx
{
    public class App : Application
    {
        public App()
        {
            var tabs = new TabbedPage
            {
                Title = "MS Cognitive Services",
                //BindingContext = new WeatherViewModel(),
                Children =
                {
                    new ImageSearch() {Title="Search Image" ,BindingContext = new ImageSearchViewModel() } ,
                    new EmotionEx() { Title="Emotion Ex.", BindingContext = new EmotionViewModel()},
                    new FaceEx() {Title="Face Ex.",BindingContext=new FaceViewModel()}
                }
            };

            MainPage = new NavigationPage(tabs)
            {
                BarBackgroundColor = Color.FromHex("3498db"),
                BarTextColor = Color.White
            };
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
