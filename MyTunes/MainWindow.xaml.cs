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

        public string TempPlay;
        MusicLib MusicLibrary;
        MediaPlayer mediaPlayer;
        Playlist current;
        public MainWindow()
        {
            InitializeComponent();

            mediaPlayer = new MediaPlayer();
            MusicLibrary = new MusicLib();

            //Pulled from Notes -> https://sites.harding.edu/fmccown/classes/comp4450-f20/notes/notes16.html        

            var task = MusicLibrary.getAPIinfoAsync();
            MusicLibrary.PrintAllTables();

            updatePlaylistBox();
            updateAllMusicPlaylist();
            PlaylistsBox.SelectedIndex = 0;
            Play.IsEnabled = false;
            Stop.IsEnabled = false;

        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            MusicLibrary.Save();
        }


        private void updatePlaylistBox()
        {
            DataSet musicDataSet = MusicLibrary.getDataSet();
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
            DataSet musicDataSet = MusicLibrary.getDataSet();
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
            if (selectedPlaylist != null)
            {
                DataTable playlistSongs = MusicLibrary.SongsForPlaylist(selectedPlaylist.name);
                List<Song> playlistSongsCollection = new List<Song>();
                DataRow[] results = playlistSongs.Select();
                foreach (DataRow row in results)
                {
                    playlistSongsCollection.Add(new Song() { Id = Int32.Parse(row["Id"].ToString()), Title = row["title"].ToString(), Album = row["album"].ToString(), Artist = row["artist"].ToString(), Genre = row["genre"].ToString(), Length = "Length" + row["length"].ToString(), AboutUrl = row["url"].ToString(), AlbumImageUrl = row["albumImage"].ToString(), Filename = row["filename"].ToString() });
                }
                this.SongsBox.ItemsSource = playlistSongsCollection;
                if (selectedPlaylist.name != "All Music")
                {
                    contextremove.Header = "Remove from playlist";
                    current = selectedPlaylist;
                }
                else
                {
                    contextremove.Header = "Remove";
                }
            }
        }

        private void SongsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Song temp = SongsBox.SelectedItem as Song;
            if (temp != null)
            {
                Console.WriteLine("Selected Song Filename: " + temp.Filename);
                mediaPlayer.Open(new Uri(temp.Filename));
                Play.IsEnabled = true;
            }
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
                if (one != null)
                {
                    Console.WriteLine("Selected item is: " + one.Id);
                    DragDrop.DoDragDrop(SongsBox, one.Id.ToString(), DragDropEffects.Copy);
                }
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
            openFile.Filter = "Media files | *.mp3; *.m4a; *.wma; *.wav | MP3(*.mp3) | *.mp3 | M4A(*.m4a) | *.m4a | Windows Media Audio(*.wma)| *.wma | Wave files(*.wav) | *.wav";
            if (openFile.ShowDialog() == true)
            {
                MusicLibrary.AddSong(openFile.FileName);
                MusicLibrary.Save();
            }
        }

        private void Playlist_Click(object sender, RoutedEventArgs e)
        {
            AddPlaylist addPlaylist = new AddPlaylist();
            if(addPlaylist.ShowDialog()==true)
            {
                MusicLibrary.AddPlaylist(addPlaylist.Temp);
                MusicLibrary.Save();
                PlaylistsBox.InvalidateArrange();
                PlaylistsBox.UpdateLayout();
                updatePlaylistBox();
                updateAllMusicPlaylist();
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            AboutForm about = new AboutForm();
            about.ShowDialog();
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Play();
            Stop.IsEnabled = true;
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            Stop.IsEnabled = false;
        }

        private void contextPlay_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Play();
            Stop.IsEnabled = true;
        }

        private void contextremove_Click(object sender, RoutedEventArgs e)
        {
            Song temp = SongsBox.SelectedItem as Song;
            if (contextremove.Header == "Remove")
            {
                RemoveForm remove = new RemoveForm();
                if(remove.ShowDialog()==true)
                {
                    MusicLibrary.DeleteSong(temp.Id);
                }
            }
            else
            {
                 MusicLibrary.RemoveSongFromPlaylist(SongsBox.SelectedIndex, temp.Id, current.name);
                //  CollectionViewSource.GetDefaultView(SongsBox.ItemsSource).Refresh();
            }
                SongsBox.InvalidateArrange();
                SongsBox.UpdateLayout();
            updatePlaylistBox();
            updateAllMusicPlaylist();
        }

        private void Playremove_Click(object sender, RoutedEventArgs e)
        {
            MusicLibrary.DeletePlaylist(current.name);
            PlaylistsBox.InvalidateArrange();
            PlaylistsBox.UpdateLayout();
            updatePlaylistBox();
            updateAllMusicPlaylist();
        }

        private void PlaylistPlay_Click(object sender, RoutedEventArgs e)
        {
            RenameWindow rename = new RenameWindow();
            if(rename.ShowDialog()==true)
            {
                if (MusicLibrary.RenamePlaylist(current.name, rename.Temp))
                {
                    PlaylistsBox.InvalidateArrange();
                    PlaylistsBox.UpdateLayout();
                    updatePlaylistBox();
                    updateAllMusicPlaylist();
                }
            }
        }
    }
}
