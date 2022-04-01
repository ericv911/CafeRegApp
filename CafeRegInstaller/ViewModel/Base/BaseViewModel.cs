using System.ComponentModel;

namespace CafeRegApp
{
    /// A base view model that fires Property Changed events as needed
    public class BaseViewModel : INotifyPropertyChanged
    {
        /// The event that is fired when any child property changes its value
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
        /// Call this to fire a <see cref="PropertyChanged"/> event
        /// <param name="name"></param>
        public void OnPropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
