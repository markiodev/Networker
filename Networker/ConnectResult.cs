namespace Networker
{
    public class ConnectResult
    {
        public ConnectResult()
        {
            Success = true;
        }

        public ConnectResult(string reasonCode)
        {
            Reason = reasonCode;
            Success = false;
        }

        public bool Success { get; set; }

        public string Reason { get; set; }
    }
}