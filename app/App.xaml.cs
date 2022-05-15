using app.Domain;
using System.Windows;

namespace app {
    public partial class App : Application {
        public App() {
            using var db = new DatabaseModel();
            db.Database.EnsureCreated();
        }
}
}
