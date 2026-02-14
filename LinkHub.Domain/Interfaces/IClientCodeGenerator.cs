using System.Collections.Generic;

namespace LinkHub.Domain.Interfaces
{
    public interface IClientCodeGenerator
    {
        string GenerateCode(string clientName, IEnumerable<string> existingCodes);
    }
}