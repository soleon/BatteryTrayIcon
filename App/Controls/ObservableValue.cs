using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Percentage.App.Controls;

internal partial class ObservableValue<T> : INotifyPropertyChanged
{
    private T _value;

    public T Value
    {
        get => _value;
        set => SetField(ref _value, value);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void SetField<TValue>(ref TValue field, TValue value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<TValue>.Default.Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}