namespace WebApi.DTO.UserDTO
{
    public record UserUpdateRequest(
        string? Username,
        string? Email,
        string? Password) ;
}
