using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
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

namespace biometria_1;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{

    string? SourceLink = null;
    Bitmap sourceImage = null;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void OpenFile(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        if (openFileDialog.ShowDialog() == true)
        {
            Uri fileUri = new Uri(openFileDialog.FileName);
            this.sourceImage = MediaStore.Images.Media.getBitmap(this.getContentResolver(), fileUri);
            ReadyImage.Source = new BitmapImage(fileUri);
        }
}
