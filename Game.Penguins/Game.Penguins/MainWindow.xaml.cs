﻿using AdonisUI;
using Game.Penguins.ViewModels;
using System.Windows;

namespace Game.Penguins
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new ApplicationViewModel();
            InitializeComponent();
			_isDark = true;
        }

        private bool _isDark;

        private void ChangeTheme(object sender, RoutedEventArgs e)
        {
            ResourceLocator.SetColorScheme(Application.Current.Resources, _isDark ? ResourceLocator.LightColorScheme : ResourceLocator.DarkColorScheme);

            _isDark = !_isDark;
        }

        private void OpenIssueDialog(object sender, RoutedEventArgs e)
        {
            //Window issueDialog = new IssueDialog
            //{
            //    DataContext = new IssueDialogViewModel()
            //};

            //issueDialog.ShowDialog();
        }
    }
}
