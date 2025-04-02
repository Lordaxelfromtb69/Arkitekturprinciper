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

        public IEnumerable<object> GetWords()
        {
            throw new NotImplementedException();
        }
    }
}
