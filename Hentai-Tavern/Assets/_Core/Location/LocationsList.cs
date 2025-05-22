using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocationsList", menuName = "GAME/Locations/LocationsList")]
public class LocationsList : ScriptableObject
{
    public List<LocationConfig> list = new();

    public LocationConfig GetLocation(Locations tag) => list.Find(x => x.tag == tag);
}
