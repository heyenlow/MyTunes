using myTunes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
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
        public class PlaylistSong{
            public string Title { get; set; }
            public string Artist { get; set; }
            public string Album { get; set; }
            public string Genre { get; set; }
        }

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
            ObservableCollection<PlaylistSong> playlistSongsCollection = new ObservableCollection<PlaylistSong>();
            DataRow[] results = playlistSongs.Select();
            foreach(DataRow row in results)
            {
                playlistSongsCollection.Add(new PlaylistSong() { Title = row["title"].ToString(), Album = row["album"].ToString(), Artist = row["artist"].ToString(), Genre = row["genre"].ToString() });
            }
            this.SongsBox.ItemsSource = playlistSongsCollection;
        }

        private void SongsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
