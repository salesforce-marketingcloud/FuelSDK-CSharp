using System;
namespace FuelSDK
{
	/// <summary>
	/// ETObjectDefinition - Represent the result of a Describe call's ObjectDefinitionRequest.
	/// </summary>
	public class ETObjectDefinition : ObjectDefinition
    {
    }

    [Obsolete("ET_ObjectDefinition will be removed in future release. Please use ETObjectDefinition instead.")]
    public class ET_ObjectDefinition : ETObjectDefinition
    {
    }

}
