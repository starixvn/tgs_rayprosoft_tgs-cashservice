namespace Tgs.CashService
{
    public class ProviderInfo
    {
        public ProviderInfo(string providerCode, string refId, string operationId, string action) 
        {
            ProviderCode = providerCode;
            RefId = refId;
            OperationId = operationId;
            Action = action;
        }

        public ProviderInfo(string providerCode, string refId,  string action)
        {
            ProviderCode = providerCode;
            RefId = refId;
            OperationId = string.Empty;
            Action = action;
        }

        public string ProviderCode { get; set; }
        public string RefId { get; set; }
        public string OperationId {  get; set; }
        public string Action { get; set; }
    }
}
