using ProductReviewer.Application.Common.Interface;
using System.Reflection;
using System.Text;

namespace ProductReviewer.Infrastructure.Services
{
    public class CsvGeneratorService : ICsvGenerator
    {
        public string Export<T>(IEnumerable<T> data)
        {
            var sb = new StringBuilder();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            sb.AppendLine(string.Join(",", properties.Select(p => p.Name)));

            foreach (var item in data)
            {
                var values = properties.Select(p =>
                {
                    var value = p.GetValue(item, null);
                    return EscapeCsv(value?.ToString() ?? string.Empty);
                });

                sb.AppendLine(string.Join(",", values));
            }

            return sb.ToString();
        }

        private string EscapeCsv(string value)
        {
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                value = value.Replace("\"", "\"\"");
                return $"\"{value}\"";
            }

            return value;
        }
    }
}
