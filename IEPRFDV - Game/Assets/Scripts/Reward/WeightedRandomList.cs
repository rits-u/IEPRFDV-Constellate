using System.Collections.Generic;
using UnityEngine;

//public enum LootType{
//    WEAPON,
//    GEAR,
//    HEALTH
//}

[System.Serializable]
public class WeightedRandomList<T>
{
    [System.Serializable]
    public struct Pair
    {
        public T item;
        public float weight;
        public LootType type;

        public Pair(T item, float weight, LootType type)
        {
            this.item = item;
            this.weight = weight;
            this.type = type;
        }
    }

    public List<Pair> list = new List<Pair>();

    public int Count
    {
        get => list.Count;
    }

    public void Add(T item, float weight, LootType type)
    {
        list.Add(new Pair(item, weight, type));
    }

    //public T GetRandom()
    //{
    //    float totalWeight = 0;

    //    foreach (Pair p in list)
    //    {
    //        totalWeight += p.weight;
    //    }

    //    float value = Random.value * totalWeight;

    //    float sumWeight = 0;

    //    foreach (Pair p in list)
    //    {
    //        sumWeight += p.weight;

    //        if (sumWeight >= value)
    //        {
    //            return p.item;
    //        }
    //    }

    //    return default(T);
    //}

    public Pair GetRandom()
    {
        float totalWeight = 0;

        foreach (Pair p in list)
            totalWeight += p.weight;

        float value = Random.value * totalWeight;

        float sumWeight = 0;

        foreach (Pair p in list)
        {
            sumWeight += p.weight;
            if (sumWeight >= value)
                return p;
        }

        return default;
    }

    public bool Remove(Pair pair)
    {
        return list.Remove(pair);
    }
}