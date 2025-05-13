using System;
using System.Collections.Generic;
namespace Shared.Model
{
    public class BEDocument
    {
        public int mId;

        public String mUrl;
       
        public String mIdxTime;

        public String mCreationTime;

        public int Id { get; set; }

        public object Title { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public IEnumerable<object> GetWords()
        {
            throw new NotImplementedException();
        }
    }
}
