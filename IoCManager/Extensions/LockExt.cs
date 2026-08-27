using JetBrains.Annotations;

namespace IoCManager.Extensions;

/// <summary>
/// Convenience utility for working with various read/write locks.
/// </summary>
public static class LockExt
{
	extension(ReaderWriterLockSlim rwLock)
	{
		/// <summary>
		/// Enter a read lock on a <see cref="ReaderWriterLockSlim"/>. Dispose the returned value to exit the read lock.
		/// </summary>
		/// <remarks>
		///	This is intended to be used with a <see langword="using"/> statement or block.
		/// </remarks>
		[MustUseReturnValue]
		public RWReadGuard ReadGuard()
		{
			rwLock.EnterReadLock();
			return new RWReadGuard(rwLock);
		}

		/// <summary>
		/// Enter a write lock on a <see cref="ReaderWriterLockSlim"/>. Dispose the returned value to exit the write lock.
		/// </summary>
		/// <remarks>
		///	This is intended to be used with a <see langword="using"/> statement or block.
		/// </remarks>
		[MustUseReturnValue]
		public RWWriteGuard WriteGuard()
		{
			rwLock.EnterWriteLock();
			return new RWWriteGuard(rwLock);
		}
	}

	extension(SemaphoreSlim semaphore)
	{
		/// <summary>
		/// Wait on a <see cref="SemaphoreSlim"/>. Dispose the returned value to release.
		/// </summary>
		/// <remarks>
		///	This is intended to be used with a <see langword="using"/> statement or block.
		/// </remarks>
		[MustUseReturnValue]
		public SemaphoreGuard WaitGuard()
		{
			semaphore.Wait();
			return new SemaphoreGuard(semaphore);
		}

		/// <summary>
		/// Wait on a <see cref="SemaphoreSlim"/> asynchronously. Dispose the returned value to release.
		/// </summary>
		/// <remarks>
		///	This is intended to be used with a <see langword="using"/> statement or block.
		/// </remarks>
		[MustUseReturnValue]
		public async ValueTask<SemaphoreGuard> WaitGuardAsync()
		{
			await semaphore.WaitAsync();
			return new SemaphoreGuard(semaphore);
		}
	}

	// ReSharper disable once InconsistentNaming
	public struct RWReadGuard : IDisposable
	{
		public readonly ReaderWriterLockSlim RwLock;
		public bool Disposed { get; private set; }

		public RWReadGuard(ReaderWriterLockSlim rwLock)
		{
			RwLock = rwLock;
			Disposed = false;
		}

		public void Dispose()
		{
			if (Disposed)
				throw new InvalidOperationException($"Double dispose of {nameof(RWReadGuard)}.");

			Disposed = true;
			RwLock.ExitReadLock();
		}
	}
	
	// ReSharper disable once InconsistentNaming
	public struct RWWriteGuard : IDisposable
	{
		public readonly ReaderWriterLockSlim RwLock;
		public bool Disposed { get; private set; }

		public RWWriteGuard(ReaderWriterLockSlim rwLock)
		{
			RwLock = rwLock;
			Disposed = false;
		}

		public void Dispose()
		{
			if (Disposed)
				throw new InvalidOperationException($"Double dispose of {nameof(RWWriteGuard)}.");

			Disposed = true;
			RwLock.ExitWriteLock();
		}
	}
	
	public struct SemaphoreGuard : IDisposable
	{
		public readonly SemaphoreSlim Semaphore;
		public bool Disposed { get; private set; }

		public SemaphoreGuard(SemaphoreSlim semaphore)
		{
			Semaphore = semaphore;
			Disposed = false;
		}

		public void Dispose()
		{
			if (Disposed)
				throw new InvalidOperationException($"Double dispose of {nameof(SemaphoreGuard)}.");

			Disposed = true;
			Semaphore.Release();
		}
	}
}