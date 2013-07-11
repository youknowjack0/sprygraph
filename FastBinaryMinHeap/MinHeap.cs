/*
 * Copyright (c) 2013, Alastri Software Pty. Ltd.
 * Author: Jack Langman <youknowjack AT gmail.com>
 * All rights reserved.
 * 
 * Based on algorithms in  Cormen, Leiserson, Rivest & Stein, Introduction to 
 * Algorithms Third edition (MIT Press, 2009)  
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met: 
 * 
 * 1. Redistributions of source code must retain the above copyright notice, this
 *    list of conditions and the following disclaimer. 
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */


using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Alastri.DataStructures
{
    /// <summary>
    /// A minimalistic keyed binary minheap with natural double key comparison only
    /// 
    /// not particularly useful as a general purpose data structure
    /// 
    /// Duplicate keys are allowed
    /// 
    /// adding an item returns an integer index which can be used to retrieve or decrease-key 
    /// (even if the position has changed)
    /// To do this it keeps a list of all items ever inserted into the heap.
    /// </summary>
    public sealed class MinHeap<TValue>
    {
        private struct Entry
        {
            public Entry(double key, TValue value, int index)
            {
                Key = key;
                Value = value;
                Index = index;
            }

            public double Key;
            public TValue Value;
            public int Index;

            public Entry(TValue value, int i)
            {
                Value = value;
                Index = i;
                Key = double.MaxValue;
            }
        }

        private Entry[] _heap;
        private int _count;
        private int _index = 0;
        private int[] _indices;

        public MinHeap(int initSize = 4)
        {
            _heap = new Entry[initSize];
            _indices = new int[initSize];
        }

        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return _count;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public KeyValuePair<double, TValue> Minimum()
        {
            return new KeyValuePair<double, TValue>(_heap[0].Key, _heap[0].Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double MinimumKey()
        {
            return _heap[0].Key;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TValue MinimumValue()
        {
            return _heap[0].Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public KeyValuePair<double, TValue> RemoveMinimum()
        {
            if (_count == 0)
                throw new InvalidOperationException("heap contains no elements");

            KeyValuePair<double, TValue> min = Minimum();

#if DEBUG
            _indices[_heap[0].Index] = -1;
#endif

            if (_count == 1)
            {
                _count = 0;
            }
            else
            {
                Move(--_count, 0);
                MinHeapify(0);
            }
            return min;
        }

        //move and overwrite
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Move(int from, int to)
        {
            _heap[to] = _heap[from];
            _indices[_heap[from].Index] = to;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int Parent(int heapIndex)
        {
            return heapIndex/2;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int Left(int heapIndex)
        {
            return 2*heapIndex;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int Right(int heapIndex)
        {
            return 2*heapIndex + 1;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]        
        private void MinHeapify(int i)
        {
            for (; ; )
            {
                int l = Left(i);
                int r = Right(i);
                int smallest;
                if (l < _count && _heap[l].Key < _heap[i].Key)
                    smallest = l;
                else
                    smallest = i;

                if (r < _count && _heap[r].Key < _heap[smallest].Key)
                    smallest = r;

                if (smallest == i)
                    return;
                
                Swap(i, smallest);
                i = smallest;
            } 
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Swap(int x, int y)
        {
            var t = _heap[x];
            _heap[x] = _heap[y];
            _heap[y] = t;
            _indices[t.Index] = y;
            _indices[_heap[x].Index] = x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DecreaseKey(int index, double newKey)
        {
            int hi = _indices[index];
            DecreateKeyInternal(newKey, hi);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DecreateKeyInternal(double newKey, int heapIndex)
        {            
            if (newKey > _heap[heapIndex].Key)
                throw new InvalidOperationException("new key is higher than old key");
            int parent;
            _heap[heapIndex].Key = newKey;
            while (heapIndex > 0 && _heap[(parent = Parent(heapIndex))].Key > _heap[heapIndex].Key)
            {
                Swap(heapIndex, parent);
                heapIndex = parent;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Add(double key, TValue value)
        {
            int index;
            int heapIndex;
            if(_count == _heap.Length)
                ExpandHeap();
            if (_index == _indices.Length)
                ExpandIndices();
            heapIndex = _count++;
            index = _index++;
            _heap[heapIndex] = new Entry(value, index);
            _indices[index] = heapIndex;
            DecreateKeyInternal(key, heapIndex);
            return index;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Add(KeyValuePair<double, TValue> kvp)
        {
            return Add(kvp.Key, kvp.Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ExpandHeap()
        {
            if(_heap.Length == 0)
                Array.Resize(ref _heap, 4);
            else 
                Array.Resize(ref _heap, _heap.Length * 2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ExpandIndices()
        {
            if (_indices.Length == 0)
                Array.Resize(ref _indices, 4);
            else
                Array.Resize(ref _indices, _indices.Length * 2);
        }

    }
}
