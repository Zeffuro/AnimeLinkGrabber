using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using GetAnimeLinks;
using HtmlAgilityPack;
using Microsoft.Win32;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using Keys = System.Windows.Forms.Keys;
using MyAnimeListSharp;
using MyAnimeListSharp.Auth;
using MyAnimeListSharp.Facade;
using PushbulletSharp;
using PushbulletSharp.Models.Requests;

namespace GetAnimeLinks
{
    public partial class Form1 : Form
    {
        public string CurrentEpisodeSource;
        public string CurrentAnimeMalUrl;
        public MyWebClient WebClient = new MyWebClient();
        public MyWebClient DownloadClient;
        public Stopwatch DownloadStopwatch = new Stopwatch();
        public PushbulletClient PClient = new PushbulletClient("");
        private bool _orderIsDesc = false;
        readonly ICredentialContext _malCredential = new CredentialContext
        {
            UserName = "",
            Password = ""
        };

        public Form1()
        {
            InitializeComponent();
        }

        public static bool SetAllowUnsafeHeaderParsing20()
        {
            //Get the assembly that contains the internal class
            var aNetAssembly = Assembly.GetAssembly(typeof (SettingsSection));
            //Use the assembly in order to get the internal type for the internal class
            var aSettingsType = aNetAssembly?.GetType("System.Net.Configuration.SettingsSectionInternal");
            //Use the internal static property to get an instance of the internal settings class.
            //If the static instance isn't created allready the property will create it for us.
            var anInstance = aSettingsType?.InvokeMember("Section",
                BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.NonPublic, null, null, new object[] {});

            if (anInstance != null)
            {
                //Locate the private bool field that tells the framework is unsafe header parsing should be allowed or not
                var aUseUnsafeHeaderParsing = aSettingsType.GetField("useUnsafeHeaderParsing",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                if (aUseUnsafeHeaderParsing != null)
                {
                    aUseUnsafeHeaderParsing.SetValue(anInstance, true);
                    return true;
                }
            }
            return false;
        }

        static string ROT13(string input)
        {
            return !string.IsNullOrEmpty(input) ? new string(input.ToCharArray().Select(s => { return (char)((s >= 97 && s <= 122) ? ((s + 13 > 122) ? s - 13 : s + 13) : (s >= 65 && s <= 90 ? (s + 13 > 90 ? s - 13 : s + 13) : s)); }).ToArray()) : input;
        }

        private void InitializeForm()
        {
            cbxSite.SelectedIndex = 0;
            loadRecentlyWatched();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeForm();
        }

        private void addRecentlyWatched()
        {
            var animeResult = cbxSearchResults.SelectedItem as AnimeResult;
            var episodeResult = cbxEpisodes.SelectedItem as EpisodeResult;

            var episodeNumber = Regex.Match(episodeResult.EpisodeName, @"\d+(?!\D*\d)").Value;
            string recentlyWatched = animeResult.AnimeName + " " + episodeNumber + " (" + cbxSite.SelectedItem.ToString() + ")";
            if (lstRecentlyWatched.Items.Count > 0 && lstRecentlyWatched.Items[0].ToString() == recentlyWatched)
            {
            }
            else
            {
                lstRecentlyWatched.Items.Insert(0, recentlyWatched);
            }
        }

        private void loadRecentlyWatched()
        {
            lstRecentlyWatched.Items.Clear();
            foreach (var item in Properties.Settings.Default.RecentlyWatched)
            {
                lstRecentlyWatched.Items.Add(item);
            }
        }

        private void saveRecentlyWatched()
        {
            Properties.Settings.Default.RecentlyWatched.Clear();
            foreach (object item in lstRecentlyWatched.Items)
            {
                Properties.Settings.Default.RecentlyWatched.Add(item.ToString());
            }
            Properties.Settings.Default.Save();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            ShowLoading();
            SearchAnime(cbxSite.SelectedItem.ToString(), txtSearch.Text);
            HideLoading();
        }

        private void ShowLoading()
        {
            this.Enable(false);
            pbxWait.Enable(true);
            pbxWait.Enabled = true;
            pbxWait.Visible = true;
        }

        private void HideLoading()
        {
            this.Enable(true);
            pbxWait.Visible = false;
        }

        private string SourceFromMP4Upload(string mp4uploadUrl)
        {
            var htmlMP4 = WebClient.DownloadString(mp4uploadUrl);
            var scriptString = Regex.Replace(htmlMP4, @"\s+", "");
            Regex regex = new Regex(@"file:\s*'(?<File>.*?)'");
            Regex regex2 = new Regex(@"'file':\s*'(?<File>.*?)'");
            Match match = regex.Match(scriptString);
            Match match2 = regex2.Match(scriptString);
            if (match.Success)
            {
                return match.Groups["File"].Value;
            }
            else if(match2.Success)
            {
                return match2.Groups["File"].Value;
            }
            return "";
        }

        private string SourceFromAnimeUploader(string animeUploadUrl)
        {
            var htmlMP4 = WebClient.DownloadString(animeUploadUrl);
            var docResults = new HtmlDocument();
            docResults.LoadHtml(htmlMP4);
            var scriptString = docResults.DocumentNode.SelectSingleNode("//script[contains(., 'setup')]").InnerText;
            scriptString = Regex.Replace(scriptString, @"\s+", "");
            Regex regex = new Regex(@"jwplayer\('player_code'\).*?'file':\s*'(?<File>.*?)'");
            Match match = regex.Match(scriptString);
            if (match.Success)
            {
                return match.Groups["File"].Value;
            }
            return "";
        }

        private string SourceFromAnimeJolt(string animeUploadUrl)
        {
            var htmlMP4 = WebClient.DownloadString(animeUploadUrl);
            var scriptString = Regex.Replace(htmlMP4, @"\s+", "");
            Regex regex = new Regex(@"file:\s*\042(?<File>.*?)\042");
            Regex regex2 = new Regex(@"'file':\s*'(?<File>.*?)'");
            Match match = regex.Match(scriptString);
            Match match2 = regex2.Match(scriptString);
            if (match.Success)
            {
                return match.Groups["File"].Value;
            }
            else if (match2.Success)
            {
                return match2.Groups["File"].Value;
            }
            return "";
        }

        private void SearchAnime(string selectedSite, string searchString)
        {
            var resultList = new List<AnimeResult>();
            switch (selectedSite)
            {
                case "AnimeDao":
                {
                    var searchResultsHtml =
                        WebClient.DownloadString("http://animedao.com/search/" + searchString.Replace(" ", "+") + "/");
                    var docResults = new HtmlDocument();
                    docResults.LoadHtml(searchResultsHtml);
                    var searchNodes =
                        docResults.DocumentNode.SelectNodes("//div[contains(@class, 'thumbnail')]");
                    if (searchNodes == null)
                    {
                        MessageBox.Show("No results found for " + searchString);
                        break;
                    }
                    
                    foreach (var resultNode in searchNodes)
                    {
                        var animeUrl = "http://animedao.com" +
                                        resultNode.SelectSingleNode("a[1]").Attributes["href"].Value;
                        var animeName = resultNode.SelectSingleNode("a[1]/div[1]/center[1]").InnerText;
                        var animePicture = "http://animedao.com" +
                                            resultNode.SelectSingleNode("a[1]/img[1]")
                                                .Attributes["data-original"].Value;

                        var result = new AnimeResult(animeName, animePicture, animeUrl);
                        resultList.Add(result);
                    }
                    if (_orderIsDesc)
                    {
                        resultList.Reverse();
                    }

                    cbxSearchResults.DataSource = resultList;
                    cbxSearchResults.DisplayMember = "AnimeName";
                    cbxSearchResults.ValueMember = "AnimeUrl";
                    return;
                }
                case "AnimeJolt":
                    {
                        var searchResultsHtml =
                            WebClient.DownloadString("http://www.animejolt.org/search?search=" +
                                                        searchString.Replace(" ", "+"));
                        var docResults = new HtmlDocument();
                        docResults.LoadHtml(searchResultsHtml);
                        var searchNodes =
                            docResults.DocumentNode.SelectNodes(
                                "//a[contains(@class, 'mse')]");
                        if (searchNodes == null)
                        {
                            MessageBox.Show("No results found for " + searchString);
                            break;
                        }
                        foreach (var resultNode in searchNodes)
                        {
                            var animeUrl = "http://www.animejolt.org" + resultNode.Attributes["href"].Value;
                            var animeName = resultNode.SelectSingleNode("div[1]/div[1]/div[1]/h2[1]").InnerText;
                            var animePicture = resultNode.SelectSingleNode("div[1]/img[1]").Attributes["src"].Value;

                            var result = new AnimeResult(animeName, animePicture, animeUrl);
                            resultList.Add(result);
                        }
                        if (_orderIsDesc)
                        {
                            resultList.Reverse();
                        }
                        cbxSearchResults.DataSource = resultList;
                        cbxSearchResults.DisplayMember = "AnimeName";
                        cbxSearchResults.ValueMember = "AnimeUrl";
                        return;
                    }
                case "GoGoAnime":
                {
                    try
                    {
                        var searchResultsHtml =
                            WebClient.DownloadString("http://gogoanime.tv/search.html?keyword=" +
                                                     searchString.Replace(" ", "%20"));
                        var docResults = new HtmlDocument();
                        docResults.LoadHtml(searchResultsHtml);
                        var searchNodes =
                            docResults.DocumentNode.SelectNodes(
                                "//div[@class='anime_movies_items']/a");
                        if (searchNodes == null)
                        {
                            MessageBox.Show("No results found for " + searchString);
                            break;
                        }
                        foreach (var resultNode in searchNodes)
                        {
                            var animeUrl = resultNode.Attributes["href"].Value;
                            var animeName = resultNode.Attributes["title"].Value;
                            var animePicture = resultNode.SelectSingleNode("img[1]").Attributes["src"].Value;

                            var result = new AnimeResult(animeName, animePicture, animeUrl);
                            resultList.Add(result);
                        }
                        if (_orderIsDesc)
                        {
                            resultList.Reverse();
                        }
                        cbxSearchResults.DataSource = resultList;
                        cbxSearchResults.DisplayMember = "AnimeName";
                        cbxSearchResults.ValueMember = "AnimeUrl";
                        return;
                    }
                    catch
                    {
                        return;
                    }
                }
                case "AnimesTV":
                {
                    var searchResultsHtml =
                        WebClient.DownloadString("http://www.animestv.us/search?keyword=" +
                                                 searchString.Replace(" ", "+"));
                    var docResults = new HtmlDocument();
                    docResults.LoadHtml(searchResultsHtml);
                    var searchNodes =
                        docResults.DocumentNode.SelectNodes(
                            "//div[contains(@class, 'thumbnail')]/a[1]");
                    if (searchNodes == null)
                    {
                        MessageBox.Show("No results found for " + searchString);
                        break;
                    }
                    foreach (var resultNode in searchNodes)
                    {
                        var animeUrl = "http://www.animestv.us" + resultNode.Attributes["href"].Value;
                        var animeName = resultNode.SelectSingleNode("img[1]").Attributes["title"].Value;
                        var animePicture = resultNode.SelectSingleNode("img[1]").Attributes["src"].Value;

                        var result = new AnimeResult(animeName, animePicture, animeUrl);
                        resultList.Add(result);
                    }
                    if (!_orderIsDesc)
                    {
                        resultList.Reverse();
                    }
                    cbxSearchResults.DataSource = resultList;
                    cbxSearchResults.DisplayMember = "AnimeName";
                    cbxSearchResults.ValueMember = "AnimeUrl";
                    return;
                }
                case "WatchAnimeOnline":
                {
                    var searchResultsHtml =
                        WebClient.DownloadString("http://watchanimeonline.io/search?keyword=" +
                                                    searchString.Replace(" ", "+"));
                    var docResults = new HtmlDocument();
                    docResults.LoadHtml(searchResultsHtml);
                    var searchNodes =
                        docResults.DocumentNode.SelectNodes(
                            "//a[contains(@class, 'readmore')]");
                    if (searchNodes == null)
                    {
                        MessageBox.Show("No results found for " + searchString);
                        break;
                    }
                    foreach (var resultNode in searchNodes)
                    {
                        var animeUrl = "http://watchanimeonline.io" + resultNode.Attributes["href"].Value;
                        var animeName = resultNode.InnerText.Replace("More ", "");
                        var animePicture = "";

                        var result = new AnimeResult(animeName, animePicture, animeUrl);
                        resultList.Add(result);
                    }
                    if (_orderIsDesc)
                    {
                        resultList.Reverse();
                    }
                    cbxSearchResults.DataSource = resultList;
                    cbxSearchResults.DisplayMember = "AnimeName";
                    cbxSearchResults.ValueMember = "AnimeUrl";
                    return;
                }
                    case "MoeTube":
                {
                    try
                    {
                        var page = 1;
                        var moreresults = true;
                        var docResults = new HtmlDocument();
                        var totalResults = "";
                        while (moreresults)
                        {
                            var searchResultsHtml =
                                WebClient.DownloadString("http://moetube.net/searchapi.php?action=search&page=" + page +
                                                         "&keyword=" + searchString.Replace(" ", "+"));
                            if (searchResultsHtml != "")
                            {
                                totalResults = totalResults + searchResultsHtml;
                                page++;
                            }
                            else
                            {
                                moreresults = false;
                            }
                        }

                        docResults.LoadHtml(totalResults);
                        var searchNodes =
                            docResults.DocumentNode.SelectNodes(
                                "//a");
                        if (searchNodes == null)
                        {
                            MessageBox.Show("No results found for " + searchString);
                            break;
                        }
                        foreach (var resultNode in searchNodes)
                        {
                            var animeUrl = "http://www.moetube.net" + resultNode.Attributes["href"].Value;
                            var animeName = resultNode.SelectSingleNode("div[2]/div[1]").InnerText;
                            var animePicture = resultNode.SelectSingleNode("div[1]/img[1]").Attributes["src"].Value;

                            var result = new AnimeResult(animeName, animePicture, animeUrl);
                            resultList.Add(result);
                        }
                        if (_orderIsDesc)
                        {
                            resultList.Reverse();
                        }
                        cbxSearchResults.DataSource = resultList;
                        cbxSearchResults.DisplayMember = "AnimeName";
                        cbxSearchResults.ValueMember = "AnimeUrl";
                        return;
                    }
                    catch
                    {
                        return;
                    }
                }
                case "MoeTube (Mobile)":
                    {
                        try
                        {
                            var page = 1;
                            var moreresults = true;
                            var docResults = new HtmlDocument();
                            var totalResults = "";
                            while (moreresults)
                            {
                                var searchResultsHtml =
                                    WebClient.DownloadString("http://moetube.net/searchapi.php?action=search&page=" + page +
                                                             "&keyword=" + searchString.Replace(" ", "+"));
                                if (searchResultsHtml != "")
                                {
                                    totalResults = totalResults + searchResultsHtml;
                                    page++;
                                }
                                else
                                {
                                    moreresults = false;
                                }
                            }

                            docResults.LoadHtml(totalResults);
                            var searchNodes =
                                docResults.DocumentNode.SelectNodes(
                                    "//a");
                            if (searchNodes == null)
                            {
                                MessageBox.Show("No results found for " + searchString);
                                break;
                            }
                            foreach (var resultNode in searchNodes)
                            {
                                var animeUrl = "http://www.moetube.net" + resultNode.Attributes["href"].Value;
                                var animeName = resultNode.SelectSingleNode("div[2]/div[1]").InnerText;
                                var animePicture = resultNode.SelectSingleNode("div[1]/img[1]").Attributes["src"].Value;

                                var result = new AnimeResult(animeName, animePicture, animeUrl);
                                resultList.Add(result);
                            }
                            if (_orderIsDesc)
                            {
                                resultList.Reverse();
                            }
                            cbxSearchResults.DataSource = resultList;
                            cbxSearchResults.DisplayMember = "AnimeName";
                            cbxSearchResults.ValueMember = "AnimeUrl";
                            return;
                        }
                        catch
                        {
                            return;
                        }
                    }
                default:
                {
                    //This never happens.
                    break;
                }
            }
        }

        private void GetEpisodes(string selectedSite, string animeUrl)
        {
            var resultList = new List<EpisodeResult>();
            switch (selectedSite)
            {
                case "AnimeDao":
                {
                    var searchResultsHtml = WebClient.DownloadString(animeUrl);
                    var docResults = new HtmlDocument();
                    docResults.LoadHtml(searchResultsHtml);
                    var resultCollection =
                        docResults.DocumentNode.SelectNodes("/html[1]/body[1]/div[1]/div[2]/div[1]/div[1]/a");
                    var resultCollection2 =
                        docResults.DocumentNode.SelectNodes("/html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/a");
                    if (resultCollection == null && resultCollection2 == null)
                    {
                        MessageBox.Show("No episodes found for this anime.");
                        break;
                    }
                    HtmlNodeCollection finalCollection = null;
                    if (resultCollection == null)
                    {
                        finalCollection = resultCollection2;
                    }

                    if (resultCollection2 == null)
                    {
                        finalCollection = resultCollection;
                    }

                    if (resultCollection != null && resultCollection2 != null)
                    {
                        finalCollection =
                            docResults.DocumentNode.SelectNodes(
                                "/html[1]/body[1]/div[1]/div[2]/div[1]/div[1]/a | /html[1]/body[1]/div[1]/div[2]/div[2]/div[1]/a");
                    }

                    foreach (var resultNode in finalCollection)
                    {
                        var episodeUrl = "http://animedao.com" + resultNode.Attributes["href"].Value;
                        var episodeName = resultNode.SelectSingleNode("p[1]/b[1]").InnerText;
                        var animeDescription = HttpUtility.HtmlDecode(resultNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[1]/div[2]/div[1]/div[2]/text()[13]").InnerText);

                        var result = new EpisodeResult(episodeName, animeDescription, episodeUrl);
                        resultList.Add(result);
                    }
                    if (_orderIsDesc)
                    {
                        resultList.Reverse();
                    }
                    cbxEpisodes.DataSource = resultList;
                    cbxEpisodes.DisplayMember = "EpisodeName";
                    cbxEpisodes.ValueMember = "EpisodeUrl";
                    return;
                }
                case "AnimeJolt":
                    {
                        try
                        {
                            var searchResultsHtml = WebClient.DownloadString(animeUrl);
                            var docResults = new HtmlDocument();
                            docResults.LoadHtml(searchResultsHtml);
                            var resultCollection =
                                docResults.DocumentNode.SelectNodes(
                                    "//a[contains(@class, 'anm_det_pop')]");

                            foreach (var resultNode in resultCollection)
                            {
                                var episodeUrl = "http://animejolt.tv" +
                                                 resultNode.Attributes["href"].Value;
                                var episodeName = resultNode.SelectSingleNode("strong").InnerText;
                                var animeDescription = docResults.DocumentNode.SelectSingleNode("//p[contains(@class, 'ptext')]").InnerText;

                                animeDescription = Regex.Replace(animeDescription, @"\s+", " ").Replace(" Description ", "");
                                var result = new EpisodeResult(episodeName, animeDescription, episodeUrl);
                                resultList.Add(result);
                            }
                            if (!_orderIsDesc)
                            {
                                resultList.Reverse();
                            }
                            cbxEpisodes.DataSource = resultList;
                            cbxEpisodes.DisplayMember = "EpisodeName";
                            cbxEpisodes.ValueMember = "EpisodeUrl";
                            return;
                        }
                        catch
                        {
                            return;
                        }
                    }
                case "GoGoAnime":
                {
                    var searchResultsHtml = WebClient.DownloadString(animeUrl);
                    var docResults = new HtmlDocument();
                    docResults.LoadHtml(searchResultsHtml);


                    var animeId =
                        docResults.DocumentNode.SelectSingleNode("//input[@id='movie_id']").Attributes["value"].Value;
                    var episodeListHtml =
                        WebClient.DownloadString("http://gogoanime.tv/site/loadEpisode?ep_start=1&ep_end=9999&id=" +
                                                 animeId);

                    var listResults = new HtmlDocument();
                    listResults.LoadHtml(episodeListHtml);

                    var resultCollection = listResults.DocumentNode.SelectNodes("//li");


                    foreach (var resultNode in resultCollection)
                    {
                        var episodeUrl = resultNode.SelectSingleNode("a[1]").Attributes["href"].Value.Replace(" ", "");
                        var episodeName = resultNode.SelectSingleNode("a[1]/div[@class='name']").InnerText;
                        var animeDescription = HttpUtility.HtmlDecode(docResults.DocumentNode.SelectSingleNode("//div[@class='anime_info_body_bg']/p[2]/text()[1]").InnerText);
                        var result = new EpisodeResult(episodeName, animeDescription, episodeUrl);
                        resultList.Add(result);
                    }
                    if (!_orderIsDesc)
                    {
                        resultList.Reverse();
                    }
                    cbxEpisodes.DataSource = resultList;
                    cbxEpisodes.DisplayMember = "EpisodeName";
                    cbxEpisodes.ValueMember = "EpisodeUrl";
                    return;
                }
                case "AnimesTV":
                {
                    var searchResultsHtml = WebClient.DownloadString(animeUrl);
                    var docResults = new HtmlDocument();
                    docResults.LoadHtml(searchResultsHtml);
                    var resultCollection =
                        docResults.DocumentNode.SelectNodes(
                            "//div[contains(@class, 'row row-9dr')]/a[1]");

                    foreach (var resultNode in resultCollection)
                    {
                        var episodeUrl = "http://www.animestv.us" + resultNode.Attributes["href"].Value;
                        var episodeName = resultNode.SelectSingleNode("div[1]/text()[1]").InnerText;
                        episodeName = Regex.Replace(episodeName, @"\s+", " ");
                        var animeDescription =
                            docResults.DocumentNode.SelectSingleNode(
                                "//div[contains(@class, 'desc-detail-film')]/p[1]")
                                .InnerText;
                        animeDescription = HttpUtility.HtmlDecode(Regex.Replace(animeDescription, @"\s+", " ").Replace(" Description ", ""));
                        var result = new EpisodeResult(episodeName, animeDescription, episodeUrl);
                        resultList.Add(result);
                    }
                    if (!_orderIsDesc)
                    {
                        resultList.Reverse();
                    }
                    cbxEpisodes.DataSource = resultList;
                    cbxEpisodes.DisplayMember = "EpisodeName";
                    cbxEpisodes.ValueMember = "EpisodeUrl";
                    return;
                }
                case "WatchAnimeOnline":
                {
                    var searchResultsHtml = WebClient.DownloadString(animeUrl);
                    var docResults = new HtmlDocument();
                    docResults.LoadHtml(searchResultsHtml);
                    var resultCollection =
                        docResults.DocumentNode.SelectNodes(
                            "//a[contains(@class, 'change_episode')]");

                    foreach (var resultNode in resultCollection)
                    {
                        var episodeUrl = "http://watchanimeonline.io" + resultNode.Attributes["href"].Value;
                        var episodeName = resultNode.InnerText;
                        var animeDescription = "";
                        try
                        {
                            animeDescription = HttpUtility.HtmlDecode(docResults.DocumentNode.SelectSingleNode("//div[contains(@class, 'description')]//div[2]/p[1]").InnerText);
                        }
                        catch (Exception)
                        {
                            
                        }                    
                        
                        animeDescription = Regex.Replace(animeDescription, @"\s+", " ").Replace(" Description ", "");
                        var result = new EpisodeResult(episodeName, animeDescription, episodeUrl);
                        resultList.Add(result);
                    }
                    if (_orderIsDesc)
                    {
                        resultList.Reverse();
                    }
                    cbxEpisodes.DataSource = resultList;
                    cbxEpisodes.DisplayMember = "EpisodeName";
                    cbxEpisodes.ValueMember = "EpisodeUrl";
                    return;
                }
                case "MoeTube":
                {
                    try
                    {
                        WebClient.Headers["User-Agent"] =
                            "Mozilla/5.0 (Linux; U; Android 4.0.3; ko-kr; LG-L160L Build/IML74K) AppleWebkit/534.30 (KHTML, like Gecko) Version/4.0 Mobile Safari/534.30";
                        var searchResultsHtml = WebClient.DownloadString(animeUrl);
                        WebClient.Headers["User-Agent"] = "NativeHost";
                        var docResults = new HtmlDocument();
                        docResults.LoadHtml(searchResultsHtml);
                        var resultCollection =
                            docResults.DocumentNode.SelectNodes(
                                "//li[contains(@class, 'collection-item')]");

                        foreach (var resultNode in resultCollection)
                        {
                            var episodeUrl = "http://moetube.net" +
                                             resultNode.SelectSingleNode("a[1]").Attributes["href"].Value;
                            var episodeName = resultNode.SelectSingleNode("a[1]/p[1]/span[2]").InnerText;
                            var animeDescription = "";

                            animeDescription = Regex.Replace(animeDescription, @"\s+", " ").Replace(" Description ", "");
                            var result = new EpisodeResult(episodeName, animeDescription, episodeUrl);
                            resultList.Add(result);
                        }
                        if (_orderIsDesc)
                        {
                            resultList.Reverse();
                        }
                        cbxEpisodes.DataSource = resultList;
                        cbxEpisodes.DisplayMember = "EpisodeName";
                        cbxEpisodes.ValueMember = "EpisodeUrl";
                        return;
                    }
                    catch
                    {
                        return;
                    }
                }
                case "MoeTube (Mobile)":
                {
                    try
                    {
                        WebClient.Headers["User-Agent"] =
                            "Mozilla/5.0 (Linux; U; Android 4.0.3; ko-kr; LG-L160L Build/IML74K) AppleWebkit/534.30 (KHTML, like Gecko) Version/4.0 Mobile Safari/534.30";
                        var searchResultsHtml = WebClient.DownloadString(animeUrl);
                        WebClient.Headers["User-Agent"] = "NativeHost";
                        var docResults = new HtmlDocument();
                        docResults.LoadHtml(searchResultsHtml);
                        var resultCollection =
                            docResults.DocumentNode.SelectNodes(
                                "//li[contains(@class, 'collection-item')]");

                        foreach (var resultNode in resultCollection)
                        {
                            var episodeUrl = "http://moetube.net" +
                                             resultNode.SelectSingleNode("a[1]").Attributes["href"].Value;
                            var episodeName = resultNode.SelectSingleNode("a[1]/p[1]/span[2]").InnerText;
                            var animeDescription = "";

                            animeDescription = Regex.Replace(animeDescription, @"\s+", " ").Replace(" Description ", "");
                            var result = new EpisodeResult(episodeName, animeDescription, episodeUrl);
                            resultList.Add(result);
                        }
                        if (_orderIsDesc)
                        {
                            resultList.Reverse();
                        }
                        cbxEpisodes.DataSource = resultList;
                        cbxEpisodes.DisplayMember = "EpisodeName";
                        cbxEpisodes.ValueMember = "EpisodeUrl";
                        return;
                    }
                    catch
                    {
                        return;
                    }
                }
                default:
                {
                    //This never happens.
                    break;
                }
            }
        }

        private void GetEpisodeSource(string selectedSite, string animeUrl)
        {
            switch (selectedSite)
            {
                case "AnimeDao":
                {
                    var searchResultsHtml = WebClient.DownloadString(animeUrl);
                    var docResults = new HtmlDocument();
                    docResults.LoadHtml(searchResultsHtml);

                    
                    string scriptString = docResults.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/script[1]/text()[1]").InnerText;
                    Regex regex1080 = new Regex("s1080p\\s=\\s\"(.*?)\"");
                    Regex regex720 = new Regex("s720p\\s=\\s\"(.*?)\"");
                    Regex regex360 = new Regex("s360p\\s=\\s\"(.*?)\"");

                    MatchCollection match1080 = regex1080.Matches(scriptString);
                    MatchCollection match720 = regex720.Matches(scriptString);
                    MatchCollection match360 = regex360.Matches(scriptString);

                    string redirectUrl1080 = "";
                    string redirectUrl720 = "";
                    string redirectUrl360 = "";

                    redirectUrl1080 = ROT13(match1080.Cast<Match>().Aggregate(redirectUrl1080, (current, m) => current + m.Groups[1].Value));
                    redirectUrl720 = ROT13(match720.Cast<Match>().Aggregate(redirectUrl720, (current, m) => current + m.Groups[1].Value));
                    redirectUrl360 = ROT13(match360.Cast<Match>().Aggregate(redirectUrl360, (current, m) => current + m.Groups[1].Value));

                    if (redirectUrl1080 != "" && redirectUrl1080 != "/redirect/")
                    {
                        if (!redirectUrl1080.Contains("http"))
                        {
                            redirectUrl1080 = "http://animedao.com/redirect/" + redirectUrl1080;
                        }
                        CurrentEpisodeSource = redirectUrl1080;
                    }
                    else if (redirectUrl720 != "" && redirectUrl720 != "/redirect/")
                    {
                            if (!redirectUrl720.Contains("http"))
                            {
                                redirectUrl720 = "http://animedao.com/redirect/" + redirectUrl720;
                            }
                            CurrentEpisodeSource = redirectUrl720;
                    }
                    else if (redirectUrl360 != "" && redirectUrl360 != "/redirect/")
                    {
                            if (!redirectUrl360.Contains("http"))
                            {
                                redirectUrl360 = "http://animedao.com/redirect/" + redirectUrl360;
                            }
                            CurrentEpisodeSource = redirectUrl360;
                    }
                    CurrentEpisodeSource = CurrentEpisodeSource.Replace(";", "&");
                    


                    //CurrentEpisodeSource = "http://animedao.com" + docResults.DocumentNode.SelectSingleNode("//video[@preload='auto']").Attributes["src"].Value;
                    return;
                }
                case "AnimeJolt":
                    {
                        var searchResultsHtml = WebClient.DownloadString(animeUrl);
                        var docResults = new HtmlDocument();
                        docResults.LoadHtml(searchResultsHtml);
                        try
                        {
                            CurrentEpisodeSource =
                                SourceFromAnimeJolt(
                                    "http://animejolt.tv" + docResults.DocumentNode.SelectSingleNode(
                                        "//iframe[contains(@class, 'embed-responsive-item')]").Attributes["src"].Value);
                        }
                        catch
                        {
                            // ignored
                        }
                        return;
                    }
                case "GoGoAnime":
                {
                    var searchResultsHtml = WebClient.DownloadString(animeUrl);
                    var docResults = new HtmlDocument();
                    docResults.LoadHtml(searchResultsHtml);
                    try
                    {
                        CurrentEpisodeSource =
                            docResults.DocumentNode.SelectSingleNode(
                                "//option[last()]")
                                .Attributes["value"].Value;
                    }
                    catch
                    {
                        // ignored
                    }
                    return;
                }
                case "AnimesTV":
                {
                    var searchResultsHtml = WebClient.DownloadString(animeUrl);
                    var docResults = new HtmlDocument();
                    docResults.LoadHtml(searchResultsHtml);

                    var comingSoon = docResults.DocumentNode.SelectSingleNode("//img[@src='http://animestv.us/images/coming.jpg']");
                    if (comingSoon != null)
                    {
                        CurrentEpisodeSource = "Coming Soon";
                        return;
                    }

                    
                    //Check for JWPlayer
                    var scriptNode = docResults.DocumentNode.SelectSingleNode("//*[text()[contains(.,'.setup(')]]");
                    if (scriptNode != null)
                    {
                        var scriptResult = scriptNode.InnerText;

                        var re1 = @"{\s*file:\s*'(?<File>.*?)',\s*label:\s*'(?<Label>.*?)',\s*type:\s*'(?<Type>.*?)'\s*}"; // Non-greedy match on filler

                        var r = new Regex(re1, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.RightToLeft);
                        var matches = r.Matches(scriptResult);
                        
                        CurrentEpisodeSource = matches[0].Groups["File"].Value;
                        return;
                    }
                    

                    //Check for AnimeUploader
                    var animeUploadNode = docResults.DocumentNode.SelectSingleNode("//iframe[@allowfullscreen='true']");
                    if (animeUploadNode != null)
                    {
                        CurrentEpisodeSource = SourceFromAnimeUploader(animeUploadNode.Attributes["src"].Value);
                        return;
                    }

                    //Check for MP4Upload
                    var mp4UploadNode = docResults.DocumentNode.SelectSingleNode("//*[text()[contains(.,'mp4upload')]]");
                    if (mp4UploadNode != null)
                    {
                        
                        var re1 = "src='(?<File>.*?)'\\s"; // Non-greedy match on filler

                        var r = new Regex(re1, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.RightToLeft);
                        var matches = r.Matches(mp4UploadNode.InnerText);

                        var file = matches[0].Groups["File"].Value;
                        CurrentEpisodeSource = SourceFromMP4Upload(file);
                        return;
                    }
                    
                    CurrentEpisodeSource = "Couldn't get source";

                    return;
                }
                    case "WatchAnimeOnline":
                {
                    var searchResultsHtml = WebClient.DownloadString(animeUrl);
                    var docResults = new HtmlDocument();
                    docResults.LoadHtml(searchResultsHtml);

                    
                    try
                    {
                        //Check for AnimeUploader
                        var animeUploadNode = docResults.DocumentNode.SelectSingleNode("//iframe[@allowfullscreen='true']");
                        if (animeUploadNode != null)
                        {
                            CurrentEpisodeSource = SourceFromAnimeUploader(animeUploadNode.Attributes["src"].Value);
                            return;
                        }

                        //Check for MP4Upload
                        var mp4UploadNode = docResults.DocumentNode.SelectSingleNode("//*[text()[contains(.,'mp4upload')]]");
                        if (mp4UploadNode != null)
                        {

                            var re1 = "src='(?<File>.*?)'\\s"; // Non-greedy match on filler

                            var r = new Regex(re1, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.RightToLeft);
                            var matches = r.Matches(mp4UploadNode.InnerText);

                            var file = matches[0].Groups["File"].Value;
                            CurrentEpisodeSource = SourceFromMP4Upload(file);
                            return;
                        }
                        var standardNode = docResults.DocumentNode.SelectSingleNode("//source").Attributes["src"].Value;
                        if (standardNode != null)
                        {
                            CurrentEpisodeSource = docResults.DocumentNode.SelectSingleNode("//source").Attributes["src"].Value;
                            return;
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                    return;
                }
                    case "MoeTube":
                {
                    var searchResultsHtml = WebClient.DownloadString(animeUrl);
                    var docResults = new HtmlDocument();
                    docResults.LoadHtml(searchResultsHtml);

                    try
                    {
                        //Check for AnimeUploader
                        var animeUploadNode = docResults.DocumentNode.SelectSingleNode("//iframe[@allowfullscreen='true']");
                        if (animeUploadNode != null)
                        {
                            CurrentEpisodeSource = SourceFromAnimeUploader(animeUploadNode.Attributes["src"].Value);
                            return;
                        }

                        //Check for MP4Upload
                        var mp4UploadNode = docResults.DocumentNode.SelectSingleNode("//*[text()[contains(.,'mp4upload')]]");
                        if (mp4UploadNode != null)
                        {

                            var re1 = "src='(?<File>.*?)'\\s"; // Non-greedy match on filler

                            var r = new Regex(re1, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.RightToLeft);
                            var matches = r.Matches(mp4UploadNode.InnerText);

                            var file = matches[0].Groups["File"].Value;
                            CurrentEpisodeSource = SourceFromMP4Upload(file);
                            return;
                        }
                        var standardNode = docResults.DocumentNode.SelectSingleNode("//source").Attributes["src"].Value;
                        if (standardNode != null)
                        {
                            CurrentEpisodeSource = docResults.DocumentNode.SelectSingleNode("//source").Attributes["src"].Value;
                            return;
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                    return;
                }
                        case "MoeTube (Mobile)":
                {
                    WebClient.Headers["User-Agent"] = "Mozilla/5.0 (Linux; U; Android 4.0.3; ko-kr; LG-L160L Build/IML74K) AppleWebkit/534.30 (KHTML, like Gecko) Version/4.0 Mobile Safari/534.30";
                    var searchResultsHtml = WebClient.DownloadString(animeUrl);
                    WebClient.Headers["User-Agent"] = "NativeHost";
                    var docResults = new HtmlDocument();
                    docResults.LoadHtml(searchResultsHtml);

                    try
                    {
                        //Check for AnimeUploader
                        var animeUploadNode = docResults.DocumentNode.SelectSingleNode("//iframe[@allowfullscreen='true']");
                        if (animeUploadNode != null)
                        {
                            CurrentEpisodeSource = SourceFromAnimeUploader(animeUploadNode.Attributes["src"].Value);
                            return;
                        }

                        //Check for MP4Upload
                        var mp4UploadNode = docResults.DocumentNode.SelectSingleNode("//*[text()[contains(.,'mp4upload')]]");
                        if (mp4UploadNode != null)
                        {

                            var re1 = "src='(?<File>.*?)'\\s"; // Non-greedy match on filler

                            var r = new Regex(re1, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.RightToLeft);
                            var matches = r.Matches(mp4UploadNode.InnerText);

                            var file = matches[0].Groups["File"].Value;
                            CurrentEpisodeSource = SourceFromMP4Upload(file);
                            return;
                        }
                        var standardNode = docResults.DocumentNode.SelectSingleNode("//source").Attributes["src"].Value;
                        if (standardNode != null)
                        {
                            CurrentEpisodeSource = docResults.DocumentNode.SelectSingleNode("//source").Attributes["src"].Value;
                            return;
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                    return;
                }
                default:
                {
                    //This never happens.
                    break;
                }
            }
        }

        private void btnCopySource_Click(object sender, EventArgs e)
        {
            System.Media.SystemSounds.Exclamation.Play();
            addRecentlyWatched();
            Clipboard.SetText(CurrentEpisodeSource);
        }

        private void btnPrevEpisode_Click(object sender, EventArgs e)
        {
            if (cbxEpisodes.SelectedIndex > 0)
            {
                cbxEpisodes.SelectedIndex--;
            }
        }

        private void btnNextEpisode_Click(object sender, EventArgs e)
        {
            if (cbxEpisodes.SelectedIndex < (cbxEpisodes.Items.Count - 1))
            {
                cbxEpisodes.SelectedIndex++;
            }
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) Keys.Return)
            {
                ShowLoading();
                SearchAnime(cbxSite.SelectedItem.ToString(), txtSearch.Text);
                HideLoading();
            }
        }

        private void cbxSearchResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowLoading();
            var animeResult = cbxSearchResults.SelectedItem as AnimeResult;
            if (animeResult != null)
            {
                string altDescription = "";
                string altPicture = "https://upload.wikimedia.org/wikipedia/en/d/d3/No-picture.jpg";

                if (animeResult.AnimePicture != "")
                {
                    altPicture = animeResult.AnimePicture;
                }

                GetEpisodes(cbxSite.SelectedItem.ToString(), animeResult.AnimeUrl);
                var episodeResult = cbxEpisodes.SelectedItem as EpisodeResult;
                if (episodeResult != null)
                {
                    altDescription = HttpUtility.HtmlDecode(episodeResult.AnimeDescription);
                }
                
                GetAnimeInfo(animeResult.AnimeName, altDescription, altPicture);
            }
            HideLoading();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var animeResult = cbxSearchResults.SelectedItem as AnimeResult;
            var episodeResult = cbxEpisodes.SelectedItem as EpisodeResult;

            if (episodeResult != null)
            {
                var episodeNumber = Regex.Match(episodeResult.EpisodeName, @"\d+(?!\D*\d)").Value;
                if (animeResult != null)
                {
                    string recentlyWatched = animeResult.AnimeName + " " + episodeNumber + " (" + cbxSite.SelectedItem.ToString() + ")";
                }
            }

            DownloadFile(CurrentEpisodeSource, @"C:\Temp\test.mp4");


            /*
            PushLinkRequest pushLinkRequest = new PushLinkRequest();
            var animeResult = cbxSearchResults.SelectedItem as AnimeResult;
            var episodeResult = cbxEpisodes.SelectedItem as EpisodeResult;

            if (episodeResult != null)
            {
                var episodeNumber = Regex.Match(episodeResult.EpisodeName, @"\d+(?!\D*\d)").Value;
                if (animeResult != null)
                {
                    string recentlyWatched = animeResult.AnimeName + " " + episodeNumber + " (" + cbxSite.SelectedItem.ToString() + ")";

                    pushLinkRequest.Title = "Anime Link Grabber: " + recentlyWatched;
                }
            }
            pushLinkRequest.Url = CurrentEpisodeSource;
            PClient.PushLink(pushLinkRequest);
            */
        }

        public void DownloadFile(string urlAddress, string location)
        {
            using (DownloadClient = new MyWebClient())
            {
                DownloadClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                DownloadClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);

                // The variable that will be holding the url address (making sure it starts with http://)
                Uri URL = urlAddress.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ? new Uri(urlAddress) : new Uri("http://" + urlAddress);

                // Start the stopwatch which we will be using to calculate the download speed
                DownloadStopwatch.Start();

                try
                {
                    // Start downloading the file
                    DownloadClient.DownloadFileAsync(URL, location);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            
            // Update the progressbar percentage only when the value is not the same.
            //progressBar.Value = e.ProgressPercentage;

            /*
            // Calculate download speed and output it to labelSpeed.
            labelSpeed.Text = string.Format("{0} kb/s", (e.BytesReceived / 1024d / sw.Elapsed.TotalSeconds).ToString("0.00"));

            // Show the percentage on our label.
            labelPerc.Text = e.ProgressPercentage.ToString() + "%";

            // Update the label with how much data have been downloaded so far and the total size of the file we are currently downloading
            labelDownloaded.Text = string.Format("{0} MB's / {1} MB's", (e.BytesReceived / 1024d / 1024d).ToString("0.00"), (e.TotalBytesToReceive / 1024d / 1024d).ToString("0.00"));
            */

        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            // Reset the stopwatch.
            DownloadStopwatch.Reset();

            if (e.Cancelled == true)
            {
                MessageBox.Show("Download has been canceled.");
            }
            else
            {
                MessageBox.Show("Download completed!");
            }
        }

        private void GetAnimeInfo(string animeName, string altDescription, string altPicture)
        {
            SearchMethods searchMethods = new SearchMethods(_malCredential);
            XmlDocument searchResult = new XmlDocument();
            try
            {
                string animeResult = searchMethods.SearchAnime(animeName);
                searchResult.LoadXml(animeResult);

                XmlNode synopsisNode = searchResult.SelectSingleNode("//synopsis[1][last()]");
                if (synopsisNode != null)
                    txtDescription.Text = HttpUtility.HtmlDecode(Regex.Replace(synopsisNode.InnerText, @"<[^>]+>|&nbsp;", "").Trim());
                XmlNode imageNode = searchResult.SelectSingleNode("//image[1][last()]");
                if (imageNode != null)
                    pcbAnime.Load(imageNode.InnerText);
                XmlNode idNode = searchResult.SelectSingleNode("//id[1][last()]");
                if (idNode != null)
                {
                    CurrentAnimeMalUrl = "http://myanimelist.net/anime/" + idNode.InnerText;
                    btnMal.Visible = true;
                } 
            }
            catch
            {
                btnMal.Visible = false;
                txtDescription.Text = altDescription;
                pcbAnime.Load(altPicture);
            }
        }

        private void cbxEpisodes_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowLoading();
            var episodeResult = cbxEpisodes.SelectedItem as EpisodeResult;
            if (episodeResult != null) GetEpisodeSource(cbxSite.SelectedItem.ToString(), episodeResult.EpisodeUrl);
            txtEpisodeLink.Text = CurrentEpisodeSource;
            HideLoading();
        }

        private void SuperFocus()
        {
            WindowState = FormWindowState.Minimized;
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void cbxSite_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveRecentlyWatched();
        }

        private void checkOrder_CheckedChanged(object sender, EventArgs e)
        {
            _orderIsDesc = checkOrder.Checked;
            if (txtSearch.Text != "")
            {
                ShowLoading();
                SearchAnime(cbxSite.SelectedItem.ToString(), txtSearch.Text);
                HideLoading();
            }
        }

        private void btnMal_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(CurrentAnimeMalUrl);
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            addRecentlyWatched();
            var message = "Play with MPC-HC or VLC, press Yes for MPC-HC and No for VLC";
            var title = "What players?";
            var result = MessageBox.Show(
                message,                  // the message to show
                title,                    // the title for the dialog box
                MessageBoxButtons.YesNo,  // show two buttons: Yes and No
                MessageBoxIcon.Question); // show a question mark icon

            // the following can be handled as if/else statements as well
            switch (result)
            {
                case DialogResult.Yes:   // Yes button pressed
                    string defaultMpcPath = @"C:\Program Files (x86)\MPC-HC\mpc-hc.exe";
                    string defaultMpcPath64 = @"C:\Program Files\MPC-HC\mpc-hc.exe";

                    if (File.Exists(defaultMpcPath))
                    {
                        System.Diagnostics.Process.Start(defaultMpcPath, "- \"" + CurrentEpisodeSource + "\"");
                    }
                    else if (File.Exists(defaultMpcPath64))
                    {
                        System.Diagnostics.Process.Start(defaultMpcPath64, "- \"" + CurrentEpisodeSource + "\"");
                    }
                    break;
                case DialogResult.No:    // No button pressed
                    string regVideoLan = @"SOFTWARE\VideoLAN\VLC";
                    string regVideoLan64 = @"SOFTWARE\Wow6432Node\VideoLAN\VLC";

                    RegistryKey regKey = Registry.LocalMachine.OpenSubKey(regVideoLan);
                    RegistryKey regKey64 = Registry.LocalMachine.OpenSubKey(regVideoLan64);

                    if (regKey != null)
                    {
                        System.Diagnostics.Process.Start(regKey.GetValue(null).ToString(), "-vvv " + CurrentEpisodeSource);
                    }
                    else if (regKey64 != null)
                    {
                        System.Diagnostics.Process.Start(regKey64.GetValue(null).ToString(), "-vvv " + CurrentEpisodeSource);
                    }
                    break;
                default:                 // Neither Yes nor No pressed (just in case)
                    MessageBox.Show("What did you press?");
                    break;
            }        

        }

        private void btnOpenSite_Click(object sender, EventArgs e)
        {
            addRecentlyWatched();
            var episodeResult = cbxEpisodes.SelectedItem as EpisodeResult;
            if (episodeResult != null) System.Diagnostics.Process.Start(episodeResult.EpisodeUrl);
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            ShowLoading();
            /*
            var animeResult = cbxSearchResults.SelectedItem as AnimeResult;
            var episodeResult = cbxEpisodes.SelectedItem as EpisodeResult;
            if (animeResult != null && episodeResult != null)
            {
                string fileName = animeResult.AnimeName + " - " + episodeResult.EpisodeName + ".mp4";
                using (SaveFileDialog dialog = new SaveFileDialog())
                {
                    dialog.FileName = fileName;
                    dialog.Filter = "MP4 Files (*.mp4)|*.mp4|All Files (*.*)|*.*";
                    dialog.FilterIndex = 2;
                    dialog.RestoreDirectory = true;

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        WebClient.DownloadFile(new Uri(CurrentEpisodeSource), dialog.FileName);
                    }
                }
                
            }
            */

            StringBuilder linkBuilder = new StringBuilder();

            foreach (EpisodeResult result in cbxEpisodes.Items)
            {
                if (result != null)
                {
                    GetEpisodeSource(cbxSite.SelectedItem.ToString(), result.EpisodeUrl);
                    linkBuilder.Append(CurrentEpisodeSource);
                    linkBuilder.Append(Environment.NewLine);
                }
            }

            System.Media.SystemSounds.Exclamation.Play();
            Clipboard.SetText(linkBuilder.ToString());
            HideLoading();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void yudofuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.yudofu.com/");
        }

        private void supportMeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://streamtip.com/t/zeffuro");
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AboutBox1 box = new AboutBox1();
            box.ShowDialog();
        }

        private void lstRecentlyWatched_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            /*
            int index = this.lstRecentlyWatched.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                string episodeInfo = lstRecentlyWatched.Items[index].ToString();

                Regex siteRegex = new Regex(@"\((.*?)\)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                Match siteMatch = siteRegex.Match(episodeInfo);
                if (siteMatch.Success)
                {
                    String site = siteMatch.Groups[1].ToString();
                    cbxSite.SelectedIndex = cbxSite.FindString(site);

                    Regex animeRegex = new Regex(@"(.*?)\(.*?\)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    Match animeMatch = animeRegex.Match(episodeInfo);
                    if (animeMatch.Success)
                    {
                        string animeInfo = Regex.Match(animeMatch.Groups[1].ToString(), @"\d+").Value;
                        string animeName = animeInfo.Trim();
                        string animeEpisode = Regex.Match(animeInfo, @"\d+").Value;

                        ShowLoading();
                        txtSearch.Text = animeName;
                        SearchAnime(cbxSite.SelectedItem.ToString(), txtSearch.Text);
                        cbxSearchResults.SelectedIndex = cbxSearchResults.FindStringExact(animeName);
                        cbxEpisodes.SelectedItem = FindItemContaining(cbxEpisodes.Items, animeEpisode);
                        HideLoading();
                    }
                }
            }
            */
        }

        // Select an item containing the target string.
        private object FindItemContaining(IEnumerable items, string target)
        {
            foreach (object item in items)
                if (item.ToString().Contains(target))
                    return item;

            // Return null;
            return null;
        }

        private void btnClearRecentlyWatched_Click(object sender, EventArgs e)
        {
            lstRecentlyWatched.Items.Clear();
        }

        private void txtEpisodeLink_Enter(object sender, EventArgs e)
        {
            txtEpisodeLink.SelectAll();
        }
    }

    internal class AnimeResult
    {
        public AnimeResult(string animeName, string animePicture, string animeUrl)
        {
            AnimeName = animeName;
            AnimePicture = animePicture;
            AnimeUrl = animeUrl;
        }

        public string AnimeName { get; set; }
        public string AnimePicture { get; set; }
        public string AnimeUrl { get; set; }
    }

    internal class EpisodeResult
    {
        public EpisodeResult(string episodeName, string animeDescription, string episodeUrl)
        {
            EpisodeName = episodeName;
            AnimeDescription = animeDescription;
            EpisodeUrl = episodeUrl;
        }

        public string EpisodeName { get; set; }
        public string AnimeDescription { get; set; }
        public string EpisodeUrl { get; set; }
    }

    public class MyWebClient : WebClient
    {
        Uri _responseUri;

        public Uri ResponseUri
        {
            get { return _responseUri; }
        }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            HttpWebRequest webRequest = (HttpWebRequest)base.GetWebRequest(uri);

            webRequest.KeepAlive = false;
            webRequest.ProtocolVersion = HttpVersion.Version10;
            return webRequest;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            ServicePointManager.UseNagleAlgorithm = false;
            WebResponse response = base.GetWebResponse(request);
            if (response != null)
            {
                _responseUri = response.ResponseUri;
                return response;
            }
            return response;
        }
    }

    public static class GuiExtensionMethods
    {
        public static void Enable(this Control con, bool enable)
        {
            if (con != null)
            {
                foreach (Control c in con.Controls)
                {
                    c.Enable(enable);
                }

                try
                {
                    con.Invoke((MethodInvoker) (() => con.Enabled = enable));
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}