using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace ComicSorter
{
    class ComicSorterFresh
    {
        List<FileObject> originalFiles = new List<FileObject>();
        List<FileObject> orphanedFiles = new List<FileObject>();

        public String startDir = "";

        public void Start()
        {
            CreateFolders();
            FolderRecursion(startDir);

            originalFiles = originalFiles.OrderBy(f => f.Filename).ToList();

            DetermineFolders();
            Sorting();
            Cleanup();
        }

        private void CreateFolders()
        {
            string[] folderPaths = new string[]
            {
                @"D:\Download Staging\Comics\Downloads",
                @"D:\Download Staging\Comics\Unsorted",
                @"D:\Download Staging\Comics\Sorted",
                @"D:\Download Staging\Comics\Unsupported",
            };

            foreach (string folderPath in folderPaths)
            {
                CreateFolderIfNotExists(folderPath);
            }
        }

        /// <summary>
        /// Find all the files in the "Downloads" folder. Recursive.
        /// </summary>
        private void FolderRecursion(String path)
        {
            List<String> directories = new List<String>(Directory.EnumerateDirectories(path).Select(d => new DirectoryInfo(d).Name));

            Int32 dirContentCount = Directory.GetFiles(path).Length;

            if (dirContentCount != 0 && directories.Count != 0)
            {
                List<Object> temp = new List<Object>(Directory.EnumerateFiles(path).Select(f => Path.GetFileName(f)));

                foreach (String file in temp)
                {
                    originalFiles.Add(new FileObject { Filename = file, FileLocation = path });
                }
            }

            if (directories.Count == 0)
            {
                List<Object> temp = new List<Object>(Directory.EnumerateFiles(path).Select(f => Path.GetFileName(f)));

                foreach (String file in temp)
                {
                    originalFiles.Add(new FileObject { Filename = file, FileLocation = path });
                }
            }
            else
            {
                foreach (String folder in directories)
                {
                    FolderRecursion(path + folder + "\\");
                }
            }
        }

        private void DetermineFolders()
        {
            // Remove file formats we're not interested in.
            List<String> goodExt = new List<String>() { "cb7", "cbr", "cbt", "cbz", "pdf" };

            foreach (FileObject file in originalFiles)
            {
                if (!goodExt.Contains(file.Filename.Substring(file.Filename.Length - 3)))
                {
                    file.Folder = "Unsupported";
                    continue;
                }

                string selectBeforeYearRegex = @"^.*?(?=\(\b(?:Jan(?:uary)?|Feb(?:ruary)?|Mar(?:ch)?|Apr(?:il)?|May|Jun(?:e)?|Jul(?:y)?|Aug(?:ust)?|Sep(?:tember)?|Oct(?:ober)?|Nov(?:ember)?|Dec(?:ember)?)\s\d{4}\)|\(\d{4}\))";
                string selectBeforeNumberingRegex = @"^.*?(?=\b(?:Vol\.?\s|VOL\.?\s|VOLUME\s|v\d{1,2}\s|\d{2,3}\s))";

                // Select before year (e.g., "(2024)", "(May 2024)".
                Match match = Regex.Match(file.Filename, selectBeforeYearRegex);
                string folderNameBeforeYear = match.Success ? match.Value.Trim() : null;

                // Folder name before series (e.g., "012", "02", "Vol.").
                match = Regex.Match(file.Filename, selectBeforeNumberingRegex);
                string folderNameBeforeSeries = match.Success ? match.Value.Trim() : folderNameBeforeYear;

                file.Folder = folderNameBeforeSeries;
            }
        }

        private void Sorting()
        {
            string sortedDestination = @"D:\Download Staging\Comics\Sorted\";
            string unsortedDestination = @"D:\Download Staging\Comics\Unsorted\";
            string unsupportedDestination = @"D:\Download Staging\Comics\Unsupported\";

            foreach (FileObject file in originalFiles)
            {
                bool successfullyCopied = false;

                try
                {
                    if (file.Folder == "Unsupported")
                    {
                        Console.WriteLine($"COPY TO UNSUPPORTED: {file.Filename}");
                        File.Copy(file.FileLocation + file.Filename, unsupportedDestination + file.Filename, true);
                        successfullyCopied = true;
                    }
                    else if (!String.IsNullOrEmpty(file.Folder))
                    {
                        Console.WriteLine($"COPY TO SORTED: {file.Folder}\\{file.Filename}");
                        CreateFolderIfNotExists(sortedDestination + file.Folder);
                        File.Copy(file.FileLocation + file.Filename, sortedDestination + file.Folder + "\\" + file.Filename, true);
                        successfullyCopied = true;
                    }
                    else
                    {
                        Console.WriteLine($"COPY TO UNSORTED: {file.Folder}\\{file.Filename}");
                        File.Copy(file.FileLocation + file.Filename, unsortedDestination + file.Filename, true);
                        successfullyCopied = true;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"EXCEPTION: {file.Folder} - {file.Filename}");
                    Console.WriteLine(e.ToString());
                }

                // Delete successfully copies files.
                if (successfullyCopied)
                {
                    File.Delete(file.FileLocation + file.Filename);
                }
            }
        }

        private void Cleanup()
        {
            //Directory.Delete(startDir, true);
            //Directory.CreateDirectory(startDir);
        }


        private static void CreateFolderIfNotExists(string folderPath)
        {
            // Check if the folder exists
            if (!Directory.Exists(folderPath))
            {
                // Create the folder
                Directory.CreateDirectory(folderPath);
                Console.WriteLine($"Folder created at: {folderPath}");
            }
            else
            {
                Console.WriteLine($"Folder already exists at: {folderPath}");
            }
        }

        class FileObject
        {
            public String Filename;
            public String Folder;
            public String FileLocation;
        }

    }
}
