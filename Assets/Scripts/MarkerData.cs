using UnityEngine;
using System;

public class MarkerData
{
    public string Id { get; private set; }
    public string Name { get; set; }
    public string ColorCode { get; set; }
    public bool IsPlusButton { get; private set; }

    public MarkerData(string name, string colorCode)
    {
        this.Id = Guid.NewGuid().ToString();
        this.Name = name;
        this.ColorCode = colorCode;
        this.IsPlusButton = false;
    }

    public static MarkerData CreatePlusButton()
    {
        return new MarkerData
        {
            Id = "PLUS_BUTTON",
            Name = "+",
            ColorCode = "Transparent",
            IsPlusButton = true
        };
    }

    private MarkerData() { }
}