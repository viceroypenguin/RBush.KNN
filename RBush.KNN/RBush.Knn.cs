using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RBush
{
	public static class RBushExtensions
	{
		public static IReadOnlyList<T> Knn<T>(
			this RBush<T> tree,
			int k,
			double x,
			double y,
			double? maxDistance = null,
			Func<T, bool> predicate = null)
			where T : ISpatialData
		{
			var items = maxDistance == null
				? tree.Search()
				: tree.Search(
					new Envelope(
						minX: x - maxDistance.Value,
						minY: y - maxDistance.Value,
						maxX: x + maxDistance.Value,
						maxY: y + maxDistance.Value));

			var distances = items
				.Select(i => new { i, dist = i.Envelope.DistanceTo(x, y), })
				.OrderBy(i => i.dist)
				.AsEnumerable();

			if (maxDistance.HasValue)
				distances = distances.TakeWhile(i => i.dist <= maxDistance.Value);

			if (predicate != null)
				distances = distances.Where(i => predicate(i.i));

			if (k > 0)
				distances = distances.Take(k);

			return distances
				.Select(i => i.i)
				.ToList();
		}

		private static double DistanceTo(this in Envelope envelope, double x, double y)
		{
			var dX = AxisDistance(x, envelope.MinX, envelope.MaxX);
			var dY = AxisDistance(y, envelope.MinY, envelope.MaxY);
			return Math.Sqrt((dX * dX) + (dY * dY));
		}

		private static double AxisDistance(double k, double min, double max) =>
			k < min ? min - k :
			k > max ? k - max :
			0;
	}
}
