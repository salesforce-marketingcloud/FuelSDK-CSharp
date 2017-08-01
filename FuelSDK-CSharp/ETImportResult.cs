using System;
namespace FuelSDK
{
    /// <summary>
    /// ETImportResult - Represent an import operation result (ImportResultSummary).
    /// </summary>
    public class ETImportResult : ImportResultsSummary
    {
        
    }

	[Obsolete("ET_ImportResult will be removed in future release. Please use ETImportResult instead.")]
    public class ET_ImportResult : ETImportResult { }
}
