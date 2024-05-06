namespace WebService.Models.AppResponse
{
    public class ApiResponse<T> : ApiResponse
    {
        public ApiResponse() : base() { }
        public ApiResponse(T _content)
            : this()
        {
            Data = _content;
        }
        public new T Data { get; set; }
    }
    public class ApiResponse
    {
        public ApiResponse()
        {
            RtnCode = "0000";
        }
        public ApiResponse(object _content)
            : this()
        {
            Data = _content;
        }

        public string RtnCode { get; set; }
        public string RtnMsg { get; set; }
        public object Data { get; set; }
    }
}
