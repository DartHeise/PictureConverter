using System.Drawing;

namespace TextPictureConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            PictureConverter pictureConverter = new PictureConverter(new Bitmap($"InputPicture/input.png"));
            Bitmap[] lines = pictureConverter.ConvertTextToLines();
            int i = 1;
            foreach (Bitmap line in lines)
            {
                line.Save(@$"OutputPicture/line{i++}.png");
            }
        }
    }
}