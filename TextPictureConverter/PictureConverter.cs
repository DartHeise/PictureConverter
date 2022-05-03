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

            Dictionary<int, int> Bounds = FillDictionary(Picture, brightnessValues, finalBrightnessValue);

            int minHeight = FindMinHeight(Bounds);

            var lines = GetLines(minHeight, Bounds);

            return lines.ToArray();
        }

        private List<Bitmap> GetLines(int minHeight, Dictionary<int, int> Bounds)
        {
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
            return lines;
        }

        private float FindBrightnessValues(Bitmap bitmap, float[] brightnessValues)
        {
            float finalBrightnessValue = 0;
            for (int i = 0; i < bitmap.Height; i++)
            {
                for (int j = 0; j < bitmap.Width; j++)
                {
                    brightnessValues[i] += bitmap.GetPixel(j, i).GetBrightness();
                }
                brightnessValues[i] /= bitmap.Width;
                finalBrightnessValue += brightnessValues[i];
            }
            return (finalBrightnessValue / bitmap.Height);
        }

        private Dictionary<int, int> FillDictionary(Bitmap bitmap, float[] brightnessValues, float finalBrightnessValue)
        {
            Dictionary<int, int> Bounds = new Dictionary<int, int>();
            bool isUpperBoundStarted = false;
            int key = 0;
            for (int i = 2; i < bitmap.Height - 3; i++)
            {

                if (!isUpperBoundStarted
                 && UpperBoundConditions(brightnessValues, finalBrightnessValue, i))
                {
                    key = i;
                    isUpperBoundStarted = true;
                }

                if (isUpperBoundStarted
                  && BottomBoundConditions(brightnessValues, finalBrightnessValue, i))
                {
                    Bounds.Add(key, i);
                    isUpperBoundStarted = false;
                }
            }
            return Bounds;
        }

        private bool UpperBoundConditions(float[] brightnessValues, float finalBrightnessValue, int index)
        {
            return (brightnessValues[index - 2] > finalBrightnessValue)
                 && (brightnessValues[index - 1] > finalBrightnessValue)
                 && (brightnessValues[index] < finalBrightnessValue)
                 && (brightnessValues[index + 1] < finalBrightnessValue)
                 && (brightnessValues[index + 2] < finalBrightnessValue)
                 && (brightnessValues[index + 3] < finalBrightnessValue);
        }

        private bool BottomBoundConditions(float[] brightnessValues, float finalBrightnessValue, int index)
        {
            return (brightnessValues[index] < finalBrightnessValue
                  && brightnessValues[index + 1] > finalBrightnessValue)
                  || (brightnessValues[index + 1] > finalBrightnessValue
                  && brightnessValues[index + 2] > finalBrightnessValue
                  && brightnessValues[index + 3] > finalBrightnessValue);
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
