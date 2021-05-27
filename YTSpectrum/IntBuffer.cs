using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YTSpectrum
{
    public class IntBuffer
    {
        private int previousValue;
        private readonly Queue<int> buffer;

        public int Value
        {
            get
            {
                if (buffer.Count > 0)
                    return buffer.Dequeue();
                return previousValue;
            }
            set
            {
                buffer.Enqueue(value);
                previousValue = value;
            }
        }

        public int LastValue => previousValue;

        public IntBuffer(int initialValue = 0)
        {
            previousValue = initialValue;
            buffer = new Queue<int>();
        }


    }
}
