using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample_DataType
{
    public interface IDataType
    {
        Object GetValue();
        void SetValue(Object obj);
    }
}
