using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace biometria_1;

public enum BitmapFlags
{
    Image=1,
    Histogram=2,
}

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{

    string fileType = "";
    Bitmap sourceImage = null;
    BitmapFlags flag = BitmapFlags.Image;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void OpenFile(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg|All files (*.*)|*.*";
        if (openFileDialog.ShowDialog() == true)
        {
            string fileName = openFileDialog.FileName;
            this.sourceImage = new Bitmap($"{fileName}");
            ReadyImage.Source = ImageSourceFromBitmap(this.sourceImage);
            this.fileType = fileName.Split('.').Last();
        }
    }

    [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DeleteObject([In] IntPtr hObject);
    public ImageSource ImageSourceFromBitmap(Bitmap bmp)
    {
        var handle = bmp.GetHbitmap();
        try
        {
            return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
        finally { DeleteObject(handle); }
    }

    private void SaveFile(object sender, RoutedEventArgs e)
    {
        if(this.sourceImage == null)
        {
            MessageBox.Show("No image to save", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        if (saveFileDialog.ShowDialog() == true)
            sourceImage.Save(saveFileDialog.FileName+this.fileType);
    }

    private void Exit(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void SetFlagToHistogram(object sender, RoutedEventArgs e)
    {
        flag = BitmapFlags.Histogram;
    }

    private void SetFlagToImage(object sender, RoutedEventArgs e)
    {
        flag = BitmapFlags.Image;
    }
}
