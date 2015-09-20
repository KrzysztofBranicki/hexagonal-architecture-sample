using System.Linq;
using Common.Querying;
using NUnit.Framework;

namespace Common.Tests.Querying
{
    [TestFixture]
    public class ResultPageTest
    {
        [Test]
        public void Pages_count_calcullation_for_fully_populated_pages()
        {
            var pageSize = 50;
            var totalItemsInSystem = 150;
            var pageNumber = 2;

            var result = new ResultPage<string>(Enumerable.Repeat("test", pageSize), pageNumber, pageSize, totalItemsInSystem);

            Assert.That(result.PagesCount, Is.EqualTo(3));
        }

        [Test]
        public void Pages_count_calcullation_for_partially_populated_pages()
        {
            var pageSize = 50;
            var totalItemsInSystem = 151;
            var pageNumber = 2;

            var result = new ResultPage<string>(Enumerable.Repeat("test", pageSize), pageNumber, pageSize, totalItemsInSystem);

            Assert.That(result.PagesCount, Is.EqualTo(4));
        }

        [Test]
        public void Pages_count_calcullation_with_single_item()
        {
            var pageSize = 50;
            var totalItemsInSystem = 1;
            var pageNumber = 1;

            var result = new ResultPage<string>(Enumerable.Repeat("test", totalItemsInSystem), pageNumber, pageSize, totalItemsInSystem);

            Assert.That(result.PagesCount, Is.EqualTo(1));
        }
    }
}
