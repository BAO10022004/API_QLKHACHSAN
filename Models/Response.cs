namespace API_QLKHACHSAN.Models
{
    public class Response
    {
        string messege;
        object data;

        public string Messege { get => messege; set => messege = value; }
        public object Data { get => data; set => data = value; }
    }
}
