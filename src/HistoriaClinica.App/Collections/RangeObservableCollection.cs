using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace HistoriaClinica.App.Collections;

// ObservableCollection que agrega o reemplaza lotes de items con una sola
// notificación, para que la UI reconstruya la lista de una vez en lugar de
// refrescarse por cada item.
public sealed class RangeObservableCollection<T> : ObservableCollection<T>
{
    public void AddRange(IEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        var nuevos = items as IList<T> ?? items.ToList();
        if (nuevos.Count == 0)
        {
            return;
        }

        foreach (var item in nuevos)
        {
            Items.Add(item);
        }

        NotificarReconstruccion();
    }

    public void ReplaceAll(IEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        var nuevos = items as IList<T> ?? items.ToList();
        var habiaItems = Items.Count > 0;

        Items.Clear();
        foreach (var item in nuevos)
        {
            Items.Add(item);
        }

        if (!habiaItems && Items.Count == 0)
        {
            return;
        }

        NotificarReconstruccion();
    }

    // Un solo evento Reset: la UI vuelve a leer toda la lista sin eventos
    // intermedios, igual que hace ObservableCollection al limpiar.
    private void NotificarReconstruccion()
    {
        OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
        OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
}
