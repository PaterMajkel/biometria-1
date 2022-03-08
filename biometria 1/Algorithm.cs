using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace biometria_1;

public static class Algorithm
{
    public static Bitmap Histogram(Bitmap bmp)
    {
        var data = bmp.LockBits(
            new Rectangle(0, 0, bmp.Width, bmp.Height),
            System.Drawing.Imaging.ImageLockMode.ReadWrite,
            System.Drawing.Imaging.PixelFormat.Format24bppRgb
        );
        var bmpData = new byte[data.Stride * data.Height];

        Marshal.Copy(data.Scan0, bmpData, 0, bmpData.Length);
        // Przerzuci z Bitmapy do tablicy

        int[] histogram = new int[256];
        foreach (byte i in bmpData)
            ++histogram[i];
        double max = histogram.Max();
        for (int i = 0; i < histogram.Length; i++)
            histogram[i] = (int)(histogram[i] / max * 512.0);

        bmpData = new byte[bmpData.Length];
        for (int i = 0; i < bmpData.Length; i++)
            bmpData[i] = 255;
        for (int i = 0; i < histogram.Length; i++)
        {
            for (int j = 0; j < histogram[i]; j++)
            {
                int index = i * 3 + (511 - j) * data.Stride;

                bmpData[index + 0] =
                bmpData[index + 1] =
                bmpData[index + 2] = 0;
            }
        }

        Marshal.Copy(bmpData, 0, data.Scan0, bmpData.Length);
        // Przerzuci z tablicy do Bitmapy

        bmp.UnlockBits(data);
        return bmp;
    }

    public static Bitmap BinaryThreshold(Bitmap bmp, byte threshold, int canal = 3)
    {
        var data = bmp.LockBits(
            new Rectangle(0, 0, bmp.Width, bmp.Height),
            System.Drawing.Imaging.ImageLockMode.ReadWrite,
            System.Drawing.Imaging.PixelFormat.Format24bppRgb
        );

        /*
         * 3x4 => 12 pikseli
         * 9x4 => 36 bajtów (RGB)
         */

        // Width - szerokość w pikselach
        // Stride - szerokość w bajtach
        // Stride => Width * 3
        var bmpData = new byte[data.Stride * data.Height];

        Marshal.Copy(data.Scan0, bmpData, 0, bmpData.Length);
        // Przerzuci z Bitmapy do tablicy

        for (int i = 0; i < bmpData.Length; i += 3)
        {
            byte r = bmpData[i + 0];
            byte g = bmpData[i + 1];
            byte b = bmpData[i + 2];

            switch (canal)
            {
                case 0:
                    {
                        bmpData[i + 0] =
                        r > threshold
                            ? byte.MaxValue
                            : byte.MinValue;
                        break;
                    }
                case 1:
                    {
                        bmpData[i + 1] =
                        g > threshold
                            ? byte.MaxValue
                            : byte.MinValue;
                        break;
                    }
                case 2:
                    {
                        bmpData[i + 1] =
                        b > threshold
                            ? byte.MaxValue
                            : byte.MinValue;
                        break;
                    }
                default:
                    {
                        byte mean = (byte)((r + g + b) / 3);

                        bmpData[i + 0] =
                        bmpData[i + 1] =
                        bmpData[i + 2] = mean > threshold
                            ? byte.MaxValue
                            : byte.MinValue;
                        break;
                    }
            }
                
        }

        Marshal.Copy(bmpData, 0, data.Scan0, bmpData.Length);
        // Przerzuci z tablicy do Bitmapy

        bmp.UnlockBits(data);
        return bmp;
    }
}
