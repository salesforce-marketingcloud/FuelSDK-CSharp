using System;
namespace FuelSDK
{
	/// <summary>
	/// ETDeliveryProfile - Contains information on a single delivery profile within an account.
	/// </summary>
	public class ETDeliveryProfile : DeliveryProfile
    {
        
    }

	[Obsolete("ET_DeliveryProfile will be removed in future release. Please use ETDeliveryProfile instead.")]
    public class ET_DeliveryProfile : ETDeliveryProfile { }
}
