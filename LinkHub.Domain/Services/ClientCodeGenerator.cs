using System;
using System.Collections.Generic;
using System.Linq;
using LinkHub.Domain.Interfaces;

namespace LinkHub.Domain.Services
{
    public class ClientCodeGenerator : IClientCodeGenerator
    {
        public string GenerateCode(string clientName, IEnumerable<string> existingCodes)
        {
            if (string.IsNullOrWhiteSpace(clientName))
                throw new ArgumentException("Client name cannot be empty.", nameof(clientName));

            string alpha;
            var words = clientName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (words.Length > 1)
            {
                // Use initials for multi-word names, up to 3 letters
                alpha = string.Concat(words.Where(w => w.Length > 0).Select(w => char.ToUpper(w[0])));
                if (alpha.Length < 3)
                    alpha = alpha.PadRight(3, 'A').Substring(0, 3);
                else
                    alpha = alpha.Substring(0, 3);
            }
            else
            {
                // Fallback to first 3 letters for single-word names
                alpha = new string(clientName.Trim().ToUpper().Where(char.IsLetter).ToArray());
                if (alpha.Length < 3)
                    alpha = alpha.PadRight(3, 'A').Substring(0, 3);
                else
                    alpha = alpha.Substring(0, 3);
            }

            int nextNumber = 1;
            var codeSet = new HashSet<string>(existingCodes.Select(c => c.ToUpper()));
            string code;
            do
            {
                code = $"{alpha}{nextNumber:D3}";
                nextNumber++;
            } while (codeSet.Contains(code));

            return code;
        }
    }
}