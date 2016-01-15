using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace eraHS.Utility
{

    public class BinarySemaphore
    {
        private Semaphore sem;
        public BinarySemaphore(int start, int end)
        {
            sem = new Semaphore(start, end);
        }

        public void WaitOne()
        {
            sem.WaitOne();
        }

        public void ReleaseOne()
        {
            try
            {
                sem.Release(1);
            }
            catch (SemaphoreFullException e) {
            };
        }
    }
}
