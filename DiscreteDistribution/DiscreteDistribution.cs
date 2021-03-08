using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Represents a distribution of discrete integer values.
/// </summary>
public class DiscreteDistribution : IEquatable<DiscreteDistribution>, IEnumerable<int>
{
	/// <summary>
	/// The buckets making up the distribution.
	/// </summary>
	private int[] m_buckets;

	/// <summary>
	/// The value corresponding to the first index in the buckets array.
	/// For example, if the distribution went from -3 to 2, this would be -3.
	/// </summary>
	private int m_firstValue;

	/// <summary>
	/// Returns the minimum bucketed value in the distribution.
	/// </summary>
	public int Min
	{
		get
		{
			for (int i = 0; i < m_buckets.Length; i++)
			{
				if (m_buckets[i] > 0)
				{
					return i + m_firstValue;
				}
			}
			throw new InvalidOperationException("Distribution is empty.");
		}
	}

	/// <summary>
	/// Returns the maximum bucketed value in the distribution.
	/// </summary>
	public int Max
	{
		get
		{
			for (int i = m_buckets.Length - 1; i >= 0; i--)
			{
				if (m_buckets[i] > 0)
				{
					return i + m_firstValue;
				}
			}
			throw new InvalidOperationException("Distribution is empty.");
		}
	}

	/// <summary>
	/// Returns the minimum bucket in the allocated space.
	/// </summary>
	public int AllocatedMin
	{
		get { return m_firstValue; }
	}

	/// <summary>
	/// Return the maximum bucket in the allocated space.
	/// </summary>
	public int AllocatedMax
	{
		get { return m_firstValue + m_buckets.Length - 1; }
	}

	/// <summary>
	/// Returns true if there are no samples in the distribution.
	/// </summary>
	public bool IsEmpty
	{
		get
		{
			foreach (int value in m_buckets)
			{
				if (value != 0)
				{
					return false;
				}
			}
			return true;
		}
	}

	/// <summary>
	/// Gets or sets the number of samples in the specified bucket.
	/// </summary>
	public int this[int key]
	{
		get
		{
			return key >= m_firstValue && key < m_firstValue + m_buckets.Length
				? m_buckets[key - m_firstValue]
				: 0;
		}
		set
		{
			EnsureCapacity(key);
			m_buckets[key - m_firstValue] = value;
		}
	}

	public DiscreteDistribution()
	{
		m_buckets = new int[0];
		m_firstValue = 0;
	}

	/// <summary>
	/// Constructs a distribution from a set of samples.
	/// </summary>
	public DiscreteDistribution(List<int> samples)
	{
		if (samples == null)
		{
			throw new ArgumentNullException("samples");
		}
		
		m_firstValue = samples.Min();
		int lastIndex = samples.Max();
		m_buckets = new int[lastIndex - m_firstValue + 1];
		foreach (int sample in samples)
		{
			m_buckets[sample - m_firstValue]++;
		}
	}

	/// <summary>
	/// Constructs an empty distribution with buckets in the specified range.
	/// </summary>
	/// <remarks>Distributions resize themselves, this is primarily for optimization.</remarks>
	public DiscreteDistribution(int min, int max)
	{
		if (min > max)
		{
			throw new ArgumentException("'min' cannot be greater than 'max'");
		}
		m_firstValue = min;
		m_buckets = new int[max - min + 1];
	}

	/// <summary>
	/// Merges another distribution into this one.
	/// </summary>
	public void Add(DiscreteDistribution other)
	{
		int otherMin = other.Min;
		int otherMax = other.Max;
		EnsureCapacity(otherMin);
		EnsureCapacity(otherMax); //OPT: might reallocate twice
		for (int i = otherMin; i <= otherMax; i++)
		{
			this[i] += other[i];
		}
	}
	
	/// <summary>
	/// Adds a sample to the distribution.
	/// </summary>
	public void Add(int sample)
	{
		Add(sample, 1);
	}

