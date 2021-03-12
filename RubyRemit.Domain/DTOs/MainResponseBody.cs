using RubyRemit.Domain.Entities;

namespace RubyRemit.Domain.DTOs
{
    public class MainResponseBody
    {
        public bool Succeeded { get; set; }


        public string Message { get; set; }


        public Payment Data { get; set; }
    }
}
