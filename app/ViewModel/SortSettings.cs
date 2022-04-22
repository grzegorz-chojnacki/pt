namespace app.ViewModel {
    public enum SortBy { Name, Extension, Size, ModifiedDate }
    public enum SortDirection { Ascending, Descending }

    public class SortSettings : ViewModelBase {
        private SortBy sortBy = SortBy.Name;
        public SortBy SortBy {
            get { return sortBy; }
            set {
                if (sortBy != value) {
                    sortBy = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private SortDirection sortDirection = SortDirection.Ascending;
        public SortDirection SortDirection {
            get { return sortDirection; }
            set {
                if (sortDirection != value) {
                    sortDirection = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