	/// <summary>
	/// Adds a quantity of the same sample to the distribution.
	/// </summary>
	public void Add(int sample, int quantity)
	{
		EnsureCapacity(sample);
		m_buckets[sample - m_firstValue] += quantity;
	}

	/// <summary>
	/// Ensures that the distribution's storage can accomodate the specified bucket.
	/// </summary>
	private void EnsureCapacity(int forBucket)
	{
		if (forBucket < m_firstValue)
		{
			int increase = m_firstValue - forBucket;
			int[] newArray = new int[m_buckets.Length + increase];
			Array.Copy(m_buckets, 0, newArray, increase, m_buckets.Length);
			m_buckets = newArray;
			m_firstValue = forBucket;
		}
		else if (forBucket >= m_firstValue + m_buckets.Length)
		{
			Array.Resize(ref m_buckets, forBucket - m_firstValue + 1);
		}
	}

	/// <summary>
	/// Removes a sample from the distribution.
	/// </summary>
	/// <param name="resize">If set, reduces the size of the backing storage if able.</param>
	public void Remove(int sample, bool resize = false)
	{
		if (sample < m_firstValue
			|| sample >= m_firstValue + m_buckets.Length
			|| m_buckets[sample - m_firstValue] <= 0)
		{
			throw new InvalidOperationException(string.Format("No samples of value '{0}' exist.", sample));
		}
		if (--m_buckets[sample - m_firstValue] <= 0)
		{
			// the value was entirely removed; check to resize
			if (resize)
			{
				throw new NotImplementedException();
			}
		}
	}

	/// <summary>
	/// Trims unused backing capacity.
	/// </summary>
	public void Trim()
	{
		if (IsEmpty)
		{
			m_buckets = new int[0];
			m_firstValue = 0;
			return;
		}

		int min = Min;
		int max = Max;
		int trimFront = min - m_firstValue;
		int trimBack = m_buckets.Length - (max - m_firstValue) - 1;
		if (trimFront > 0 || trimBack > 0)
		{
			int[] newArray = new int[max - min + 1];
			for (int i = min; i <= max; i++)
			{
				newArray[i - min] = m_buckets[i - m_firstValue];
			}
			m_buckets = newArray;
			m_firstValue = min;
		}
	}

	/// <summary>
	/// Calculates the mean of the distribution.
	/// </summary>
	public double GetMean()
	{
		GetMeanAndSampleCount(out double mean, out int sampleCount);
		return mean;
	}

	/// <summary>
	/// Calculates the mean of the distribution and the total number of samples.
	/// </summary>
	/// <remarks>Faster than getting both individually.</remarks>
	public void GetMeanAndSampleCount(out double mean, out int sampleCount)
	{
		double totalValue = 0.0;
		sampleCount = 0;
		for (int i = 0; i < m_buckets.Length; i++)
		{
			sampleCount += m_buckets[i];
			totalValue += m_buckets[i] * (i + m_firstValue);
		}
		mean = totalValue / sampleCount;
	}

	/// <summary>
	/// Calculates the standard deviation of the distribution.
	/// </summary>
	public double GetStandardDeviation()
	{
		GetMeanAndStandardDeviation(out double mean, out double stdDev);
		return stdDev;
	}

	/// <summary>
	/// Calculates the mean and standard deviation of the distribution.
	/// </summary>
	/// <remarks>Faster than getting both individually.</remarks>
	public void GetMeanAndStandardDeviation(out double mean, out double stdDev)
	{
		GetMeanAndSampleCount(out mean, out int volume);

		// sum squared differences
		double sumSquared = 0.0;
		for (int i = 0; i < m_buckets.Length; i++)
		{
			int value = i + m_firstValue;
			double difference = value - mean;
			sumSquared += m_buckets[i] * difference * difference;
		}

		stdDev = Math.Sqrt(sumSquared / volume);
	}

	/// <summary>
	/// Calculates the total number of samples.
	/// </summary>
	public int GetSampleCount()
	{
		return m_buckets.Sum();
	}

