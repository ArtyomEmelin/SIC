using System;
using System.Collections.Generic;

namespace Системная_информация
{
    internal class Computer
    {
        public bool CPUEnabled { get; internal set; }
        public IEnumerable<object> Hardware { get; internal set; }

        internal void Open()
        {
            throw new NotImplementedException();
        }
    }
}