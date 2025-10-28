using System;
using System.Globalization;
using System.Text;

namespace EFCore.NamingConventions.Internal
{
    public class CustomCaseNameWriter : INameRewriter
    {
        private readonly CultureInfo _culture;

        public CustomCaseNameWriter(CultureInfo culture)
            => _culture = culture;

        public virtual string RewriteName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            var sb = new StringBuilder();
            sb.Append(char.ToUpperInvariant(name[0]));

            for (int i = 1; i < name.Length; i++)
            {
                char c = name[i];
                char prev = name[i - 1];

                bool isBoundary = char.IsLetter(c) && char.IsUpper(c) &&
                                  char.IsLetter(prev) && char.IsLower(prev);

                if (char.IsLetter(c) && char.IsUpper(c))
                {
                    bool nextIsLower = (i + 1 < name.Length &&
                                        char.IsLetter(name[i + 1]) &&
                                        char.IsLower(name[i + 1]));

                    if (nextIsLower)
                    {
                        if (char.IsDigit(prev))
                        {
                            sb.Append('_');
                        }
                        else if (char.IsLetter(prev) && char.IsUpper(prev))
                        {
                            int j = i - 1;
                            while (j >= 0 && (char.IsUpper(name[j]) || char.IsDigit(name[j])))
                                j--;

                            int blockLength = (i - 1) - j;
                            if (blockLength >= 2)
                                sb.Append('_');
                        }
                    }
                }

                if (isBoundary)
                    sb.Append('_');

                sb.Append(char.IsLetter(c)
                    ? char.ToUpperInvariant(c)
                    : c);
            }

            if (char.IsDigit(name[^1]))
            {
                int lastIndex = name.Length - 1;
                while (lastIndex >= 0 && char.IsDigit(name[lastIndex]))
                    lastIndex--;

                if (lastIndex >= 0 && char.IsLetter(name[lastIndex]) && char.IsLower(name[lastIndex]))
                {
                    int digitsCount = name.Length - lastIndex - 1;
                    sb.Insert(sb.Length - digitsCount, "_");
                }
            }

            return sb.ToString();
        }
    }
}
