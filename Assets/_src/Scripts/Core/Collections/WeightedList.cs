using System;
using System.Collections.Generic;
using System.Linq;

using Random = System.Random;

namespace _src.Scripts.Core.Collections
{
    /// <summary>
    /// The more weight an object has, the more chance it gets to spawn
    /// </summary>
    /// <typeparam name="T">Generic Typed</typeparam>
    public class WeightedList<T>
    {
        [Serializable]
        private struct Element {
            public T obj;
            public double weight;
            public Element(T obj, double weight = 0f) {
                this.obj = obj;
                this.weight = weight;
            }
        }

        private List<Element> _elements = new List<Element>();
        private double _sumWeight;
        private Random _rand = new Random();

        public T GetRandomItem() {
            double randWeight = _rand.NextDouble() * _sumWeight;
            return _elements.FirstOrDefault(x => x.weight >= randWeight).obj;
        }

        public void AddElement(T element, double weight = 0f) {
            _sumWeight += weight;
            _elements.Add(new Element(element, _sumWeight));   
        }
        public void Remove(T element) => _elements.Remove(_elements.FirstOrDefault(x => x.obj.Equals(element)));
        public void Clear() => _elements.Clear();
    }
}