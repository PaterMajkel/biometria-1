using Microsoft.Win32;
using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static biometria_1.Algorithm;

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
    bool meanValueChanges=false;
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OpenFile(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Image files (*.jpg;*.png)|*.jpg;*.png|All files (*.*)|*.*";
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

    private void RedValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        RedLabel.Content = $"Red Value: {RedValue.Value}";
        if (meanValueChanges)
            return;
        RedLabel.Content = $"Red Value: {Math.Round(RedValue.Value).ToString()}";

    }

    private void BlueValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        BlueLabel.Content = $"Blue Value: {BlueValue.Value}";
        if (meanValueChanges)
            return;
        BlueLabel.Content = $"Blue Value: {Math.Round(BlueValue.Value).ToString()}";

    }

    private void GreenValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        GreenLabel.Content = $"Green Value: {GreenValue.Value}";
        if (meanValueChanges)
            return;
        if (this.flag == BitmapFlags.Image)
        {
            ReadyImage.Source = ImageSourceFromBitmap(BinaryThreshold(this.sourceImage, (byte)GreenValue.Value,  2));

        }
    }

    private void MeanValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        meanValueChanges = true;
        MeanLabel.Content = $"Mean Value: {MeanValue.Value}";
        GreenValue.Value = RedValue.Value = BlueValue.Value = MeanValue.Value;
        if (this.flag == BitmapFlags.Image)
        {
            ReadyImage.Source = ImageSourceFromBitmap(BinaryThreshold(this.sourceImage, (byte)MeanValue.Value, 4));
        GreenLabel.Content = $"Green Value: {Math.Round(GreenValue.Value).ToString()}";

    }

    private void MeanValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        MeanLabel.Content = $"Mean Value: {Math.Round(MeanValue.Value).ToString()}";

        }
        meanValueChanges = false;
    }
}
