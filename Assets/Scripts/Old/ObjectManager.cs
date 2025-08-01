using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Instance;
    private List<BoulderScript> boulders = new List<BoulderScript>();
    private List<CrateScript> crates = new List<CrateScript>();
    private List<HoleScript> holes = new List<HoleScript>();

    void Awake()
    {
        Instance = this;
    }

    public void RegisterBoulder(BoulderScript b)
    {
        boulders.Add(b);
    }

    public void UnregisterBoulder(BoulderScript b)
    {
        boulders.Remove(b);
    }

    public List<BoulderScript> GetAllBoulders()
    {
        return boulders;
    }
    
    public void RegisterCrate(CrateScript c)
    {
        crates.Add(c);
    }
    
    public void UnregisterCrate(CrateScript c)
    {
        crates.Remove(c);
    }
    
    public List<CrateScript> GetAllCrates()
    {
        return crates;
    }

    public void RegisterHole(HoleScript h)
    {
        holes.Add(h);
    }

    public void UnregisterHole(HoleScript h)
    {
        holes.Remove(h);
    }
    
    public List<HoleScript> GetAllHoles()
    {
        return holes;
    }

}
