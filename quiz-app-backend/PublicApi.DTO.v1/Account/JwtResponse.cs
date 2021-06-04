namespace PublicApi.DTO.v1.Account
{
    public class JwtResponse
    {
        public string Token { get; set; } = default!;
        public string Firstname { get; set; } = default!;
        public string Lastname { get; set; } = default!;
    }
}