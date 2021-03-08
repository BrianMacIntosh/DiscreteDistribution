using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace DiscreteDistributionTests
{
	[TestClass]
	public class UnitTests
	{
		[TestMethod]
		public void AddRemoveTests()
		{
			DiscreteDistribution testDistribution = new DiscreteDistribution();
			testDistribution.Add(-1);

			Assert.AreEqual(-1, testDistribution.Min);
			Assert.AreEqual(-1, testDistribution.Max);
			Assert.AreEqual(-1, testDistribution.AllocatedMin);
			Assert.AreEqual(-1, testDistribution.AllocatedMax);

			testDistribution.Add(1);

			Assert.AreEqual(-1, testDistribution.Min);
			Assert.AreEqual(1, testDistribution.Max);
			Assert.AreEqual(-1, testDistribution.AllocatedMin);
			Assert.AreEqual(1, testDistribution.AllocatedMax);

			testDistribution.Remove(-1);

			Assert.AreEqual(1, testDistribution.Min);
			Assert.AreEqual(1, testDistribution.Max);
			Assert.AreEqual(-1, testDistribution.AllocatedMin);
			Assert.AreEqual(1, testDistribution.AllocatedMax);
		}

		[TestMethod]
		public void ConstructionTests()
		{
			DiscreteDistribution testDistribution1 = new DiscreteDistribution(-5, 10);

			Assert.AreEqual(-5, testDistribution1.AllocatedMin);
			Assert.AreEqual(10, testDistribution1.AllocatedMax);

			testDistribution1.Add(-1);
			testDistribution1.Add(3);
			testDistribution1.Add(3);
			testDistribution1.Add(5);

			Assert.AreEqual(-1, testDistribution1.Min);
			Assert.AreEqual(5, testDistribution1.Max);
			Assert.AreEqual(-5, testDistribution1.AllocatedMin);
			Assert.AreEqual(10, testDistribution1.AllocatedMax);

			List<int> samples = new List<int>() { -1, 3, 3, 5 };
			DiscreteDistribution testDistribution2 = new DiscreteDistribution(samples);

			Assert.AreEqual(-1, testDistribution2.Min);
			Assert.AreEqual(5, testDistribution2.Max);
			Assert.AreEqual(-1, testDistribution2.AllocatedMin);
			Assert.AreEqual(5, testDistribution2.AllocatedMax);
		}

		[TestMethod]
		public void IndexingTests()
		{
			DiscreteDistribution testDistribution = new DiscreteDistribution()
				{ -1, 1, 1, 2 };

			Assert.AreEqual(0, testDistribution[-1000]);
			Assert.AreEqual(0, testDistribution[-2]);
			Assert.AreEqual(1, testDistribution[-1]);
			Assert.AreEqual(0, testDistribution[0]);
			Assert.AreEqual(2, testDistribution[1]);
			Assert.AreEqual(1, testDistribution[2]);
			Assert.AreEqual(0, testDistribution[3]);
			Assert.AreEqual(0, testDistribution[1000]);

			testDistribution[1]--;

			Assert.AreEqual(0, testDistribution[-1000]);
			Assert.AreEqual(0, testDistribution[-2]);
			Assert.AreEqual(1, testDistribution[-1]);
			Assert.AreEqual(0, testDistribution[0]);
			Assert.AreEqual(1, testDistribution[1]);
			Assert.AreEqual(1, testDistribution[2]);
			Assert.AreEqual(0, testDistribution[3]);
			Assert.AreEqual(0, testDistribution[1000]);

			testDistribution[0] = 5;

			Assert.AreEqual(0, testDistribution[-1000]);
			Assert.AreEqual(0, testDistribution[-2]);
			Assert.AreEqual(1, testDistribution[-1]);
			Assert.AreEqual(5, testDistribution[0]);
			Assert.AreEqual(1, testDistribution[1]);
			Assert.AreEqual(1, testDistribution[2]);
			Assert.AreEqual(0, testDistribution[3]);
			Assert.AreEqual(0, testDistribution[1000]);
		}

		[TestMethod]
		public void AddMergeTests()
		{
			DiscreteDistribution testDistribution1 = new DiscreteDistribution()
				{ -1, 2, 2 };
			DiscreteDistribution testDistribution2 = new DiscreteDistribution()
				{ -5, -1, 2 };
			testDistribution1.Add(testDistribution2);

			Assert.AreEqual(1, testDistribution1[-5]);
			Assert.AreEqual(0, testDistribution1[-2]);
			Assert.AreEqual(2, testDistribution1[-1]);
			Assert.AreEqual(3, testDistribution1[2]);
			Assert.AreEqual(0, testDistribution1[3]);

			testDistribution1 = new DiscreteDistribution()
				{ -1, 2, 2 };
			testDistribution2 = new DiscreteDistribution()
				{ -1, 2, 5 };
			testDistribution1.Add(testDistribution2);

			Assert.AreEqual(0, testDistribution1[-2]);
			Assert.AreEqual(2, testDistribution1[-1]);
			Assert.AreEqual(3, testDistribution1[2]);
			Assert.AreEqual(0, testDistribution1[3]);
			Assert.AreEqual(1, testDistribution1[5]);

			testDistribution1 = new DiscreteDistribution()
				{ -1, 2, 2 };
			testDistribution1.Add(testDistribution1);

			Assert.AreEqual(0, testDistribution1[-2]);
			Assert.AreEqual(2, testDistribution1[-1]);
			Assert.AreEqual(4, testDistribution1[2]);
			Assert.AreEqual(0, testDistribution1[3]);
		}

		[TestMethod]
		public void TrimTests()
		{
			DiscreteDistribution testDistribution = new DiscreteDistribution();
			testDistribution.Trim();

			testDistribution = new DiscreteDistribution()
				{ -1, 2, 2 };
			testDistribution.Remove(-1);
			Assert.AreEqual(-1, testDistribution.AllocatedMin);
			Assert.AreEqual(2, testDistribution.AllocatedMax);
			testDistribution.Trim();
			Assert.AreEqual(2, testDistribution.AllocatedMin);
			Assert.AreEqual(2, testDistribution.AllocatedMax);

			testDistribution = new DiscreteDistribution()
				{ -1, -1, 2 };
			testDistribution.Remove(2);
			Assert.AreEqual(-1, testDistribution.AllocatedMin);
			Assert.AreEqual(2, testDistribution.AllocatedMax);
			testDistribution.Trim();
			Assert.AreEqual(-1, testDistribution.AllocatedMin);
			Assert.AreEqual(-1, testDistribution.AllocatedMax);
		}

		[TestMethod]
		public void StatsTests()
		{
			DiscreteDistribution testDistribution = new DiscreteDistribution()
				{ -1, 1 };
			Assert.AreEqual(0, testDistribution.GetMean());
			Assert.AreEqual(1, testDistribution.GetStandardDeviation());
			Assert.AreEqual(2, testDistribution.GetSampleCount());
			Assert.AreEqual(1, testDistribution.GetMaxHeight());

			testDistribution = new DiscreteDistribution()
				{ 1, 2, 4, 5 };
			Assert.AreEqual(3, testDistribution.GetMean());
			Assert.AreEqual(Math.Sqrt(2.5), testDistribution.GetStandardDeviation());
			Assert.AreEqual(4, testDistribution.GetSampleCount());
			Assert.AreEqual(1, testDistribution.GetMaxHeight());

			testDistribution = new DiscreteDistribution()
				{ 1, 2, 5, 5, 5 };
			Assert.AreEqual(18.0 / 5.0, testDistribution.GetMean());
			//Assert.AreEqual(0, testDistribution.GetStandardDeviation());
			Assert.AreEqual(5, testDistribution.GetSampleCount());
			Assert.AreEqual(3, testDistribution.GetMaxHeight());
		}

		[TestMethod]
		public void EqualityTests()
		{
			DiscreteDistribution testDistribution1 = new DiscreteDistribution()
				{ -1, 2, 2 };
			DiscreteDistribution testDistribution2 = new DiscreteDistribution()
				{ -5, -1, 2 };
			Assert.IsFalse(testDistribution1.Equals(testDistribution2));

			testDistribution1 = new DiscreteDistribution()
				{ -1, 2, 2 };
			testDistribution2 = new DiscreteDistribution()
				{ -1, 2, 2 };
			Assert.IsTrue(testDistribution1.Equals(testDistribution2));

			testDistribution1 = new DiscreteDistribution()
				{ -1, 2, 2 };
			testDistribution2 = new DiscreteDistribution()
				{ -1, 2, 2, 3 };
			testDistribution2.Remove(3);
			Assert.IsTrue(testDistribution1.Equals(testDistribution2));

			testDistribution1 = new DiscreteDistribution()
				{ -1, 2, 2 };
			testDistribution2 = new DiscreteDistribution()
				{ -1, 2, 2, -2 };
			testDistribution2.Remove(-2);
			Assert.IsTrue(testDistribution1.Equals(testDistribution2));

			testDistribution1 = new DiscreteDistribution()
				{ -1, 2, 2 };
			testDistribution2 = new DiscreteDistribution()
				{ -1, 2, 2 };
			testDistribution2.Add(3);
			Assert.IsFalse(testDistribution1.Equals(testDistribution2));
		}

		[TestMethod]
		public void EnumerationTests()
		{
			DiscreteDistribution testDistribution = new DiscreteDistribution()
				{ -1, 2, 2 };
			var enumerator = testDistribution.GetEnumerator();
			Assert.IsTrue(enumerator.MoveNext());
			Assert.AreEqual(-1, enumerator.Current);
			Assert.IsTrue(enumerator.MoveNext());
			Assert.AreEqual(2, enumerator.Current);
			Assert.IsTrue(enumerator.MoveNext());
			Assert.AreEqual(2, enumerator.Current);
			Assert.IsFalse(enumerator.MoveNext());
			enumerator.Reset();
			Assert.IsTrue(enumerator.MoveNext());
			Assert.AreEqual(-1, enumerator.Current);
			Assert.IsTrue(enumerator.MoveNext());
			Assert.AreEqual(2, enumerator.Current);
			Assert.IsTrue(enumerator.MoveNext());
			Assert.AreEqual(2, enumerator.Current);
			Assert.IsFalse(enumerator.MoveNext());

			testDistribution = new DiscreteDistribution()
				{ -1, 2, 4, 5 };
			testDistribution.Remove(-1);
			enumerator = testDistribution.GetEnumerator();
			Assert.IsTrue(enumerator.MoveNext());
			Assert.AreEqual(2, enumerator.Current);
			Assert.IsTrue(enumerator.MoveNext());
			Assert.AreEqual(4, enumerator.Current);
			Assert.IsTrue(enumerator.MoveNext());
			Assert.AreEqual(5, enumerator.Current);
			Assert.IsFalse(enumerator.MoveNext());
		}
	}
}
