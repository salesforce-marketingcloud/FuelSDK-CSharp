using System;
namespace FuelSDK
{
	/// <summary>
	/// ETSendClassification - Represents a send classification in a Marketing Cloud account.
	/// </summary>
	public class ETSendClassification : SendClassification
    {
        
    }

    [Obsolete("ET_SendClassification will be removed in future release. Please use ETSendClassification instead.")]
    public class ET_SendClassification : ETSendClassification 
    {
    }
}
