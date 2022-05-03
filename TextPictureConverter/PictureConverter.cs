using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextPictureConverter
{
    public class PictureConverter
    {
        public Bitmap Picture { get; }
        public PictureConverter(Bitmap picture)
        {
            Picture = picture;
        }

        public Bitmap[] ConvertTextToLines()
        {

            float[] brightnessValues = new float[Picture.Height];
            float finalBrightnessValue = FindBrightnessValues(Picture, brightnessValues);

            float upperBound = finalBrightnessValue;
            float bottomBound = finalBrightnessValue;
            Dictionary<int, int> Bounds = FillDictionary(Picture, brightnessValues, upperBound, bottomBound);


            int minHeight = FindMinHeight(Bounds);

            var lines = new List<Bitmap>();
            int deltaHeight = (int)(minHeight * 0.7);
            foreach (var bound in Bounds)
            {
                int lineUpperBound = bound.Key - deltaHeight;
                if (lineUpperBound < 0)
                    lineUpperBound = 0;
                int lineBottomBound = bound.Value + deltaHeight;
                if (lineBottomBound > Picture.Height)
                    lineBottomBound = Picture.Height;
                Bitmap line = new Bitmap(Picture.Width, lineBottomBound - lineUpperBound);

                for (int i = 0; i < line.Height; i++)
                {
                    for (int j = 0; j < line.Width; j++)
                    {
                        line.SetPixel(j, i, Picture.GetPixel(j, lineUpperBound + i));
                    }
                }
                lines.Add(line);
            }
            return lines.ToArray(); 
        }

        private float FindBrightnessValues(Bitmap bitmap, float[] brightnessValues)
        {
            float finalBrightnessValue = 0;
            for (int i = 0; i < bitmap.Height; i++)
            {
                for (int j = 0; j < bitmap.Width; j++)
                {
                    var pixel = bitmap.GetPixel(j, i);
                    brightnessValues[i] += (float)(0.212 * pixel.R + 0.715 * pixel.G + 0.072 * pixel.B);
                }
                brightnessValues[i] /= bitmap.Width;
                finalBrightnessValue += brightnessValues[i];
            }
            return (finalBrightnessValue / bitmap.Height);
        }

        private Dictionary<int, int> FillDictionary(Bitmap bitmap, float[] brightnessValues, float upperBound, float bottomBound)
        {
            Dictionary<int, int> Bounds = new Dictionary<int, int>();
            bool isUpperBoundStarted = false;
            int key = 0;
            for (int i = 2; i < bitmap.Height - 3; i++)
            {

                if (!isUpperBoundStarted
                 && (brightnessValues[i - 2] > upperBound)
                 && (brightnessValues[i - 1] > upperBound)
                 && (brightnessValues[i] < bottomBound)
                 && (brightnessValues[i + 1] < bottomBound)
                 && (brightnessValues[i + 2] < bottomBound)
                 && (brightnessValues[i + 3] < bottomBound))
                {
                    key = i;
                    isUpperBoundStarted = true;
                }

                if ((isUpperBoundStarted
                  && brightnessValues[i] < upperBound
                  && brightnessValues[i + 1] > bottomBound)
                  || (isUpperBoundStarted
                  && brightnessValues[i + 1] > bottomBound
                  && brightnessValues[i + 2] > bottomBound
                  && brightnessValues[i + 3] > bottomBound))
                {
                    Bounds.Add(key, i);
                    isUpperBoundStarted = false;
                }
            }
            return Bounds;
        }

        private int FindMinHeight(Dictionary<int, int> Bounds)
        {
            int minHeight = 0;
            bool firstElement = true;
            foreach (var bound in Bounds)
            {
                if (firstElement)
                {
                    minHeight = bound.Value - bound.Key;
                    firstElement = false;
                }
                else
                {
                    if (minHeight > (bound.Value - bound.Key))
                        minHeight = bound.Value - bound.Key;
                }
            }
            return minHeight;
        }
    }
}
