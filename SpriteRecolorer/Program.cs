using System.Drawing;

namespace SpriteRecolorer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("SpriteRecolorer");
                Console.WriteLine("\nA program for quickly recoloring a big amount of sprites");
                Console.WriteLine("This program allows you to quickly recolor sprites that don't use palettes");

                Console.WriteLine("\n\nUsage:");
                Console.WriteLine("SpriteRecolorer.exe {OriginalPaletteFile} {NewPaletteFile} {DirectoryWithSprites} (optional){OutputDirectory}");
                Console.WriteLine("\nOriginalPaletteFile can be either a sprite using the original colors or a file with only colors you wish to replace");
                Console.WriteLine("\nNewPaletteFile needs to be the same sprite using new colors or image with the same color layout as the originalPaletteFile");
                Console.WriteLine("\nDirectoryWithSprites needs to be the directory where the sprites you wish to recolor are contained");
                Console.WriteLine("\nOutputDirectory is the directory where the edited sprites will be saved to (if not specified {DirectoryWithSprites}_recolored is used)");
                Quit();
            }
            string OgFilePath = args[0];
            string NewFilePath = args[1];
            string DirPath = args[2];
            
            string OutDirPath = args.Length > 3 ? args[3] : DirPath + "_recolored";

            string[] Files = Directory.GetFiles(DirPath, "*.*", SearchOption.AllDirectories);

            if (!File.Exists(OgFilePath))
            {
                Console.WriteLine($"File: {OgFilePath} doesn't exist!");
                Quit();
            }
            if (!File.Exists(NewFilePath))
            {
                Console.WriteLine($"File: {NewFilePath} doesn't exist!");
                Quit();
            }
            if (!Directory.Exists(DirPath))
            {
                Console.WriteLine($"Directory: {DirPath} doesn't exist!");
                Quit();
            }

            Bitmap Og = new Bitmap(OgFilePath);
            Bitmap New = new Bitmap(NewFilePath);
            if (Og.Width != New.Width || Og.Height != New.Height)
            {
                Console.WriteLine("The dimentions of palette files don't match!");
                Quit();
            }

            List<Color> OldColors = new List<Color>();
            List<Color> NewColors = new List<Color>();

            for (int i = 0; i < Og.Width; i++)
            {
                for (int j = 0; j < Og.Height; j++)
                {
                    Color ogPixel = Og.GetPixel(i, j);
                    Color newPixel = New.GetPixel(i, j);
                    if (ogPixel != newPixel)
                    {
                        if (!OldColors.Contains(ogPixel))
                        {
                            OldColors.Add(ogPixel);
                            NewColors.Add(newPixel);
                        }
                    }
                }
            }

            int SavedFiles = 0;
            foreach (string file in Files)
            {
                try
                {
                    Bitmap ed = new Bitmap(file);
                    for (int i = 0; i < ed.Width; i++)
                    {
                        for (int j = 0; j < ed.Height; j++)
                        {
                            int index = OldColors.IndexOf(ed.GetPixel(i, j));
                            if (index > -1)
                                ed.SetPixel(i, j, NewColors[index]);
                        }
                    }
                    string output = Path.Combine(OutDirPath, Path.GetRelativePath(DirPath, file));
                    string outputDir = Path.GetDirectoryName(output);
                    if (!Directory.Exists(outputDir))
                        Directory.CreateDirectory(outputDir);
                    ed.Save(output);
                    SavedFiles++;
                }
                catch
                {

                }
            }
            if (SavedFiles == 0)
            {
                Console.WriteLine($"Unable to save any files!");
                Quit();
            }

            Console.WriteLine($"Saved {SavedFiles} files to: {OutDirPath}!");
        }
        static void Quit()
        {
            Console.WriteLine("\n\nPress any key to exit");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}