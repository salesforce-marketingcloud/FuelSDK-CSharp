using System;
namespace FuelSDK
{
	/// <summary>
	/// ETSendDefinitionList - Specifies audience associated with an email send definition.
	/// </summary>
	public class ETSendDefinitionList : SendDefinitionList
    {
        
    }

	[Obsolete("ET_SendDefinitionList will be removed in future release. Please use ETSendDefinitionList instead.")]
    public class ET_SendDefinitionList : ETSendDefinitionList { }
}
