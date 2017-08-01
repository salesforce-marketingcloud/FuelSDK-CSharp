using System;
namespace FuelSDK
{
	/// <summary>
	/// ETTriggerSend - Represents a specific instance of a triggered email send.
	/// </summary>
	public class ETTriggerSend : TriggeredSend
    {
        public ETTriggerSend()
        {
        }
    }

    [Obsolete("ET_Trigger will be removed in future release. Please use ETTriggerSend instead.")]
    public class ET_Trigger : ETTriggerSend { }
}
