using System;

namespace CoreMultiTenancy.Identity.Results
{
    public class InviteDecodeResult
    {
        public Guid DecodedValue { get; set; } = Guid.Empty;
        public string ErrorMessage { get; set; } = String.Empty;
        public static InviteDecodeResult Success(Guid decodedValue) 
            => new InviteDecodeResult { DecodedValue = decodedValue };
        public static InviteDecodeResult Invalid()
            => new InviteDecodeResult() { ErrorMessage = "The invitation code provided is invalid." };
    }
}