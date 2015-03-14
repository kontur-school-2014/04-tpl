using System;
using System.Collections.Generic;
using System.Threading;

namespace Kontur.Shpora.MT
{
	class MyThreadPool
	{
		private Thread[] workers;
		private Queue<Action> tasksQueue = new Queue<Action>();

		public MyThreadPool(int workerThreadsCount)
		{
			if(workerThreadsCount <= 0)
				throw new ArgumentException("Value should be positive", "workerThreadsCount");
			workers = new Thread[workerThreadsCount];
			for(var i = 0; i < workerThreadsCount; i++)
			{
				workers[i] = new Thread(Worker);
				workers[i].Start();
			}

		}

		public void QueueTask(Action task)
		{
			lock(tasksQueue)
			{
				tasksQueue.Enqueue(task);
				Monitor.Pulse(tasksQueue);
			}
		}

		private void Worker()
		{
			while(true)
			{
				Action task;
				lock(tasksQueue)
				{
					while(tasksQueue.Count == 0)
					{
						Monitor.Wait(tasksQueue);
					}
					task = tasksQueue.Dequeue();
				}

				try
				{
					task.Invoke();
				}
				catch(Exception e)
				{
					
				}
			}
		}
	}
}
