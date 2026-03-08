using CoworkingApp.BusinessLogic;
using CoworkingApp.BusinessLogic.Models;
using CoworkingApp.BusinessLogic.Services;
using CoworkingApp.BusinessLogic.Database; // <- za ConfigReader
using System.Collections.ObjectModel;
using System.ComponentModel;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly CoworkingFacade _facade;

    private string _chainName = "";
    public string ChainName
    {
        get => _chainName;
        set { _chainName = value; OnPropertyChanged(nameof(ChainName)); }
    }

    public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>();
    public ObservableCollection<Location> Locations { get; set; } = new ObservableCollection<Location>();
    public ObservableCollection<Reservation> Reservations { get; set; } = new ObservableCollection<Reservation>();

    // <<< Konstruktor sada prima i ConfigReader
    public MainWindowViewModel(CoworkingFacade facade, ConfigReader config)
    {
        _facade = facade;

        // Chain name iz fajla
        ChainName = config.ChainName;

        // Ucitaj podatke iz baze
        LoadUsers();
        LoadLocations();
        //LoadReservations(); // ako zelis
    }

    private void LoadUsers()
    {
        var dbUsers = _facade.GetAllUsers();
        Users.Clear();
        foreach (var u in dbUsers)
            Users.Add(u);
    }

    private void LoadLocations()
    {
        var dbLocations = _facade.GetAllLocations();
        Locations.Clear();
        foreach (var l in dbLocations)
            Locations.Add(l);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}