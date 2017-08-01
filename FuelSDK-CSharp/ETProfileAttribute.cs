using System;
namespace FuelSDK
{
	/// <summary>
	/// ETProfileAttribute - Defines any additional attribute for a subscriber.
	/// </summary>
	public class ETProfileAttribute :Attribute
    {
        
    }
    
    [Obsolete("ET_ProfileAttribute will be removed in future release. Please use ETProfileAttribute instead.")]
    public class ET_ProfileAttribute : ETProfileAttribute { }
}
