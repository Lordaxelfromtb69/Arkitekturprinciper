using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Shared.Model;

namespace Indexer
{
    public class Indexer
    {
        private IDatabase mdatabase;
        private int documentCounter = 0;

        public Indexer(IDatabase database)
        {
            mdatabase = database;
        }

        public void IndexFilesIn(DirectoryInfo dir, List<string> extensions)
        {
            Console.WriteLine($"Crawling {dir.FullName}");

            // Dictionary for counting word frequencies
            Dictionary<string, int> wordFrequencies = new Dictionary<string, int>();

            foreach (var file in dir.EnumerateFiles())
            {
                if (extensions.Contains(file.Extension))
                {
                    documentCounter++;
                    BEDocument newDoc = new BEDocument
                    {
                        mId = documentCounter,
                        mUrl = file.FullName,
                        mIdxTime = DateTime.Now.ToString(),
                        mCreationTime = file.CreationTime.ToString()
                    };

                    // Insert the document into the database
                    mdatabase.InsertDocument(newDoc);

                    // Extract words from the file
                    ISet<string> wordsInFile = ExtractWordsInFile(file);

                    // Count word occurrences
                    foreach (var word in wordsInFile)
                    {
                        if (!wordFrequencies.ContainsKey(word))
                            wordFrequencies[word] = 0;

                        wordFrequencies[word]++;
                    }

                    // Insert words and their occurrences into the database
                    mdatabase.InsertAllWords(new Dictionary<string, int> { });
                    mdatabase.InsertAllOcc(newDoc.mId, GetWordIdFromWords(wordsInFile));
                }
            }

            // Crawl subdirectories
            // Crawl subdirectories
            foreach (var subDir in dir.EnumerateDirectories())
            {
                IndexFilesIn(subDir, extensions);
            }


            // Ask the user how many words they want to see
            Console.WriteLine("Hvor mange ord vil du se?");
            int numberOfWords = int.Parse(Console.ReadLine());

            // Sort words by frequency in descending order
            var sortedWords = wordFrequencies.OrderByDescending(w => w.Value).Take(numberOfWords);

            // Display the top words
            Console.WriteLine($"De {numberOfWords} hyppigste ord:");
            foreach (var word in sortedWords)
            {
                Console.WriteLine($"<{word.Key}, {word.Value}>");
            }
        }

        // Example method to extract words from a file
        private ISet<string> ExtractWordsInFile(FileInfo file)
        {
            // Here you would implement your logic for extracting words from a file
            // For now, let's just return a mock set of words
            return new HashSet<string> { "example", "word", "text", "document" };
        }

        // Example method to get word IDs for a set of words
        private ISet<int> GetWordIdFromWords(ISet<string> words)
        {
            // In a real implementation, this would look up word IDs from the database
            // For now, let's just return a mock set of IDs
            return new HashSet<int> { 1, 2, 3, 4 };
        }
    }
}
