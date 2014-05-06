using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace SlicerV2
{
    class Program
    {
        static void Main(string[] args)
        {
            Boolean quit = false;
            String workingDirectory = "";
            String[] allowExt = new String[] { "png", "jpeg", "jpg","gif" };
            Console.WriteLine("Which folder? ex: $slicer -go [the folder path]");
            while (quit == false) {
                String userInput = Console.ReadLine();
                if (userInput == "c")
                {
                    quit = true;
                }
                else if (userInput.IndexOf("$slicer") > -1)
                {
                    if (userInput.IndexOf("$slicer -go ") > -1)
                    {
                        try
                        {
                            String keyword = "$slicer -go ";
                            workingDirectory = userInput.Substring(keyword.Length, userInput.Length - keyword.Length);
                            DirectoryInfo dir = new DirectoryInfo(@workingDirectory);
                            Image img;
                            String fileExt = "";
                            Int32 foundValidExt = 0;
                            foreach (FileInfo file in dir.GetFiles("*.*"))
                            {
                                fileExt = file.Name.Split('.').Last();
                                foundValidExt = 0;
                                for (int i = 0; i < allowExt.Length; i++) {
                                    if (fileExt == allowExt[i]) {
                                        foundValidExt++;
                                    }
                                }
                                if (foundValidExt > 0) {
                                    img = Image.FromFile(@workingDirectory + @"\" + file.Name);
                                    Console.WriteLine("{0}, {1}", file.Name, img.Width + "x" + img.Height);
                                }
                            }
                            Console.WriteLine("\n\nI'm going to slice those images.\nHow many slices do you want?\n ex: $slicer -slice 2x5");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    if (userInput.IndexOf("$slicer -slice ") > -1)
                    {
                        try
                        {
                            String xk = "$slicer -slice ";
                            String context = taskContext(userInput, xk);
                            Int32 columns = Convert.ToInt32(context.Substring(0, 1));
                            Int32 rows = Convert.ToInt32(context.Substring(context.IndexOf("x") + 1, context.Length - 1 - context.IndexOf("x")));
                            int[,] cropRectOffset;
                            Rectangle cropRect;
                            Image srcImg;
                            Bitmap srcBm, cropBm;
                            DirectoryInfo dir = new DirectoryInfo(@workingDirectory);
                            Int32 colLoc, rowLoc;
                            String fileExt = "";
                            Int32 foundValidExt = 0;
                            foreach (FileInfo file in dir.GetFiles("*.*")) {
                                fileExt = file.Name.Split('.').Last();
                                foundValidExt = 0;
                                for (int i = 0; i < allowExt.Length; i++)
                                {
                                    if (fileExt == allowExt[i])
                                    {
                                        foundValidExt++;
                                    }
                                }
                                if (foundValidExt > 0)
                                {
                                    colLoc = 0;
                                    rowLoc = 0;
                                    srcImg = Image.FromFile(file.FullName);
                                    Console.WriteLine("=>do image " + file.Name);
                                    srcBm = new Bitmap(srcImg);
                                    Decimal srcBmWidth = srcBm.Width;
                                    Decimal srcBmHeight = srcBm.Height;
                                    cropRectOffset = new int[columns * rows, 4];
                                    for (int i = 0; i < cropRectOffset.GetLength(0); i++)
                                    {
                                        cropRectOffset[i, 0] = Convert.ToInt32(Math.Round(srcBmWidth / columns)) * colLoc;
                                        cropRectOffset[i, 1] = Convert.ToInt32(Math.Round(srcBmHeight / rows)) * rowLoc;
                                        if (colLoc < (columns - 1))
                                        {
                                            cropRectOffset[i, 2] = Convert.ToInt32(Math.Round(srcBmWidth / columns) * (colLoc + 1)) - cropRectOffset[i, 0];
                                        }
                                        else
                                        {
                                            cropRectOffset[i, 2] = Convert.ToInt32(srcBmWidth) - cropRectOffset[i, 0];
                                        }
                                        if (rowLoc < (rows - 1))
                                        {
                                            cropRectOffset[i, 3] = Convert.ToInt32(Math.Round(srcBmHeight / rows) * (rowLoc + 1)) - cropRectOffset[i, 1];
                                        }
                                        else
                                        {
                                            cropRectOffset[i, 3] = Convert.ToInt32(srcBmHeight) - cropRectOffset[i, 1];
                                        }
                                        colLoc++;
                                        if (colLoc == columns)
                                        {
                                            colLoc = 0;
                                            rowLoc++;
                                        }
                                        try
                                        {
                                            Console.WriteLine("=>=>Try cropping(" + cropRectOffset[i, 0].ToString() + "," + cropRectOffset[i, 1].ToString() + "," + cropRectOffset[i, 2].ToString() + "," + cropRectOffset[i, 3].ToString() + ")");
                                            cropRect = new Rectangle(cropRectOffset[i, 0], cropRectOffset[i, 1], cropRectOffset[i, 2], cropRectOffset[i, 3]);
                                            cropBm = srcBm.Clone(cropRect, srcBm.PixelFormat);
                                            cropBm.Save(@workingDirectory + @"\c" + Convert.ToString(i) + file.Name, ImageFormat.Png);
                                            Console.WriteLine("=>=>File is saved as c" + Convert.ToString(colLoc * rowLoc) + file.Name);
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine("crop image error:" + ex.Message);
                                        }
                                    }
                                    Console.WriteLine("Type c then press enter to exit the program.");
                                }
                            }
                        }
                        catch (Exception ex) {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
                else {
                    Console.WriteLine("Can't find the command keyword.");
                }
            }
        }
        static private String taskContext(String userInput, String keyword) {
            return userInput.Substring(keyword.Length, userInput.Length - keyword.Length);
        }
    }
   
}
