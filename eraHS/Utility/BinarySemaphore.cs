using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace eraHS.Utility
{

    class BinarySemaphore
    {
        private Semaphore _sem;
        private int _count;
        public BinarySemaphore(int start, int end)
        {
            _count = 0;
            _sem = new Semaphore(start, end);
        }

        public void WaitOne()
        {
            _count++;
            _sem.WaitOne();
        }

        public void ReleaseOne()
        {
            try
            {
                if (_count > 0)
                {
                    _sem.Release(1);
                    _count--;
                }
            }
            catch (SemaphoreFullException e) {
            };
        }
    }
}
