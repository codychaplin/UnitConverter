using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace UnitConverter.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    public Category unitCategory;

    [ObservableProperty]
    public decimal topValue;

    [ObservableProperty]
    public decimal bottomValue;

    [ObservableProperty]
    public string topUnit;

    [ObservableProperty]
    public string bottomUnit;

    [ObservableProperty]
    public string selectedTop;

    [ObservableProperty]
    public string selectedBottom;

    [ObservableProperty]
    public ObservableCollection<string> topUnits;

    [ObservableProperty]
    public ObservableCollection<string> bottomUnits;

    public MainViewModel()
    {
        TopUnits = new ObservableCollection<string>()
        {
            "hello",
            "there",
            "test",
            "great"
        };

        BottomUnits = new ObservableCollection<string>()
        {
            "test",
            "there",
            "hello",
            "great"
        };

        TopUnit = "cm";
        BottomUnit = "km";
        TopValue = 10;
        BottomValue = 90;

        SelectedTop = TopUnits[0];
        SelectedBottom = BottomUnits[0];
    }
}

public enum Category
{
    Length,
    Temperature,
    Area
}