	/// <summary>
	/// Calculates the maximum quantity in any one bucket.
	/// </summary>
	public int GetMaxHeight()
	{
		return m_buckets.Max();
	}

	public override bool Equals(object obj)
	{
		DiscreteDistribution otherDist = obj as DiscreteDistribution;
		if (otherDist != null)
		{
			return Equals(otherDist);
		}
		else
		{
			return false;
		}
	}

	public override int GetHashCode()
	{
		int hashCode = m_firstValue;
		foreach (int value in m_buckets)
		{
			hashCode = unchecked(hashCode * 31 + value);
		}
		return hashCode;
	}

	public bool Equals(DiscreteDistribution other)
	{
		int minMin = Math.Min(Min, other.Min);
		int maxMax = Math.Max(Max, other.Max);
		for (int i = minMin; i <= maxMax; i++)
		{
			if (this[i] != other[i])
			{
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// Produces an ASCII horizontal bar graph of the distribution.
	/// </summary>
	/// <param name="maxWidth">The max width of the bars. 0 uses the native value.</param>
	/// <param name="keyFormat">Format specifier to use to format the key values.</param>
	public string GetHorizontalBarGraph(int maxWidth = 0, string keyFormat = "D")
	{
		// find max key length
		int maxKeyLen = Math.Max(Min.ToString(keyFormat).Length, Max.ToString(keyFormat).Length);
		string keyFormat2 = "{0," + maxKeyLen.ToString() + "}";

		double barScale = maxWidth > 0 ? maxWidth / GetMaxHeight() : 1.0;

		//OPT: pool builders
		StringBuilder builder = new StringBuilder();
		for (int i = 0; i < m_buckets.Length; i++)
		{
			if (i > 0)
			{
				builder.AppendLine();
			}
			int value = i - m_firstValue;
			builder.AppendFormat(keyFormat2, value.ToString(keyFormat));
			builder.Append('|');
			int barSize = (int)Math.Ceiling(m_buckets[i] * barScale);
			for (int c = 0; c < barSize; c++)
			{
				builder.Append('X');
			}
		}
		return builder.ToString();
	}

	public IEnumerator<int> GetEnumerator()
	{
		return new SampleEnumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new SampleEnumerator(this);
	}

	/// <summary>
	/// Enumerates each sample in the distribution.
	/// </summary>
	/// <remarks>Can handle distribution being modified as long as MoveNext is called after the change.</remarks>
	public struct SampleEnumerator : IEnumerator, IEnumerator<int>
	{
		private DiscreteDistribution m_distribution;

		/// <summary>
		/// The current bucket being iterated.
		/// </summary>
		private int? m_currentBucket;

		/// <summary>
		/// Number of samples in the current bucket that have been iterated.
		/// </summary>
		private int m_currentSamples;

		public SampleEnumerator(DiscreteDistribution distribution)
		{
			m_distribution = distribution;
			m_currentBucket = null;
			m_currentSamples = 0;
		}

		public int Current
		{
			get
			{
				if (!m_currentBucket.HasValue)
				{
					throw new InvalidOperationException("SampleEnumerator has not been started with MoveNext.");
				}
				return m_currentBucket.Value;
			}
		}

		object IEnumerator.Current => Current;

		public void Dispose()
		{
			
		}

		public bool MoveNext()
		{
			if (!m_currentBucket.HasValue)
			{
				m_currentBucket = m_distribution.Min;
				m_currentSamples = 0;
				return true;
			}
			else
			{
				int distMax = m_distribution.Max;
				m_currentSamples++;
				while (m_currentSamples >= m_distribution[m_currentBucket.Value])
				{
					if (m_currentBucket < distMax)
					{
						m_currentBucket++;
						m_currentSamples = 0;
					}
					else
					{
						return false;
					}
				}
				return true;
			}
		}

		public void Reset()
		{
			m_currentBucket = null;
			m_currentSamples = 0;
		}
	}
}
