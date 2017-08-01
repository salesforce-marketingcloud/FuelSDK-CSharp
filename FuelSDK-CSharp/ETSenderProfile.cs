using System;
namespace FuelSDK
{
	/// <summary>
	/// ETSenderProfile - The send profile used in conjunction with an email send definition.
	/// </summary>
	public class ETSenderProfile : SenderProfile
    {
        
    }

    [Obsolete("ET_SenderProfile will be removed in future release. Please use ETSenderProfile instead.")]
    public class ET_SenderProfile : ETSenderProfile { }
}
