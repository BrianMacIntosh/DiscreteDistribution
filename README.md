# DiscreteDistribution
A single-file C# discrete distribution class. This class allows you to aggregate samples into integer buckets and perform analysis on the set of samples.

# API

## Constructors

### DiscreteDistribution(List<int> samples)
Constructs a distribution from a set of samples.

### DiscreteDistribution(int min, int max)
Constructs an empty distribution with preallocated space.

The class also supports collection initialization.

## Other Important Members

### int Min
The value of the smallest sample in the distribution.

### int Max
The value of the largest sample in the distribution.

### this[int key]
Gets or sets the number of samples in the specified bucket.

### Add(DiscreteDistribution other)
Adds all the samples in the `other` distribution to this one.

### Add(int sample)
Adds a sample to the distribution.

### Add(int sample, int quantity)
Adds a quantity of the same sample to the distribution.

### Remove(int sample)
Removes a sample from the distribution.

### double GetMean()
Calculates the mean of the distribution.

### double GetStandardDeviation()
Calculates the standard deviation of the distribution.

### int GetSampleCount()
Returns the total number of samples in the distribution.

### string GetHorizontalBarGraph(int maxWidth, string keyFormat)
Produces an ASCII bar graph of the distribution.
* `maxWidth`: The maximum width of the bars. 0 uses the raw value.
* `keyFormat`: Format specifier to use to format the key values.
