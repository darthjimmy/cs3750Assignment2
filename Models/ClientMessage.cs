
namespace Conway
{
    public class ClientMessage
    {
        public string Command { get; set; }
        public bool[,] Data {get;set;}
    }
}