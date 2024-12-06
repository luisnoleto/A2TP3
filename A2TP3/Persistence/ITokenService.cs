using A2TP3.Models;

namespace A2TP3.Services
{
    public interface ITokenService
    {
        string GenerateToken(Usuario usuario);
    }
}
