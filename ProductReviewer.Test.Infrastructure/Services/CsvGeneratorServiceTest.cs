using FluentAssertions;
using ProductReviewer.Infrastructure.Services;

namespace ProductReviewer.Test.Infrastructure.Services
{
    public class CsvGeneratorServiceTest
    {

        [Trait("Category", "Infrastructure")]
        [Fact]
        public void Export_ReturnCorrectCsv_GivenSampleData()
        {
            var generator = new CsvGeneratorService();
            var data = new List<TestData>
            {
                new TestData { Id = 1, Name = "Test1", Description = "Description1" },
                new TestData { Id = 2, Name = "Test2", Description = "Description2" }
            };

            var csv = generator.Export(data);

            csv.Should().Contain("Id,Name,Description");
            csv.Should().Contain("1,Test1,Description1");
            csv.Should().Contain("2,Test2,Description2");
        }


        [Trait("Category", "Infrastructure")]
        [Fact]
        public void Export_EscapesCommaQuotesNewlines()
        {
            var generator = new CsvGeneratorService();
            var data = new List<TestData>
            {
                new TestData { Id = 1, Name = "Test1", Description = "He said, \"Hello\"\nHow are you?"},
            };

            var csv = generator.Export(data);

            csv.Should().Contain("Id,Name,Description");
            csv.Should().Contain("1,Test1,\"He said, \"\"Hello\"\"\nHow are you?\"");
        }

        [Trait("Category", "Infrastructure")]
        [Fact]
        public void Export_EmptyList_ReturnsOnlyHeader()
        {
            var generator = new CsvGeneratorService();
            var data = new List<TestData>();

            var csv = generator.Export(data);

            csv.Should().Contain("Id,Name,Description");
            csv.Should().NotContain("\n1,");
        }


        public class TestData
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
        }
    }
}
