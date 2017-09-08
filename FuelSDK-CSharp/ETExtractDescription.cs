using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuelSDK
{
    public class ETExtractDescription : ExtractDescription
    {
        /// <summary>
        /// Get this instance.
        /// </summary>
        /// <returns>The <see cref="T:FuelSDK.GetReturn"/> object..</returns>
        public GetReturn Get() { var r = new GetReturn(this); LastRequestID = r.RequestID; return r; }
    }
}
