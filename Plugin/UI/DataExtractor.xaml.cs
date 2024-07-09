using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;



namespace Plugin.UI
{

    public partial class DataExtractor : System.Windows.Window
    {

        public string SelectedProjectPath { get; set; }

        public string SelectedViewPath { get; set; }

        public bool IsReadProjectEnabled { get; set; } = false;

        public bool IsCloseButtonClicked { get; set; } = false;

        Plugin Plugin;
        public DataExtractor()
        {
            InitializeComponent();
        }

        private void ProjectRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if(SelectedProjectPath != null)
            {
                viewTextPath.IsEnabled = false;
                viewBrowseButton.IsEnabled = false;
                viewTextPath.Text = string.Empty;
                projectTextPath.Text = SelectedProjectPath;
                apjTextPath.IsEnabled = true;
                projectTextPath.IsEnabled = true;
                projectBrowseButton.IsEnabled = false;
                IsReadProjectEnabled = true;
                SelectedViewPath = null;
            }
            else
            {
                projectRadioButton.IsChecked = false;
                MessageBox.Show("Please choose a project APJ file first.", "Data Extractor", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        private void ViewsRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (SelectedProjectPath != null)
            {
                viewTextPath.IsEnabled = true;
                viewBrowseButton.IsEnabled = true;
                projectTextPath.IsEnabled = false;
                projectBrowseButton.IsEnabled = false;
                projectTextPath.Text = string.Empty;
                IsReadProjectEnabled = false;
            }
            else
            {
                viewsRadioButton.IsChecked = false;
                MessageBox.Show("Please choose a project APJ file first.", "Data Extractor", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        private void ApjBrowseButton_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "APJ Files (*.apj)|*.apj|All Files (*.*)|*.*"; // Set file filter
            openFileDialog.FilterIndex = 1;

            if (openFileDialog.ShowDialog() == true)
            {
                SelectedProjectPath = openFileDialog.FileName;

                if (System.IO.Path.GetExtension(SelectedProjectPath).Equals(".apj", StringComparison.OrdinalIgnoreCase))
                {
                    apjTextPath.Text = SelectedProjectPath;
                }
            }           

        }
        
        private void ProjectBrowseButton_Click(object sender, RoutedEventArgs e)
        {

            if (SelectedProjectPath!= null)
            {
                projectTextPath.Text = SelectedProjectPath;
                IsReadProjectEnabled = true;
            }else
            {
                MessageBox.Show("Please choose a project APJ file.", "Data Extractor", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        private void ViewsBrowseButton_Click(object sender, RoutedEventArgs e)
        {

            if (viewTextPath.IsEnabled  && viewBrowseButton.IsEnabled)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "DWG Files (*.dwg)|*.dwg|All Files (*.*)|*.*"; // Set file filter
                openFileDialog.FilterIndex = 1;

                if (openFileDialog.ShowDialog() == true)
                {
                    SelectedViewPath = openFileDialog.FileName;

                    if(!SelectedViewPath.Contains(Path.GetDirectoryName(SelectedProjectPath)))
                    {
                        MessageBox.Show("The DWG file must be from the previously browsed project.", "Data Extractor", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (System.IO.Path.GetExtension(SelectedViewPath).Equals(".dwg", StringComparison.OrdinalIgnoreCase))
                    {
                        viewTextPath.Text = SelectedViewPath;
                    }
                    else
                    {
                        MessageBox.Show("Please choose a file that has the .dwg extension.", "Data Extractor", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                IsReadProjectEnabled = false;
            }
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProjectPath != null && IsReadProjectEnabled==false)
            {
                Plugin = new Plugin();
                if(viewTextPath.Text!="")
                {
                    Plugin.ProjectPath = SelectedProjectPath;
                    Plugin.ViewsPath = viewTextPath.Text;
                    IsCloseButtonClicked = true;
                    this.Close();
                    Plugin.InitiateReadView();

                }
                else
                {
                    if(viewBrowseButton.IsEnabled)
                    {
                        MessageBox.Show("Please choose a file that has the .dwg extension.", "Data Extractor", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }else
                    {
                        MessageBox.Show("Please select one of the options provided earlier.", "Data Extractor", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }  
            }
            else if(SelectedProjectPath != null && IsReadProjectEnabled == true)
            {
                Plugin = new Plugin();
                Plugin.ProjectPath = SelectedProjectPath;
                IsCloseButtonClicked = true;
                this.Close();
                Plugin.InitiateReadProject();
                
            }
            else
            {
                MessageBox.Show("Please select one of the options provided earlier.", "Data Extractor", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            

        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            projectTextPath.IsEnabled = false;
            projectBrowseButton.IsEnabled = false;
            viewTextPath.IsEnabled = false;
            viewBrowseButton.IsEnabled = false;
            apjTextPath.Text = "";
            viewTextPath.Text = "";
            projectTextPath.Text = "";
            projectRadioButton.IsChecked = false;
            viewsRadioButton.IsChecked = false;
            SelectedProjectPath = null;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            IsCloseButtonClicked = true;
            this.Close();
        }

    }
}
