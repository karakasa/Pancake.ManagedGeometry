using BenchmarkDotNet.Attributes;
using Pancake.ManagedGeometry;
using Pancake.ManagedGeometry.Unsafe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossLibBenchmark.UnsafeOperation
{
    [MemoryDiagnoser]
    public class Conversion
    {
        public struct SomeCustomCoordType
        {
            public double X;
            public double Y;
            public double Z;
        }
        const int POINT_COUNT = 100_000;

        public Coord[] _ptGrp;
        public SomeCustomCoordType[] _ptGrp2;

        private static Coord[] CreateGroup(Random random)
        {
            return Enumerable.Repeat(0, POINT_COUNT).Select(_ => (Coord)(
            random.NextDouble(),
            random.NextDouble(),
            random.NextDouble()
            )).ToArray();
        }
        [GlobalSetup]
        public void Setup()
        {
            _ptGrp = CreateGroup(new Random(42));

            var array = _ptGrp2 = new SomeCustomCoordType[POINT_COUNT];

            for (var i = 0; i < _ptGrp.Length; i++)
                array[i] = new SomeCustomCoordType()
                {
                    X = _ptGrp[i].X,
                    Y = _ptGrp[i].Y,
                    Z = _ptGrp[i].Z,
                };
        }
        [Benchmark]
        public double SafeConversion()
        {
            var array = new SomeCustomCoordType[_ptGrp.Length];
            for (var i = 0; i < _ptGrp.Length; i++)
                array[i] = new SomeCustomCoordType()
                {
                    X = _ptGrp[i].X,
                    Y = _ptGrp[i].Y,
                    Z = _ptGrp[i].Z,
                };

            var sum = 0.0;

            foreach(var it in array)
            {
                sum += it.X;
                sum += it.Y;
                sum += it.Z;
            }

            return sum;
        }
        [Benchmark]
        public object PureAlloc()
        {
            var array = new SomeCustomCoordType[_ptGrp.Length];
            return array;
        }


        [Benchmark]
        public double UnsafeConversion()
        {
            var array = RawConversion.ReinterpretArray<Coord, SomeCustomCoordType>(_ptGrp);

            var sum = 0.0;

            foreach (var it in array)
            {
                sum += it.X;
                sum += it.Y;
                sum += it.Z;
            }

            return sum;
        }

        [Benchmark]
        public double NoConversion()
        {
            var array = _ptGrp2;

            var sum = 0.0;

            foreach (var it in array)
            {
                sum += it.X;
                sum += it.Y;
                sum += it.Z;
            }

            return sum;
        }
    }
}
