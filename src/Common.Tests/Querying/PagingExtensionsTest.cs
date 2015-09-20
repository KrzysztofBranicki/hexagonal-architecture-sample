using System;
using System.Collections.Generic;
using System.Linq;

using Common.Querying;

using NUnit.Framework;

namespace Common.Tests.Querying
{

    [TestFixture]
    public class PagingExtensionsTest
    {
        [Test]
        public void Pick_third_page_with_50_items_orderby_value_desc()
        {
            var criteria = new GridSearchCriteria(pageNumber: 3, pageSize: 50, sortColunName: "Value", ascending: false);
            var result = CreateTestList().AsQueryable().ApplyGridSearchCriteria(criteria);

            Assert.That(result.Items.Count(), Is.EqualTo(50));
            Assert.That(result.Items.First().Value, Is.EqualTo("TestValue050"));
            Assert.That(result.Items.Last().Value, Is.EqualTo("TestValue001"));
        }

        public List<KeyValuePair<Guid, string>> CreateTestList()
        {
            var list = new List<KeyValuePair<Guid, string>>();
            for (int i = 0; i < 151; i++)
            {
                list.Add(new KeyValuePair<Guid, string>(Guid.NewGuid(), $"TestValue{i:D3}"));
            }

            return list;
        }
    }
}
