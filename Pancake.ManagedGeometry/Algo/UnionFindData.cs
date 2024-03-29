﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pancake.ManagedGeometry.Algo
{
    /// <summary>
    /// Disjoint set implementation with ranks
    /// </summary>
    public class UnionFindData
    {
        private int[] _father;
        private int[] _rank;

        public UnionFindData(int length)
        {
            _father = new int[length];
            _rank = new int[length];
            InitArray();
        }

        private void InitArray()
        {
            for (var i = 0; i < _father.Length; i++)
            {
                _father[i] = i;
                _rank[i] = 1;
            }
        }

        public int Find(int x)
        {
            return x == _father[x] ? x : (_father[x] = Find(_father[x]));
        }

        public void Union(int i, int j)
        {
            var x = Find(i);
            var y = Find(j);

            if (_rank[x] <= _rank[y])
                _father[x] = y;
            else
                _father[y] = x;

            if (_rank[x] == _rank[y] && x != y)
                _rank[y]++;
        }

        /// <summary>
        /// Group data by binary judger. The implementation is based on disjoint set and its overall time complexity is close to O(n^2).
        /// </summary>
        /// <typeparam name="T">Type of input data</typeparam>
        /// <param name="data">Input data</param>
        /// <param name="judger">A function determines if two data should be unioned into one set. It should be commutative.</param>
        /// <returns>Grouped data</returns>
        public static IEnumerable<IEnumerable<T>> CategorizeData<T>(List<T> data, Func<T, T, bool> judger)
        {
            var cnt = data.Count;
            var union = new UnionFindData(cnt);

            for (var i = 0; i < cnt; i++)
                for (var j = i + 1; j < cnt; j++)
                    if (judger(data[i], data[j]))
                        union.Union(i, j);

            return data.Select((item, i) => new
            {
                Item = item,
                GroupIndex = union.Find(i)
            })
                .GroupBy(static it => it.GroupIndex)
                .Select(static grp => grp.Select(static it2 => it2.Item));
        }
    }
}
