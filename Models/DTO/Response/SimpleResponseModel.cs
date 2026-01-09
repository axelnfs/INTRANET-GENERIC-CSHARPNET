#nullable disable

namespace INTRANET_GENERIC.Models.DTO.Response
{
    public class SimpleResponseModel
    {
        public bool IsError { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public object Data2 { get; set; } = "";
    }
}
