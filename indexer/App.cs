﻿using System;
using System.Collections.Generic;
using System.IO;
using Shared;

namespace Indexer
{
    public class App
    {
        public void Run(){
            DatabaseSqlite db = new DatabaseSqlite();
            Crawler crawler = new Crawler(db);

            var root = new DirectoryInfo(Paths.FOLDER);

            DateTime start = DateTime.Now;

            crawler.IndexFilesIn(root, new List<string> { ".txt"});        

            TimeSpan used = DateTime.Now - start;
            Console.WriteLine("DONE! used " + used.TotalMilliseconds);

            var all = db.GetAllWords();

            Console.WriteLine($"Indexed {db.GetDocumentCounts()} documents");
            Console.WriteLine($"Number of different words: {all.Count}");
            int count = 10;
            Console.WriteLine($"The first {count} is:");
            foreach (var p in all) {
                Console.WriteLine("<" + p.Key + ", " + p.Value + ">");
                count--;
                if (count == 0) break;
            }
        }
    }

    internal class Crawler
    {
        public Crawler(DatabaseSqlite db)
        {
            Db = db;
        }

        public DatabaseSqlite Db { get; }

        internal void IndexFilesIn(DirectoryInfo root, List<string> list)
        {
            throw new NotImplementedException();
        }
    }
}
