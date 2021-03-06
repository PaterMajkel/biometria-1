using Microsoft.Win32;
using System;
using System.Drawing;
using System.Drawing.Imaging;
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
    Image = 1,
    Histogram = 2,
}

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{

    string fileType = "";
    Bitmap sourceImage = null;
    BitmapFlags flag = BitmapFlags.Image;
    Bitmap finalImage;
    bool meanValueChanges = false;
    public MainWindow()
    {
        InitializeComponent();
        foreach (var btn in GridMenu.Children.OfType<System.Windows.Controls.Button>())
        {
            btn.IsEnabled = false;
        }
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
        if (!CheckIfFile()) return;

        SaveFileDialog saveFileDialog = new SaveFileDialog();
        saveFileDialog.Filter = "Images|*.png;*.bmp;*.jpg";
        ImageFormat format = ImageFormat.Png;
        if (saveFileDialog.ShowDialog() == true)
        {
            string ext = System.IO.Path.GetExtension(saveFileDialog.FileName);
            switch (ext)
            {
                case ".jpg":
                    format = ImageFormat.Jpeg;
                    break;
                case ".png":
                    format = ImageFormat.Png;
                    break;
            }
            finalImage.Save(saveFileDialog.FileName, format);
        }
    }

    private void Exit(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void SetFlagToHistogram(object sender, RoutedEventArgs e)
    {
        flag = BitmapFlags.Histogram;
        foreach (var btn in GridMenu.Children.OfType<System.Windows.Controls.Button>())
        {
            btn.IsEnabled = true;
        }
    }

    private void SetFlagToImage(object sender, RoutedEventArgs e)
    {
        flag = BitmapFlags.Image;
        foreach (var btn in GridMenu.Children.OfType<System.Windows.Controls.Button>())
        {
            btn.IsEnabled = false;
        }
    }

    private void RedValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!CheckIfFile()) return;
        RedLabel.Content = $"Red Value: {Math.Round(RedValue.Value)}";
        if (meanValueChanges)
            return;

        if (this.flag == BitmapFlags.Image)
        {
            Bitmap bitmap = new Bitmap(this.sourceImage.Width, this.sourceImage.Height);
            bitmap = (Bitmap)this.sourceImage.Clone();
            finalImage = BinaryThreshold(bitmap, (byte)RedValue.Value, 2);
            ReadyImage.Source = ImageSourceFromBitmap(finalImage);
        }
        else
        {
            Bitmap bitmap = new Bitmap(this.sourceImage.Width, this.sourceImage.Height);
            bitmap = (Bitmap)this.sourceImage.Clone();
            finalImage = Histogram(bitmap, 2);
            ReadyImage.Source = ImageSourceFromBitmap(finalImage);
        }
    }

    private void BlueValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!CheckIfFile()) return;
        BlueLabel.Content = $"Blue Value: {Math.Round(BlueValue.Value)}";
        if (meanValueChanges)
            return;
        if (this.flag == BitmapFlags.Image)
        {
            Bitmap bitmap = new Bitmap(this.sourceImage.Width, this.sourceImage.Height);
            bitmap = (Bitmap)this.sourceImage.Clone();
            finalImage = BinaryThreshold(bitmap, (byte)BlueValue.Value, 0);
            ReadyImage.Source = ImageSourceFromBitmap(finalImage);

        }
        else
        {
            Bitmap bitmap = new Bitmap(this.sourceImage.Width, this.sourceImage.Height);
            bitmap = (Bitmap)this.sourceImage.Clone();
            finalImage = Histogram(bitmap, 0);
            ReadyImage.Source = ImageSourceFromBitmap(finalImage);
        }
    }

    private void GreenValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!CheckIfFile()) return;
        GreenLabel.Content = $"Green Value: {Math.Round(GreenValue.Value)}";
        if (meanValueChanges)
            return;
        if (this.flag == BitmapFlags.Image)
        {
            Bitmap bitmap = new Bitmap(this.sourceImage.Width, this.sourceImage.Height);
            bitmap = (Bitmap)this.sourceImage.Clone();
            finalImage = BinaryThreshold(bitmap, (byte)GreenValue.Value, 1);
            ReadyImage.Source = ImageSourceFromBitmap(finalImage);
        }
        else
        {
            Bitmap bitmap = new Bitmap(this.sourceImage.Width, this.sourceImage.Height);
            bitmap = (Bitmap)this.sourceImage.Clone();
            finalImage = Histogram(bitmap, 1);
            ReadyImage.Source = ImageSourceFromBitmap(finalImage);
        }
    }

    private void MeanValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!CheckIfFile()) return;
        meanValueChanges = true;
        MeanLabel.Content = $"Mean Value: {Math.Round(MeanValue.Value).ToString()}";
        GreenValue.Value = RedValue.Value = BlueValue.Value = MeanValue.Value;
        if (this.flag == BitmapFlags.Image)
        {
            Bitmap bitmap = new Bitmap(this.sourceImage.Width, this.sourceImage.Height);
            bitmap = (Bitmap)this.sourceImage.Clone();
            finalImage = BinaryThreshold(bitmap, (byte)MeanValue.Value);
            ReadyImage.Source = ImageSourceFromBitmap(finalImage);
        }
        else
        {
            Bitmap bitmap = new Bitmap(this.sourceImage.Width, this.sourceImage.Height);
            bitmap = (Bitmap)this.sourceImage.Clone();
            finalImage = Histogram(bitmap);
            ReadyImage.Source = ImageSourceFromBitmap(finalImage);
        }
        meanValueChanges = false;
    }

    private void RedBtn(object sender, RoutedEventArgs e)
    {
        if (!CheckIfFile()) return;
        Bitmap bitmap = new Bitmap(this.sourceImage.Width, this.sourceImage.Height);
        bitmap = (Bitmap)this.sourceImage.Clone();
        finalImage = Histogram(bitmap, 2);
        ReadyImage.Source = ImageSourceFromBitmap(finalImage);
    }

    private void BlueBtn(object sender, RoutedEventArgs e)
    {
        if (!CheckIfFile()) return;
        Bitmap bitmap = new Bitmap(this.sourceImage.Width, this.sourceImage.Height);
        bitmap = (Bitmap)this.sourceImage.Clone();
        finalImage = Histogram(bitmap, 0);
        ReadyImage.Source = ImageSourceFromBitmap(finalImage);
    }

    private void GreenBtn(object sender, RoutedEventArgs e)
    {
        if (!CheckIfFile()) return;
        Bitmap bitmap = new Bitmap(this.sourceImage.Width, this.sourceImage.Height);
        bitmap = (Bitmap)this.sourceImage.Clone();
        finalImage = Histogram(bitmap, 1);
        ReadyImage.Source = ImageSourceFromBitmap(finalImage);
    }
    private void MeanBtn(object sender, RoutedEventArgs e)
    {
        if (!CheckIfFile()) return;
        Bitmap bitmap = new Bitmap(this.sourceImage.Width, this.sourceImage.Height);
        bitmap = (Bitmap)this.sourceImage.Clone();
        finalImage = Histogram(bitmap);
        ReadyImage.Source = ImageSourceFromBitmap(finalImage);
    }

    public bool CheckIfFile()
    {
        if (sourceImage == null)
        {
            MessageBox.Show("No image added", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        return true;
    }
}
