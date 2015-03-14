using System;
using System.Threading;
using NUnit.Framework;

namespace Kontur.Shpora.MT
{
	[TestFixture]
	class ThreadPoolTest
	{
		[Test]
		public static void TestCorrectness()
		{
			const int TasksCount = 100500;
			var count = 0;

			var tp = new MyThreadPool(Environment.ProcessorCount);
			for(var i = 0; i < TasksCount; i++)
			{
				tp.QueueTask(() => Interlocked.Increment(ref count));
			}
			Thread.Sleep(1000);
			Assert.AreEqual(TasksCount, count);
		}

		[Test]
		public static void TestSingleThread()
		{
			var count = 0;

			var tp = new MyThreadPool(1);
			tp.QueueTask(() =>
			{
				Thread.Sleep(1000);
				Interlocked.Increment(ref count);
			});
			tp.QueueTask(() =>
			{
				Thread.Sleep(1000);
				Interlocked.Increment(ref count);
			});
			Thread.Sleep(1500);
			Assert.AreEqual(1, count);
			Thread.Sleep(1500);
			Assert.AreEqual(2, count);
		}

		[Test]
		public static void TestMultiThread()
		{
			var count = 0;

			var tp = new MyThreadPool(2);
			tp.QueueTask(() =>
			{
				Thread.Sleep(1000);
				Interlocked.Increment(ref count);
			});
			tp.QueueTask(() =>
			{
				Thread.Sleep(1000);
				Interlocked.Increment(ref count);
			});
			Thread.Sleep(1500);
			Assert.AreEqual(2, count);
		}
	}
}
