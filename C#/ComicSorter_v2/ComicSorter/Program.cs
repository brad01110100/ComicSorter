using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ComicSorter
{
     class Program
     {
          static void Main(string[] args)
          {
               ComicSorterFresh cs = new ComicSorterFresh();

               String downloadLocation = @"D:\Download Staging\Comics\Downloads\";
               String holdingLocation = @"D:\Download Staging\Comics\Unsorted\";

               Console.WriteLine("Where do you want to sort from?  Enter your choice:  ");
               Console.WriteLine("1 \t Downloads folder\n2 \t TO BE SORTED folder\n");

               Int32 input = Convert.ToInt32(Console.ReadLine());

               switch (input)
               {
                    case 1:
                         //1 downloads folder
                         cs.startDir = downloadLocation;
                         break;
                    case 2:
                         //2 TO BE SORTED FOLDER
                         cs.startDir = holdingLocation;
                         break;
                    default:
                         Console.WriteLine("Failed to comply to directions. Terminating Life.");
                         Environment.Exit(0);
                         break;
               }

               cs.Start();

               Console.ReadKey();
          }
     }
}
