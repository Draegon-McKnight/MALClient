﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MALClient.Comm;
using HtmlAgilityPack;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages
{

    public class ProfileData
    {
        //Anime
        public int AnimeWatching { get; set; }
        public int AnimeCompleted { get; set; }
        public int AnimeOnHold { get; set; }
        public int AnimeDropped { get; set; }
        public int AnimePlanned { get; set; }
        public int AnimeTotal { get; set; }
        public int AnimeRewatched { get; set; }
        public int AnimeEpisodes { get; set; }
        //Manga
        public int MangaReading { get; set; }
        public int MangaCompleted { get; set; }
        public int MangaOnHold { get; set; }
        public int MangaDropped { get; set; }
        public int MangaPlanned { get; set; }
        public int MangaTotal { get; set; }
        public int MangaReread { get; set; }
        public int MangaVolumes { get; set; }
        public int MangaChapters{ get; set; }
        //Days
        public float AnimeDays { get; set; }
        public float MangaDays { get; set; }
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProfilePage : Page
    {
        private ProfileData _data;

        public ProfileData Data
        {
            get { return _data; }
            set
            {
                _data = value;
                Utils.GetMainPageInstance()?.SaveProfileData(_data);
            }
        }

        public ProfilePage()
        {
            this.InitializeComponent();         
            SpinnerLoading.Background = new SolidColorBrush(Color.FromArgb(160,230,230,230));  
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SpinnerLoading.Visibility = Visibility.Visible;
            if (e.Parameter != null)
            {
                _data = e.Parameter as ProfileData;
                PopulateAnimeData();
                PopulateMangaData();
                SpinnerLoading.Visibility = Visibility.Collapsed;
            }
            else
                 PullData();

            
            Utils.GetMainPageInstance()?.SetStatus($"{Creditentials.UserName} - Profile");
            base.OnNavigatedTo(e);
        }

        private async void PullData()
        {
            var data = await new MALProfileQuery().GetRequestResponse();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(data);
            //AnimeStats
            var watching = doc.DocumentNode.Descendants("li")
               .Where(li => li.InnerText.Contains("Watching") && li.ChildNodes.Count == 2).ToList();
            var plantowatch = doc.DocumentNode.Descendants("li")
                .Where(li => li.InnerText.Contains("Plan to Watch") && li.ChildNodes.Count == 2).ToList();
            var rewatched = doc.DocumentNode.Descendants("li")
                .Where(li => li.InnerText.Contains("Rewatched") && li.ChildNodes.Count == 2).ToList();
            var episodes = doc.DocumentNode.Descendants("li")
                .Where(li => li.InnerText.Contains("Episodes") && li.ChildNodes.Count == 2).ToList();
            //MangaStats
            var reading = doc.DocumentNode.Descendants("li")
                .Where(li => li.InnerText.Contains("Reading") && li.ChildNodes.Count == 2).ToList();
            var plantoread = doc.DocumentNode.Descendants("li")
                .Where(li => li.InnerText.Contains("Plan to Read") && li.ChildNodes.Count == 2).ToList();
            var reread = doc.DocumentNode.Descendants("li")
                .Where(li => li.InnerText.Contains("Reread") && li.ChildNodes.Count == 2).ToList();
            var chapters = doc.DocumentNode.Descendants("li")
                .Where(li => li.InnerText.Contains("Chapters") && li.ChildNodes.Count == 2).ToList();
            var volumes = doc.DocumentNode.Descendants("li")
                .Where(li => li.InnerText.Contains("Volumes") && li.ChildNodes.Count == 2).ToList();
            //Shared
            var total = doc.DocumentNode.Descendants("li")
                .Where(li => li.InnerText.Contains("Total Entries") && li.ChildNodes.Count == 2).ToList();
            var completed = doc.DocumentNode.Descendants("li")
                .Where(li => li.InnerText.Contains("Completed") && li.ChildNodes.Count == 2).ToList();
            var onhold = doc.DocumentNode.Descendants("li")
                .Where(li => li.InnerText.Contains("On-Hold") && li.ChildNodes.Count == 2).ToList();
            var dropped = doc.DocumentNode.Descendants("li")
                .Where(li => li.InnerText.Contains("Dropped") && li.ChildNodes.Count == 2).ToList();
            //Days
            var days = doc.DocumentNode.Descendants("div")
                .Where(div => div.InnerText.Contains("Days:") && div.ChildNodes.Count == 2).ToList();
            Data = new ProfileData
            {
                //Anime
                AnimeWatching = int.Parse(watching[0].ChildNodes[1].InnerText),
                AnimeCompleted = int.Parse(completed[0].ChildNodes[1].InnerText),
                AnimeOnHold = int.Parse(onhold[0].ChildNodes[1].InnerText),
                AnimeDropped = int.Parse(dropped[0].ChildNodes[1].InnerText),
                AnimePlanned = int.Parse(plantowatch[0].ChildNodes[1].InnerText),

                AnimeTotal = int.Parse(total[0].ChildNodes[1].InnerText),
                AnimeEpisodes = int.Parse(episodes[0].ChildNodes[1].InnerText),
                AnimeRewatched = int.Parse(rewatched[0].ChildNodes[1].InnerText),
                //Manga
                MangaReading = int.Parse(reading[0].ChildNodes[1].InnerText),
                MangaCompleted = int.Parse(completed[1].ChildNodes[1].InnerText),
                MangaOnHold = int.Parse(onhold[1].ChildNodes[1].InnerText),
                MangaDropped = int.Parse(dropped[1].ChildNodes[1].InnerText),
                MangaPlanned = int.Parse(plantoread[0].ChildNodes[1].InnerText),

                MangaTotal = int.Parse(total[1].ChildNodes[1].InnerText),
                MangaReread = int.Parse(reread[0].ChildNodes[1].InnerText),
                MangaChapters = int.Parse(chapters[0].ChildNodes[1].InnerText),
                MangaVolumes = int.Parse(volumes[0].ChildNodes[1].InnerText),
                //Days
                AnimeDays = float.Parse(days[0].ChildNodes[1].InnerText),
                MangaDays = float.Parse(days[1].ChildNodes[1].InnerText),
            };
            PopulateAnimeData();
            PopulateMangaData();
            SpinnerLoading.Visibility = Visibility.Collapsed; //end of exec path
        }

        private void PopulateAnimeData()
        {
            List<KeyValuePair<string, int>> s1 = new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("", Data.AnimeWatching)
            };
            SeriesWatching.ItemsSource = s1;

            List<KeyValuePair<string, int>> s2 = new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("", Data.AnimeCompleted)
            };
            SeriesCompleted.ItemsSource = s2;

            List<KeyValuePair<string, int>> s3 = new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("", Data.AnimeOnHold)
            };
            SeriesOnHold.ItemsSource = s3;

            List<KeyValuePair<string, int>> s4 = new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("", Data.AnimeDropped)
            };
            SeriesDropped.ItemsSource = s4;

            List<KeyValuePair<string, int>> s5 = new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("", Data.AnimePlanned)
            };
            SeriesPlanned.ItemsSource = s5;

            TxtAnimeWatchingStats.Text = Data.AnimeWatching.ToString();
            TxtAnimeCompleted.Text = Data.AnimeCompleted.ToString();
            TxtAnimeOnHold.Text = Data.AnimeOnHold.ToString();
            TxtAnimeDropped.Text = Data.AnimeDropped.ToString();
            TxtAnimePlanned.Text = Data.AnimePlanned.ToString();
            TxtAnimeTotal.Text = Data.AnimeTotal.ToString();
            TxtAnimeRewatched.Text = Data.AnimeRewatched.ToString();
            TxtAnimeEpisodes.Text = Data.AnimeEpisodes.ToString();
            TxtAnimeDays.Text = $"Days :  {Data.AnimeDays}";
        }

        private void PopulateMangaData()
        {
            List<KeyValuePair<string, int>> s1 = new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("", Data.MangaReading)
            };
            SeriesReading.ItemsSource = s1;

            List<KeyValuePair<string, int>> s2 = new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("", Data.MangaCompleted)
            };
            SeriesMCompleted.ItemsSource = s2;

            List<KeyValuePair<string, int>> s3 = new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("", Data.MangaOnHold)
            };
            SeriesMOnHold.ItemsSource = s3;

            List<KeyValuePair<string, int>> s4 = new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("", Data.MangaDropped)
            };
            SeriesMDropped.ItemsSource = s4;

            List<KeyValuePair<string, int>> s5 = new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("", Data.MangaPlanned)
            };
            SeriesMPlanned.ItemsSource = s5;

            TxtMangaReadingStats.Text = Data.AnimeWatching.ToString();
            TxtMangaCompleted.Text = Data.MangaCompleted.ToString();
            TxtMangaOnHold.Text = Data.MangaOnHold.ToString();
            TxtMangaDropped.Text = Data.MangaDropped.ToString();
            TxtMangaPlanned.Text = Data.MangaPlanned.ToString();
            TxtMangaTotal.Text = Data.MangaTotal.ToString();
            TxtMangaReread.Text = Data.MangaReread.ToString();
            TxtMangaChapters.Text = Data.MangaChapters.ToString();
            TxtMangaVolumes.Text = Data.MangaVolumes.ToString();
            TxtMangaDays.Text = $"Days :  {Data.MangaDays}";
        }

    }
}