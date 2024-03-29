﻿namespace UnitConverter.Models;

public class Unit
{
    public string Name { get; set; }
    public string Abbreviation { get; set; }
    public decimal ToBase { get; set; }
    public decimal? Offset { get; set; }

    public override string ToString()
    {
        return Name;
    }
}

public enum Category
{
    Length,
    Mass,
    Temperature,
    Volume,
    Area,
    Speed,
    Time,
    Data,
    Energy
}