using Microsoft.Win32;
using myTunes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyTunes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {

        public class Playlist
        {
            public string name { get; set; }
        }


        private DataSet musicDataSet;
        MusicLib MusicLibrary;
        public MainWindow()
        {
            InitializeComponent();


            MusicLibrary = new MusicLib();

            //Pulled from Notes -> https://sites.harding.edu/fmccown/classes/comp4450-f20/notes/notes16.html        
            musicDataSet = new DataSet();
            musicDataSet.ReadXmlSchema("music.xsd");
            musicDataSet.ReadXml("music.xml");

            var task = MusicLibrary.getAPIinfoAsync();
            MusicLibrary.PrintAllTables();

            updatePlaylistBox();
            updateAllMusicPlaylist();

        }


        private void updatePlaylistBox()
        {
            List<Playlist> playlists = new List<Playlist>();
            DataTable table = musicDataSet.Tables["playlist"];
            DataRow[] results = table.Select();
            foreach (DataRow row in results)
            {
                playlists.Add(new Playlist { name = row["name"].ToString() });
                //this.PlaylistsBox.Items.Add(row["name"].ToString());        //https://www.c-sharpcorner.com/UploadFile/mahesh/wpf-combobox/
            }
            PlaylistsBox.ItemsSource = playlists;
        }

        //adds all songs in the song DB to the "All Songs" playlist
        private void updateAllMusicPlaylist()
        {
            DataTable allSongs = musicDataSet.Tables["song"];
            DataRow[] results = allSongs.Select();
            foreach (DataRow row in results)
            {
                MusicLibrary.AddSongToPlaylist(Int32.Parse(row["id"].ToString()), "All Music");
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Playlist selectedPlaylist = PlaylistsBox.SelectedItem as Playlist;
            DataTable playlistSongs = MusicLibrary.SongsForPlaylist(selectedPlaylist.name);
            List<Song> playlistSongsCollection = new List<Song>();
            DataRow[] results = playlistSongs.Select();
            foreach (DataRow row in results)
            {
                playlistSongsCollection.Add(new Song() { Id = Int32.Parse(row["Id"].ToString()), Title = row["title"].ToString(), Album = row["album"].ToString(), Artist = row["artist"].ToString(), Genre = row["genre"].ToString(), Length = "Length" + row["length"].ToString(), AboutUrl = row["url"].ToString(), AlbumImageUrl = row["albumImage"].ToString() });
            }
            this.SongsBox.ItemsSource = playlistSongsCollection;
        }

        private void SongsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //await MusicLibrary.getAPIinfoAsync();
        }

        private void playlistListBox_DragOver(object sender, DragEventArgs e)
        {
            // By default, don't allow dropping
            e.Effects = DragDropEffects.None;
            
            // If the DataObject contains string data, extract it
            if (e.Data.GetDataPresent(DataFormats.StringFormat) && !sender.ToString().Contains("All Music"))
            {
                e.Effects = DragDropEffects.Copy;
            }
        }

        private void Label_Drop(object sender, DragEventArgs e)
        {
            /*  ------------Debug------------
             *  Console.WriteLine("Sender -- " + sender.ToString());
             *  Console.WriteLine("E      -- " + e.ToString());
             *  Console.WriteLine("E Data -- " + e.Data.ToString());
             *  Console.WriteLine("E OBJ  -- " + e.Data.GetDataPresent(DataFormats.StringFormat));
             */
            // If the DataObject contains string data, extract it
            if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                string dataString = (string)e.Data.GetData(DataFormats.StringFormat);
                Label p = sender as Label;
                MusicLibrary.AddSongToPlaylist(int.Parse(dataString), p.Content.ToString());
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)  //https://stackoverflow.com/questions/10238694/example-using-hyperlink-in-wpf
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private Point startPoint;
        private void SongsBox_MouseMove(object sender, MouseEventArgs e) {

            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            // Start the drag-drop if mouse has moved far enough
            if(e.LeftButton == MouseButtonState.Pressed &&
                    (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                     Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))                           
            {
                // Initiate dragging the text from the textbox
                Song one = SongsBox.SelectedItem as Song;
                Console.WriteLine("Selected item is: " + one.Id);
                DragDrop.DoDragDrop(SongsBox, one.Id.ToString(), DragDropEffects.Copy);
            }
        }

        private void SongsBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Store the mouse position
            startPoint = e.GetPosition(null);
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "*.mp3 | *.m4a | *.wma | *.wav";
            if (openFile.ShowDialog() == true)
            {

            }
        }

        private void Playlist_Click(object sender, RoutedEventArgs e)
        {
            AddPlaylist addPlaylist = new AddPlaylist();
            if(addPlaylist.ShowDialog()==true)
            {

            }
            
            //DataRow thing;
            //DataTable table = musicDataSet.Tables["playlist"];
            //DataRow[] results = table.Select();
            //foreach (DataRow Line in results)
            //{
            //    if(Line == thing)
            //    {

            //    }
            //}
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            AboutForm about = new AboutForm();
            about.ShowDialog();
        }
    }
}
