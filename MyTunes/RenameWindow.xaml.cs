﻿using myTunes;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace MyTunes
{
    /// <summary>
    /// Interaction logic for RenameWindow.xaml
    /// </summary>
    public partial class RenameWindow : Window
    {
        MusicLib musicLib = new MusicLib();
        

        public string Temp { get; set; }
        public RenameWindow()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (musicLib.PlaylistExists(named.Text) == false && named.Text != "")
            {
                Temp = named.Text;
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("That Playlist already exists");
                this.DialogResult = false;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
