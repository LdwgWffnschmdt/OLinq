﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OLinq.Tests
{

    [TestClass]
    public class SelectManyTests
    {

        [TestMethod]
        public void SelectManyTest()
        {
            var c1 = new ObservableCollection<string>()
            {
                "Item1",
                "Item2",
                "Item3",
                "Item4",
                "Item5",
            };

            var c2 = new ObservableCollection<ObservableCollection<string>>()
            {
                c1,
                c1,
                c1,
            };

            var o = new SelectManyOperation<IEnumerable<string>, string>(new OperationContext(),
                Expression.Call(typeof(Enumerable), "SelectMany", new Type[] { typeof(IEnumerable<string>), typeof(string) },
                    Expression.Constant(c2),
                    Expression.Lambda<Func<IEnumerable<string>, IEnumerable<string>>>(
                        Expression.Parameter(typeof(IEnumerable<string>), "p"),
                        Expression.Parameter(typeof(IEnumerable<string>), "p"))));

            int added = 0;
            int removed = 0;
            o.CollectionChanged += (s, a) =>
            {
                switch (a.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        added++;
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        removed++;
                        break;
                }
            };

            c1.Add("Item6");
            Assert.AreEqual(3, added);
            Assert.AreEqual(0, removed);

            c1.Remove("Item6");
            Assert.AreEqual(3, added);
            Assert.AreEqual(3, removed);
        }

    }

}
