namespace BE_Teaching.Models
{
    public class Response<T>
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; } 
        public T Data { get; set; }
        public List<string> Errors { get; set; } 

        public Response()
        {
            Errors = new List<string>();
        }
    }
}
