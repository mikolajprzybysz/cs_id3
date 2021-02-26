using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ID3
{
    /// <summary>
    ///Class used to count number of appearances of a class in a column
    /// </summary>
    public class ClassCounter
    {
        //Class
        String value;

        public String Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        //Number of appearances. Initially one, but there is constructor that can set any value 
        Int32 counter;

        public Int32 Counter
        {
            get { return counter; }
            set { counter = value; }
        }

        public ClassCounter(String value) 
        {
            this.value = value;
            counter = 1;
        }

        public ClassCounter(String value, int counter)
        {
            this.value = value;
            this.counter = counter;
        }
    }
}
