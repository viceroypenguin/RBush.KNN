using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RBush.KNN.Test
{
	public class Box : ISpatialData, IComparable<Box>
	{
		private Envelope _envelope;
		public ref readonly Envelope Envelope => ref _envelope;

		public double DistanceTo(double x, double y) =>
			DistanceTo(Envelope, x, y);

		private static double DistanceTo(in Envelope envelope, double x, double y)
		{
			var dX = AxisDistance(x, envelope.MinX, envelope.MaxX);
			var dY = AxisDistance(y, envelope.MinY, envelope.MaxY);
			return Math.Sqrt((dX * dX) + (dY * dY));
		}

		private static double AxisDistance(double k, double min, double max) =>
			k < min ? min - k :
			k > max ? k - max :
			0;

		public int Version { get; set; } = 0;

		public int CompareTo(Box other)
		{
			if (this.Envelope.MinX != other.Envelope.MinX)
				return this.Envelope.MinX.CompareTo(other.Envelope.MinX);
			if (this.Envelope.MinY != other.Envelope.MinY)
				return this.Envelope.MinY.CompareTo(other.Envelope.MinY);
			if (this.Envelope.MaxX != other.Envelope.MaxX)
				return this.Envelope.MaxX.CompareTo(other.Envelope.MaxX);
			if (this.Envelope.MaxY != other.Envelope.MaxY)
				return this.Envelope.MaxY.CompareTo(other.Envelope.MaxY);
			return 0;
		}

		public static Box[] CreateBoxes(double[,] data)
		{
			return Enumerable.Range(0, data.GetLength(0))
				.Select(i => new Box
				{
					_envelope = new Envelope
					(
						minX: data[i, 0],
						minY: data[i, 1],
						maxX: data[i, 2],
						maxY: data[i, 3]
					),
					Version = i + 1,
				})
				.ToArray();
		}

		public static Box CreateBox(double[] data) =>
			new Box
			{
				_envelope = new Envelope(
					minX: data[0],
					minY: data[1],
					maxX: data[2],
					maxY: data[3])
			};
	}
}
